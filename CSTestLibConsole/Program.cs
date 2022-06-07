using System;
using System.Linq;
using static System.MathF;

using CSTestLib;

namespace CSTestLibConsole
{
    class Program
    {

        public static float[] LinSpace(float startVal, float endVal, int steps)
        {
          float interval = (endVal / MathF.Abs(endVal)) * MathF.Abs(endVal - startVal)/(steps -1);
          return Enumerable.Range(0, steps).Select(i => (startVal + (i*interval))).ToArray();
        }
	
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

	    KNNClassifier knn = new KNNClassifier(5);
	    Console.WriteLine($"Configured knn to have {knn.HowManyClasses()} classes.");

	    var testArray = LinSpace(-1, 1, 1000);
	    Console.WriteLine($"Created a test array of lenght [{testArray.Length}]");

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
