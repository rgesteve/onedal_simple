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

    [Flags]
    public enum ArgumentType
    {
        /// <summary>
        /// Indicates that this field is required. An error will be displayed
        /// if it is not present when parsing arguments.
        /// </summary>
        Required = 0x01,

        /// <summary>
        /// Only valid in conjunction with Multiple.
        /// Duplicate values will result in an error.
        /// </summary>
        Unique = 0x02,

        /// <summary>
        /// Indicates that the argument may be specified more than once.
        /// Only valid if the argument is a collection
        /// </summary>
        Multiple = 0x04,

        /// <summary>
        /// The default type for non-collection arguments.
        /// The argument is not required, but an error will be reported if it is specified more than once.
        /// </summary>
        AtMostOnce = 0x00,

        /// <summary>
        /// For non-collection arguments, when the argument is specified more than
        /// once no error is reported and the value of the argument is the last
        /// value which occurs in the argument list.
        /// </summary>
        LastOccurrenceWins = Multiple,

        /// <summary>
        /// The default type for collection arguments.
        /// The argument is permitted to occur multiple times, but duplicate
        /// values will cause an error to be reported.
        /// </summary>
        MultipleUnique = Multiple | Unique,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ArgumentAttribute : Attribute
    {
        public enum VisibilityType
        {
            Everywhere,
            CmdLineOnly,
            EntryPointsOnly
        }

        private string _shortName;
        private string _name;

        /// <summary>
        /// Allows control of command line parsing.
        /// </summary>
        /// <param name="type"> Specifies the error checking to be done on the argument. </param>
        public ArgumentAttribute(ArgumentType type)
        {
            Type = type;
            SortOrder = 150;
        }

        /// <summary>
        /// The error checking to be done on the argument.
        /// </summary>
        public ArgumentType Type { get; }

        /// <summary>
        /// The short name(s) of the argument.
        /// Set to null means use the default short name if it does not
        /// conflict with any other parameter name.
        /// Set to String.Empty for no short name.
        /// More than one short name can be separated by commas or spaces.
        /// This property should not be set for DefaultArgumentAttributes.
        /// </summary>
        public string ShortName
        {
            get => _shortName;
            set
            {
                // Contracts.Check(value == null || !(this is DefaultArgumentAttribute));
                _shortName = value;
            }
        }

        /// <summary>
        /// The help text for the argument.
        /// </summary>
        public string HelpText { get; set; }

        public bool Hide { get; set; }

        public double SortOrder { get; set; }

        public string NullName { get; set; }

        public bool IsInputFileName { get; set; }

        /// <summary>
        /// Allows the GUI or other tools to inspect the intended purpose of the argument and pick a correct custom control.
        /// </summary>
        public string Purpose { get; set; }

        public VisibilityType Visibility { get; set; }

        public string Name
        {
            get => _name;
            set { _name = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string[] Aliases
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_shortName))
                    return null;
                return _shortName.Split(',').Select(name => name.Trim()).ToArray();
            }
        }

        public bool IsRequired => ArgumentType.Required == (Type & ArgumentType.Required);

        public Type SignatureType { get; set; }
    }
}