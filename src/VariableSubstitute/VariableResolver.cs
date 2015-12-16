using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace VariableSubstitute
{
    public class VariableResolver
    { 
        public string Resolve(string content, Variable[] variables)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            if (variables.Any(v => v == null))
            {
                throw new ArgumentException("One or more variables are null.", nameof(variables));
            }

            var processor = new VariableProcessor(variables);
            return processor.Resolve(content);
        }
    }

    internal class VariableProcessor
    {
        private static readonly Regex replaceRegex = new Regex(@"\$\{(?<variablename>[a-zA-Z]+)(\:(?<defaultvalue>[^\{\}]+))?\}");

        private readonly IDictionary<string, Variable> variables;

        public VariableProcessor(Variable[] variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            this.variables = variables.ToDictionary(v => v.Name, v => v);
        }

        public string Resolve(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var context = new VariableContext(new ReadOnlyDictionary<string, Variable>(this.variables));
            return InnerResolve(content, context);
        }

        private static string InnerResolve(string content, VariableContext context)
        {
            return replaceRegex.Replace(content, match => MatchEvaluator(match, context));
        }

        private static string MatchEvaluator(Match match, VariableContext context)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var variableName = match.Groups["variablename"].Value;
            if (context.CallStack.Contains(variableName))
            {
                throw new InvalidOperationException($"Circuar reference of the variable {variableName} detected.");
            }

            context.CallStack.Push(variableName);

            string value = match.Value;

            if (context.Variables.ContainsKey(variableName))
            {
                value = InnerResolve(context.Variables[variableName].Value, context);
            }
            else if (match.Groups["defaultvalue"].Success)
            {
                value = InnerResolve(match.Groups["defaultvalue"].Value, context);
            }

            context.CallStack.Pop();

            return value;
        }
    }

    internal class VariableContext
    {
        public VariableContext(IReadOnlyDictionary<string, Variable> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            this.Variables = variables;
        }


        public IReadOnlyDictionary<string, Variable> Variables { get; }

        public Stack<string> CallStack { get; } = new Stack<string>();
    }
}