namespace Orbit.CodeAnalysis
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;

        private string _text;

        private List<string> _diagnostics = new List<string>();

        public Parser(string text)
        {
            _text = text;
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if(token.Kind != SyntaxKind.WhitespaceToken
                && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        private void AddDiagWithMarker(string errorStr, SyntaxToken token)
        {
            string diag = _text+"\n";
            for(int i=0; i<token.Position; i++)
                diag += " ";
            diag += "^";
            _diagnostics.Add($"{diag}\n{errorStr}\n");
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if(index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if(Current.Kind == kind)
                return NextToken();
            
            AddDiagWithMarker($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>", Current);
            return new SyntaxToken(kind, Current.Position, "");
        }
    
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var eof = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, eof);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOpPrec = Current.Kind.GetUnaryOperatorPrecedence();
            if(unaryOpPrec != 0 && unaryOpPrec >= parentPrecedence)
            {
                var opToken = NextToken();
                var operand = ParseExpression(unaryOpPrec);
                left = new UnaryExpressionSyntax(opToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while(true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if(precedence == 0 || precedence <= parentPrecedence)
                    break;
                
                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}