namespace Apophis.Types.Policys.Check
{
    public struct SafePolicy : ICheckPolicy
    {
        public bool NeedCheck
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return true;
            }
        }
    }
}