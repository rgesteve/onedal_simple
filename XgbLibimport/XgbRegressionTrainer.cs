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
  public class XgbRegressionTrainerBase : XgbTrainerBase<XgbRegressionTrainerBase.Options>
  {

    public class Options : OptionBase {
      
      public enum EvaluateMetricType
      {
          None,
          Default,
          MeanAbsoluteError,
          RootMeanSquaredError,
          MeanSquaredError
      };

      [Argument(ArgumentType.AtMostOnce, HelpText = "Evaluation metrics.")]
      public EvaluateMetricType EvaluationMetric = EvaluateMetricType.RootMeanSquaredError;

      public override Dictionary<string, object> ToDictionary()
      {
        var res = base.ToDictionary();
        res[GetOptionName(nameof(EvaluateMetricType))] = GetOptionName(EvaluationMetric.ToString());
        return res;
      }

    }

    public int TestFieldDerived = 10;

  }
}
