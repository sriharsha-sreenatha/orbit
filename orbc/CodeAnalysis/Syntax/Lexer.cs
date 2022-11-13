namespace Orbit.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        private List<string> _diagnostics = new List<string>();
        
        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        private void AddDiagWithMarker(string errorStr)
        {
            string diag = _text+"\n";
            for(int i=0; i<_position; i++)
                diag += " ";
            diag += "^";
            _diagnostics.Add($"{diag}\n{errorStr}\n");
        }

        public SyntaxToken Lex()
        {
            // <numbers>
            // <operators>
            // <whitespaces>
            // <parens>

            if(_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0");
            }

            if(char.IsDigit(Current))
            {
                var start = _position;
                while(char.IsDigit(Current))
                    Next();
                var len = _position - start;
                var text = _text.Substring(start, len);
                if(!int.TryParse(text, out var value))
                {
                    AddDiagWithMarker($"ERROR: The number cannot be represented by int32: '{_text}'");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if(char.IsWhiteSpace(Current))
            {
                var start = _position;
                while(char.IsWhiteSpace(Current))
                    Next();
                var len = _position - start;
                var text = _text.Substring(start, len);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text);
            }

            if(char.IsLetter(Current))
            {
                var start = _position;
                while(char.IsLetter(Current))
                    Next();
                var len = _position - start;
                var text = _text.Substring(start, len);
                var tokenKind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(tokenKind, start, text);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+");
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-");
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*");
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/");
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenToken, _position++, "(");
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenToken, _position++, ")");
                case '!':
                    if(LookAhead == '=')
                        return new SyntaxToken(SyntaxKind.NotEqualsToken, _position+=2, "!=");
                    return new SyntaxToken(SyntaxKind.NotToken, _position++, "!");
                case '&':
                    if(LookAhead == '&')
                        return new SyntaxToken(SyntaxKind.DoubleAmpersandToken, _position+=2, "&&");
                    break;
                case '|':
                    if(LookAhead == '|')
                        return new SyntaxToken(SyntaxKind.DoublePipeToken, _position+=2, "||");
                    break;
                case '=':
                    if(LookAhead == '=')
                        return new SyntaxToken(SyntaxKind.DoubleEqualsToken, _position+=2, "==");
                    break;
            }

            AddDiagWithMarker($"ERROR: Bad character input: '{Current}'");
            
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1));
        }
    }
}