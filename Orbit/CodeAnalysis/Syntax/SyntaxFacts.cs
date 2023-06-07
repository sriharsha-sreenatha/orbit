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
                    return 3;
                
                case SyntaxKind.DoubleAmpersandToken:
                case SyntaxKind.AndKeyword:
                    return 2;
                
                case SyntaxKind.DoublePipeToken:
                case SyntaxKind.OrKeyword:
                    return 1;
                
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
                case "not":
                    return SyntaxKind.NotKeyword;
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
                case SyntaxKind.NotToken: 
                    return "!";
                case SyntaxKind.DoubleAmpersandToken: 
                    return "&&";
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
                case SyntaxKind.NotEqualsToken: 
                    return "!=";
                case SyntaxKind.DoubleEqualsToken: 
                    return "==";
                case SyntaxKind.EqualsToken: 
                    return "=";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.AndKeyword:
                    return "and";
                case SyntaxKind.OrKeyword:
                    return "or";
                case SyntaxKind.NotKeyword:
                    return "not";
                default:
                    return null;
            }

        }
    }
}