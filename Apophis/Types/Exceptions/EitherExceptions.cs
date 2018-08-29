using System;

namespace Apophis.Types.Exceptions
{
    public static class EitherExceptions
    {
        public sealed class EitherNullRef : Exception {}
        
        public sealed class EitherNotFound : Exception {}

        public static void NotFound()
        {
            throw new EitherNotFound();
        }
    }
}