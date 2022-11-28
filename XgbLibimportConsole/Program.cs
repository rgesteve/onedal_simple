using XgbLibimport;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime;
using Microsoft.Data.Analysis;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
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

Console.WriteLine("Done!");
