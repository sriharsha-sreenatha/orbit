using Orbit.CodeAnalysis.Text;

namespace Orbit.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText _text;
        private int _position;
        private int _start;
        private object _value;
        private SyntaxKind _kind;

        private DiagnosticBag _diagnostics = new DiagnosticBag();
        
        public Lexer(SourceText text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void AddDiagWithMarker(string errorStr)
        {
            string diag = _text+"\n";
            for(int i=0; i<_position; i++)
                diag += " ";
            diag += "^";
            // _diagnostics.Add($"{diag}\n{errorStr}\n");
        }

        public SyntaxToken Lex()
        {
            // <numbers>
            // <operators>
            // <whitespaces>
            // <parens>

            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                case '*':
                    _kind = SyntaxKind.StarToken;
                    _position++;
                    break;
                case '/':
                    _kind = SyntaxKind.SlashToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenToken;
                    _position++;
                    break;
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position++;
                    break;
                case '!':
                    _position++;
                    if(Current == '=')
                    {
                        _kind = SyntaxKind.NotEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _kind = SyntaxKind.NotToken;
                    }
                    break;
                case '&':
                    if(LookAhead == '&')
                    {
                        _kind = SyntaxKind.DoubleAmpersandToken;
                        _position+=2;
                    }
                    break;
                case '|':
                    if(LookAhead == '|')
                    {
                        _kind = SyntaxKind.DoublePipeToken;
                        _position+=2;
                    }
                    break;
                case '=':
                    _position++;
                    if(Current == '=')
                    {
                        _kind = SyntaxKind.DoubleEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }
                    break;
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    {
                        ReadNumberToken();
                    }
                    break;
                case ' ': case '\t': case '\n': case '\r':
                    {
                        ReadWhitespaceToken();
                    }
                    break;
                default:
                    if(char.IsLetter(Current))
                    {
                        ReadIdentifierAndKeywordToken();
                    }
                    else if(char.IsWhiteSpace(Current))
                    {
                        ReadWhitespaceToken();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_position, Current);
                        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.ToString(_position - 1, 1));
                    }
                    break;
            }

            var len = _position - _start;
            var text = SyntaxFacts.GetText(_kind);
            if (text == null)
                text = _text.ToString(_start, len);
                        
            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadIdentifierAndKeywordToken()
        {
            while (char.IsLetter(Current))
                _position++;
            var len = _position - _start;
            var text = _text.ToString(_start, len);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }

        private void ReadWhitespaceToken()
        {
            while (char.IsWhiteSpace(Current))
                _position++;
            
            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;
            
            var len = _position - _start;
            var text = _text.ToString(_start, len);
            int value;
            if (!int.TryParse(text, out value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, len), text, typeof(int));
            }
            _kind = SyntaxKind.NumberToken;
            _value = value;
        }
    }
}