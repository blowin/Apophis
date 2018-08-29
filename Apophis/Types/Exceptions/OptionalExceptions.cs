using System;

namespace Apophis.Types.Exceptions
{
    public static class OptionalExceptions
    {
        public static void NotFount()
        {
            throw new OptionalNotFoundValue();
        }
        
        public sealed class OptionalNotFoundValue : Exception{}
    }
}