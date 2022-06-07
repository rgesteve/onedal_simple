using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime;
using Microsoft.Data.Analysis;

using Microsoft.Win32.SafeHandles;


namespace CSTestLib
{
  public class Calculator
  {

    public class KNNSampleDatapoint
    {
      [LoadColumn(0)]
      public float f0 { get; set; }

      [LoadColumn(1)]
      public float f1 { get; set; }

      [LoadColumn(2)]
      public float f2 { get; set; }

      [LoadColumn(3)]
      public float f3 { get; set; }

      [LoadColumn(4)]
      public float f4 { get; set; }

      [LoadColumn(5)]
      public int Label { get; set; }
    }

    public double Add(double a, double b)
    {
      return a * b;
    }

    private static void DisplaySpan<T>(string title, ReadOnlySpan<T> span) 
    { 
      Console.WriteLine(title); 
      for (int i = 0; i < span.Length; i++) { 
        Console.Write($"{span[i]}."); 
      } 
      Console.WriteLine(); 
    }

    private static void DisplayLinearArray<T>(string title, T[] array)
    {
      Console.WriteLine($"Displaying array of length [{array.Length}]: {title}");
      for (int i = 0; i < array.Length; i++) {
         Console.Write($"{array[i]}, ");
      }
      Console.WriteLine();
    }

    public static void ReadDataFilesUsingDF(string rootPath)
    {
    	var dataPath = Path.Join(rootPath, "k_nearest_neighbors_test.csv");
	if (!File.Exists(dataPath)) {
	  Console.WriteLine($"Cannot find specified path");
	  return;
	}
	var dataFrame = DataFrame.LoadCsv(dataPath, header: false);
	var idf = dataFrame as IDataView;
	Console.WriteLine($"[ {idf.Schema} ], with {dataFrame.Rows.Count} rows.");

	MLContext ctx = new MLContext();
	var ppl = ctx.Transforms.Concatenate("Features", "Column0", "Column1", "Column2", "Column3", "Column4")
	//.Append(ctx.Transforms.Conversion.ConvertType("Label", "Column5", DataKind.Int32)) // ML.NET claims it doesn't know how to carry this out [?!]
	.Append(ctx.Transforms.Conversion.MapValueToKey("Label", "Column5")) // not quite the same as above, returns Key<UInt32, 0-4>
	;

	// transformed dataview
	var tdv = ppl.Fit(idf).Transform(idf);
	var featuresColumn = tdv.Schema["Features"];
	var labelColumn = tdv.Schema["Label"];
	//var labelColumn = tdv.Schema["Column5"];
	var feature0Column = tdv.Schema["Column0"];

	Console.WriteLine($"The type of the (hopefully recoded) labels is {labelColumn.Type}");

	int samples = 0;
	int maxSamples = 10;

	int featureDimensionality = 5;  // should be able to get this from featuresColumn.Type
	float[] data = new float[ maxSamples * featureDimensionality];
	Span<float> dataSpan = new Span<float>(data);

	using (var cursor = tdv.GetRowCursor(new[] {feature0Column, featuresColumn, labelColumn})) {
	  float f0Value = default;
  	  VBuffer<float> featureValues = default(VBuffer<float>);

	  var f0Getter = cursor.GetGetter<float>(feature0Column);
  	  var featureGetter = cursor.GetGetter< VBuffer<float> >(featuresColumn);

	  while (cursor.MoveNext() && samples < maxSamples) {
	    f0Getter(ref f0Value);
	    Console.Write($"row {samples}: {f0Value}");
  	  
	    featureGetter(ref featureValues);
	    DisplaySpan<float>($"features at row {samples}: ", featureValues.GetValues());

	    int offset = samples * featureDimensionality;

	    /*
	    Span<float> target = new Span<float>(dataSpan, offset, featureDimensionality);
	    */
	    Span<float> target = dataSpan.Slice(offset, featureDimensionality);
	    featureValues.GetValues().CopyTo(target);
	    /*
	    float[] t = featureValues.GetValues().ToArray();
	    for (int i = 0; i < featureDimensionality; i++) {
	      Console.Write($"{t[i]}");
	    }*/

	    samples++;
	  }
	}
	DisplayLinearArray("aggregate of data", data);
    }

