using System;
using Xunit;

using CSPureLibSimple;

namespace CSPureLibSimpleUnitTests
{
    public class EvaluatorUnitTest1
    {
        [Fact]
        public void TestEvaluator1()
        {
          int a = 2;
          int b = 2;
          int expected = 4;
          int actual = Evaluator.Add(a,b);
          Assert.Equal(expected, actual);
        }
    }
}
