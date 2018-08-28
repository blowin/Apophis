using System;

namespace FunctionalProgramming.Apophis.Types.Monads.Option
{
    public static class OptionalExceptions
    {
        public static void NotFount()
        {
            throw new OptionalNotFoundValue();
        }

        public static void NullHandler(string msg = "Function for handler not be null")
        {
            throw new ArgumentNullException(msg);
        }

        public static void NullHandlerCheck<T>(T func, string msg = "Function for handler not be null")
            where T : class
        {
            if(func == null)
                NullHandler();
        }
        
        public sealed class OptionalNotFoundValue : Exception{}
    }
}
