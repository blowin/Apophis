using Apophis.Types.Monads.Either;

namespace Apophis.Types.Extensions
{
    public static class EitherExtensions
    {
        public static Either<L, R> ToLeft<L, R>(this L obj)
        {
            return new Either<L, R>(obj);
        }
        
        public static Either<L, R> ToRight<L, R>(this R obj)
        {
            return new Either<L, R>(obj);
        }

        public static Either<L, R> Flatten<L, R>(this Either<Either<L, R>, R> obj)
        {
            return new Either<L, R>(obj);
        }
        
        public static Either<L, R> Flatten<L, R>(this Either<L, Either<L, R>> obj)
        {
            return new Either<L, R>(obj);
        }
        
        public static Either<L, R> Flatten<L, R>(this Either<Either<L, R>, Either<L, R>> obj)
        {
            return new Either<L, R>(obj);
        }
    }
}