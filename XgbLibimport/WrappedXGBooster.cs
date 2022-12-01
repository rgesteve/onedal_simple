using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Text;

#if false
using Microsoft.ML.Runtime;

using Microsoft.ML.Trainers.FastTree;
#endif

using Microsoft.ML;
#if false
using Microsoft.ML.CommandLine;
using Microsoft.ML.Data;
using Microsoft.ML.EntryPoints;
using Microsoft.ML.Internal.CpuMath;
using Microsoft.ML.Internal.Utilities;
//using Microsoft.ML.Model.OnnxConverter;
using Microsoft.ML.Numeric;
using Microsoft.ML.Transforms;
//#endif
#endif

namespace XgbLibimport
{

    /// <summary>
    /// Wrapper of Booster object of XGBoost.
    /// </summary>
    public class Booster : IDisposable
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
            var dmats = new[] { trainDMatrix.Handle };
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
            var dmats = new[] { trainDMatrix.Handle };
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

        public unsafe void DumpAttributes()
        {
	    ulong attrs_len;
	    byte** attrs;
            var errp = WrappedXGBoostInterface.XGBoosterGetAttrNames(_handle, out attrs_len, out attrs);
            if (errp == -1)
            {
                string reason = WrappedXGBoostInterface.XGBGetLastError();
                throw new XGBoostDLLException(reason);
            }
	    Console.WriteLine($"**** Got {attrs_len} attributes in booster.");
        }

	public unsafe void DumpModel()
	{	
	    ulong boosters_len;
	    byte** booster_raw_arr;
            var errp = WrappedXGBoostInterface.XGBoosterDumpModelEx(_handle, "", 0, "json", out boosters_len, out booster_raw_arr);
            if (errp == -1)
            {
                string reason = WrappedXGBoostInterface.XGBGetLastError();
                throw new XGBoostDLLException(reason);
            }

	    var result = new string[boosters_len];
	    var boosterPattern = @"^booster\[\d+\]";

	    for (ulong i = 0; i < boosters_len; ++i) {
            	result[i] = Marshal.PtrToStringUTF8((nint)booster_raw_arr[i]) ?? "";
		Console.WriteLine($"**** Trying to parse booster {i}, which is {result[i]}");
#if false
 		var doc = JsonDocument.Parse(result[i]);
                TreeNode t = TreeNode.Create(doc.RootElement);
                ensemble.Add(t);
#else
		Console.WriteLine($"**** Calling the TablePopulator on booster {i}..");
                var table = new TablePopulator(result[i]);
		var arrs = table.Sequentialize();
		//Console.WriteLine($"**** Booster {i} has an element of type: {doc.RootElement.ValueKind}.");
		Console.WriteLine($"**** I coud get {arrs.Item1.Length} arrays from Booster {i}.");
#endif
	    }

    	    Console.WriteLine($"**** The length of the boosters are {result.Length}.");
       	    //Console.WriteLine($"**** The first booster is {result[0]}.");

	}

	public string DumpConfig()
	{	
	    ulong config_len;
	    string result = default;
	    unsafe {
    	      byte* config_result;
              var errp = WrappedXGBoostInterface.XGBoosterSaveJsonConfig(_handle, out config_len, &config_result);
              if (errp == -1)
              {
                  string reason = WrappedXGBoostInterface.XGBGetLastError();
                  throw new XGBoostDLLException(reason);
              }
    	      result = Marshal.PtrToStringUTF8((nint)config_result) ?? "";
	    }
	    return result;

	}

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

