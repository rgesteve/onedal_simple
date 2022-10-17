using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace XGBoostProto
{
    /// <summary>
    /// Wrapper of the c interfaces of XGBoost
    /// Refer to https://xgboost.readthedocs.io/en/stable/tutorials/c_api_tutorial.html to get the details.
    /// </summary>
    #if false
    internal
    #else
    public
    #endif
    static class WrappedXGBoostInterface
    {
#if false
        public enum CApiDType : int
        {
            Float32 = 0,
            Float64 = 1,
            Int32 = 2,
            Int64 = 3
        }

        public enum CApiPredictType : int
        {
            Normal = 0,
            Raw = 1,
            LeafIndex = 2,
        }
#endif

#if false
        private const string DllName = "xgboost";
#else
        //private const string DllName = "/home/rgesteve/projects/xgboost/build/install/lib/libxgboost.so";
        private const string DllName =  @"C:\Users\perf\projects\xgboost\build\install\bin\xgboost.dll";
#endif

	[DllImport(DllName)]
        public static extern void XGBoostVersion(out int major, out int minor, out int patch);

        #region Error API 

	[DllImport(DllName)]
	public static extern string XGBGetLastError();

        #endregion

        #region DMatrix API 

        public sealed class SafeDMatrixHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeDMatrixHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                XGBoostInterfaceUtils.Check(XGDMatrixFree(handle));
                return true;
            }
        }

    	[DllImport(DllName)]
	public static extern int XGDMatrixCreateFromMat(float[] data, ulong nrow, ulong ncol, 
                                                    float missing, out SafeDMatrixHandle handle);

	[DllImport(DllName)]
	public static extern int XGDMatrixFree(IntPtr handle);

	[DllImport(DllName)]
	public static extern int XGDMatrixNumRow(SafeDMatrixHandle handle, out ulong nrows);

	[DllImport(DllName)]
	public static extern int XGDMatrixNumCol(SafeDMatrixHandle handle, out ulong ncols);

	[DllImport(DllName)]
    	public static extern int XGDMatrixGetFloatInfo(SafeDMatrixHandle handle, string field, 
                                                       out ulong len, out IntPtr result);

        [DllImport(DllName)]
	public static extern int XGDMatrixSetFloatInfo(SafeDMatrixHandle handle, string field,
                                                   IntPtr array, ulong len);
        #endregion


        #region API Booster

        public sealed class SafeBoosterHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeBoosterHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                XGBoostInterfaceUtils.Check(XGBoosterFree(handle));
                return true;
            }
        }

	[DllImport(DllName)]
	public static extern int XGBoosterCreate(SafeDMatrixHandle[] dmats, 
                                             ulong len, out SafeBoosterHandle handle);

	[DllImport(DllName)]
	public static extern int XGBoosterFree(IntPtr handle);

        #endregion


        #region API train
	[DllImport(DllName)]
	public static extern int XGBoosterUpdateOneIter(SafeBoosterHandle bHandle, int iter, 
                                                        SafeDMatrixHandle dHandle);

#if false
        [DllImport(DllName, EntryPoint = "LGBM_BoosterGetEvalCounts", CallingConvention = CallingConvention.StdCall)]
        public static extern int BoosterGetEvalCounts(SafeBoosterHandle handle, ref int outLen);

        [DllImport(DllName, EntryPoint = "LGBM_BoosterGetEval", CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int BoosterGetEval(SafeBoosterHandle handle, int dataIdx,
                                 ref int outLen, double* outResult);
#endif
        #endregion

        #region API predict
	[DllImport(DllName)]
	public static extern int XGBoosterPredict(SafeBoosterHandle bHandle, SafeDMatrixHandle dHandle, 
                                              int optionMask, int ntreeLimit, 
                                              out ulong predsLen, out IntPtr predsPtr);
        #endregion

    }

    internal static class XGBoostInterfaceUtils
    {
        /// <summary>
        /// Checks if LightGBM has a pending error message. Raises an exception in that case.
        /// </summary>
        public static void Check(int res)
        {
            if (res != 0)
            {
                string mes = WrappedXGBoostInterface.XGBGetLastError();
                throw new Exception($"XGBoost Error, code is {res}, error message is '{mes}'.");
            }
        }

#if false
        /// <summary>
        /// Join the parameters to key=value format.
        /// </summary>
        public static string JoinParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null)
                return "";
            List<string> res = new List<string>();
            foreach (var keyVal in parameters)
                res.Add(keyVal.Key + "=" + string.Format(CultureInfo.InvariantCulture, "{0}", keyVal.Value));
            return string.Join(" ", res);
        }

        /// <summary>
        /// Helper function used for generating the LightGbm argument name.
        /// When given a name, this will convert the name to lower-case with underscores.
        /// The underscore will be placed when an upper-case letter is encountered.
        /// </summary>
        public static string GetOptionName(string name)
        {
            // Otherwise convert the name to the light gbm argument
            StringBuilder strBuf = new StringBuilder();
            bool first = true;
            foreach (char c in name)
            {
                if (char.IsUpper(c))
                {
                    if (first)
                        first = false;
                    else
                        strBuf.Append('_');
                    strBuf.Append(char.ToLower(c));
                }
                else
                    strBuf.Append(c);
            }
            return strBuf.ToString();
        }
#endif

        /// <summary>
        /// Convert the pointer of c string to c# string.
        /// </summary>
        public static string GetString(IntPtr src)
        {
            return Marshal.PtrToStringAnsi(src);
        }
    }
}
