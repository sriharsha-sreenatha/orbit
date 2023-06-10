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
        LessOrEqualsToken,
        LessToken,
        GreaterOrEqualsToken,
        GreaterToken,

        IdentifierToken,
        
        // Keywords
        TrueKeyword,
        FalseKeyword,
        AndKeyword,
        OrKeyword,
        NotKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,

        // Nodes
        CompilationUnit,

        // Statements
        ExpressionStatement,
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ElseClause,

        // Expressions
        LiteralExpression,
        NameExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}