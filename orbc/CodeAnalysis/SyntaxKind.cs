namespace Orbit.CodeAnalysis
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
        OpenParenToken,
        CloseParenToken,
        
        // Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}