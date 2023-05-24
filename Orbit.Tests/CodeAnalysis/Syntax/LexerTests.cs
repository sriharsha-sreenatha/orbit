using Orbit.CodeAnalysis.Syntax;
using Xunit;

namespace Orbit.Tests.CodeAnalysis.Syntax
{
    public class LexerTests
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_Token_Pairs(SyntaxKind t1Kind, string t1Text, 
                                            SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(t1Kind, tokens[0].Kind);
            Assert.Equal(t1Text, tokens[0].Text);

            Assert.Equal(t2Kind, tokens[1].Kind);
            Assert.Equal(t2Text, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSepData))]
        public void Lexer_Lexes_Token_Pairs_With_Sep(   SyntaxKind t1Kind, string t1Text, 
                                                        SyntaxKind sepKind, string sepText, 
                                                        SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + sepText + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(t1Kind, tokens[0].Kind);
            Assert.Equal(t1Text, tokens[0].Text);
            
            Assert.Equal(sepKind, tokens[1].Kind);
            Assert.Equal(sepText, tokens[1].Text);

            Assert.Equal(t2Kind, tokens[2].Kind);
            Assert.Equal(t2Text, tokens[2].Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens().Concat(GetSeparators()))
                yield return new object[] { t.kind, t.text};
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
                yield return new object[] { t.t1Kind, t.t1Text, t.t2Kind, t.t2Text};
        }

        public static IEnumerable<object[]> GetTokenPairsWithSepData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
                yield return new object[] { t.t1Kind, t.t1Text, t.sepKind, t.sepText, t.t2Kind, t.t2Text};
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
            {
                // definitive tokens
                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.NotToken, "!"),
                (SyntaxKind.DoubleAmpersandToken, "&&"),
                (SyntaxKind.DoublePipeToken, "||"),
                (SyntaxKind.OpenParenToken, "("),
                (SyntaxKind.CloseParenToken, ")"),
                (SyntaxKind.NotEqualsToken, "!="),
                (SyntaxKind.DoubleEqualsToken, "=="),
                (SyntaxKind.EqualsToken, "="),

                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.FalseKeyword, "false"),
                (SyntaxKind.AndKeyword, "and"),
                (SyntaxKind.OrKeyword, "or"), 
                (SyntaxKind.NotKeyword, "not"),
                
                // non-definitive tokens
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),
                (SyntaxKind.NumberToken, "455"),

                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
        {
            return new[]
            {
                // non-definitive tokens
                (SyntaxKind.WhitespaceToken, " "),
                (SyntaxKind.WhitespaceToken, "  "),
                (SyntaxKind.WhitespaceToken, "\r"),
                (SyntaxKind.WhitespaceToken, "\n"),
                (SyntaxKind.WhitespaceToken, "\r\n"),
            };
        }

        private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
        {
            var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
            var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

            if (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken)
                return true;
            
            if (t1IsKeyword && t2IsKeyword)
                return true;
            
            if (t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t1Kind == SyntaxKind.IdentifierToken && t2IsKeyword)
                return true;
            
            // 'number number'
            if (t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken)
                return true;
            
            // '!=' -> single token
            if (t1Kind == SyntaxKind.NotToken && t2Kind == SyntaxKind.EqualsToken)
                return true;
            
            // '!==' -> '!= ='
            if (t1Kind == SyntaxKind.NotToken && t2Kind == SyntaxKind.DoubleEqualsToken)
                return true;
            
            // '==' -> single token
            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            // '===' -> '== ='
            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.DoubleEqualsToken)
                return true;
            // More cases

            return false;
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text, 
                                    SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    
                    if(!RequiresSeparator(t1.kind, t2.kind))
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
            }
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text,
                                    SyntaxKind sepKind, string sepText,
                                    SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if(RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach (var s in GetSeparators())
                        {
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                        }
                    }
                }
            }
        }
    }
}