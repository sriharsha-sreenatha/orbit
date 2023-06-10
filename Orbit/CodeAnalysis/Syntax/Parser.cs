using System.Collections.Generic;
using System.Collections.Immutable;
using Orbit.CodeAnalysis.Text;

namespace Orbit.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly SourceText _text;
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private int _position;


        public Parser(SourceText text)
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

            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

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
            
            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, "");
        }
    
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var eof = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(statement, eof);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.VarKeyword:
                case SyntaxKind.LetKeyword:
                    return ParseVariableDeclaration();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private StatementSyntax ParseWhileStatement()
        {
            var whileToken = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();

            return new WhileStatementSyntax(whileToken, condition, body);
        }

        private StatementSyntax ParseIfStatement()
        {
            var ifToken = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var thenStatements = ParseStatement();
            var elseClause = ParseElseClause();

            return new IfStatementSyntax(ifToken, condition, thenStatements, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
                return null;
            
            var elseToken = MatchToken(SyntaxKind.ElseKeyword);
            var elseStatements = ParseStatement();
            return new ElseClauseSyntax(elseToken, elseStatements);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBrace = MatchToken(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.EndOfFileToken
                && Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }
            var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBrace, statements.ToImmutable(), closeBrace);
        }

        private VariableDeclarationSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.VarKeyword ? SyntaxKind.VarKeyword : SyntaxKind.LetKeyword;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var expression = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifier, equalsToken, expression);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if(Peek(0).Kind == SyntaxKind.IdentifierToken
            && (Peek(1).Kind == SyntaxKind.EqualsToken)
            || (Peek(1).Kind == SyntaxKind.PlusEqualsToken)
            || (Peek(1).Kind == SyntaxKind.MinusEqualsToken)
            || (Peek(1).Kind == SyntaxKind.StarEqualsToken)
            || (Peek(1).Kind == SyntaxKind.SlashEqualsToken))
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();

                return operatorToken.Kind == SyntaxKind.EqualsToken
                        ? new AssignmentExpressionSyntax(identifierToken, operatorToken, right)
                        : new OperatorAssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOpPrec = Current.Kind.GetUnaryOperatorPrecedence();
            if(unaryOpPrec != 0 && unaryOpPrec >= parentPrecedence)
            {
                var opToken = NextToken();
                var operand = ParseBinaryExpression(unaryOpPrec);
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
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenToken:
                    return ParseParenthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanExpression();

                case SyntaxKind.NumberToken:
                    return ParseLiteralExpression();
                

                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenToken);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanExpression()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }

        private ExpressionSyntax ParseLiteralExpression()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}