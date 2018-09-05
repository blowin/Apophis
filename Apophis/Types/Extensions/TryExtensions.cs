using Apophis.Types.Monads;
using Apophis.Types.Monads.Try;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Extensions
{
    public static class TryExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, TCheckPolicy> ToTry<T, TCheckPolicy>(this T val)
            where TCheckPolicy : struct, ICheckPolicy => new Try<T, TCheckPolicy>(val);
    }
}