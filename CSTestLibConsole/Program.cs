using System;

using CSTestLib;

namespace CSTestLibConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var calc = new Calculator();
            Console.WriteLine($"Calling library with 4 and 5: {calc.Add(4, 5)}...");
            Calculator.CSCreateTable();
            Console.WriteLine("Done!");
        }
    }
}
