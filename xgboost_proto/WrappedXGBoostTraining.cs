using System.Collections.Generic;

namespace XGBoostProto
{
    /// <summary>
    /// Helpers to train a booster with given parameters.
    /// Follows the `xgboost.train` Python API design
    /// </summary>
    internal static class WrappedXGBoostTraining
    {
        /// <summary>
        /// Train and return a booster.
        /// </summary>
        public static Booster Train(
	#if false
	    IChannel ch, IProgressChannel pch,
	#endif
            Dictionary<string, object> parameters, DMatrix dtrain, int numIteration = 100
	    #if false
, bool verboseEval = true, int earlyStoppingRound = 0
	    #endif
	    )
        {

            // create Booster.
            Booster bst = new Booster(parameters, dtrain);

#if false
            ch.Info("Starting training.");
#endif

            int iter = 0;

            for (iter = 0; iter < numIteration; ++iter)
            {
                bst.Update(dtrain, iter);
            }

            return bst;

        }
    }
}
