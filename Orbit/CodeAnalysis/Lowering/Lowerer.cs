
using System.Collections.Immutable;
using Orbit.CodeAnalysis.Binding;

namespace Orbit.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;
        private Lowerer()
        {
            
        }

        private BoundLabel GenerateSymbol()
        {
            var name = $"__Label{++_labelCount}__";
            return new BoundLabel(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();

            stack.Push(statement);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    foreach (var s in block.Statements.Reverse())
                        stack.Push(s);
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatements == null)
            {
                // if <condition>
                //      <then>
                // ----------------------
                // gotoIfFalse <condition> $end
                // <then>
                // $end:

                var endLabel = GenerateSymbol();
                var gotoStatement = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(gotoStatement, node.ThenStatements, endLabelStatement));
                return RewriteStatement(result);
            }
            else
            {
                // if <condition>
                //      <then>
                // else
                //      <else>
                // ----------------------
                // gotoIfFalse <condition> $else
                // <then>
                // goto $end
                // $else:
                // <else>
                // $end:

                var endLabel = GenerateSymbol();
                var elseLabel = GenerateSymbol();

                var gotoElseStatement = new BoundConditionalGotoStatement(elseLabel, node.Condition, false);
                // then part
                var gotoEndStatement = new BoundGotoStatement(endLabel);
                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                // else part
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                                gotoElseStatement, 
                                node.ThenStatements,
                                gotoEndStatement,
                                elseLabelStatement,
                                node.ElseStatements,
                                endLabelStatement));
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            // while <condition>
            //      <body>
            // ---------------------
            //
            // $checkAgain:
            // gotoIfFalse <condition> $end
            // <body>
            // goto $checkAgain
            // $end:

            var checkLabel = GenerateSymbol();
            var endLabel = GenerateSymbol();

            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoEndStatement = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
            var gotoCheckStatement = new BoundGotoStatement(checkLabel);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                checkLabelStatement,
                gotoEndStatement,
                node.Body,
                gotoCheckStatement,
                endLabelStatement));
            return RewriteStatement(result);

            // --------------------------- OR --------------------
            // goto $check
            // $continue:
            // <body>
            // $check:
            // gotoIfTrue <condition> $continue
            // $end:
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            // for <loopvar> = <lower> to <upper>
            //      <body>
            //
            // ---------------
            //
            // {
            //      var <loopvar> = <lower>
            //      while <loopvar> <= <upper>
            //      {
            //          <body>
            //          <loopvar> += 1
            //      }
            // }

            var loopvar = new BoundVariableDeclaration(node.LoopVar, node.Lower);
            var uboundSymbol = new VariableSymbol("ubound", true, typeof(int));
            var upperBoundDecl = new BoundVariableDeclaration(uboundSymbol, node.Upper);
            var condition = new BoundBinaryExpression(
                                new BoundVariableExpression(loopvar.Variable),
                                BoundBinaryOperator.Bind(Syntax.SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)),
                                new BoundVariableExpression(uboundSymbol)
                            );
            var increment = new BoundExpressionStatement(
                                new BoundOperatorAssignmentExpression(
                                    loopvar.Variable,
                                    BoundBinaryOperator.Bind(Syntax.SyntaxKind.PlusEqualsToken, typeof(int), typeof(int)),
                                    new BoundLiteralExpression(1)
                                )
                            );
            var whileStatement = new BoundWhileStatement(condition, new BoundBlockStatement(ImmutableArray.Create(node.Body, increment)));
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(loopvar, upperBoundDecl, whileStatement));
            return RewriteStatement(result);
        }
    }
}