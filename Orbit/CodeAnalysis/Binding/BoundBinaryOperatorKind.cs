namespace Orbit.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LessThan,
        LessOrEqualsTo,
        GreaterThan,
        GreaterOrEqualsTo,
        LogicalAnd,
        LogicalOr,
        IsEquals,
        IsNotEquals,
    }
}