using System;
using System.Collections;
using System.Collections.Generic;

namespace FunctionalProgramming.Apophis.Types.Monads.Option
{
    /// <summary>
    /// Represents optional values. It is in one of two states. There is an object(Some) or not(None).
    /// </summary>
    /// <typeparam name="T">Type of hold value</typeparam>
    public struct Option<T> : 
        IEquatable<Option<T>>, 
        IEquatable<T>, 
        IEnumerable<T>, 
        IComparable<Option<T>>, 
        IComparable<T>
    {
        private readonly bool _hasValue;
        
        private T _value;

        /// <summary>
        /// Return option state Some or None
        /// </summary>
        public OptionType Type => _hasValue ? OptionType.Some : OptionType.None;
        
        /// <summary>
        /// Return false if option None
        /// </summary>
        public bool Empty => _hasValue == false;
        
        /// <summary>
        /// Return true if option Some
        /// </summary>
        public bool Contain => _hasValue;
        
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

        public override bool Equals(object obj)
        {
            if (obj is Option<T>)
                return Equals((Option<T>) obj);
            if (obj is T)
                return Equals((T) obj);
            
            return false;
        }

        public override int GetHashCode()
        {
            if (_hasValue && _value != null)
                return _value.GetHashCode();

            return 0;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            if (_hasValue)
                yield return _value;
        }

        public int CompareTo(Option<T> other)
        {
            if (_hasValue)
                return other._hasValue ? Comparer<T>.Default.Compare(_value, other._value) : 1;
            
            return other._hasValue ? -1 : 0;
        }
        
        public int CompareTo(T other) => _hasValue ? Comparer<T>.Default.Compare(_value, other) : -1;

        public override string ToString() => _hasValue ? $"Some({_value})" : "None";

        public bool Equals(Option<T> other) => _hasValue && other._hasValue && _value.Equals(other._value);

        public bool Equals(T other) => _hasValue && _value.Equals(other);

        public static implicit operator Option<T>(T val) => val.ToOption();
        
        public static explicit operator bool(Option<T> opt) => opt.Contain;

        /// <summary>
        /// Returns the result of applying handler to this Option's value if
        /// this Option is nonempty.
        /// Returns None if this Option is empty.
        /// Slightly different from `map` in that handler is expected to
        /// return an Option (which could be None).
        /// </summary>
        /// <param name="handler">Applying func</param>
        /// <typeparam name="R">Return type</typeparam>
        /// <returns></returns>
        public Option<R> FlatMap<R>(Func<T, Option<R>> handler) => _hasValue ? handler(_value) : EmptyOption<R>();
        
        /// <summary>
        /// Apply handler for value if option is nonempty. Otherwise return none
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="R">New hold option type</typeparam>
        /// <returns>Return none if empty. Otherwise return new option for value return from handler</returns>
        public Option<R> Map<R>(Func<T, R> handler) => _hasValue ? handler(_value).ToOption() : EmptyOption<R>();

        /// <summary>
        /// Returns this Option if it is nonempty and applying the predicate to
        /// this Option value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicat">Predicat for testing</param>
        public Option<T> Filter(Func<T, bool> predicat) => _hasValue && predicat(_value) ? this : EmptyOption<T>();
        
        /// <summary>
        /// Returns this Option if it is nonempty and applying the predicate to
        /// this Option value returns false. Otherwise, return None.
        /// </summary>
        /// <param name="predicat">Predicat for testing</param>
        /// <returns></returns>
        public Option<T> FilterNot(Func<T, bool> predicat) => _hasValue && !predicat(_value) ? this : EmptyOption<T>();

        /// <summary>
        /// Returns true if this option is nonempty and the predicate
        /// predicat returns true when applied to this Option's value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicat">Predicat for testing</param>
        public bool Exist(Func<T, bool> predicat) => _hasValue && predicat(_value);

        /// <summary>
        /// Returns true if this option is empty or the predicate
        /// predicat returns true when applied to this $option's value.
        /// </summary>
        /// <param name="predicat">Predicat for testing</param>
        public bool Forall(Func<T, bool> predicat) => !_hasValue || predicat(_value);

        /// <summary>
        /// Returns the result of applying handler to this Option's value if the Option is nonempty.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Function for applying, if option nonempty</param>
        public T Fold(T init, Func<T, T, T> handler) => _hasValue ? handler(_value, init) : init;
        
        /// <summary>
        /// Returns the result of applying handler to this Option's value if the Option is nonempty.
        /// Otherwise, evaluates expression ifEmpty.    
        /// </summary>
        /// <param name="ifEmpty">the expression to evaluate if empty.</param>
        /// <param name="handler">the function to apply if nonempty.</param>
        public R Fold<R>(Func<R> ifEmpty, Func<T, R> handler) => _hasValue ? handler(_value) : ifEmpty();
        
        /// <summary>
        /// Applies a binary operator to a start value, going left to right.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Applying function if nonempty</param>
        public R FoldLeft<R>(R init, Func<T, R, R> handler) => _hasValue ? handler(_value, init) : init;
        
        /// <summary>
        /// Applies a binary operator to a start value, going right to left.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Applying function if nonempty</param>
        public R FoldRigth<R>(R init, Func<R, T, R> handler) => _hasValue ? handler(init, _value) : init;
        
        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the `value`
        /// </summary>
        /// <param name="value">Return if option is empty</param>
        public T OrElse(T value) => _hasValue ? _value : value;
        
        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the `value` from factory func
        /// </summary>
        /// <param name="factory">Run factory and return value if option is empty</param>
        public T OrElse(Func<T> factory) => _hasValue ? _value : factory();

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// return the default value for type.
        /// </summary>
        /// <returns>May return null</returns>
        public T OrDefault() => _hasValue ? _value : default(T);

        /// <summary>
        /// Returns the option's value if the option is nonempty, otherwise
        /// throw 'OptionalNotFoundValue' exception.
        /// </summary>
        public T OrThrow()
        {
            if (!_hasValue) 
                OptionalExceptions.NotFount();
            
            return _value;
        }
        
        #region Pattern matching

        /// <summary>
        /// Pattern matching for option
        /// </summary>
        /// <param name="some">Function if nonempty</param>
        /// <param name="none">Function if empty</param>
        public void Match(Action<T> some, Action none)
        {
            if (_hasValue)
                some(_value);
            else
                none();
        }

        /// <summary>
        /// Pattern matching for option with result
        /// </summary>
        /// <param name="some">Function if nonempty</param>
        /// <param name="none">Function if empty</param>
        public R Match<R>(Func<T, R> some, Func<R> none) => _hasValue ? some(_value) : none();

        /// <summary>
        /// Math for some value, if empty, function not be call
        /// </summary>
        public void Match(Action<T> some)
        {
            if (_hasValue)
                some(_value);
        }

        /// <summary>
        /// Math for none, if nonempty, function not be call
        /// </summary>
        /// <param name="none"></param>
        public void Match(Action none)
        {
            if (!_hasValue)
                none();
        }
        
        #endregion
        
        #region Equals operations

        #region Option<T> equal Option<T>

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

        public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) == -1;

