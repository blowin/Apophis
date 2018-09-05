using System;
using System.Collections;
using System.Collections.Generic;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Mixin;
using Apophis.Types.Monads.Either;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Monads.Option
{
    public struct OptionRef<T, TCheckPolicy> :
            IEquatable<OptionRef<T, TCheckPolicy>>,
            IEquatable<T>,
            IEnumerable<T>,
            IComparable<OptionRef<T, TCheckPolicy>>,
            IComparable<T>,
            IComparable,
            ITypeClass<OptionType>
        where TCheckPolicy : struct, ICheckPolicy
        where T : class
    {
        private T _value;

        #region Properties

        /// <summary>
        /// Return option state Some or None
        /// </summary>
        public OptionType Type
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return NonEmpty ? OptionType.Some : OptionType.None;
            }
        }

        /// <summary>
        /// Return false if option None
        /// </summary>
        public bool Empty
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _value == null;
            }
        }

        /// <summary>
        /// Return true if option Some
        /// </summary>
        public bool NonEmpty
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _value != null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create Option from value
        /// </summary>
        /// <param name="value">may be null</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef(T value)
        {
                _value = value;
        }

        /// <summary>
        /// Create Option from another Option
        /// </summary>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef(OptionRef<T, TCheckPolicy> value)
        {
                _value = value._value;
        }

        #endregion

        #region Equals

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => CompareTo(obj) == 0;
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(OptionRef<T, TCheckPolicy> other) => NonEmpty && other.NonEmpty && _value.Equals(other._value);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(T other) => NonEmpty && _value.Equals(other);

        #endregion

        public override int GetHashCode() => NonEmpty && _value != null ? _value.GetHashCode() : 689435;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
                if (NonEmpty)
                        yield return _value;
        }

        #region CompareTo

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is OptionRef<T, TCheckPolicy>)
                return CompareTo((OptionRef<T, TCheckPolicy>) obj);

            if (obj is T)
                return CompareTo((T) obj);

            return -1;
        }
        
        public int CompareTo(OptionRef<T, TCheckPolicy> other)
        {
                if (NonEmpty)
                        return other.NonEmpty ? Comparer<T>.Default.Compare(_value, other._value) : 1;

                return other.NonEmpty ? -1 : 0;
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other) => NonEmpty ? Comparer<T>.Default.Compare(_value, other) : -1;

        #endregion

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => NonEmpty ? string.Concat("Some(", _value.ToString(), ")") : "None";

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionRef<T, TCheckPolicy>(T val) => new OptionRef<T, TCheckPolicy>(val);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator bool(OptionRef<T, TCheckPolicy> opt) => opt.NonEmpty;

        #region Operators

        /// <summary>
        /// Returns the result of applying handler to this Option's value if
        /// this Option is nonempty.
        /// Returns None if this Option is empty.
        /// Slightly different from `map` in that handler is expected to
        /// return an Option (which could be None).
        /// </summary>
        /// <param name="handler">Applying func</param>
        /// <typeparam name="R">Return type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef<R, TOtherCheckPolicy> FlatMap<
        R, TOtherCheckPolicy>(Func<T, OptionRef<R, TOtherCheckPolicy>> handler)
            where TOtherCheckPolicy : struct, ICheckPolicy 
            where R : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return NonEmpty ? handler(_value) : EmptyOption<R, TOtherCheckPolicy>();
        }

        /// <summary>
        /// Apply handler for value if option is nonempty. Otherwise return none
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="R">New hold option type</typeparam>
        /// <returns>Return none if empty. Otherwise return new option for value return from handler</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef<R, TCheckPolicy> Map<R>(Func<T, R> handler) 
            where R : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);
            
            return NonEmpty ? new OptionRef<R, TCheckPolicy>(handler(_value)) : EmptyOption<R, TCheckPolicy>();
        }

        /// <summary>
        /// Returns this Option if it is nonempty and applying the predicate to
        /// this Option value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef<T, TCheckPolicy> Filter(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);
            
            return NonEmpty && predicate(_value) ? this : EmptyOption<T, TCheckPolicy>();
        }

        /// <summary>
        /// Returns this Option if it is nonempty and applying the predicate to
        /// this Option value returns false. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public OptionRef<T, TCheckPolicy> FilterNot(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return NonEmpty && !predicate(_value) ? this : EmptyOption<T, TCheckPolicy>();
        }

        /// <summary>
        /// Returns true if this option is nonempty and the
        /// predicate returns true when applied to this Option's value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Exist(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return NonEmpty && predicate(_value);
        }

        /// <summary>
        /// Returns true if this option is empty or the
        /// predicate returns true when applied to this $option's value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Forall(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return Empty || predicate(_value);
        }

        /// <summary>
        /// Returns the result of applying handler to this Option's value if the Option is nonempty.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Function for applying, if option nonempty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T Fold(T init, Func<T, T, T> handler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return NonEmpty ? handler(_value, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Option's value if the Option is nonempty.
        /// Otherwise, evaluates expression ifEmpty.    
        /// </summary>
        /// <param name="ifEmpty">the expression to evaluate if empty.</param>
        /// <param name="handler">the function to apply if nonempty.</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R Fold<R>(Func<R> ifEmpty, Func<T, R> handler)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(handler);
                ExceptionUtility.NullHandlerCheck(ifEmpty);   
            }
            
            return NonEmpty ? handler(_value) : ifEmpty();
        }

        /// <summary>
        /// Applies a binary operator to a start value, going left to right.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Applying function if nonempty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldLeft<R>(R init, Func<T, R, R> handler)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return NonEmpty ? handler(_value, init) : init;
        }

        /// <summary>
        /// Applies a binary operator to a start value, going right to left.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Applying function if nonempty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldRigth<R>(R init, Func<R, T, R> handler)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return NonEmpty ? handler(init, _value) : init;
        }

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the `value`
        /// </summary>
        /// <param name="value">Return if option is empty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T OrElse(T value) => NonEmpty ? _value : value;

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the `value` from factory func
        /// </summary>
        /// <param name="factory">Run factory and return value if option is empty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T OrElse(Func<T> factory)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(factory, "Factory function not be null");

            return NonEmpty ? _value : factory();
        }

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the default value for type.
        /// </summary>
        /// <returns>May return null</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T OrDefault() => NonEmpty ? _value : default(T);

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// throw 'OptionalNotFoundValue' exception.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T OrThrow()
        {
                if (Empty)
                        OptionalExceptions.NotFount();

                return _value;
        }

        #endregion

        #region Pattern matching

        /// <summary>
        /// Pattern matching for option
        /// </summary>
        /// <param name="some">Function if nonempty</param>
        /// <param name="none">Function if empty</param>
        public Unit Match(Action<T> some, Action none)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
                ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
            }

            if (NonEmpty)
                    some(_value);
            else
                    none();
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for option with result
        /// </summary>
        /// <param name="some">Function if nonempty</param>
        /// <param name="none">Function if empty</param>
        public R Match<R>(Func<T, R> some, Func<R> none)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
                ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
            }

            return NonEmpty ? some(_value) : none();
        }
        
        /// <summary>
        /// Pattern matching for option with result
        /// </summary>
        /// <param name="some">Function if nonempty</param>
        /// <param name="none">Function if empty</param>
        public R Match<R>(Func<T, R> some, R none)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");

            return NonEmpty ? some(_value) : none;
        }

        /// <summary>
        /// Math for some value, if empty, function not be call
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Unit MatchSome(Action<T> some)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");

            if (NonEmpty)
                    some(_value);
            
            return Unit.Def;
        }

        /// <summary>
        /// Math for none, if nonempty, function not be call
        /// </summary>
        /// <param name="none"></param>
        public Unit MatchNone(Action none)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
            
            if (Empty)
                    none();
        
            return Unit.Def;
        }

        #endregion

        #region Option<T> equal Option<T>

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => !left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => left.CompareTo(right) == -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => left.CompareTo(right) == 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => left.CompareTo(right) != 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(OptionRef<T, TCheckPolicy> left, OptionRef<T, TCheckPolicy> right) => left.CompareTo(right) != -1;

        #endregion

        #region Convert To Other

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Option<T, TCheckPolicy>(OptionRef<T, TCheckPolicy> obj) =>
            obj._value.ToOption<T, TCheckPolicy>();

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionRef<T, TCheckPolicy>(Option<T, TCheckPolicy> obj) =>
            new OptionRef<T, TCheckPolicy>(obj.OrDefault());
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<T, R, TCheckPolicy> ToLeftRef<R>() 
            where R : class => new EitherRef<T, R, TCheckPolicy>(_value);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<L, T, TCheckPolicy> ToRightRef<L>() 
            where L : class => new EitherRef<L, T, TCheckPolicy>(_value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try.Try<T, TCheckPolicy> ToTryRef() => NonEmpty
            ? new Try.Try<T, TCheckPolicy>(_value)
            : new Try.Try<T, TCheckPolicy>(new NullReferenceException());

        #endregion

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static OptionRef<R, TCheckPolicy> EmptyOption<R, TCheckPolicy>()
            where TCheckPolicy : struct, ICheckPolicy 
            where R : class => new OptionRef<R, TCheckPolicy>();
        
    }
}