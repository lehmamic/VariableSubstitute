using System;
using Xunit;

namespace VariableSubstitute.Tests
{
    public class VariableResolverTest
    {
        [Fact]
        public void Resolve_ContentWithAnExistingVariable_ReturnsSubstitutedString()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new[]
            {
                new Variable("MyVar", "MyValue")
            };

            // act
            string actual = target.Resolve("This test contains a variable 'MyVar' with the value ${MyVar}.", variables);

            // assert
            Assert.Equal("This test contains a variable 'MyVar' with the value MyValue.", actual);
        }

        [Fact]
        public void Resolve_DefaultValueSpecifiedAndVariableIsAvailable_ReturnsVariableValue()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new[]
            {
                new Variable("MyVar", "MyValue")
            };

            // act
            string actual = target.Resolve("This test contains a variable 'MyVar' with the value ${MyVar:DefaultValue}.", variables);

            // assert
            Assert.Equal("This test contains a variable 'MyVar' with the value MyValue.", actual);
        }

        [Fact]
        public void Resolve_DefaultValueSpecifiedAndVariableIsNotAvailable_ReturnsDefaultValue()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new Variable[0];

            // act
            string actual = target.Resolve("This test contains a variable 'MyVar' with the value ${MyVar:DefaultValue}.", variables);

            // assert
            Assert.Equal("This test contains a variable 'MyVar' with the value DefaultValue.", actual);
        }

        [Fact]
        public void Resolve_WhetherVariableNorDefaultValueSpecified_ReturnsOriginalString()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new Variable[0];

            // act
            string actual = target.Resolve("This test contains a variable 'MyVar' with the value ${MyVar}.", variables);

            // assert
            Assert.Equal("This test contains a variable 'MyVar' with the value ${MyVar}.", actual);
        }

        [Fact]
        public void Resolve_VariableUsesVariable_ResolvesValueRecursive()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new[]
            {
                new Variable("MyVar", "This Variable uses another variable ${OtherVar}"),
                new Variable("OtherVar", "OtherValue")
            };

            // act
            string actual = target.Resolve("This test contains a variable 'MyVar' with the value [${MyVar}].", variables);

            // assert
            Assert.Equal("This test contains a variable 'MyVar' with the value [This Variable uses another variable OtherValue].", actual);
        }

        [Fact]
        public void Resolve_CircularVariableDependency_ThrowsException()
        {
            // arrange
            var target = new VariableResolver();
            var variables = new[]
            {
                new Variable("MyVar", "This Variable uses another variable ${MyVar}")
            };

            // act
            Assert.Throws<InvalidOperationException>(() =>target.Resolve("This test contains a variable 'MyVar' with the value [${MyVar}].", variables));
        }
    }
}
