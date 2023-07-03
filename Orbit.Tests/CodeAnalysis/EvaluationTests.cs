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
        
        [InlineData("{ var a = 0 (a = 10) * a}", 100)]

        [InlineData("3 < 4", true)]
        [InlineData("5 < 4", false)]
        [InlineData("4 <= 4", true)]
        [InlineData("8 <= 4", false)]
        [InlineData("8 > 4", true)]
        [InlineData("3 > 4", false)]
        [InlineData("4 >= 4", true)]
        [InlineData("2 >= 4", false)]

        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!false", !false)]
        [InlineData("!true", !true)]
        [InlineData("true == true", true)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("true == !false", true)]
        
        [InlineData("true && true", true)]
        [InlineData("true && false", false)]
        [InlineData("false && true", false)]
        [InlineData("false && false", false)]

        [InlineData("true || true", true)]
        [InlineData("true || false", true)]
        [InlineData("false || true", true)]
        [InlineData("false || false", false)]

        [InlineData("true and true", true)]
        [InlineData("true and false", false)]
        [InlineData("false and true", false)]
        [InlineData("false and false", false)]
        
        [InlineData("true or true", true)]
        [InlineData("true or false", true)]
        [InlineData("false or true", true)]
        [InlineData("false or false", false)]

        [InlineData("not true", false)]
        [InlineData("not false", true)]
        
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 0", 0)]
        [InlineData("2 & 3", 2)]
        [InlineData("2 ^ 3", 1)]
        [InlineData("1 ^ 2", 3)]
        [InlineData("~1", -2)]

        [InlineData("true | true", true)]
        [InlineData("true | false", true)]
        [InlineData("false | true", true)]
        [InlineData("false | false", false)]

        [InlineData("false & false", false)]
        [InlineData("false & true", false)]
        [InlineData("true & false", false)]
        [InlineData("true & true", true)]

        [InlineData("false ^ false", false)]
        [InlineData("false ^ true", true)]
        [InlineData("true ^ false", true)]
        [InlineData("true ^ true", false)]
        
        [InlineData("false xor false", false)]
        [InlineData("false xor true", true)]
        [InlineData("true xor false", true)]
        [InlineData("true xor true", false)]

        [InlineData("{ var a = 10 if a == 10 a = 5 a }", 5)]
        [InlineData("{ var a = 7 if a == 10 a = 5 a }", 7)]
        [InlineData("{ var a = 10 if a == 10 a = 5 else a = 1 a}", 5)]
        [InlineData("{ var a = 4 if a == 10 a = 5 else a = 1 a}", 1)]

        [InlineData("{ var n = 10 var i = 1 while i < n { i = i + 1 } i}", 10)]

        [InlineData("{var i = 10 i += 1 i}", 11)]
        [InlineData("{var i = 5 i -= 1 i}", 4)]
        [InlineData("{var i = 4 i *= 10 i}", 40)]
        [InlineData("{var i = 100 i /= 4 i}", 25)]

        [InlineData("{ var result = 0 for i = 1 to 10 { result += i } result}", 55)]
        public void Evaulator_Computes_CorrectValues(string text, object actualValue)
        {
            AssertValue(text, actualValue);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
            {
                var x = 10
                var y = 100
                {
                    var x = 10
                }
                var [x] = 5
            }
            ";

            var diagnostic = @"
                Variable 'x' already declared.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            var text = @"
            {
                [)][]
            ";

            var diagnostic = @"
                Unexpected token <CloseParenToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_Undefined()
        {
            var text = @"[x] = 10";

            var diagnostic = @"
                Variable name 'x' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
        {
            var text = @"[]";

            var diagnostic = @"
                Unexpected token <EndOfFileToken>, expected <IdentifierToken>.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotAssign()
        {
            var text = @"
            {
                let x = 10
                x [=] 15
            }";

            var diagnostic = @"
                Cannot assign value to read-only variable 'x'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 10
                x = [false]
            }";

            var diagnostic = @"
                Cannot convert from type 'System.Boolean' to type 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 0
                if [10]
                    x = 10
            }";

            var diagnostic = @"
                Cannot convert from type 'System.Int32' to type 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 0
                while [10]
                    x = 10
            }";

            var diagnostic = @"
                Cannot convert from type 'System.Int32' to type 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            var text = @"
            {
                var result = 0
                for i = [false] to 10
                    result += 1
            }";

            var diagnostic = @"
                Cannot convert from type 'System.Boolean' to type 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            var text = @"
            {
                var result = 0
                for i = 10 to [false]
                    result += 1
            }";

            var diagnostic = @"
                Cannot convert from type 'System.Boolean' to type 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_UnaryExpression_Reports_Undefined()
        {
            var text = @"
            {
                [+]true
            }";

            var diagnostic = @"
                Unary operator '+' is not defined for type 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluator_BinaryExpression_Reports_Undefined()
        {
            var text = @"
            {
                10 [+] false
            }";

            var diagnostic = @"
                Binary operator '+' is not defined for types 'System.Int32' and 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostic);
        }

        private static void AssertValue(string text, object actualValue)
        {
            var expression = SyntaxTree.Parse(text);
            var compilation = new Compilation(expression);
            var variables = new Dictionary<VariableSymbol, object>();
            var evaluator = compilation.Evaluate(variables);

            Assert.Empty(evaluator.Diagnostics);
            Assert.Equal(actualValue, evaluator.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("ERROR: Must have same number of markings and diagnostics messages.");

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (int i=0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}