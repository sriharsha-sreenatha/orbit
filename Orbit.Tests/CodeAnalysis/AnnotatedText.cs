using System.Collections.Immutable;
using System.Text;
using Orbit.CodeAnalysis.Text;

namespace Orbit.Tests.CodeAnalysis
{
    public class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            int position = 0;

            foreach (char ch in text)
            {
                if (ch == '[')
                {
                    startStack.Push(position);
                }
                else if (ch == ']')
                {
                    if (startStack.Count == 0)
                        throw new ArgumentException("Too many ']' in text", nameof(text));
                    
                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position++;
                    textBuilder.Append(ch);
                }
            }

            if (startStack.Count != 0)
                throw new ArgumentException("Too few ']' in text", nameof(text));
            
            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        private static string Unindent(string text)
        {
            var lines = UnindentLines(text);
            return string.Join('\n', lines);
        }

        public static string[] UnindentLines(string text)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }

            int minIndent = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                int indent = line.Length - line.TrimStart().Length;
                minIndent = Math.Min(minIndent, indent);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == string.Empty)
                    continue;

                lines[i] = lines[i].Substring(minIndent);
            }

            while (lines.Count > 0 && lines[0].Length == 0)
                lines.RemoveAt(0);

            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                lines.RemoveAt(lines.Count - 1);
            
            return lines.ToArray();
        }
    }
}