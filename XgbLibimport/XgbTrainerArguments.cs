using System;
using System.Reflection;
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

  public interface IBoosterParameterFactory 
  {
      new BoosterParameterBase CreateComponent();
  }
    
  public abstract class BoosterParameterBase {

    private protected static Dictionary<string, string> NameMapping = new Dictionary<string, string>()
    {
           {nameof(OptionsBase.MinimumSplitGain),               "min_split_gain" },
           {nameof(OptionsBase.MaximumTreeDepth),               "max_depth"},
	   /*
           {nameof(OptionsBase.MinimumChildWeight),             "min_child_weight"},
           {nameof(OptionsBase.SubsampleFraction),              "subsample"},
           {nameof(OptionsBase.SubsampleFrequency),             "subsample_freq"},
           {nameof(OptionsBase.L1Regularization),               "reg_alpha"},
           {nameof(OptionsBase.L2Regularization),               "reg_lambda"},
	    */
    };

    public abstract string BoosterName { get; }

    public abstract class OptionsBase : IBoosterParameterFactory {
      [Argument(ArgumentType.AtMostOnce,
                HelpText = "Minimum loss reduction required to make a further partition on a leaf node of the tree. the larger, the more conservative the algorithm will be.")]
      public double MinimumSplitGain = 0;

      /// <summary>
      /// The maximum depth of a tree.
      /// </summary>
      /// <value>
      /// 0 means no limit.
      /// </value>
      [Argument(ArgumentType.AtMostOnce,
          HelpText = "Maximum depth of a tree. 0 means no limit. However, tree still grows by best-first.")]
      public int MaximumTreeDepth = 0;

      BoosterParameterBase IBoosterParameterFactory.CreateComponent() => BuildOptions();
      public abstract BoosterParameterBase BuildOptions();
    }

    private protected OptionsBase BoosterOptions;

    public BoosterParameterBase(OptionsBase options)
    {
      BoosterOptions = options;
    }

    public void UpdateParameters(Dictionary<string, object> res)
    {
      FieldInfo[] fields = BoosterOptions.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (var field in fields) {
        var attribute = field.GetCustomAttribute<ArgumentAttribute>(false);
        if (attribute == null) continue;

        var name = NameMapping.ContainsKey(field.Name) ? NameMapping[field.Name] : XGBoostInterfaceUtils.GetOptionName(field.Name);
        res[name] = field.GetValue(BoosterOptions);
      }
    }
  }

  public class GradientBooster : BoosterParameterBase {
    private const string Name = "gbtree";
    public override string BoosterName => Name;

    public class Options : OptionsBase {
      public override BoosterParameterBase BuildOptions() => new GradientBooster(this);
    }

    public GradientBooster(Options options)
            : base(options)
    {
    }
  }

  public class DartBooster : BoosterParameterBase {
    private const string Name = "dart";
    public override string BoosterName => Name;

    public class Options : OptionsBase {
      [Argument(ArgumentType.AtMostOnce, HelpText = "The drop ratio for trees. Range:(0,1).")]
      public double RateDrop = 0.1;

      [Argument(ArgumentType.AtMostOnce, HelpText = "The probability of dropping at least one tree.")]
      public int OneDrop = 0;

      public override BoosterParameterBase BuildOptions() => new DartBooster(this);
    }

    public DartBooster(Options options)
            : base(options)
    {
      BoosterOptions = options;
    }
  }
}

