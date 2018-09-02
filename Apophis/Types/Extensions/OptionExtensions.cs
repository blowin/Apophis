using Apophis.Types.Monads.Option;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Extensions
{
    public static class OptionalExtensions
    {
        public static Option<T, SafePolicy> ToSafeOption<T>(this T obj)
        {
            return new Option<T, SafePolicy>(obj);
        }
        
        public static Option<T, UnsafePolicy> ToUnsafeOption<T>(this T obj)
        {
            return new Option<T, UnsafePolicy>(obj);
        }
        
        public static Option<T, TCheckPolicy> ToOption<T, TCheckPolicy>(this T obj)
            where TCheckPolicy : struct, ICheckPolicy
        {
            return new Option<T, TCheckPolicy>(obj);
        }

        public static Option<T, TOthCheckPolicy> Flatten<T, TCheckPolicy, TOthCheckPolicy>(this Option<Option<T, TOthCheckPolicy>, TCheckPolicy> obj)
            where TCheckPolicy : struct, ICheckPolicy 
            where TOthCheckPolicy : struct, ICheckPolicy
        {
            return obj.FlatMap(x => x);
        }
    }
}