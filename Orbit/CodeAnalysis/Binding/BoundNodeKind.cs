namespace Orbit.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
        IfStatement,
        WhileStatement,
        ForStatement,

        // Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        OperatorAssignmentExpression,
    }
}