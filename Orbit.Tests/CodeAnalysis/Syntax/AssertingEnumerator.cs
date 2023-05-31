using Orbit.CodeAnalysis.Syntax;
using Xunit;

namespace Orbit.Tests.CodeAnalysis.Syntax
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> _enumarator;
        private bool _hasErrors;
        public AssertingEnumerator(SyntaxNode node)
        {
            _enumarator = Flatten(node).GetEnumerator();
        }

        private bool MarkFailed()
        {
            _hasErrors = true;
            return false;
        }

        public void Dispose()
        {
            if (!_hasErrors)
                Assert.False(_enumarator.MoveNext());
            
            _enumarator.Dispose();
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);
            while(stack.Count > 0)
            {
                var n = stack.Pop();
                yield return n;

                foreach(var child in n.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertToken(SyntaxKind kind, string text)
        {
            try{
                Assert.True(_enumarator.MoveNext());
                Assert.Equal(kind, _enumarator.Current.Kind);
                var token = Assert.IsType<SyntaxToken>(_enumarator.Current);
                Assert.Equal(text, token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try{
                Assert.True(_enumarator.MoveNext());
                Assert.Equal(kind, _enumarator.Current.Kind);
                Assert.IsNotType<SyntaxToken>(_enumarator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }
    }
}