using XGBoostProto;
using xgboost_proto_test_support;

namespace xgboost_proto_test;

public class SanityTest
{
    [Fact]
    public void TestVersion()
    {
       var ver = XGBoost.Version();
       Assert.True(ver.Major >= 2);
    }

    [Fact]
    public void TestDMatrix()
    {
	const uint lim = 100;
	float[] arr = Enumerable.Range(0,(int)lim).Select(x => (float)x).ToArray();
	uint rows = 50;
	uint cols = 2;
	float[] labels = Enumerable.Range(0,(int)rows).Select(x => (float)x).ToArray();
	Assert.True(rows * cols == lim);
	DMatrix m = new DMatrix(arr, rows, cols, labels);
	Assert.True(arr.Length == lim);
	Assert.True(labels.Length == rows);
	Assert.True(m.GetNumRows() == rows);
	Assert.True(m.GetNumCols() == cols);
    }

    [Theory]
    [InlineData(10, 20)]
    [InlineData(2, 3)]
    public void TestingTheory(int a, int b)
    { 
      Console.WriteLine($"***** The values are {a}, {b}.");
      int c = a * b;
      Assert.Equal(c, a * b); 
    }

    [Fact(Skip = "Just trying out skip functionality")]
    public void TestTest()
    {
       Assert.False(true);
    }

//    [Fact]
    [NotArm32Fact("This test is disabled on ARM")]
    public void TestAttribute()
    {
       Assert.False(true);
    }

}
