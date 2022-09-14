using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XGBoostProto
{

    public class XGBoostDLLException : Exception
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
	  return new XGBoostVersion {
	  	 Major = major,
		 Minor = minor,
		 Patch = patch
	  };
	}
    }
}