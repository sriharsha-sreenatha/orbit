using System;
using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
            : this(syntaxKind, kind, type, type, type)
        {

        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type, Type resType)
            : this(syntaxKind, kind, type, type, resType)
        {

        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResType = resType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResType { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),
            
            new BoundBinaryOperator(SyntaxKind.PlusEqualsToken, BoundBinaryOperatorKind.AdditionAssignment, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusEqualsToken, BoundBinaryOperatorKind.SubtractionAssignment, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarEqualsToken, BoundBinaryOperatorKind.MultiplicationAssignment, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashEqualsToken, BoundBinaryOperatorKind.DivisionAssignment, typeof(int)),
            
            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, typeof(int)),
            
            new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.LessThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEqualsTo, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.GreaterThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEqualsTo, typeof(int), typeof(bool)),
            
            new BoundBinaryOperator(SyntaxKind.DoubleEqualsToken, BoundBinaryOperatorKind.IsEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.NotEqualsToken, BoundBinaryOperatorKind.IsNotEquals, typeof(int), typeof(bool)),
            
            new BoundBinaryOperator(SyntaxKind.DoubleEqualsToken, BoundBinaryOperatorKind.IsEquals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.NotEqualsToken, BoundBinaryOperatorKind.IsNotEquals, typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.DoubleAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.AndKeyword, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
            
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.DoublePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.OrKeyword, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.XorKeyword, BoundBinaryOperatorKind.BitwiseXor, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
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