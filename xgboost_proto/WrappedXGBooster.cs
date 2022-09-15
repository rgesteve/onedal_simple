using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XGBoostProto
{
    /// <summary>
    /// Wrapper of Booster object of XGBoost.
    /// </summary>
    internal sealed class Booster : IDisposable
    {

#if false
        private readonly bool _hasValid;
        private readonly bool _hasMetric;
#endif
	WrappedXGBoostInterface.SafeBoosterHandle _handle;

        public WrappedXGBoostInterface.SafeBoosterHandle Handle {
         get { return _handle; }
	 /* private set; */
	}
#if false
        public int BestIteration { get; set; }
#endif

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
            fixed (double* ptr = res)
                LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.BoosterGetEval(Handle, dataIdx, ref outLen, ptr));
            return res[0];
        }

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

#if false
        public InternalTreeEnsemble GetModel(int[] categoricalFeatureBoudaries)
        {
            InternalTreeEnsemble res = new InternalTreeEnsemble();
            string modelString = GetModelString();
            string[] lines = modelString.Split('\n');
            int i = 0;
            for (; i < lines.Length;)
            {
                if (lines[i].StartsWith("Tree="))
                {
                    Dictionary<string, string> kvPairs = new Dictionary<string, string>();
                    ++i;
                    while (!lines[i].StartsWith("Tree=") && lines[i].Trim().Length != 0)
                    {
                        string[] kv = lines[i].Split('=');
                        Contracts.Check(kv.Length == 2);
                        kvPairs[kv[0].Trim()] = kv[1].Trim();
                        ++i;
                    }
                    int numberOfLeaves = int.Parse(kvPairs["num_leaves"], CultureInfo.InvariantCulture);
                    int numCat = int.Parse(kvPairs["num_cat"], CultureInfo.InvariantCulture);
                    if (numberOfLeaves > 1)
                    {
                        var leftChild = Str2IntArray(kvPairs["left_child"], ' ');
                        var rightChild = Str2IntArray(kvPairs["right_child"], ' ');
                        var splitFeature = Str2IntArray(kvPairs["split_feature"], ' ');
                        var threshold = Str2DoubleArray(kvPairs["threshold"], ' ');
                        var splitGain = Str2DoubleArray(kvPairs["split_gain"], ' ');
                        var leafOutput = Str2DoubleArray(kvPairs["leaf_value"], ' ');
                        var decisionType = Str2UIntArray(kvPairs["decision_type"], ' ');
                        var defaultValue = GetDefalutValue(threshold, decisionType);
                        var categoricalSplitFeatures = new int[numberOfLeaves - 1][];
                        var categoricalSplit = new bool[numberOfLeaves - 1];
                        if (categoricalFeatureBoudaries != null)
                        {
                            // Add offsets to split features.
                            for (int node = 0; node < numberOfLeaves - 1; ++node)
                                splitFeature[node] = categoricalFeatureBoudaries[splitFeature[node]];
                        }

                        if (numCat > 0)
                        {
                            var catBoundaries = Str2IntArray(kvPairs["cat_boundaries"], ' ');
                            var catThreshold = Str2UIntArray(kvPairs["cat_threshold"], ' ');
                            for (int node = 0; node < numberOfLeaves - 1; ++node)
                            {
                                if (GetIsCategoricalSplit(decisionType[node]))
                                {
                                    int catIdx = (int)threshold[node];
                                    var cats = GetCatThresholds(catThreshold, catBoundaries[catIdx], catBoundaries[catIdx + 1]);
                                    categoricalSplitFeatures[node] = new int[cats.Length];
                                    // Convert Cat thresholds to feature indices.
                                    for (int j = 0; j < cats.Length; ++j)
                                        categoricalSplitFeatures[node][j] = splitFeature[node] + cats[j];

                                    splitFeature[node] = -1;
                                    categoricalSplit[node] = true;
                                    // Swap left and right child.
                                    int t = leftChild[node];
                                    leftChild[node] = rightChild[node];
                                    rightChild[node] = t;
                                }
                                else
                                {
                                    categoricalSplit[node] = false;
                                }
                            }
                        }
                        InternalRegressionTree tree = InternalRegressionTree.Create(numberOfLeaves, splitFeature, splitGain,
                            threshold.Select(x => (float)(x)).ToArray(), defaultValue.Select(x => (float)(x)).ToArray(), leftChild, rightChild, leafOutput,
                            categoricalSplitFeatures, categoricalSplit);
                        res.AddTree(tree);
                    }
                    else
                    {
                        InternalRegressionTree tree = new InternalRegressionTree(2);
                        var leafOutput = Str2DoubleArray(kvPairs["leaf_value"], ' ');
                        if (leafOutput[0] != 0)
                        {
                            // Convert Constant tree to Two-leaf tree, avoid being filter by TLC.
                            var categoricalSplitFeatures = new int[1][];
                            var categoricalSplit = new bool[1];
                            tree = InternalRegressionTree.Create(2, new int[] { 0 }, new double[] { 0 },
                                new float[] { 0 }, new float[] { 0 }, new int[] { -1 }, new int[] { -2 }, new double[] { leafOutput[0], leafOutput[0] },
                                categoricalSplitFeatures, categoricalSplit);
                        }
                        res.AddTree(tree);
                    }
                }
                else
                    ++i;
            }
            return res;
        }
#endif
        #region IDisposable Support
        public void Dispose()
        {
            _handle?.Dispose();
#pragma warning disable CS8625
            _handle = null;
#pragma warning restore CS8625
        }
        #endregion
    }
}
