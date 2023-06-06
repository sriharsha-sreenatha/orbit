using Orbit.CodeAnalysis.Syntax;
using Orbit.CodeAnalysis.Text;
using Xunit;

namespace Orbit.Tests.CodeAnalysis.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expectedLines)
        {
            var sourceText = SourceText.CreateFrom(text);
            Assert.Equal(expectedLines, sourceText.Lines.Length);
        }
    }
}