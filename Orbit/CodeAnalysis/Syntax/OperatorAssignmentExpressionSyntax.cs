namespace Orbit.CodeAnalysis.Syntax
{
    public sealed class OperatorAssignmentExpressionSyntax : ExpressionSyntax
    {
        public OperatorAssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken opEqualsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            OpEqualsToken = opEqualsToken;
            Expression = expression;
        }
        
        public override SyntaxKind Kind => SyntaxKind.OperatorAssignmentExpression;
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken OpEqualsToken { get; }
        public ExpressionSyntax Expression { get; }
    }
}