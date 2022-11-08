using XgbLibimport;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var xgbversion = XGBoost.Version();
Console.WriteLine($"testing library: {xgbversion.Major}, {xgbversion.Minor}...");
Console.WriteLine("Done!");
