using System;
using System.Collections.Generic;
using System.Linq;
using Apophis.Types.Extensions;
using Apophis.Types.Monads;
using Apophis.Types.Monads.Either;
using Apophis.Types.Policys.Check;

namespace Apophis.Examples
{
    public static class Either
    {
        public static void MergeExample()
        {
            var e = new int[]{1, 2, 3}.ToUnsafeLeft<int[], List<int>>();
            var e2 = "hello".ToUnsafeRight<float, string>();

            IList<int> result;
            
            e.Merge(out result);
            foreach (var i in result)
            {
                i.Print();
            }
        }

        sealed class FoundDigitException : Exception
        {
            public override string ToString()
            {
                return nameof(FoundDigitException);
            }
        }
        
        public static void UsecaseErrorHandling()
        {
            // var data = "Hello my frend"; // Right(Hello my frend)

            var data = "Hello my fr1end"; // Left(FoundDigitException)

            Either<Exception, string, SafePolicy> result = string.Empty.ToSafeRight<Exception, string>();
            
            result = data.Any(char.IsDigit) ? (Either<Exception, string, SafePolicy>) new FoundDigitException() : data;
            
            result.Print();
        }

        public static void ContainExample()
        {
            var left = 10.ToSafeLeft<int, double>();
            
            "Contain left 20: ".Print("");
            left.ContainLeft(20).Print(); // false
            
            "Contain left 10: ".Print("");
            left.ContainLeft(10).Print(); // true

            "Contain right 2.2: ".Print("");
            left.ContainRight(2.2).Print(); // false
            
            "Contain 10 or 2.2: ".Print("");
            left.Contain(10, 2.2).Print(); // true
        }
    }
}