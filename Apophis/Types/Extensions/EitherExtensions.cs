using Apophis.Types.Monads.Either;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Extensions
{
    public static class EitherExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, TCheckPolicy> ToLeft<L, R, TCheckPolicy>(this L obj)
            where TCheckPolicy : struct, ICheckPolicy => new Either<L, R, TCheckPolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, SafePolicy> ToSafeLeft<L, R>(this L obj) => new Either<L, R, SafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, UnsafePolicy> ToUnsafeLeft<L, R>(this L obj) => new Either<L, R, UnsafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, TCheckPolicy> ToRight<L, R, TCheckPolicy>(this R obj)
            where TCheckPolicy : struct, ICheckPolicy => new Either<L, R, TCheckPolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, SafePolicy> ToSafeRight<L, R>(this R obj) => new Either<L, R, SafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, UnsafePolicy> ToUnsafeRight<L, R>(this R obj) => new Either<L, R, UnsafePolicy>(obj);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<L, R, TCheckPolicy> Merge<Ret, L, R, TCheckPolicy>(this Either<L, R, TCheckPolicy> eith, out Ret val)
            where L : Ret
            where R : Ret
            where TCheckPolicy : struct, ICheckPolicy
        {
            val = eith.IsLeft ? (Ret) eith.LeftOrDefault() : eith.RightOrDefault();
            return eith;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckInPolicy> Flatten<TLeft, TRight, TCheckOutPolicy, TCheckInPolicy>(this Either<Either<TLeft, TRight, TCheckInPolicy>, TRight, TCheckOutPolicy> other)
            where TCheckOutPolicy : struct, ICheckPolicy 
            where TCheckInPolicy : struct, ICheckPolicy 
            => Either<TLeft, TRight, TCheckInPolicy>.Of(other);
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckInPolicy> Flatten<TLeft, TRight, TCheckOutPolicy, TCheckInPolicy>(this Either<TLeft, Either<TLeft, TRight, TCheckInPolicy>, TCheckOutPolicy> other)
        where TCheckOutPolicy : struct, ICheckPolicy 
        where TCheckInPolicy : struct, ICheckPolicy 
            => Either<TLeft, TRight, TCheckInPolicy>.Of(other);
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckHoldPolicy> Flatten<TLeft, TRight, TCheckLeftPolicy, TCheckRightPolicy, TCheckHoldPolicy>(this Either<Either<TLeft, TRight, TCheckLeftPolicy>, Either<TLeft, TRight, TCheckRightPolicy>, TCheckHoldPolicy> other)
            where TCheckLeftPolicy : struct, ICheckPolicy 
            where TCheckRightPolicy : struct, ICheckPolicy 
            where TCheckHoldPolicy : struct, ICheckPolicy 
            => Either<TLeft, TRight, TCheckHoldPolicy>.Of(other);
    }
}