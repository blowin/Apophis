using System;

namespace FunctionalProgramming.Apophis.Types.Monads.Option
{
    public static class OptionalExceptions
    {
        public static void NotFount() => throw new OptionalNotFoundValue();
        
        public sealed class OptionalNotFoundValue : Exception{}
    }
}