namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(VariableSymbol loopVar, BoundExpression lower, BoundExpression upper, BoundStatement body)
        {
            LoopVar = loopVar;
            Lower = lower;
            Upper = upper;
            Body = body;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
        public VariableSymbol LoopVar { get; }
        public BoundExpression Lower { get; }
        public BoundExpression Upper { get; }
        public BoundStatement Body { get; }
    }
}