namespace Apophis.Types.Mixin
{
    public interface ITypeClass<T>
        where T : struct
    {
        T Type { get; }
    }
}