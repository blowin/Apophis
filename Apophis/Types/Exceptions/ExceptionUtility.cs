using System;

namespace Apophis.Types.Exceptions
{
    public class ExceptionUtility
    {
        public static void ThrowIfTrue<T>(bool cond) where T : Exception, new()
        {
            if(cond)
                throw new T();
        }
        
        public static void ThrowIfFalse<T>(bool cond) where T : Exception, new()
        {
            ThrowIfTrue<T>(!cond);
        }

        public static void NullHandlerCheck<T>(T func, string msg = "Function for handler not be null")
            where T : class
        {
            if(func == null)
                throw new ArgumentNullException(msg);
        }
        
        public static void NullPredicatCheck<T>(T func, string msg = "Function for check, not be null")
            where T : class
        {
            if(func == null)
                throw new ArgumentNullException(msg);
        }
    }
}