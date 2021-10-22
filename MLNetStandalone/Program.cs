using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
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
            // Create a list of training data points.
            var dataPoints = GenerateRandomDataPoints(10);
            Console.WriteLine($"Generated {dataPoints.Count()} data points.");
        }
    }
}
