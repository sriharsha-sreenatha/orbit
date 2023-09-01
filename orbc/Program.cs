using System;
using System.Collections.Generic;
using Orbit.CodeAnalysis.Binding;

namespace Orbit
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var repl = new OrbitRepl();
            repl.Run();
        }
    }

}
