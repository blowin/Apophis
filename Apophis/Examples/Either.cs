using System;
using System.Collections.Generic;
using System.Linq;
using Apophis.Types.Extensions;
using Apophis.Types.Monads.Either;

namespace Apophis.Examples
{
    public static class Either
    {
        public static void MergeExample()
        {
            var e = new int[]{1, 2, 3}.ToLeft<int[], List<int>>();
            var e2 = "hello".ToRight<float, string>();

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

            Either<Exception, string> result = string.Empty.ToRight<Exception, string>();
            
            result = data.Any(char.IsDigit) ? (Either<Exception, string>) new FoundDigitException() : data;
            
            result.Print();
        }

        public static void ContainExample()
        {
            var left = 10.ToLeft<int, double>();
            
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