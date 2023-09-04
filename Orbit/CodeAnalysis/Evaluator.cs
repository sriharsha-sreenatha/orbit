using System;
using System.Collections.Generic;
using Orbit.CodeAnalysis.Binding;
using Orbit.CodeAnalysis.Symbols;
using Orbit.CodeAnalysis.Syntax;

namespace Orbit.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labelIndex = new Dictionary<BoundLabel, int>();

            for (int i=0; i<_root.Statements.Length; i++)
            {
                if (_root.Statements[i] is BoundLabelStatement l)
                    labelIndex.Add(l.Label, i+1);
            }

            var index = 0;
            while (index < _root.Statements.Length)
            {
                var stmt = _root.Statements[index];

                switch (stmt.Kind)
                {
                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)stmt);
                        index++;
                        break;
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)stmt);
                        index++;
                        break;
                    case BoundNodeKind.GotoStatement:
                        var gt = ((BoundGotoStatement)stmt);
                        index = labelIndex[gt.Label];
                        break;
                    case BoundNodeKind.ConditionalGotoStatement:
                        var cgt = ((BoundConditionalGotoStatement)stmt);
                        var condition = (bool)EvaluateExpression(cgt.Condition);
                        if (condition == cgt.JumpIfTrue)
                            index = labelIndex[cgt.Label];
                        else
                            index++;
                        break;
                    case BoundNodeKind.LabelStatement:
                        index++;
                        break;
                    default:
                        throw new Exception($"Unexpected node {stmt.Kind}\n");
                }
            }
            //EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateBoundLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateBoundVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateBoundAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.OperatorAssignmentExpression:
                    return EvaluateBoundOperatorAssignmentExpression((BoundOperatorAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateBoundUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBoundBinaryExpression((BoundBinaryExpression)node);
                default:
                    throw new Exception($"Unexpected node {node.Kind}\n");
            }
        }

        private static object EvaluateBoundLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

        private object EvaluateBoundVariableExpression(BoundVariableExpression v)
        {
            return _variables[v.Variable];
        }

        private object EvaluateBoundAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
            return value;
        }

        private object EvaluateBoundOperatorAssignmentExpression(BoundOperatorAssignmentExpression a)
        {
            var prevValue = _variables[a.Variable];
            var rightValue = EvaluateExpression(a.Expression);
            switch (a.Operator.Kind)
            {
                case BoundBinaryOperatorKind.AdditionAssignment:
                    if (a.Type == TypeSymbol.Int)
                    {
                        _variables[a.Variable] = (int)prevValue + (int)rightValue;
                        return _variables[a.Variable];
                    }
                    else if (a.Type == TypeSymbol.String)
                    {
                        _variables[a.Variable] = (string)prevValue + (string)rightValue;
                        return _variables[a.Variable];
                    }
                    else
                        throw new Exception($"Unexpected binary {a.Operator.Kind} expression for type '{a.Type}'\n");
                case BoundBinaryOperatorKind.SubtractionAssignment:
                    _variables[a.Variable] = (int)prevValue - (int)rightValue;
                    return _variables[a.Variable];
                case BoundBinaryOperatorKind.MultiplicationAssignment:
                    _variables[a.Variable] = (int)prevValue * (int)rightValue;
                    return _variables[a.Variable];
                case BoundBinaryOperatorKind.DivisionAssignment:
                    if ((int)rightValue == 0)
                        throw new Exception($"Division by zero error\n");
                    _variables[a.Variable] = (int)prevValue / (int)rightValue;
                    return _variables[a.Variable];
                default:
                    throw new Exception($"Unexpected binary assignment operator {a.Operator.Kind}\n");
            }
        }

        private object EvaluateBoundUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.Operator.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorKind.OnesComplement:
                    return ~(int)operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.Operator.Kind}\n");
            }
        }

        private object EvaluateBoundBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Operator.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if (b.Type == TypeSymbol.Int) 
                        return (int)left + (int)right;
                    else if (b.Type == TypeSymbol.String)
                        return (string)left + (string)right;
                    else
                        throw new Exception($"Unexpected binary {b.Operator.Kind} expression for type '{b.Type}'\n");
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                
                case BoundBinaryOperatorKind.LessThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.LessOrEqualsTo:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorKind.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.GreaterOrEqualsTo:
                    return (int)left >= (int)right;

                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                
                case BoundBinaryOperatorKind.BitwiseAnd:
                    if (b.Type == TypeSymbol.Int) 
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorKind.BitwiseOr:
                    if (b.Type == TypeSymbol.Int) 
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorKind.BitwiseXor:
                    if (b.Type == TypeSymbol.Int) 
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;

                case BoundBinaryOperatorKind.IsEquals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.IsNotEquals:
                    return !Equals(left, right);
                
                default:
                    throw new Exception($"Unexpected binary operator {b.Operator.Kind}\n");
            }
        }
    }
}