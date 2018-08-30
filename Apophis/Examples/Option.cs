using Apophis.Types.Extensions;
using Apophis.Types.Monads.Option;

namespace Apophis.Examples
{
    public class Option
    {
        public static void ToString()
        {
            Optional.Some(2).Print(); // Some(2)
            
            "Hello Apophis".ToOption().ToOption().ToOption().Print(); // Some(Some(Some(Hello Apophis)))
            Optional.None<int>().Print(); // None
        }

        public static void CreateWay()
        {
            Option<string> optionNone = Optional.None<string>();
            Option<string> optionNone2 = null;
            Option<string> optionNone3 = new Option<string>();
            
            Option<int> option = 20;
            Option<int> option2 = Optional.Some(20);
            Option<int> option3 = 20.ToOption();
            Option<int> option4 = new Option<int>(20);
        }

        public static void Math()
        {
            Option<string> none = null;

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

            var someOpt = "Hello".ToOption();

            string returnMath2 = someOpt.Match(
                s => s,
                () => "world"
            ); // return "Hello"
        }

        public static void Operations()
        {
            var option1 = 2.ToOption();
            
            var option2 = Optional.Some(System.Math.PI).Map(d => d * System.Math.E);
            
            var result = option1
                .Map(i => i * 200) // 2 * 200 == 400
                .Filter(i => i > 200) // 400 > 200 
                .FlatMap(i => option2.Map(d => d * i)) // unbox value from option2
                .FoldLeft(23.2, (d, d1) => d / d1); // return 23.2 if empty, or holdValue / 23.2
            
            result.Print();
        }
    }
}