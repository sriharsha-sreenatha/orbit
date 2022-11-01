using System;
using System.Collections.Generic;
using Orbit.CodeAnalysis;
using Orbit.CodeAnalysis.Syntax;
using Orbit.CodeAnalysis.Binding;

namespace Orbit
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var showTree = false;
            while(true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(line) || line == "exit")
                    return;
                
                if(line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                    continue;
                }
                else if(line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);

                var diagnostics = binder.Diagnostics.Concat(syntaxTree.Diagnostics).ToArray();

                if(showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }
                
                if (!diagnostics.Any())
                {
                    var eval = new Evaluator(boundExpression);
                    var result = eval.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diag in diagnostics)
                        Console.WriteLine(diag);

                    Console.ResetColor();
                }
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // └──
            // │ 
            // ├──

            var marker = isLast ? "└── " : "├── ";

            Console.Write(indent+marker+node.Kind);

            if(node is SyntaxToken t && t.Value != null )
            {
                Console.Write(" " + t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }

}
