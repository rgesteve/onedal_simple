//using Microsoft.ML;
using XgbLibimport;

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
}