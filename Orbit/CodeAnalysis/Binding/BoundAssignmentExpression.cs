using System;
using Orbit.CodeAnalysis.Symbols;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public override TypeSymbol Type => Variable.Type;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}