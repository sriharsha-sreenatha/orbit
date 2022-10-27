namespace Orbit.CodeAnalysis
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;
        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if(node is LiteralExpressionSyntax n)
                return (int) n.LiteralToken.Value;
            if(node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        return left + right;
                    case SyntaxKind.MinusToken:
                        return left - right;
                    case SyntaxKind.StarToken:
                        return left * right;
                    case SyntaxKind.SlashToken:
                        return left / right;
                    default:
                        throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}\n");
                }
            }
            if(node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);
            
            throw new Exception($"Unexpected node {node.Kind}\n");
        }
    }
}