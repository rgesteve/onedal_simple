using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;

// docs/samples/Microsoft.ML.Samples/Dynamic/Trainers/Regression/OnlineGradientDescent.cs

namespace MLNetStandalone
{
    class Program
    {
        // Example with label and 50 feature values. A data set is a collection of
        // such examples.
        private class DataPoint
        {
            public float Label { get; set; }
            [VectorType(50)]
            public float[] Features { get; set; }
        }

        private static IEnumerable<DataPoint> GenerateRandomDataPoints(int count, int seed = 0)
        {
            var random = new Random(seed);
            for (int i = 0; i < count; i++) {
                float label = (float)random.NextDouble();
                yield return new DataPoint {
                    Label = label,
                    // Create random features that are correlated with the label.
                    Features = Enumerable.Repeat(label, 50).Select(
                        x => x + (float)random.NextDouble()).ToArray()
                };
            }
        }

        static void Main(string[] args)
        {
            // Create a new context for ML.NET operations. It can be used for
            // exception tracking and logging, as a catalog of available operations
            // and as the source of randomness. Setting the seed to a fixed number
            // in this example to make outputs deterministic.
            var mlContext = new MLContext(seed: 0);

            // Create a list of training data points.
            var dataPoints = GenerateRandomDataPoints(100);
            Console.WriteLine($"Generated {dataPoints.Count()} data points.");

            // Convert the list of data points to an IDataView object, which is
            // consumable by ML.NET API.
            var trainingData = mlContext.Data.LoadFromEnumerable(dataPoints);

            // Define the trainer.
            var pipeline = mlContext.Regression.Trainers.OnlineGradientDescent(
                labelColumnName: nameof(DataPoint.Label),
                featureColumnName: nameof(DataPoint.Features));

            // Train the model.
            var model = pipeline.Fit(trainingData);

            LinearRegressionModelParameters modelParameters = ((ISingleFeaturePredictionTransformer<object>)model).Model as LinearRegressionModelParameters;

            Console.WriteLine($"Done, the bias is: {modelParameters.Bias}, the weights are: [ {modelParameters.Weights.First()} ]!");

        }
    }
}
