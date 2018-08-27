namespace FunctionalProgramming.Apophis.Types.Monads.Option
{
    public static class OptionalExtensions
    {
        public static Option<T> ToOption<T>(this T obj) => new Option<T>(obj);
    }
}