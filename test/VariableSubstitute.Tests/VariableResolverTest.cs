using Xunit;
using VariableSubstitute;

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
    }
}
