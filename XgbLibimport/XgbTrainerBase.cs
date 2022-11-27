using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.ML;
#if false
using Microsoft.ML.Runtime;
#endif

namespace XgbLibimport
{
  public class XgbTrainerBase<TOptions>
    where TOptions : XgbTrainerBase<TOptions>.OptionBase {

    public class OptionBase {
//      public int NumberOfIterations = Defaults.NumberOfIterations;
      public int NumberOfIterations = 10;

      [Argument(ArgumentType.AtMostOnce, HelpText = "Maximum number of bucket bin for features.", ShortName = "mb")]
      public int MaximumBinCountPerFeature = 255;

    private BoosterParameterBase.OptionsBase _boosterParameter;
    public IBoosterParameterFactory BoosterFactory = new GradientBooster.Options();

    public BoosterParameterBase.OptionsBase Booster
    {
        get => _boosterParameter;

        set
        {
            _boosterParameter = value;
            BoosterFactory = _boosterParameter;
        }
    }

private protected static Dictionary<string, string> NameMapping = new Dictionary<string, string>()
            {
	    	       #if false
               {nameof(MinimumExampleCountPerLeaf),           "min_data_per_leaf"},
               {nameof(NumberOfLeaves),                       "num_leaves"},
	       #endif
               {nameof(MaximumBinCountPerFeature),            "max_bin" },
	       #if false
               {nameof(MinimumExampleCountPerGroup),          "min_data_per_group" },
               {nameof(MaximumCategoricalSplitPointCount),    "max_cat_threshold" },
               {nameof(CategoricalSmoothing),                 "cat_smooth" },
               {nameof(L2CategoricalRegularization),          "cat_l2" },
               {nameof(HandleMissingValue),                   "use_missing" },
               {nameof(UseZeroAsMissingValue),                "zero_as_missing" }
	       #endif
            };

    private protected string GetOptionName(string name)
    {
        if (NameMapping.ContainsKey(name))
            return NameMapping[name];
#if false
        return LightGbmInterfaceUtils.GetOptionName(name);
#else
	return "not_found";
#endif
    }

    public virtual Dictionary<string, object> ToDictionary()
    {
      Dictionary<string, object> res = new Dictionary<string, object>();

        var boosterParams = BoosterFactory.CreateComponent();
        boosterParams.UpdateParameters(res);
        res["boosting_type"] = boosterParams.BoosterName;

	#if false
        res["verbose"] = Silent ? "-1" : "1";
        if (NumberOfThreads.HasValue)
            res["nthread"] = NumberOfThreads.Value;

        res["seed"] = (Seed.HasValue) ? Seed : host.Rand.Next();
	#endif

#if false
                res[GetOptionName(nameof(MaximumBinCountPerFeature))] = MaximumBinCountPerFeature;
                res[GetOptionName(nameof(HandleMissingValue))] = HandleMissingValue;
                res[GetOptionName(nameof(UseZeroAsMissingValue))] = UseZeroAsMissingValue;
                res[GetOptionName(nameof(MinimumExampleCountPerGroup))] = MinimumExampleCountPerGroup;
                res[GetOptionName(nameof(MaximumCategoricalSplitPointCount))] = MaximumCategoricalSplitPointCount;
                res[GetOptionName(nameof(CategoricalSmoothing))] = CategoricalSmoothing;
                res[GetOptionName(nameof(L2CategoricalRegularization))] = L2CategoricalRegularization;
		#endif

                return res;
            }

    }

    public int TestField = 10;

  }
}
