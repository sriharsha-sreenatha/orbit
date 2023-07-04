
using System.Collections.Immutable;
using Orbit.CodeAnalysis.Binding;

namespace Orbit.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private Lowerer()
        {
            
        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
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
            var condition = new BoundBinaryExpression(
                                new BoundVariableExpression(loopvar.Variable),
                                BoundBinaryOperator.Bind(Syntax.SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)),
                                node.Upper
                            );
            var increment = new BoundExpressionStatement(
                                new BoundOperatorAssignmentExpression(
                                    loopvar.Variable,
                                    BoundBinaryOperator.Bind(Syntax.SyntaxKind.PlusEqualsToken, typeof(int), typeof(int)),
                                    new BoundLiteralExpression(1)
                                )
                            );
            var whileStatement = new BoundWhileStatement(condition, new BoundBlockStatement(ImmutableArray.Create(node.Body, increment)));
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(loopvar, whileStatement));
            return RewriteStatement(result);
        }
    }
}