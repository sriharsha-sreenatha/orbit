namespace Orbit.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        // binary
        Addition,
        Subtraction,
        Multiplication,
        Division,

        // binary assignment
        AdditionAssignment,
        SubtractionAssignment,
        MultiplicationAssignment,
        DivisionAssignment,

        // logical
        LessThan,
        LessOrEqualsTo,
        GreaterThan,
        GreaterOrEqualsTo,
        LogicalAnd,
        LogicalOr,
        IsEquals,
        IsNotEquals,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
    }
}