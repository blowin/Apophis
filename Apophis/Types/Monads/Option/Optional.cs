// #define APOPHIS_CHECK
// #define NET_4_6

using System;
using System.Collections;
using System.Collections.Generic;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Milxn;
using Apophis.Types.Monads.Either;

namespace Apophis.Types.Monads.Option
{
        /// <summary>
        /// Represents optional values. It is in one of two states. There is an object(Some) or not(None).
        /// </summary>
        /// <typeparam name="T">Type of hold value</typeparam>
        [Serializable]
        public struct Option<T> :
                IEquatable<Option<T>>,
                IEquatable<T>,
                IEnumerable<T>,
                IComparable<Option<T>>,
                IComparable<T>,
                IComparable,
                ITypeClass<OptionType>
        {
                private T _value;
                
                private bool _hasValue;

                #region Properties

                /// <summary>
                /// Return option state Some or None
                /// </summary>
                public OptionType Type
                {
                        get { return _hasValue ? OptionType.Some : OptionType.None; }
                }

                /// <summary>
                /// Return false if option None
                /// </summary>
                public bool Empty
                {
                        get { return _hasValue == false; }
                }

                /// <summary>
                /// Return true if option Some
                /// </summary>
                public bool NonEmpty
                {
                        get { return _hasValue; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Create Option from value
                /// </summary>
                /// <param name="value">may be null</param>
                public Option(T value)
                {
                        _value = value;
                        _hasValue = value != null;
                }

                /// <summary>
                /// Create Option from another Option
                /// </summary>
                /// <param name="value"></param>
                public Option(Option<T> value)
                {
                        _value = value._value;
                        _hasValue = value._hasValue;
                }

                #endregion

                #region Equals

                public override bool Equals(object obj)
                {
                        if (obj == null)
                                return false;
                        
                        if (obj is Option<T>)
                                return Equals((Option<T>) obj);
                        if (obj is T)
                                return Equals((T) obj);

                        return false;
                }
                
                public bool Equals(Option<T> other)
                {
                        return _hasValue && other._hasValue && _value.Equals(other._value);
                }

                public bool Equals(T other)
                {
                        return _hasValue && _value.Equals(other);
                }

                #endregion

                public override int GetHashCode()
                {
                        if (_hasValue && _value != null)
                                return _value.GetHashCode();

                        return 0;
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                public IEnumerator<T> GetEnumerator()
                {
                        if (_hasValue)
                                yield return _value;
                }

                #region CompareTo

                public int CompareTo(object obj)
                {
                        if (obj == null)
                                return 1;

                        if (obj is Option<T>)
                                return CompareTo((Option<T>) obj);

                        if (obj is T)
                                return CompareTo((T) obj);

                        return -1;
                }
                
                public int CompareTo(Option<T> other)
                {
                        if (_hasValue)
                                return other._hasValue ? Comparer<T>.Default.Compare(_value, other._value) : 1;

                        return other._hasValue ? -1 : 0;
                }

                public int CompareTo(T other)
                {
                        return _hasValue ? Comparer<T>.Default.Compare(_value, other) : -1;
                }

                #endregion

                public override string ToString()
                {
                        return _hasValue ? string.Concat("Some(", _value.ToString(), ")") : "None";
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static implicit operator Option<T>(T val)
                {
                        return val.ToOption();
                }

#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static explicit operator bool(Option<T> opt)
                {
                        return opt.NonEmpty;
                }

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
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Option<R> FlatMap<R>(Func<T, Option<R>> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
#endif
                        return _hasValue ? handler(_value) : EmptyOption<R>();
                }

                /// <summary>
                /// Apply handler for value if option is nonempty. Otherwise return none
                /// </summary>
                /// <param name="handler">Function for applying</param>
                /// <typeparam name="R">New hold option type</typeparam>
                /// <returns>Return none if empty. Otherwise return new option for value return from handler</returns>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Option<R> Map<R>(Func<T, R> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
#endif
                        return _hasValue ? handler(_value).ToOption() : EmptyOption<R>();
                }

                /// <summary>
                /// Returns this Option if it is nonempty and applying the predicate to
                /// this Option value returns true. Otherwise, return None.
                /// </summary>
                /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Option<T> Filter(Func<T, bool> predicate)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullPredicatCheck(predicate);
#endif
                        return _hasValue && predicate(_value) ? this : EmptyOption<T>();
                }

                /// <summary>
                /// Returns this Option if it is nonempty and applying the predicate to
                /// this Option value returns false. Otherwise, return None.
                /// </summary>
                /// <param name="predicate">Predicate for testing</param>
                /// <returns></returns>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Option<T> FilterNot(Func<T, bool> predicate)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullPredicatCheck(predicate);
#endif
                        return _hasValue && !predicate(_value) ? this : EmptyOption<T>();
                }

                /// <summary>
                /// Returns true if this option is nonempty and the
                /// predicate returns true when applied to this Option's value.
                /// Otherwise, returns false.
                /// </summary>
                /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public bool Exist(Func<T, bool> predicate)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullPredicatCheck(predicate);
#endif
                        return _hasValue && predicate(_value);
                }

                /// <summary>
                /// Returns true if this option is empty or the
                /// predicate returns true when applied to this $option's value.
                /// </summary>
                /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public bool Forall(Func<T, bool> predicate)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullPredicatCheck(predicate);
#endif
                        return !_hasValue || predicate(_value);
                }

