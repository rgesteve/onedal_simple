//using Microsoft.ML;
using XgbLibimport;
using Microsoft.ML.Data;

namespace XgbLibimportTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
      var xgbversion = XGBoost.Version();
      //var x = new VBuffer<float>();
      Assert.True(xgbversion.Major >= 1);
    }

    [Fact]
    public void TestInternalRegressionTree()
    {
      int[] Lte = new int[] { -1 };
      int[] Gt = new int[] { -2 };
      int[] splitFeatures = new int[] { 0 };
      float[] rawThresholds = new float[] { (float)0.5 };
      float[] defaultValueForMissing = new float[] { 0 };
      double[] leafValues = new double[] { 0.0, 0.1, 1.3 };

      var tree = new InternalRegressionTree(splitFeatures, null, null, rawThresholds, defaultValueForMissing, Lte, Gt, leafValues, null, null);
      var x = new VBuffer<float>(1, new float[] { (float)0.0});

      Assert.True(splitFeatures.Length == 1);
    }


}