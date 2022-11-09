using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;

namespace XgbLibimport
{
    /// <summary>
    /// Wrapper of the c interfaces of XGBoost
    /// Refer to https://xgboost.readthedocs.io/en/stable/tutorials/c_api_tutorial.html to get the details.
    /// </summary>

    public static partial class WrappedXGBoostInterface
    {

        private const string DllName = "xgboost";

        [LibraryImport(DllName, EntryPoint="XGBoostVersion")]
        public static partial void XGBoostVersion(out int major, out int minor, out int patch);

        [LibraryImport(DllName, EntryPoint="XGBuildInfo")]
        public unsafe static partial int XGBuildInfo(byte** result);

        #region Error API 

        [LibraryImport(DllName, EntryPoint="XGBGetLastError", StringMarshalling = StringMarshalling.Utf8)]
        public static partial string XGBGetLastError();

        #endregion

        #region DMatrix API 

	[LibraryImport(DllName, EntryPoint="XGDMatrixCreateFromMat")]
        public static partial int XGDMatrixCreateFromMat(ReadOnlySpan<float> data, ulong nrow, ulong ncol,
                                                    float missing, out IntPtr handle);

	[LibraryImport(DllName, EntryPoint="XGDMatrixFree")]
        public static partial int XGDMatrixFree(IntPtr handle);

#if false
        [DllImport(DllName)]
        public static extern int XGDMatrixNumRow(IntPtr handle, out ulong nrows);

        [DllImport(DllName)]
        public static extern int XGDMatrixNumCol(IntPtr handle, out ulong ncols);

        [DllImport(DllName)]
        public static extern int XGDMatrixGetFloatInfo(IntPtr handle, string field,
                                                           out ulong len, out IntPtr result);

        [DllImport(DllName)]
        public static extern int XGDMatrixSetFloatInfo(IntPtr handle, string field,
                                                   IntPtr array, ulong len);
#endif
        #endregion

#if false
        #region API Booster

        [DllImport(DllName)]
        public static extern int XGBoosterCreate(IntPtr[] dmats,
                                                 ulong len, out IntPtr handle);

        [DllImport(DllName)]
        public static extern int XGBoosterFree(IntPtr handle);

        [DllImport(DllName)]
        public static extern int XGBoosterSetParam(IntPtr handle, string name, string val);

        #endregion


        #region API train
        [DllImport(DllName)]
        public static extern int XGBoosterUpdateOneIter(IntPtr bHandle, int iter,
                                                            IntPtr dHandle);

        [DllImport(DllName)]
        public static extern int XGBoosterEvalOneIter();
        #endregion

        #region API predict
        [DllImport(DllName)]
        public static extern int XGBoosterPredict(IntPtr bHandle, IntPtr dHandle,
                                                  int optionMask, int ntreeLimit, int training,
                                                  out ulong predsLen, out IntPtr predsPtr);
        #endregion

        #region API serialization
        [DllImport(DllName)]
        public static extern int XGBoosterDumpModel(IntPtr handle, string fmap, int with_stats, out int out_len, out IntPtr dumpStr);

        [DllImport(DllName)]
        public static extern int XGBoosterDumpModelEx(IntPtr handle, string fmap, int with_stats, string format, out int out_len, out IntPtr dumpStr);
        #endregion
#endif

    }

#if false
    internal static class XGBoostInterfaceUtils
    {
        /// <summary>
        /// Checks if XGBoost has a pending error message. Raises an exception in that case.
        /// </summary>
        public static void Check(int res)
        {
            if (res != 0)
            {
                string mes = WrappedXGBoostInterface.XGBGetLastError();
                throw new Exception($"XGBoost Error, code is {res}, error message is '{mes}'.");
            }
        }

        public static float[] GetPredictionsArray(IntPtr predsPtr, ulong predsLen)
        {
            var length = unchecked((int)predsLen);
            var preds = new float[length];
            for (var i = 0; i < length; i++)
            {
                var floatBytes = new byte[4];
                for (var b = 0; b < 4; b++)
                {
                    floatBytes[b] = Marshal.ReadByte(predsPtr, 4 * i + b);
                }
                preds[i] = BitConverter.ToSingle(floatBytes, 0);
            }
            return preds;
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

        /// <summary>
        /// Convert the pointer of c string to c# string.
        /// </summary>
        public static string GetString(IntPtr src)
        {
            return Marshal.PtrToStringAnsi(src);
        }
    }
#endif
}