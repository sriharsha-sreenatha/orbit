namespace Orbit.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        SlashToken,
        StarToken,
        NotToken,
        DoubleAmpersandToken,
        DoublePipeToken,
        OpenParenToken,
        CloseParenToken,
        OpenBraceToken,
        CloseBraceToken,
        NotEqualsToken,
        DoubleEqualsToken,
        EqualsToken,
        IdentifierToken,
        
        // Keywords
        TrueKeyword,
        FalseKeyword,
        AndKeyword,
        OrKeyword,
        NotKeyword,

        // Nodes
        CompilationUnit,

        // Statements
        ExpressionStatement,
        BlockStatement,

        // Expressions
        LiteralExpression,
        NameExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}