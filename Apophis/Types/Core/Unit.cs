using System;

namespace Apophis.Types.Core
{
    public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        public static Unit Def = new Unit();

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is Unit;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => 421 << 2;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj) => obj is Unit ? 0 : -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => "Unit";

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        bool IEquatable<Unit>.Equals(Unit other) => true;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        int IComparable<Unit>.CompareTo(Unit other) => 0;

        #region Equals operators

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Unit lhs, Unit rhs) => true;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Unit lhs, Unit rhs) => true;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Unit lhs, Unit rhs) => true;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Unit lhs, Unit rhs) => true;
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Unit lhs, Unit rhs) => true;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Unit lhs, Unit rhs) => true;

        #endregion
    }
}