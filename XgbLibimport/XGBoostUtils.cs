using Microsoft.ML;
using Microsoft.ML.Data;

namespace XgbLibimport
{
  public static partial class Utils
  {
    public static void Test()
    {
      VBuffer<float> v = default;
      Console.WriteLine("**** Testing from library");
    }

    public static float[] LoadFeaturesFromDataView(IDataView input) {
      if (input.Schema.GetColumnOrNull("Features") == null) {
        throw new Exception("The dataview does not have needed field Features");
      }
      if (input.Schema.GetColumnOrNull("Labels") == null) {
        throw new Exception("The dataview does not have needed field Labels");
      }
      int dim = (input.Schema["Features"].Type is VectorDataViewType vt) ? vt.Size : -1;
      if (dim <= 0) {
        throw new Exception("Features field needs to be of Vector type");
      }
      // Should probably check for 'Labels' as well
      var rows = input.GetRowCount() ?? 0;

      // Should probably throw if rows is 0
      var result = new float[rows * dim];
      Span<float> resSpan = new Span<float>(result);

      var featureCol = input.Schema["Features"];
      //var labelCol = input.Schema["Labels"];

      using (var cursor = input.GetRowCursor( new[] { featureCol /*, labelCol */ })) {

	  //float labelValue = default;
  	  VBuffer<float> featureValues = default;

  	  var featureGetter = cursor.GetGetter< VBuffer<float> >(featureCol);
  	  // var labelGetter = cursor.GetGetter<float>(labelColumn);

	  int curIndex = 0;
	  while (cursor.MoveNext()) {
  	  
	    featureGetter(ref featureValues);
	    //labelGetter(ref labelValue);

	    Span<float> target = resSpan.Slice(curIndex * dim, dim);
	    featureValues.GetValues().CopyTo(target);
	    // dataLabels[curIndex] = labelValue;

	    curIndex++;
	  }

      }

      return result;
    }
  }
}

