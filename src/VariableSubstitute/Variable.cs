using System;
using System.Text.RegularExpressions;

namespace VariableSubstitute
{
    public class Variable
    {
        public Variable(string name, string value)
        {
            if (!Regex.IsMatch(name, "[a-zA-Z]+"))
            {
                throw new ArgumentException("The variable name is null, empty or contains non characters [a-z, A-Z], please provide a meaningfull variable name.", nameof(name));
            }

            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}