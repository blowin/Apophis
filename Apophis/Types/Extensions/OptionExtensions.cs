using Apophis.Types.Monads.Option;

namespace Apophis.Types.Extensions
{
    public static class OptionalExtensions
    {
        public static Option<T> ToOption<T>(this T obj){ return new Option<T>(obj); }

        public static Option<T> Flatten<T>(this Option<Option<T>> obj)
        {
            return obj.FlatMap(x => x);
        }
    }
}