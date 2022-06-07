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

	    var testArray = LinSpace(0, 1, 1000);
	    float[] outArray = new float[ testArray.Length ];
	    Console.WriteLine($"Created a test array of length [{testArray.Length}]");

	    float sumFromNative = knn.SanityCheckBlock(testArray, outArray);
    	    Console.WriteLine($"The result from calculating the sum in native code: [{sumFromNative}]");
	    Console.WriteLine("The first elements of output array are:");
	    for (int i = 0; i < 10; i++) {
	      Console.Write($"({testArray[i]}, {outArray[i]}), ");
	    }
	    Console.WriteLine();

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
