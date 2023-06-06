using System;
using System.Collections.Generic;
using Orbit.CodeAnalysis;
using Orbit.CodeAnalysis.Syntax;
using Orbit.CodeAnalysis.Binding;
using System.Text;
using Orbit.CodeAnalysis.Text;

namespace Orbit
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            Compilation previous = null;

            while(true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                if (textBuilder.Length == 0)
                    Console.Write("$ ");
                else
                    Console.Write("» ");
                Console.ResetColor();
                
                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if(input == "exit")
                        return;
                    else if (isBlank)
                        break;
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                        continue;
                    }
                    else if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#reset")
                    {
                        previous = null;
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;
                
                var compilation = previous == null
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);
                
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();

                    previous = compilation;
                }
                else
                {
                    foreach (var diag in diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diag.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diag.Span.Start - line.Start + 1;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"({lineNumber}:{character})");
                        Console.WriteLine(diag);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diag.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diag.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diag.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

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
                textBuilder.Clear();
            }
        }
    }

}
