using XgbLibimport;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime;
using Microsoft.Data.Analysis;

Console.WriteLine("Starting process!");
var xgbversion = XGBoost.Version();
Console.WriteLine($"testing library: {xgbversion.Major}, {xgbversion.Minor}...");
Console.WriteLine($"with buildinfo: {XGBoost.BuildInfo()}...");

Console.WriteLine("Now trying DMatrix...");

var arr = Enumerable.Range(start:0, count:120).Select(x => (float)x).ToArray();
Console.WriteLine($"Array I intend to use as input has {arr.Length} elements.");
var arrlabels = Enumerable.Range(start:0, count:10).Select(x => (float)x).ToArray();

DMatrix dmat = new DMatrix(arr, 10, 12, arrlabels);
Console.WriteLine($"DMatrix has {dmat.GetNumRows()} rows and {dmat.GetNumCols()} columns.");

var labfrommat = dmat.GetLabels();
//Console.WriteLine($"DMatrix has {dmat.GetNumRows()} rows and {dmat.GetNumCols()} columns.");
Console.WriteLine("Just tried getting labels");

Console.WriteLine("Instantiating a booster");
Booster bst = new Booster(dmat);
bst.SetParameter("objective", "reg:squarederror");

Console.WriteLine("Starting training loop");
int numBoostRound = 1;
for (int i = 0; i < numBoostRound; i++) {
    bst.Update(dmat, i);
}


Console.WriteLine($"------- Dumping the model into an internal regression tree ---------------");

bst.DumpModel();

Console.WriteLine($"------- the booster configuration is << {bst.DumpConfig()} >>");

Console.WriteLine($"------- Now looking at the options --------------");

var resopts = new XgbRegressionTrainerBase.Options();
Console.WriteLine($"Stupid check of regression options: {resopts.EvaluationMetric}");
var resoptsdict = resopts.ToDictionary();
Console.WriteLine($"Regression options have: {resoptsdict.Count} elements: ");
foreach (var kv in resoptsdict) {
    Console.WriteLine($"Key: {kv.Key} has value: [{kv.Value}]");
}

var dartopts = new XgbRegressionTrainerBase.Options { Booster = new DartBooster.Options { MaximumTreeDepth = 6 } } ;
var dartoptsdict = dartopts.ToDictionary();
Console.WriteLine($"Regression options (now trying DART) has: {dartoptsdict.Count} elements: ");
foreach (var kv in dartoptsdict) {
    Console.WriteLine($"Key: {kv.Key} has value: [{kv.Value}]");
}

Console.WriteLine("Testing constructor of tree");

{
      int[] Lte = new int[] { -1 };
      int[] Gt = new int[] { -2 };
      int[] splitFeatures = new int[] { 0 };
      float[] rawThresholds = new float[] { (float)0.5 };
      float[] defaultValueForMissing = new float[] { 0 };
      double[] leafValues = new double[] { 0.0, 0.1, 1.3 };

      var tree = new InternalRegressionTree(splitFeatures, null, null, rawThresholds, defaultValueForMissing, Lte, Gt, leafValues, null, null);
      var x = new VBuffer<float>(1, new float[] { (float)0.0});
    }

Console.WriteLine("----------------------");


Console.WriteLine("Done!");
