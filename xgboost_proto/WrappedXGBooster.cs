using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XGBoostProto
{
    /// <summary>
    /// Wrapper of Booster object of XGBoost.
    /// </summary>
    #if false
    internal
    #else
    public
    #endif
    class Booster : IDisposable
    {

	private bool disposed;
	private readonly IntPtr _handle;
	private const int normalPrediction = 0; // Value for the optionMask in prediction
#pragma warning disable CS0414
	private int numClass = 1;
#pragma warning restore CS0414

	public IntPtr Handle => _handle;

        public Booster(Dictionary<string, object> parameters, DMatrix trainDMatrix)
	{
	  var dmats = new [] { trainDMatrix.Handle };
	  var len = unchecked((ulong)dmats.Length);
 	  var errp = WrappedXGBoostInterface.XGBoosterCreate(dmats, len, out _handle);
	  if (errp == -1)
	  {
  	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);
	  }
	  SetParameters(parameters);
	}

        public Booster(DMatrix trainDMatrix)
	{
	  var dmats = new [] { trainDMatrix.Handle };
	  var len = unchecked((ulong)dmats.Length);
 	  var errp = WrappedXGBoostInterface.XGBoosterCreate(dmats, len, out _handle);
	  if (errp == -1)
	  {
  	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);
	  }
	}

        public void Update(DMatrix train, int iter)
        {
	   var errp = WrappedXGBoostInterface.XGBoosterUpdateOneIter(_handle, iter, train.Handle);
	   if (errp == -1)
	   {
  	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);
	   }
        }

#if false
// Should expose this interface for XGBoosterEvalOneIter
        public double EvalTrain()
        {
            return Eval(0);
        }

        public double EvalValid()
        {
            if (_hasValid)
                return Eval(1);
            else
                return double.NaN;
        }

        private unsafe double Eval(int dataIdx)
        {
            if (!_hasMetric)
                return double.NaN;
            int outLen = 0;
            double[] res = new double[1];
            fixed (double* ptr = res) {
    	        var errp = WrappedXGBoostInterface.XGBoosterEvalOneIter(_handle, iter, train.Handle, ptr); //FIXME
                // Handle, dataIdx, ref outLen, ptr));
	    }
            return res[0];
        }
#endif

#if false
    public float[] Predict(DMatrix test)
    {
      ulong predsLen;
      IntPtr predsPtr;
/*
allowed values of optionmask:
         0:normal prediction
         1:output margin instead of transformed value
         2:output leaf index of trees instead of leaf value, note leaf index is unique per tree
         4:output feature contributions to individual predictions

// using `0` for ntreeLimit means use all the trees
*/

      var errp = WrappedXGBoostInterface.XGBoosterPredict(_handle, test.Handle, 0, 0, out predsLen, out predsPtr);
	if (errp == -1)
	{
  	   string reason = WrappedXGBoostInterface.XGBGetLastError();
           throw new XGBoostDLLException(reason);
	}
      return XGBoostInterfaceUtils.GetPredictionsArray(predsPtr, predsLen);
    }
#endif

#if false
	// Should enable XGBoosterSaveModelToBuffer
        [BestFriend]
        internal unsafe string GetModelString()
        {
            int bufLen = 2 << 15;
            byte[] buffer = new byte[bufLen];
            int size = 0;
            fixed (byte* ptr = buffer)
                LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.BoosterSaveModelToString(Handle, 0, BestIteration, bufLen, ref size, ptr));
            // If buffer size is not enough, reallocate buffer and get again.
            if (size > bufLen)
            {
                bufLen = size;
                buffer = new byte[bufLen];
                fixed (byte* ptr = buffer)
                    LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.BoosterSaveModelToString(Handle, 0, BestIteration, bufLen, ref size, ptr));
            }
            byte[] content = new byte[size];
            Array.Copy(buffer, content, size);
            fixed (byte* ptr = content)
                return LightGbmInterfaceUtils.GetString((IntPtr)ptr);
        }
#endif

    public void SetParameters(Dictionary<string, object> parameters)
    {
    #if false
      // support internationalisation i.e. support floats with commas (e.g. 0,5F)
      var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

      SetParameter("max_depth", ((int)parameters["max_depth"]).ToString());
      SetParameter("learning_rate", ((float)parameters["learning_rate"]).ToString(nfi));
      SetParameter("n_estimators", ((int)parameters["n_estimators"]).ToString());
      SetParameter("silent", ((bool)parameters["silent"]).ToString());
      SetParameter("objective", (string)parameters["objective"]);
      SetParameter("booster", (string)parameters["booster"]);
      SetParameter("tree_method", (string)parameters["tree_method"]);

      SetParameter("nthread", ((int)parameters["nthread"]).ToString());
      SetParameter("gamma", ((float)parameters["gamma"]).ToString(nfi));
      SetParameter("min_child_weight", ((int)parameters["min_child_weight"]).ToString());
      SetParameter("max_delta_step", ((int)parameters["max_delta_step"]).ToString());
      SetParameter("subsample", ((float)parameters["subsample"]).ToString(nfi));
      SetParameter("colsample_bytree", ((float)parameters["colsample_bytree"]).ToString(nfi));
      SetParameter("colsample_bylevel", ((float)parameters["colsample_bylevel"]).ToString(nfi));
      SetParameter("reg_alpha", ((float)parameters["reg_alpha"]).ToString(nfi));
      SetParameter("reg_lambda", ((float)parameters["reg_lambda"]).ToString(nfi));
      SetParameter("scale_pos_weight", ((float)parameters["scale_pos_weight"]).ToString(nfi));

      SetParameter("base_score", ((float)parameters["base_score"]).ToString(nfi));
      SetParameter("seed", ((int)parameters["seed"]).ToString());
      SetParameter("missing", ((float)parameters["missing"]).ToString(nfi));
      
      SetParameter("sample_type", (string)parameters["sample_type"]);
      SetParameter("normalize_type ", (string)parameters["normalize_type"]);
      SetParameter("rate_drop", ((float)parameters["rate_drop"]).ToString(nfi));
      SetParameter("one_drop", ((int)parameters["one_drop"]).ToString());
      SetParameter("skip_drop", ((float)parameters["skip_drop"]).ToString(nfi));

      if (parameters.TryGetValue("num_class",out var value))
      {
          numClass = (int)value;
          SetParameter("num_class", numClass.ToString());
      }
      #endif
    }

    public void SetParameter(string name, string val)
    {
 	  var errp = WrappedXGBoostInterface.XGBoosterSetParam(_handle, name, val);

	  if (errp == -1)
	  {
  	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);
	  }
    }

	#region IDisposable Support
        public void Dispose()
        {
	  Dispose(true);
	  GC.SuppressFinalize(this);
        }

	protected virtual void Dispose(bool disposing)
	{
	  if (disposed)
	  {
	    return;
	  }
	  WrappedXGBoostInterface.XGBoosterFree(_handle);
	  disposed = true;
	}
        #endregion
    }
}
