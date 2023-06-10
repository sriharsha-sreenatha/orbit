using System.Collections.Immutable;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement thenStatements, BoundStatement elseStatements)
        {
            Condition = condition;
            ThenStatements = thenStatements;
            ElseStatements = elseStatements;
        }

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatements { get; }
        public BoundStatement ElseStatements { get; }
        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}