namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorKind opKind, BoundExpression operand)
        {
            OperatorKind = opKind;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override Type Type => Operand.Type;
        public BoundUnaryOperatorKind OperatorKind { get; }
        public BoundExpression Operand { get; }
    }
}