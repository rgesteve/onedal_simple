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

	// TODO: Should probably return a dictionary by parsing the JSON output
	public static string BuildInfo()
	{
	    // should probably check this doesn't return an error
  	    unsafe {
	      byte* resultPtr;
  	      WrappedXGBoostInterface.XGBuildInfo(&resultPtr);
              // this uses ANSI on Windows and non-ANSI on other OSs, so use Marshal.PtrToStringUTF8 instead
              // string result = new string((sbyte*)resultPtr);
              string result = Marshal.PtrToStringUTF8((nint)resultPtr) ?? "";
	      return result;
            }
        }

	public static void DMatrixFromDV(IDataView dv)
	{
	}
    }
}