                /// <summary>
                /// Returns the result of applying handler to this Option's value if the Option is nonempty.
                /// Otherwise, return init.
                /// </summary>
                /// <param name="init">Initial value</param>
                /// <param name="handler">Function for applying, if option nonempty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public T Fold(T init, Func<T, T, T> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
#endif
                        return _hasValue ? handler(_value, init) : init;
                }

                /// <summary>
                /// Returns the result of applying handler to this Option's value if the Option is nonempty.
                /// Otherwise, evaluates expression ifEmpty.    
                /// </summary>
                /// <param name="ifEmpty">the expression to evaluate if empty.</param>
                /// <param name="handler">the function to apply if nonempty.</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public R Fold<R>(Func<R> ifEmpty, Func<T, R> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
                        ExceptionUtility.NullHandlerCheck(ifEmpty);
#endif
                        return _hasValue ? handler(_value) : ifEmpty();
                }

                /// <summary>
                /// Applies a binary operator to a start value, going left to right.
                /// </summary>
                /// <param name="init">Initial value</param>
                /// <param name="handler">Applying function if nonempty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public R FoldLeft<R>(R init, Func<T, R, R> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
#endif
                        return _hasValue ? handler(_value, init) : init;
                }

                /// <summary>
                /// Applies a binary operator to a start value, going right to left.
                /// </summary>
                /// <param name="init">Initial value</param>
                /// <param name="handler">Applying function if nonempty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public R FoldRigth<R>(R init, Func<R, T, R> handler)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(handler);
#endif
                        return _hasValue ? handler(init, _value) : init;
                }

                /// <summary>
                /// Returns the option's value if the option is nonempty, otherwise
                /// return the `value`
                /// </summary>
                /// <param name="value">Return if option is empty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public T OrElse(T value)
                {
                        return _hasValue ? _value : value;
                }

                /// <summary>
                /// Returns the option's value if the option is nonempty, otherwise
                /// return the `value` from factory func
                /// </summary>
                /// <param name="factory">Run factory and return value if option is empty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public T OrElse(Func<T> factory)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(factory, "Factory function not be null");
#endif
                        return _hasValue ? _value : factory();
                }

                /// <summary>
                /// Returns the option's value if the option is nonempty, otherwise
                /// return the default value for type.
                /// </summary>
                /// <returns>May return null</returns>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public T OrDefault()
                {
                        return _hasValue ? _value : default(T);
                }

                /// <summary>
                /// Returns the option's value if the option is nonempty, otherwise
                /// throw 'OptionalNotFoundValue' exception.
                /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public T OrThrow()
                {
                        if (!_hasValue)
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
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Unit Match(Action<T> some, Action none)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
                        ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
#endif
                        if (_hasValue)
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
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public R Match<R>(Func<T, R> some, Func<R> none)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
                        ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
#endif
                        return _hasValue ? some(_value) : none();
                }
                
                /// <summary>
                /// Pattern matching for option with result
                /// </summary>
                /// <param name="some">Function if nonempty</param>
                /// <param name="none">Function if empty</param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public R Match<R>(Func<T, R> some, R none)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
#endif
                        return _hasValue ? some(_value) : none;
                }

                /// <summary>
                /// Math for some value, if empty, function not be call
                /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Unit MatchSome(Action<T> some)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(some, "Function for some match not be null");
#endif
                        if (_hasValue)
                                some(_value);
                    
                    return Unit.Def;
                }

                /// <summary>
                /// Math for none, if nonempty, function not be call
                /// </summary>
                /// <param name="none"></param>
#if NET_4_6 || NET_STANDARD_2_0
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public Unit MatchNone(Action none)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(none, "Function for none match not be null");
#endif
                        if (!_hasValue)
                                none();
                    
                    return Unit.Def;
                }

                #endregion

                #region Option<T> equal Option<T>

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator ==(Option<T> left, Option<T> right)
                {
                        return left.Equals(right);
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator !=(Option<T> left, Option<T> right)
                {
                        return !left.Equals(right);
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator <(Option<T> left, Option<T> right)
                {
                        return left.CompareTo(right) == -1;
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator >(Option<T> left, Option<T> right)
                {
                        return left.CompareTo(right) == 1;
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator <=(Option<T> left, Option<T> right)
                {
                        return left.CompareTo(right) != 1;
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public static bool operator >=(Option<T> left, Option<T> right)
                {
                        return left.CompareTo(right) != -1;
                }

                #endregion

                #region Convert To Other

                public Either<T, R> ToLeft<R>()
                {
                        return new Either<T, R>(_value);
                }

                public Either<L, T> ToRight<L>()
                {
                        return new Either<L, T>(_value);
                }
                
                #endregion
                
                private static Option<R> EmptyOption<R>() { return new Option<R>(); }
        }

        public static class Optional
        {
                public static Option<T> Some<T>(T val) { return new Option<T>(val); }
                
                public static Option<T> None<T>() { return new Option<T>(); }
        }
}