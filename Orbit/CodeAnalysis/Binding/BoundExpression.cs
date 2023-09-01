using Orbit.CodeAnalysis.Symbols;

namespace Orbit.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}