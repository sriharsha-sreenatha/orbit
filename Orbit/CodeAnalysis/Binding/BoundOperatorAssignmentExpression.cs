using Orbit.CodeAnalysis.Symbols;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundOperatorAssignmentExpression : BoundExpression
    {
        public BoundOperatorAssignmentExpression(VariableSymbol variable, BoundBinaryOperator oper, BoundExpression expression)
        {
            Variable = variable;
            Operator = oper;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.OperatorAssignmentExpression;
        public override TypeSymbol Type => Variable.Type;
        public VariableSymbol Variable { get; }
        public BoundBinaryOperator Operator { get; }
        public BoundExpression Expression { get; }
    }
}