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

	    knn.CreateTable(testArray, 10, 100);

	    var rootPath = Environment.GetEnvironmentVariable("DATAPATH");
            Console.WriteLine($"trying with rootPath {rootPath}!");
	    //Calculator.ReadDataFiles(rootPath);
#nullable enable
	    KNNTrainBundle? bundle = Calculator.ReadDataFilesUsingDF(rootPath);
#nullable disable
	    if (bundle == null) {
	      Console.WriteLine("Error reading and parsing file");
	      return;
	    } 
	    Console.WriteLine($"Got a set of {bundle.NumFeatures} rows and {bundle.NumObservations} columns, with labels: ");
	    var labels = bundle.FlattenedTrainingSetLabels;
	    for (int i = 0; i < labels.Length; i++) {
	      Console.Write($"{labels[i]} ");
	    }
	    Console.WriteLine();

	    knn.CreateTable(bundle.FlattenedTrainingSetFeatures, bundle.NumFeatures, bundle.NumObservations);

#if false
            var calc = new Calculator();
            Console.WriteLine($"Calling library with 4 and 5: {calc.Add(4, 5)}...");
            Calculator.CSCreateTable();
            Calculator.LinRegTrain(1, 100);
#endif
            Console.WriteLine("Done!");

        }
    }
}
