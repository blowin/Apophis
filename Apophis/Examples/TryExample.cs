using System;
using Apophis.Types.Monads.Try;
using Apophis.Types.Policys.Check;

namespace Apophis.Examples
{
    public class TryExample
    {
        public static void StatesExample()
        {
            var okRes = IsCSharp("c#");
            PrintData(okRes);
            "".Print();
            /*
             Is ok: True
             Is error: None
             Type: Ok
             Value: Some(This is C#)
             Pattern matching: Ok: This is C#
             */
            
            var errorRes = IsCSharp("haskell");
            PrintData(errorRes);
            /*
             Is ok: False
             Is error: Some(System.InvalidCastException: Specified cast is not valid.)
             Type: Error
             Value: None
             Pattern matching: Error: System.InvalidCastException: Specified cast is not valid.
            */
        }

        private static void PrintData(Try<string, SafePolicy> tryObj)
        {
            $"Is ok: {tryObj.IsOk}".Print();
            $"Is error: {tryObj.Error}".Print();
            $"Type: {tryObj.Type}".Print();
            $"Value: {tryObj.Value}".Print();
            "Pattern matching: ".Print(string.Empty);
            tryObj.Match(
                ok: s => $"Ok: {s}".Print(),
                error: err => $"Error: {err}".Print()
            );
            
            // support default operations - map, flatmap, filter, contain, fold and etc
        }

        private static Try<string, SafePolicy> IsCSharp(string funcLang)
        {
            if(string.Compare(funcLang, "C#", StringComparison.CurrentCultureIgnoreCase) != 0)
                return Try.FromSafe<string, InvalidCastException>();

            return Try.FromSafe("This is C#");
        }
    }
}