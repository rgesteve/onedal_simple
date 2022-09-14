using Xunit;

namespace xgboost_proto_test_support;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public abstract class EnvironmentSpecificFactAttribute : FactAttribute
{
  private readonly string _skipMessage;

  protected EnvironmentSpecificFactAttribute(string skipMessage)
  {
    _skipMessage = skipMessage ?? throw new ArgumentNullException(nameof(skipMessage));
  }

  public sealed override string Skip => IsEnvironmentSupported() ? null : _skipMessage;

  protected abstract bool IsEnvironmentSupported();
  
}
