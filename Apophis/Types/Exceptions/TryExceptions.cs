using System;

namespace Apophis.Types.Exceptions
{
    public class TryExceptions
    {
        public static void NotFount()
        {
            throw new TryNotFoundValue ();
        }
        
        public sealed class TryNotFoundValue : Exception{}
    }
}