            var errp = WrappedXGBoostInterface.XGBoosterPredict(_handle, test.Handle, 0, 0, 0, out predsLen, out predsPtr);
            if (errp == -1)
            {
                string reason = WrappedXGBoostInterface.XGBGetLastError();
                throw new XGBoostDLLException(reason);
            }
            return XGBoostInterfaceUtils.GetPredictionsArray(predsPtr, predsLen);
        }

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
#endif

        public void SetParameters(Dictionary<string, object> parameters)
        {
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

            if (parameters.TryGetValue("num_class", out var value))
            {
                numClass = (int)value;
                SetParameter("num_class", numClass.ToString());
            }

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


        #region Create Models

	public class XGBNode
	{
	    public int nodeid { get; set; }
	}

	public class XGBNodeLeaf : XGBNode
	{
	    public float leaf { get; set; }
	}

	public class XGBNodeSplit : XGBNode
	{
	    public int depth { get; set; }
	    public int split { get; set; }
	    public float split_condition { get; set; }
	    public int yes { get; set; }
	    public int no { get; set; }
	    public float missing { get; set; }
	}

class TablePopulator
        {
            public Dictionary<int, XGBNodeLeaf> leaves = new();
            public Dictionary<int, XGBNodeSplit> decisions = new();
            
            public TablePopulator(string jsonFragment)
            {
                PopulateTable(JsonDocument.Parse(jsonFragment).RootElement);
            }

            public void PopulateTable(JsonElement elm)
            {
                int nodeId = default;
                if (elm.TryGetProperty("nodeid", out JsonElement nodeidElm))
                {
                    // If this test fails, should probably bail, as the syntax of the booster is incorrect
                    nodeId = nodeidElm.GetInt32();
                }

                if (elm.TryGetProperty("leaf", out JsonElement leafJsonNode))
                {
                    leaves.Add(nodeId, new XGBNodeLeaf { nodeid = nodeId, leaf = leafJsonNode.GetSingle() });
                }
                else if (elm.TryGetProperty("children", out JsonElement internalJsonNode))
                {
                    var node = new XGBNodeSplit { nodeid = nodeId };
                    decisions.Add(nodeId, node);
                    if (elm.TryGetProperty("yes", out JsonElement yesNodeId))
                    {
                        node.yes = yesNodeId.GetInt32();
                    }

                    if (elm.TryGetProperty("no", out JsonElement noNodeId))
                    {
                        node.no = noNodeId.GetInt32();
                    }
                                        
                    // TODO: missing "missing"
                    if (elm.TryGetProperty("split", out JsonElement splitFeature))
                    {
                        var candidate = splitFeature.GetString();
                        if (Regex.IsMatch(candidate, "f[0-9]+")) {
                            if (int.TryParse(candidate.Substring(1), out int splitFeatureIndex)) {
                              node.split = splitFeatureIndex;
                            }
                        }
                    }
                    
                    if (elm.TryGetProperty("split_condition", out JsonElement splitThreshold))
                    {
                        node.split_condition = splitThreshold.GetSingle();
                    }
                    

                    foreach (var e in internalJsonNode.EnumerateArray())
                    {
                        PopulateTable(e);
                    }
                }
                else
                {
                    throw new Exception("Invalid booster content");
                }
            }

	    // TODO: Maybe this should return an InternalRegressionTree
            public (int[], int[]) Sequentialize()
            {
                int nextNode = 0;
                int nextLeaf = 1;
                Dictionary<int, int> mapNodes = new(); // internal nodes original id-to-seq id
                Dictionary<int, int> mapLeaves = new(); // leaves original id-to-seq-id map
                
                foreach(var n in decisions) {
                  if (!mapNodes.ContainsKey(n.Key)) {
                      mapNodes.Add(n.Key, nextNode++);
                  }                  
                }
                foreach(var n in leaves) {
                  if (!mapLeaves.ContainsKey(n.Key)) {
                      mapLeaves.Add(n.Key, nextLeaf++);
                  }                  
                }
                
                int[] lte = new int[mapNodes.Count];
                int[] gt = new int[mapNodes.Count];
                int[] splitFeatures = new int[mapNodes.Count];
                float[] rawThresholds = new float[mapNodes.Count];
		double[] leafValues = new double[mapLeaves.Count];

		// TODO: Can this be done with LINQ in a better way?
                foreach(var n in decisions) {
                  if (leaves.ContainsKey(n.Value.yes)) {
                    lte [ mapNodes[n.Key] ] = -mapLeaves[n.Value.yes];
                  } else {
                    lte [ mapNodes[n.Key] ] = mapNodes[n.Value.yes];
                  }
                  
                  if (leaves.ContainsKey(n.Value.no)) {
                    gt [ mapNodes[n.Key] ] = -mapLeaves[n.Value.no];
                  } else {
                    gt [ mapNodes[n.Key] ] = mapNodes[n.Value.no];
                  }
		  splitFeatures[ mapNodes[n.Key] ] = n.Value.split;
  		  rawThresholds[ mapNodes[n.Key] ] = n.Value.split_condition;
		  // TODO: The rest
                }

		foreach(var l in leaves) {
		  leafValues [ mapLeaves[l.Key] - 1 ] = l.Value.leaf;
		}

		Console.WriteLine($"----------------- running constraints -------------------");
		Console.WriteLine($"Number of leaves: {leaves.Count}.");
		Console.WriteLine($"Size of lte: [{lte.Length}] ");
		Console.WriteLine($"LTE: [{lte}] ");		
		Console.WriteLine($"Size of gt: [{gt.Length}]");
		
		var tree = InternalRegressionTree.Create(leaves.Count,
		splitFeatures,
		null, // double[] splitGain
		rawThresholds,
		null, // float[] defaultValueForMissing
		lte,
		gt,
		null, // double[] leafValues
		null, // int[][] categoricalSplitFeatures
		null // bool[] categoricalSplit
        );

                return (lte, gt);
            }
        }

#if false
        public string[] DumpModelEx(string fmap, int with_stats, string format)
        {
            int length;
            IntPtr treePtr;
            var intptrSize = IntPtr.Size;

            WrappedXGBoostInterface.XGBoosterDumpModelEx(_handle, fmap, with_stats, format, out length, out treePtr);

            var trees = new string[length];
            int readSize = 0;
            var handle2 = GCHandle.Alloc(treePtr, GCHandleType.Pinned);

            //iterate through the length of the tree ensemble and pull the strings out from the returned pointer's array of pointers. prepend python's api convention of adding booster[i] to the beginning of the tree
            for (var i = 0; i < length; i++)
            {
                var ipt1 = Marshal.ReadIntPtr(Marshal.ReadIntPtr(handle2.AddrOfPinnedObject()), intptrSize * i);
                string s = Marshal.PtrToStringAnsi(ipt1);
                trees[i] = string.Format("booster[{0}]\n{1}", i, s);
                var bytesToRead = (s.Length * 2) + IntPtr.Size;
                readSize += bytesToRead;
            }
            handle2.Free();
            return trees;
        }

        public void GetModel()
        {
            var ss = DumpModelEx("", with_stats: 0, format: "json");
            var boosterPattern = @"^booster\[\d+\]";
#if false
            List<TreeNode> ensemble = new List<TreeNode>(); // should probably return this
#endif

            for (int i = 0; i < ss.Length; i++)
            {
                var m = Regex.Matches(ss[i], boosterPattern, RegexOptions.IgnoreCase);
                if ((m.Count >= 1) && (m[0].Groups.Count >= 1))
                {
                    // every booster representation should match
                    var structString = ss[i].Substring(m[0].Groups[0].Value.Length);
                    var doc = JsonDocument.Parse(structString);
#if false
                    TreeNode t = TreeNode.Create(doc.RootElement);
                    ensemble.Add(t);
#else
                    //var table = new TablePopulator(doc);
#endif
                }
            }
        }

        private class TablePopulator
        {
            public Dictionary<int, List<JsonElement>> dict = new(); // nodes in level (root is level 0)
            public Dictionary<string, int> nodes = new(); // nodes-to-level
            public Dictionary<string, string> lte = new(); // node-to-their-left-branch
            public Dictionary<string, string> gt = new(); // node-to-their-right-branch

            public TablePopulator(JsonElement elm)
            {
                PopulateTable(elm, 0);
            }

            public void PopulateTable(JsonElement elm, int level)
            {
                string nodeId = "";
                if (elm.TryGetProperty("nodeid", out JsonElement nodeidElm))
                {
                    // If this test fails, should probably bail, as the syntax of the booster is incorrect
                    nodeId = nodeidElm.ToString();
                    nodes.Add(nodeId, level);
                }

                if (!dict.ContainsKey(level))
                {
                    dict.Add(level, new List<JsonElement>());
                }

                if (elm.TryGetProperty("leaf", out JsonElement leafJsonNode))
                {
                    dict[level].Add(elm);
                }
                else if (elm.TryGetProperty("children", out JsonElement internalJsonNode))
                {
                    dict[level].Add(elm);
                    if (elm.TryGetProperty("yes", out JsonElement yesNodeId))
                    {
                        lte.Add(nodeId, yesNodeId.ToString());
                    }

                    if (elm.TryGetProperty("no", out JsonElement noNodeId))
                    {
                        gt.Add(nodeId, noNodeId.ToString());
                    }

                    foreach (var e in internalJsonNode.EnumerateArray())
                    {
                        PopulateTable(e, level + 1);
                    }
                }
                else
                {
                    throw new Exception("Invalid booster content");
                }
            }
        }
#endif
        #endregion

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

    public static partial class Utils 
    {
	public const string Version = "version 0.0.1";

	public class DictionaryStringObjectConverter : JsonConverter< Dictionary<string, object> >
        {
            public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, options);
            }

            public override Dictionary<string, object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException($"JsonTokenType is not StartObject. Token type is {reader.TokenType}");
                }
                var dict = new Dictionary<string, object>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dict;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("JsonTokenType is not PropertyName");
                    }

                    var propertyName = reader.GetString();

                    if (string.IsNullOrWhiteSpace(propertyName))
                    {
                        throw new JsonException("Property name is null or empty");
                    }
                    reader.Read();
                    dict.Add(propertyName, ExtractValue(ref reader, options));
                }

                return dict;
            }

            private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        if (reader.TryGetDateTime(out DateTime dateTime))
                        {
                            return dateTime;
                        }
                        return reader.GetString();
                    case JsonTokenType.False:
                        return false;
                    case JsonTokenType.True:
                        return true;
                    case JsonTokenType.Null:
                        return null;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt64(out long int64))
                        {
                            return int64;
                        }
                        return reader.GetDecimal();
                    case JsonTokenType.StartObject:
                        return Read(ref reader, null, options);
                    case JsonTokenType.StartArray:
                        var list = new List<object>();
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            list.Add(ExtractValue(ref reader, options));
                        }
                        return list;
                    default:
                        throw new JsonException($"Unexpected token type {reader.TokenType}");
                }
            }
        }

	public static Dictionary<string, object> ParseBoosterConfig(string boosterConfig)
	{
	  var options = new JsonSerializerOptions
          {
             Converters = { new Utils.DictionaryStringObjectConverter() }
	  };
	  return JsonSerializer.Deserialize<Dictionary<string, object>>(boosterConfig, options);
	}
    }

}
