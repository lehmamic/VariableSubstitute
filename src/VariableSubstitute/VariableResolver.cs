using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VariableSubstitute
{
    public class VariableResolver
    {
        private readonly Regex replaceRegex = new Regex(@"(<variable>?${[a-zA-Z]+})");

        public string Resolve(string value, Variable[] variables)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            if (variables.Any(v => v == null))
            {
                throw new ArgumentException("One or more variables are null.", nameof(variables));
            }

            Dictionary<string, string> variableLookup = variables.ToDictionary(v => v.Name, v => v.Value);

            return null;
        }
    }

    internal class VariableProcessor
    {
        private readonly Regex replaceRegex = new Regex(@"\${(<variable>?[a-zA-Z]+)}");

        private readonly IDictionary<string, string> variables;

        public VariableProcessor(Variable[] variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            this.variables = variables.ToDictionary(v => v.Name, v => v.Value);
        }

        public string Resolve(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return this.replaceRegex.Replace(content, this.MatchEvaluator);
        }

        private string MatchEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            return this.variables[match.Groups["variable"].Value];
        }
    }
}