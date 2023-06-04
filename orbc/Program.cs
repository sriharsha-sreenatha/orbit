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
            var variables = new Dictionary<VariableSymbol, object>();

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
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if(showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }
                
                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {

                    foreach (var diag in diagnostics)
                    {
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(diag);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diag.Span.Start);
                        var error = line.Substring(diag.Span.Start, diag.Span.Length);
                        var suffix = line.Substring(diag.Span.End);

                        Console.Write("   ");
                        Console.Write(prefix);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                    Console.WriteLine();

                }
            }
        }
    }

}
