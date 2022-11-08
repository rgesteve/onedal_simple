using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if false
using Microsoft.ML.Runtime;
#endif

namespace XgbLibimport
{

#pragma warning disable MSML_GeneralName
    public class XGBoostDLLException : Exception
#pragma warning restore MSML_GeneralName
    {
        public XGBoostDLLException()
        {
            /* empty */
        }

        public XGBoostDLLException(string message)
          : base(message)
        {
            /* empty */
        }

    }

    public static class XGBoost
    {
        public struct XGBoostVersion
        {
            public int Major;
            public int Minor;
            public int Patch;
        }

        public static XGBoostVersion Version()
        {
            int major, minor, patch;
            WrappedXGBoostInterface.XGBoostVersion(out major, out minor, out patch);
            return new XGBoostVersion
            {
                Major = major,
                Minor = minor,
                Patch = patch
            };
        }

	public static string BuildInfo()
	{
	  // should probably check this doesn't return an error
          WrappedXGBoostInterface.XGBuildInfo(out string result);
	  return result;
	}
    }
}