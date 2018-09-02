using System;

namespace Apophis.Types.Core
{
    public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        public static Unit Def = new Unit();

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public int CompareTo(object obj)
        {
            return obj is Unit ? 0 : -1;
        }

        public override string ToString()
        {
            return "Unit";
        }

        bool IEquatable<Unit>.Equals(Unit other)
        {
            return true;
        }

        int IComparable<Unit>.CompareTo(Unit other)
        {
            return 0;
        }

        #region Equals operators

        public static bool operator ==(Unit lhs, Unit rhs)
        {
            return true;
        }

        public static bool operator !=(Unit lhs, Unit rhs)
        {
            return true;
        }

        public static bool operator >(Unit lhs, Unit rhs)
        {
            return true;
        }

        public static bool operator >=(Unit lhs, Unit rhs)
        {
            return true;
        }

        public static bool operator <(Unit lhs, Unit rhs)
        {
            return true;
        }

        public static bool operator <=(Unit lhs, Unit rhs)
        {
            return true;
        }

        #endregion
    }
}