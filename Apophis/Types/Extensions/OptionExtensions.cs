using Apophis.Types.Monads;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Extensions
{
    public static class OptionalExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Option<T, SafePolicy> ToSafeOption<T>(this T obj) => new Option<T, SafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Option<T, UnsafePolicy> ToUnsafeOption<T>(this T obj) => new Option<T, UnsafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Option<T, TCheckPolicy> ToOption<T, TCheckPolicy>(this T obj)
            where TCheckPolicy : struct, ICheckPolicy => new Option<T, TCheckPolicy>(obj);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Option<T, TOthCheckPolicy> Flatten<T, TCheckPolicy, TOthCheckPolicy>(this Option<Option<T, TOthCheckPolicy>, TCheckPolicy> obj)
            where TCheckPolicy : struct, ICheckPolicy 
            where TOthCheckPolicy : struct, ICheckPolicy => obj.FlatMap(x => x);
    }
}