namespace Orbit.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch(kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.NotToken:
                case SyntaxKind.NotKeyword:
                    return 5;
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
                    return 4;
                
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
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
    }
}