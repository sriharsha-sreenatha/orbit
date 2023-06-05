using Orbit.CodeAnalysis.Syntax;
using Orbit.CodeAnalysis;
using Xunit;

namespace Orbit.Tests.CodeAnalysis
{
    public class EvaulationTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("+1", 1)]
        
        [InlineData("1 + 2", 3)]
        [InlineData("4 - 2", 2)]
        [InlineData("3 * -2", -6)]
        [InlineData("10 / 5", 2)]
        
        [InlineData("3 * (5 - 2)", 9)]
        [InlineData("(3 * 5) - 2", 13)]
        
        [InlineData("(a = 10) * a", 100)]

        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!false", !false)]
        [InlineData("!true", !true)]
        [InlineData("true == true", true)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("true == !false", true)]
        [InlineData("true and false", false)]
        [InlineData("true or false", true)]
        [InlineData("not true", false)]

        public void Evaulation_GetText_RoundTrips(string text, object actualValue)
        {
            var expression = SyntaxTree.Parse(text);
            var compilation = new Compilation(expression);
            var variables = new Dictionary<VariableSymbol, object>();
            var evaluator = compilation.Evaluate(variables);

            Assert.Empty(evaluator.Diagnostics);
            Assert.Equal(actualValue, evaluator.Value);
        }
    }
}