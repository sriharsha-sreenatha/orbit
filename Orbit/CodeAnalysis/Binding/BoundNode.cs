using System.Reflection;

namespace Orbit.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach(var property in properties) 
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if(typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach(var child in children)
                        if (child != null)
                            yield return child;
                }
            }
        }

        public IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach(var property in properties) 
            {
                if (property.Name == nameof(Kind)
                || property.Name == nameof(BoundBinaryExpression.Operator)
                || property.Name == nameof(BoundUnaryExpression.Operator)
                || property.Name == nameof(BoundOperatorAssignmentExpression.Operator))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType)
                ||  typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;
                

                var value = property.GetValue(this);
                if (value != null)
                    yield return (property.Name, value);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            // └──
            // │ 
            // ├──
            var isToConsole = writer == Console.Out;
            
            var marker = isLast ? "└── " : "├── ";

            writer.Write(indent);

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(marker);

            WriteNode(isToConsole, writer, node);
            WriteProperties(isToConsole, writer, node);

            if(isToConsole)
                Console.ResetColor();

            writer.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);
        }

        private static void WriteProperties(bool isToConsole, TextWriter writer, BoundNode node)
        {
            bool isFirst = false;
            foreach (var p in node.GetProperties())
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    if(isToConsole) 
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    writer.Write(",");
                }
                writer.Write(" ");

                if(isToConsole) 
                    Console.ForegroundColor = ConsoleColor.Yellow;
                writer.Write(p.Name);
                
                if(isToConsole) 
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                writer.Write(" = ");
                
                if(isToConsole) 
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                writer.Write(p.Value);
            }
        }

        private static void WriteNode(bool isToConsole, TextWriter writer, BoundNode node)
        {
            if(isToConsole)
                Console.ForegroundColor = GetConsoleColor(node);
            
            var text = GetText(node);
            writer.Write(text);
            
            Console.ResetColor();
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundBinaryExpression b)
                return b.Operator.Kind.ToString() + "Expression";
            if (node is BoundUnaryExpression u)
                return u.Operator.Kind.ToString() + "Expression";
            
            return node.Kind.ToString();
        }

        private static ConsoleColor GetConsoleColor(BoundNode node)
        {
            if (node is BoundExpression)
                return ConsoleColor.Blue;
            if (node is BoundStatement)
                return ConsoleColor.Cyan;
            
            return ConsoleColor.Yellow;
        }
    }
}