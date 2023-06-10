namespace Orbit.CodeAnalysis.Syntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, StatementSyntax thenBlock, ElseClauseSyntax elseNode)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenBlock = thenBlock;
            ElseNode = elseNode;
        }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenBlock { get; }
        public ElseClauseSyntax ElseNode { get; }
    }
}