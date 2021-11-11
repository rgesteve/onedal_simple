using System;

using CSPureLibSimple;

namespace CSPureLibSimpleConsoleConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
	    int testing = Evaluator.Add(3,4);
            Console.WriteLine($"Result from library: [{testing}]");
            Console.WriteLine("Done!");
        }
    }
}
