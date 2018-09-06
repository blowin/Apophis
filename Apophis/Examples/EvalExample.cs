using System;
using Apophis.Types.Monads.Eval;
using Apophis.Types.Policys.Check;

namespace Apophis.Examples
{
    public class EvalExample
    {
        public static void Always()
        {
            var seed = 1;
            
            // Always call factory func, for create value
            var r = Eval.Always<int, SafePolicy>(() => seed <<= 2);
            
            r.Value.Print(); // 4
            r.Value.Print(); // 16
            r.Value.Print(); // 64
            r.Value.Print(); // 256
            r.Value.Print(); // 1024
            r.Type.Print(); // Always
        }

        public static void Later()
        {
            //Calculate value once
            var late = Eval.Later<string, SafePolicy>(() =>
            {
                "Big Calculate".Print();
                return "Hello Apophis";
            });
            
            late.Value.Print(); // Big Calulate
                                // Hello apophis
            late.Value.Print(); //Hello apophis
            late.Value.Print(); //Hello apophis
            late.Type.Print(); // Later
        }

        public static void Now()
        {
            // Calculate immediately
            var now = Eval.Now<double, UnsafePolicy>(() =>
            {
                "First call".Print();
                return Math.PI;
            }); // First call
            
            now.Value.Print(); // 3,14159265358979
            now.Value.Print(); // 3,14159265358979
            now.Type.Print(); // Now
        }

        public static void PatternMatching()
        {
            var date = new DateTime(2000, 2, 3);
            var ev = Eval.Always<long, SafePolicy>(() =>
            {
                date = date.AddDays(4);
                return date.Day;
            });

            ev.Match( // Ev is always: 7
                now: i => $"Ev is Now: {i}".Print(),
                later: i => $"Ev is later: {i}".Print(),
                always: i => $"Ev is always: {i}".Print());
            
            ev.Match( // Ev is always: 11
                now: i => $"Ev is Now: {i}".Print(),
                later: i => $"Ev is later: {i}".Print(),
                always: i => $"Ev is always: {i}".Print());
        }

        public static void Operations()
        {
            var now = Eval.Now<int, SafePolicy>(10);
            
            var calculateValue = now
                .Map(i => i - 5)
                .FlatMap(val 
                    => Eval.Later<string, SafePolicy>(() => {
                        "This is later Eval".Print();
                        return val % 2 == 0 ? "Even" : "Odd"; })
                ).Fold("Hold value is - ", (d, d1) => d + d1); // This is later Eval
            
            calculateValue.Print(); // Hold value is - Odd
        }
    }
}