        public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) == 1;

        public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) != 1;

        public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) != -1;

        #endregion

        #region T equals Option<T>

        public static bool operator ==(T left, Option<T> right) => right.Equals(left);
        
        public static bool operator !=(T left, Option<T> right) => !right.Equals(left);
        
        public static bool operator <(T left, Option<T> right) => right.CompareTo(left) == 1;
        
        public static bool operator >(T left, Option<T> right) => right.CompareTo(left) == -1;
        
        public static bool operator <=(T left, Option<T> right) => right.CompareTo(left) != -1;
        
        public static bool operator >=(T left, Option<T> right) => right.CompareTo(left) != 1;

        #endregion

        #region Option<T> equals T

        public static bool operator ==(Option<T> left, T right) => left.Equals(right);

        public static bool operator !=(Option<T> left, T right) => !left.Equals(right);

        public static bool operator <(Option<T> left, T right) => left.CompareTo(right) == -1;

        public static bool operator >(Option<T> left, T right) => left.CompareTo(right) == 1;

        public static bool operator <=(Option<T> left, T right) => left.CompareTo(right) != 1;

        public static bool operator >=(Option<T> left, T right) => left.CompareTo(right) != -1;

        #endregion

        #endregion
        
        private static Option<R> EmptyOption<R>() => new Option<R>();
    }
    
    public static class Optional
    {
        public static Option<T> Some<T>(T val) => new Option<T>(val);
        
        public static Option<T> None<T>() => new Option<T>();
    }
}