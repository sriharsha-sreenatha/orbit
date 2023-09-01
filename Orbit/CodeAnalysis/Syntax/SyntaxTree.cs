using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Orbit.CodeAnalysis.Text;

namespace Orbit.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }
        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.CreateFrom(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.CreateFrom(text);
            return ParseTokens(sourceText);
        }
        
        public static ImmutableArray<SyntaxToken> ParseTokens(string text, out IEnumerable<Diagnostic> diagnostics)
        {
            var sourceText = SourceText.CreateFrom(text);
            return ParseTokens(sourceText, out diagnostics);
        }
        
        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text)
        {
            return ParseTokens(text, out _);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, out IEnumerable<Diagnostic> diagnostics)
        {
            IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
            {
                while (true)
                {
                    var token = lexer.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;
                    
                    yield return token;
                }
            }
            var lexer = new Lexer(text);
            var result = LexTokens(lexer).ToImmutableArray();
            diagnostics = lexer.Diagnostics.ToImmutableArray();
            return result;
        }
    }
}