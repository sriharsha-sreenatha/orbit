using System;
using Orbit.CodeAnalysis.Symbols;
using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type)
            : this(syntaxKind, kind, type, type, type)
        {

        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type, TypeSymbol resType)
            : this(syntaxKind, kind, type, type, resType)
        {

        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResType = resType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResType { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeSymbol.Int),
            
            new BoundBinaryOperator(SyntaxKind.PlusEqualsToken, BoundBinaryOperatorKind.AdditionAssignment, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.MinusEqualsToken, BoundBinaryOperatorKind.SubtractionAssignment, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarEqualsToken, BoundBinaryOperatorKind.MultiplicationAssignment, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.SlashEqualsToken, BoundBinaryOperatorKind.DivisionAssignment, TypeSymbol.Int),

            // For string concats
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxKind.PlusEqualsToken, BoundBinaryOperatorKind.AdditionAssignment, TypeSymbol.String),
            
            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Int),
            
            new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEqualsTo, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEqualsTo, TypeSymbol.Int, TypeSymbol.Bool),
            
            new BoundBinaryOperator(SyntaxKind.DoubleEqualsToken, BoundBinaryOperatorKind.IsEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.NotEqualsToken, BoundBinaryOperatorKind.IsNotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            
            new BoundBinaryOperator(SyntaxKind.DoubleEqualsToken, BoundBinaryOperatorKind.IsEquals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.NotEqualsToken, BoundBinaryOperatorKind.IsNotEquals, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.DoubleAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.AndKeyword, BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Bool),
            
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.DoublePipeToken, BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.OrKeyword, BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.XorKeyword, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Bool),
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
        {
            foreach(var op in _operators)
            {
                if(op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
                    return op;
            }
            return null;
        }
    }
}