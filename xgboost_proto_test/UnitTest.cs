using XGBoostProto;

namespace xgboost_proto_test;

public class SanityTest
{
    [Fact]
    public void Test1()
    {
       var ver = XGBoost.Version();
       Console.WriteLine($"The type is: {ver.GetType()}, with Major bit: {ver.Major}, {ver.Minor}.");
       Assert.True(Test.IsReady());
    }
}