namespace Apophis.Types.Tuple
{
    public interface ITuple
    {
        int Length { get; }

        object this[int index] { get; }
    }
}