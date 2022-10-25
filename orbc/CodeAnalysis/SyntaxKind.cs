namespace Orbit.CodeAnalysis
{
    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        SlashToken,
        StarToken,
        MinusToken,
        CloseParenToken,
        OpenParenToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}