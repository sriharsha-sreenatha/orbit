using Orbit.CodeAnalysis.Symbols;
using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol opType)
            : this(syntaxKind, kind, opType, opType)
        {

        }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol opType, TypeSymbol resType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OpType = opType;
            ResType = resType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public TypeSymbol OpType { get; }
        public TypeSymbol ResType { get; }

        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.NotToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            new BoundUnaryOperator(SyntaxKind.NotKeyword, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.TildaToken, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.Int),
        };

        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol opType)
        {
            foreach(var op in _operators)
            {
                if(op.SyntaxKind == syntaxKind && op.OpType == opType)
                    return op;
            }
            return null;
        }
    }
}