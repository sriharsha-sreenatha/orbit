namespace Orbit.CodeAnalysis
{
    class Parser
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
                token = lexer.NextToken();

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

        private SyntaxToken Match(SyntaxKind kind)
        {
            if(Current.Kind == kind)
                return NextToken();
            
            AddDiagWithMarker($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>", Current);
            return new SyntaxToken(kind, Current.Position, "");
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }
    
        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var eof = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, eof);
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while ( Current.Kind == SyntaxKind.PlusToken ||
                    Current.Kind == SyntaxKind.MinusToken)
            {
                var opToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, opToken, right);
            }
            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while ( Current.Kind == SyntaxKind.StarToken ||
                    Current.Kind == SyntaxKind.SlashToken)
            {
                var opToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, opToken, right);
            }
            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }
}