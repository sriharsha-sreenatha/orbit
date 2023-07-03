using System;
using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type opType)
            : this(syntaxKind, kind, opType, opType)
        {

        }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type opType, Type resType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OpType = opType;
            ResType = resType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public Type OpType { get; }
        public Type ResType { get; }

        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.NotToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.NotKeyword, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.TildaToken, BoundUnaryOperatorKind.OnesComplement, typeof(int)),
        };

        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, Type opType)
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