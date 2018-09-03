using Apophis.Types.Monads;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Extensions
{
    public static class TryExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, TCheckPolicy> ToTry<T, TCheckPolicy>(this T val)
            where TCheckPolicy : struct, ICheckPolicy => new Try<T, TCheckPolicy>(val);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, SafePolicy> ToSafeTry<T>(this T val) => new Try<T, SafePolicy>(val);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, UnsafePolicy> ToUnsafeTry<T>(this T val) => new Try<T, UnsafePolicy>(val);
    }
}