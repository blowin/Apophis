using System.Collections.Generic;
using Apophis.Types.Mixin;

namespace Apophis.Types.Extensions.Mixln
{
    public static class TypeClassExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool Is<T, R>(this T obj, R type)
            where R : struct
            where T : struct, ITypeClass<R> => Comparer<R>.Default.Compare(obj.Type, type) == 0;
    }
}