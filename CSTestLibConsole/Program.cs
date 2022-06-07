using System;

using CSTestLib;

namespace CSTestLibConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

	    KNNClassifier knn = new KNNClassifier(5);
	    Console.WriteLine($"Configured knn to have {knn.HowManyClasses()} classes.");

/*
	    var rootPath = Environment.GetEnvironmentVariable("DATAPATH");
            Console.WriteLine($"trying with rootPath {rootPath}!");
	    //Calculator.ReadDataFiles(rootPath);
	    Calculator.ReadDataFilesUsingDF(rootPath);
	    */

/*
            var calc = new Calculator();
            Console.WriteLine($"Calling library with 4 and 5: {calc.Add(4, 5)}...");
            Calculator.CSCreateTable();
            Calculator.LinRegTrain(1, 100);
	    	    */
            Console.WriteLine("Done!");

        }
    }
}
