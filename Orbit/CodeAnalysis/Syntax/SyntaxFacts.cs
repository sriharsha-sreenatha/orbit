namespace Orbit.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch(kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.NotToken:
                case SyntaxKind.NotKeyword:
                case SyntaxKind.TildaToken:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch(kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 5;
                
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.DoubleEqualsToken:
                case SyntaxKind.NotEqualsToken:
                case SyntaxKind.LessToken:
                case SyntaxKind.LessOrEqualsToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                    return 3;
                
                case SyntaxKind.DoubleAmpersandToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AndKeyword:
                    return 2;
                
                case SyntaxKind.DoublePipeToken:
                case SyntaxKind.PipeToken:
                case SyntaxKind.OrKeyword:
                case SyntaxKind.HatToken:
                case SyntaxKind.XorKeyword:
                    return 1;
                
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.StarEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string kind)
        {
            switch(kind)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "and":
                    return SyntaxKind.AndKeyword;
                case "or":
                    return SyntaxKind.OrKeyword;
                case "xor":
                    return SyntaxKind.XorKeyword;
                case "not":
                    return SyntaxKind.NotKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "let":
                    return SyntaxKind.LetKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds) 
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds) 
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch(kind) 
            {
                case SyntaxKind.PlusToken: 
                    return "+";
                case SyntaxKind.MinusToken: 
                    return "-";
                case SyntaxKind.SlashToken: 
                    return "/";
                case SyntaxKind.StarToken: 
                    return "*";
                case SyntaxKind.PlusEqualsToken: 
                    return "+=";
                case SyntaxKind.MinusEqualsToken: 
                    return "-=";
                case SyntaxKind.SlashEqualsToken: 
                    return "/=";
                case SyntaxKind.StarEqualsToken: 
                    return "*=";
                case SyntaxKind.NotToken: 
                    return "!";
                case SyntaxKind.HatToken:
                    return "^";
                case SyntaxKind.TildaToken:
                    return "~";
                case SyntaxKind.AmpersandToken:
                    return "&";
                case SyntaxKind.DoubleAmpersandToken: 
                    return "&&";
                case SyntaxKind.PipeToken:
                    return "|";
                case SyntaxKind.DoublePipeToken: 
                    return "||";
                case SyntaxKind.OpenParenToken: 
                    return "(";
                case SyntaxKind.CloseParenToken: 
                    return ")";
                case SyntaxKind.OpenBraceToken: 
                    return "{";
                case SyntaxKind.CloseBraceToken: 
                    return "}";
                case SyntaxKind.CommaToken: 
                    return ",";
                case SyntaxKind.NotEqualsToken: 
                    return "!=";
                case SyntaxKind.DoubleEqualsToken: 
                    return "==";
                case SyntaxKind.EqualsToken: 
                    return "=";
                case SyntaxKind.LessToken: 
                    return "<";
                case SyntaxKind.LessOrEqualsToken: 
                    return "<=";
                case SyntaxKind.GreaterToken: 
                    return ">";
                case SyntaxKind.GreaterOrEqualsToken: 
                    return ">=";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.AndKeyword:
                    return "and";
                case SyntaxKind.OrKeyword:
                    return "or";
                case SyntaxKind.XorKeyword:
                    return "xor";
                case SyntaxKind.NotKeyword:
                    return "not";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.ToKeyword:
                    return "to";
                default:
                    return null;
            }

        }
    }
}