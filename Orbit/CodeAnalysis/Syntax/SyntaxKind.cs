namespace Orbit.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        StringToken,
        PlusToken,
        MinusToken,
        SlashToken,
        StarToken,
        PlusEqualsToken,
        MinusEqualsToken,
        SlashEqualsToken,
        StarEqualsToken,
        NotToken,
        DoubleAmpersandToken,
        DoublePipeToken,
        AmpersandToken,
        PipeToken,
        HatToken,
        TildaToken,
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
        XorKeyword,
        NotKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,

        // Nodes
        CompilationUnit,

        // Statements
        ExpressionStatement,
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ElseClause,
        WhileStatement,
        ForStatement,

        // Expressions
        LiteralExpression,
        NameExpression,
        AssignmentExpression,
        OperatorAssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}