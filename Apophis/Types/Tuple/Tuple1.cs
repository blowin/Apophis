using System;
using System.Collections.Generic;
using Apophis.Types.Enums;
using Apophis.Types.Mixin;

namespace Apophis.Types.Tuple
{
    public struct Tuple1<T1> : 
        IEquatable<Tuple1<T1>>, 
        IComparable, 
        IComparable<Tuple1<T1>>,
        ITuple,
        ITypeClass<TupleType>
    {
        public T1 Item1;
        
        int ITuple.Length
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return 1; }
        }
        
        object ITuple.this[int index]
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index != 0)
                    throw new IndexOutOfRangeException();
                return (object) this.Item1;
            }
        }

        TupleType ITypeClass<TupleType>.Type
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return TupleType.Tup1; }
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Tuple1(T1 val)
        {
            if(val == null)
                throw new NullReferenceException();
            
            Item1 = val;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj != null && 
                   obj is Tuple1<T1> && 
                   Comparer<T1>.Default.Compare(((Tuple1<T1>)obj).Item1, Item1) == 0;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Item1.GetHashCode();

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => string.Concat("(", Item1.ToString(), ")");

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Tuple1<T1> Of(T1 val) => new Tuple1<T1>(val);

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Tuple1<T1> other) => CompareTo(other) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            if (obj != null && obj is Tuple1<T1>)
                return CompareTo((Tuple1<T1>) obj);

            return -1;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R apply<R>(Func<T1, R> func) => func(Item1);
        
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void apply(Action<T1> func) => func(Item1);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Tuple1<T1> other) => Comparer<T1>.Default.Compare(Item1, other.Item1);
    }
}