using Orbit.CodeAnalysis.Syntax;
using Xunit;

namespace Orbit.Tests.CodeAnalysis.Syntax
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void Parser_BinaryExpression_HonorsPrecedence(SyntaxKind op1, SyntaxKind op2)
        {
            var op1Prec = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            var op2Prec = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
            var op1Text = SyntaxFacts.GetText(op1);
            var op2Text = SyntaxFacts.GetText(op2);

            var text = $"a {op1Text} b {op2Text} c";

            var expression = SyntaxTree.Parse(text).Root;

            if (op1Prec >= op2Prec)
            {
                //     op2
                //    /   \
                //  op1    c
                // /   \
                // a   b
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression); // op2
                    e.AssertNode(SyntaxKind.BinaryExpression); // op1
                    e.AssertNode(SyntaxKind.NameExpression); // a
                    e.AssertToken(SyntaxKind.IdentifierToken, "a"); // a
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxKind.NameExpression); // b
                    e.AssertToken(SyntaxKind.IdentifierToken, "b"); // b
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression); // c
                    e.AssertToken(SyntaxKind.IdentifierToken, "c"); // c
                }
            }
            else
            {
                //  op1
                // /   \
                // a    op2
                //      /  \
                //     b    c
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression); // op1
                    e.AssertNode(SyntaxKind.NameExpression); // a
                    e.AssertToken(SyntaxKind.IdentifierToken, "a"); // a
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxKind.BinaryExpression); // op2
                    e.AssertNode(SyntaxKind.NameExpression); // b
                    e.AssertToken(SyntaxKind.IdentifierToken, "b"); // b
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression); // c
                    e.AssertToken(SyntaxKind.IdentifierToken, "c"); // c
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void Parser_UnaryExpression_HonorsPrecedence(SyntaxKind unaryKind, SyntaxKind binaryKind)
        {
            var unaryPrec = SyntaxFacts.GetUnaryOperatorPrecedence(unaryKind);
            var binaryPrec = SyntaxFacts.GetBinaryOperatorPrecedence(binaryKind);
            var unaryText = SyntaxFacts.GetText(unaryKind);
            var binaryText = SyntaxFacts.GetText(binaryKind);

            var text = $"{unaryText} a {binaryText} b";

            var expression = SyntaxTree.Parse(text).Root;

            if (unaryPrec >= binaryPrec)
            {
                //   binary
                //  /     \
                // unary   b
                //  |
                //  a    
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression); // binary
                    e.AssertNode(SyntaxKind.UnaryExpression); // unary
                    e.AssertToken(unaryKind, unaryText);
                    e.AssertNode(SyntaxKind.NameExpression); // a
                    e.AssertToken(SyntaxKind.IdentifierToken, "a"); // a
                    e.AssertToken(binaryKind, binaryText);
                    e.AssertNode(SyntaxKind.NameExpression); // b
                    e.AssertToken(SyntaxKind.IdentifierToken, "b"); // b
                }
            }
            else
            {
                // unary
                //   |   
                // binary
                // /   \
                // a   b
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.UnaryExpression); // unary
                    e.AssertToken(unaryKind, unaryText);
                    e.AssertNode(SyntaxKind.BinaryExpression); // binary
                    e.AssertNode(SyntaxKind.NameExpression); // a
                    e.AssertToken(SyntaxKind.IdentifierToken, "a"); // a
                    e.AssertToken(binaryKind, binaryText);
                    e.AssertNode(SyntaxKind.NameExpression); // b
                    e.AssertToken(SyntaxKind.IdentifierToken, "b"); // b
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach(var op1 in SyntaxFacts.GetBinaryOperatorKinds())
            {
                foreach(var op2 in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[]{op1, op2};
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach(var unary in SyntaxFacts.GetUnaryOperatorKinds())
            {
                foreach(var binary in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[]{unary, binary};
                }
            }
        }
    }
}