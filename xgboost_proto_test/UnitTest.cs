using XGBoostProto;

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
}
