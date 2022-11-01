using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch(syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOpKind = BindUnaryOpeartorKind(syntax.OperatorToken.Kind, boundOperand.Type);
            if(boundOpKind == null)
            {
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}.");
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOpKind.Value, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOpKind = BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if(boundOpKind == null)
            {
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeft.Type} and {boundRight.Type}.");
                return boundLeft;
            }
            return new BoundBinaryExpression(boundLeft, boundOpKind.Value, boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOpeartorKind(SyntaxKind kind, Type operandType)
        {
            if(operandType == typeof(int))
            {
                switch(kind)
                {
                    case SyntaxKind.PlusToken:
                        return BoundUnaryOperatorKind.Identity;
                    case SyntaxKind.MinusToken:
                        return BoundUnaryOperatorKind.Negation;
                }
            }
            if(operandType == typeof(bool))
            {
                switch(kind)
                {
                    case SyntaxKind.NotToken:
                        return BoundUnaryOperatorKind.LogicalNegation;
                }
            }

            throw new Exception($"Unexpected unary operator {kind}.");
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if(leftType == typeof(int) || rightType == typeof(int))
            {
                switch(kind)
                {
                    case SyntaxKind.PlusToken:
                        return BoundBinaryOperatorKind.Addition;
                    case SyntaxKind.MinusToken:
                        return BoundBinaryOperatorKind.Subtraction;
                    case SyntaxKind.StarToken:
                        return BoundBinaryOperatorKind.Multiplication;
                    case SyntaxKind.SlashToken:
                        return BoundBinaryOperatorKind.Division;
                }
            }

            if(leftType == typeof(bool) || rightType == typeof(bool))
            {
                switch(kind)
                {
                    case SyntaxKind.DoubleAmpersandToken:
                        return BoundBinaryOperatorKind.LogicalAnd;
                    case SyntaxKind.DoublePipeToken:
                        return BoundBinaryOperatorKind.LogicalOr;
                }
            }
            
            throw new Exception($"Unexpected binary operator {kind}");
        }
    }
}