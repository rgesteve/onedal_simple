using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSTestLib
{
  public class Calculator
  {
    public double Add(double a, double b)
    {
      return a * b;
    }

    public static void CSCreateTable()
    {
      // There must be an easier way to do this
      int dimensionality = 3;
      int observations = 10;
      List<float> labelsList = Enumerable.Range(0, dimensionality * observations).Select(x => (float)x).ToList();

      Console.WriteLine($"Hello World, looking at lib in [{libDirPath}]");
      if (Directory.Exists(libDirPath)) {
        Console.WriteLine("The path is there, should be OK");
      } else {
        Console.WriteLine("Can't find the path, there is a problem, bailing");
        return;
      }

      Console.WriteLine($"Trying to call the external library.");
      float[] labelsArray = labelsList.ToArray();
      int result;
      unsafe {
        fixed(float* labelsPtr = &labelsArray[0]) {
          result = Native.createTable(labelsPtr, dimensionality, observations);                     
        }
      }

      Console.WriteLine($"The list contains [{labelsList.Count}] elements.");
      Console.WriteLine($"The result of calling the native function is [{result}].");
      Console.WriteLine($"Done!!");
      return;
    }

    public static void LinRegTrain(int featureCount, int rowCount)
    {
      int m = featureCount + 1;
      var betasArray = new float[m];
      /*
      List<float> labelsList = new List<float>();
      List<float> featuresList = new List<float>();
      */

      List<float> labelsList = Enumerable.Range(0, rowCount).Select(x => (float)x).ToList();
      List<float> featuresList = Enumerable.Range(0, rowCount).Select(x => (float)x).ToList();

      float[] labels = labelsList.ToArray();
      float[] features = featuresList.ToArray();

      unsafe {
        fixed (void* featuresPtr = &features[0], labelsPtr = &labels[0], betasPtr = &betasArray[0]) {
                    Native.LinearRegressionSingle(featuresPtr, labelsPtr, betasPtr, rowCount, m - 1);
        }
      }

      float bias = betasArray[0];
      var weights = new float[m - 1];
      for (int i = 1; i < m; ++i) {
        weights[i - 1] = betasArray[i];
      }
      // can we turn this into a Span?
      //var weightsBuffer = new VBuffer<float>(m - 1, weights);
    }

#if _WINDOWS
    const string libDirPath = @"C:\Users\rgesteve\Documents\projects\onedal_simple\build";
    const string libPath = libDirPath + @"\Debug\OneDALNative_lib.dll";
#else
    const string libDirPath = @"/data/Documents/Snippets/onedal/first/build";
    const string libPath = libDirPath + "/libOneDALNative_lib.so";
#endif

    internal static class Native
    {
      [DllImport(libPath, EntryPoint="createTable")]
      public unsafe static extern int createTable(float* data, int numFeatures, int numObservations);

      [DllImport(libPath, EntryPoint = "linearRegressionSingle")]
      public unsafe static extern void LinearRegressionSingle(void* features, void* labels, void* betas, int nRows, int nColumns);
    }
  }
}
