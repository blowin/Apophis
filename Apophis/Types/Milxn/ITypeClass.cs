namespace Apophis.Types.Milxn
{
    public interface ITypeClass<T>
        where T : struct
    {
        T Type { get; }
    }
}