using System;
using System.Collections.Generic;
using System.Text;
using Apophis.Types.Enums;
using Apophis.Types.Mixin;

namespace Apophis.Types.Tuple
{
    public struct Tup2<T1, T2> : 
        IEquatable<Tuple<T1, T2>>, 
        IComparable, 
        IComparable<Tuple<T1, T2>>,
        ITuple,
        ITypeClass<TupleType>
    {
        public T1 Item1;
        public T2 Item2;
        
        int ITuple.Length
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return 2; }
        }
        
        object ITuple.this[int index]
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 1: return Item1;
                    case 2: return Item2;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        TupleType ITypeClass<TupleType>.Type
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return TupleType.Tup2; }
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Tup2(T1 val, T2 val2)
        {
            if(val == null || val2 == null)
                throw new NullReferenceException();
            
            Item1 = val;
            Item2 = val2;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj != null && 
                   obj is Tuple<T1, T2> && 
                   Comparer<T1>.Default.Compare(((Tuple<T1, T2>)obj).Item1, Item1) == 0;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Item1.GetHashCode() ^ Item2.GetHashCode();

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            var s1 = Item1.ToString();
            var s2 = Item2.ToString();
            return new StringBuilder(2 + s1.Length + s2.Length + 1)
                .Append('(').Append(s1).Append(',').Append(s2).Append(')').ToString();
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Tuple<T1, T2> Of(T1 val, T2 val2) => new Tuple<T1, T2>(val, val2);

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Tuple<T1, T2> other) => CompareTo(other) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            if (obj is Tuple<T1, T2>)
                return CompareTo((Tuple<T1, T2>) obj);

            return -1;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R apply<R>(Func<T1, T2, R> func) => func(Item1, Item2);
        
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void apply(Action<T1, T2> func) => func(Item1, Item2);

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Tuple<T1, T2> other)
        {
            var v1 = Comparer<T1>.Default.Compare(Item1, other.Item1);
            var v2 = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (v1 == v2)
                return v1;

            if (v1 == 1 && v2 == 0)
                return 1;

            return -1;
        }
    }
}