using System.Collections.Immutable;

namespace Orbit.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string _text;
        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }

        public static SourceText CreateFrom(string text)
        {
            return new SourceText(text);
        }

        public char this[int index] => _text[index];

        public int Length => _text.Length;

        public int GetLineIndex(int position)
        {
            var low = 0;
            var high = Lines.Length - 1;

            while (low <= high)
            {
                var mid = low + (high-low)/2;
                var lineStart = Lines[mid].Start;
                if (position == lineStart)
                    return mid;
                
                if (lineStart > position)
                    high = mid - 1;
                else
                    low = mid + 1;
            }
            return low-1;
        }

        public ImmutableArray<TextLine> Lines { get; }
        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var position = 0;
            var lineStart = 0;

            while (position < text.Length)
            {
                var linebreakLength = GetLineBreakLength(text, position);

                if (linebreakLength == 0)
                {
                    position++;
                }
                else
                {
                    AddTextLine(sourceText, result, position, lineStart, linebreakLength);
                    position += linebreakLength;
                    lineStart = position;
                }
            }
            if (position >= lineStart)
                AddTextLine(sourceText, result, position, lineStart, 0);

            return result.ToImmutable();
        }

        private static void AddTextLine(SourceText sourceText, ImmutableArray<TextLine>.Builder result, int position, int lineStart, int linebreakLength)
        {
            var lineLength = position - lineStart;
            var lineLengthWithLinebreak = lineLength + linebreakLength;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthWithLinebreak);
            result.Add(line);
        }

        private static int GetLineBreakLength(string text, int position)
        {
            char current = text[position];
            char next = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (current == '\r' && next == '\n')
                return 2;
            if (current == '\r' || current == '\n')
                return 1;
            return 0;
        }

        public override string ToString() => _text;

        public string ToString(int start, int length) => _text.Substring(start, length);

        public string ToString(TextSpan span) => ToString(span.Start, span.Length);
    }
}