    public static void ReadDataFiles(string rootPath)
    {
	var dataPath = Path.Join(rootPath, "k_nearest_neighbors_test.csv");
	if (!File.Exists(dataPath)) {
	  Console.WriteLine($"Cannot find specified path");
	  return;
	}
	MLContext ctx = new MLContext();
	int maxSamples = 10;
	var dv = ctx.Data.LoadFromTextFile<KNNSampleDatapoint>(dataPath, hasHeader: false);
	var ppl = ctx.Transforms.Concatenate("Features", "f0", "f1", "f2", "f3", "f4");
	// transformed dataview
	var tdv = ppl.Fit(dv).Transform(dv);
	var featureColumn = tdv.Schema["Features"];
	var labelColumn = tdv.Schema["Label"];

	int samples = 0;
	int featureDimensionality = 5;
	float[] data = new float[ maxSamples * featureDimensionality];
	Span<float> dataSpan = new Span<float>(data);
	//data.Clear();

	Console.WriteLine($"The accumulated buffer is of length [{data.Length}]");

	using (var cursor = tdv.GetRowCursor(new[] {featureColumn, labelColumn})) {
	  VBuffer<float> featureValues = default(VBuffer<float>);
	  int labelValue = default;

	  var featureGetter = cursor.GetGetter< VBuffer<float> >(featureColumn);
  	  var labelGetter = cursor.GetGetter<int>(labelColumn);

	  while (cursor.MoveNext() && samples < maxSamples) {
	    featureGetter(ref featureValues);
	    labelGetter(ref labelValue);
	    int offset = samples * featureDimensionality;
	    //Span<float> target = new Span<float>(dataSpan, offset, featureDimensionality);
	    Span<float> target = dataSpan.Slice(offset, featureDimensionality);
	    featureValues.GetValues().CopyTo(target);
	    float[] t = featureValues.GetValues().ToArray();
	    for (int i = 0; i < featureDimensionality; i++) {
	      Console.Write($"{t[i]}");
	    }
	    Console.WriteLine($"--- {labelValue}");
	    samples++;
	    Console.WriteLine($"this features is {featureValues.IsDense} dense of {featureValues.Length} dimensionality.  Want to copy to ({offset} of length {featureValues.GetValues().ToString()})");
	  }
	}
	Console.WriteLine($"Should be working: {featureColumn.Name} with type {featureColumn.Type.ToString()} (read {samples} samples)!");
	float[] data1 = dataSpan.ToArray();
	for (int i = 0; i < maxSamples; i++) {
           Console.Write($"{i} : ");
	   for (int j = 0; j < featureDimensionality; j++) {
	      Console.Write($"{data1[i * featureDimensionality + j]}");
	   }
	   Console.WriteLine();
	}
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
    //const string libDirPath = @"/data/Documents/Snippets/onedal/first/build";
    const string libDirPath = @"/home/rgesteve/projects/onedal_simple/build";
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

  internal static class KNNInterface
  {
    public sealed class SafeKNNAlgorithmHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeKNNAlgorithmHandle()
        : base(true)
      {
        /* empty */
      }

      protected override bool ReleaseHandle()
      {
        DestroyHandle(handle);
        return true;
      }
    }

#if _WINDOWS
    const string libDirPath = @"C:\Users\rgesteve\Documents\projects\onedal_simple\build";
    const string libPath = libDirPath + @"\Debug\OneDALNative_lib.dll";
#else
    //const string libDirPath = @"/data/Documents/Snippets/onedal/first/build";
    const string libDirPath = @"/home/rgesteve/projects/onedal_simple/build";
    const string libPath = libDirPath + "/libOneDALNative_lib.so";
#endif

    [DllImport(libPath)]
    public unsafe static extern SafeKNNAlgorithmHandle CreateEngine(int numClasses);

    [DllImport(libPath)]
    private unsafe static extern void DestroyHandle(IntPtr algorithm);

    [DllImport(libPath)]
    public unsafe static extern int HowManyClasses(SafeKNNAlgorithmHandle engine);

    [DllImport(libPath)]
    public unsafe static extern float SanityCheckBlock(SafeKNNAlgorithmHandle engine, void* block, int blockSize, void* outputArray);

  }

  public sealed class KNNClassifier : IDisposable
  {
    private readonly KNNInterface.SafeKNNAlgorithmHandle _engine;
    private bool _isDisposed;

    public KNNClassifier(int numClasses)
    {
      _engine = KNNInterface.CreateEngine(numClasses);
    }

    public int HowManyClasses()
    {
      return KNNInterface.HowManyClasses(_engine);
    }

    public float SanityCheckBlock(float[] block, float[] outData)
    {
      float ret = default(float);
      unsafe {
        fixed (void* dataPtr = &block[0], outputPtr = &outData[0]) {
	  ret = KNNInterface.SanityCheckBlock(_engine, dataPtr, block.Length, outputPtr);  // not sure if I should return from inside a fixed block
	}
      }
      return ret;
    }

    public void Dispose()
    {
      if (_isDisposed) return;
      _isDisposed = true;
      _engine.Dispose();
    }
  }
}
