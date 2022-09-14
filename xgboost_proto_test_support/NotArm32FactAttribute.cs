using System;
using System.Runtime.InteropServices;

namespace xgboost_proto_test_support;

public class NotArm32FactAttribute : EnvironmentSpecificFactAttribute 
{
  public NotArm32FactAttribute(string skipMessage)
  : base(skipMessage)
  {
   // empty
  }

  protected override bool IsEnvironmentSupported()
  {
    return RuntimeInformation.ProcessArchitecture == Architecture.Arm;
  }
  
}