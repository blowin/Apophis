using Apophis.Types.Extensions;
using Apophis.Types.Monads;
using Apophis.Types.Policys.Check;

namespace Apophis.Examples
{
    public class Option
    {
        public static void ToString()
        {
            Optional.SomeSafe(2).Print(); // Some(2)
            
            "Hello Apophis".ToSafeOption().ToSafeOption().ToSafeOption().Print(); // Some(Some(Some(Hello Apophis)))
            Optional.NoneSafe<int>().Print(); // None
        }

        public static void CreateWay()
        {
            Option<string, SafePolicy> optionNone = Optional.NoneSafe<string>();
            Option<string, SafePolicy> optionNone2 = null;
            Option<string, UnsafePolicy> optionNone3 = new Option<string, UnsafePolicy>();
            
            Option<int, SafePolicy> option = 20;
            Option<int, UnsafePolicy> option2 = Optional.SomeUnsafe(20);
            Option<int, SafePolicy> option3 = 20.ToSafeOption();
            Option<int, SafePolicy> option4 = new Option<int, SafePolicy>(20);
        }

        public static void Math()
        {
            Option<string, SafePolicy> none = null;

            none.Match( // Option is empty
                some: s => $"Hold value is: {s}".Print(), 
                none: () => "Option is empty".Print()
            );

            none.MatchNone(() => "Empty".Print()); // print Empty
            none.MatchSome(s => $"Has {s}".Print()); // not print

            string returnMath = none.Match(
                s => s, 
                () => "default value"
            ); // return "default value"

            var someOpt = "Hello".ToUnsafeOption();

            string returnMath2 = someOpt.Match(
                s => s,
                () => "world"
            ); // return "Hello"
        }

        public static void Operations()
        {
            var option1 = 2.ToSafeOption();
            
            var option2 = Optional.SomeUnsafe(System.Math.PI).Map(d => d * System.Math.E);
            
            var result = option1
                .Map(i => i * 200) // 2 * 200 == 400
                .Filter(i => i > 200) // 400 > 200 
                .FlatMap(i => option2.Map(d => d * i)) // unbox value from option2
                .FoldLeft(23.2, (d, d1) => d / d1); // return 23.2 if empty, or holdValue / 23.2
            
            result.Print();
        }
    }
}