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

        // Expressions
        LiteralExpression,
        NameExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}