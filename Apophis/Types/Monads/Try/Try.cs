using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Mixin;
using Apophis.Types.Monads.Either;
using Apophis.Types.Monads.Option;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Monads.Try
{
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct Try<T, TCheckPolicy>  :
        IEquatable<Try<T, TCheckPolicy>>,
        IEquatable<T>,
        IEnumerable<T>,
        IComparable<Try<T, TCheckPolicy>>,
        IComparable<T>,
        IComparable,
        ITypeClass<TryType>
        where TCheckPolicy : struct, ICheckPolicy
    {
        private Exception _error;
        private T _value;
        
        public TryType Type
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return _error != null ? TryType.Error : TryType.Ok; }
        }

        public bool IsError
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return _error != null; }
        }
        
        public bool IsOk
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return _error == null; }
        }

        public Option<T, TCheckPolicy> Value
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return IsOk ? _value.ToOption<T, TCheckPolicy>() : Optional.None<T, TCheckPolicy>(); }
        }
        
        public Option<Exception, TCheckPolicy> Error
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return IsError ? _error.ToOption<Exception, TCheckPolicy>() : Optional.None<Exception, TCheckPolicy>(); }
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try(Func<T> tryFunc)
        {
            try
            {
                _value = tryFunc();
                _error = null;
            }
            catch (Exception e)
            {
                _error = e;
                _value = default(T);
            }
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try(T val)
        {
            _value = val;
            _error = null;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try(Exception error)
        {
            _value = default(T);
            _error = error;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Try(T val, Exception error)
        {
            _value = val;
            _error = error;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => CompareTo(obj) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _error == null ? _value.GetHashCode() : _error.GetHashCode();

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _error != null ? string.Concat("Error(", _error.ToString(), ")") : string.Concat("Ok(", _error.ToString(), ")");

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Try<T, TCheckPolicy> other) => CompareTo(other) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(T other) => _error != null && _value.Equals(other);

        public IEnumerator<T> GetEnumerator()
        {
            if (_error != null)
                yield return _value;
        }
        
        #region Operators
        
        /// <summary>
        /// Returns the result of applying handler to this Try value if the Try has ok status.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="okHandler">Function for applying, if try has ok</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldOk<R>(R init, Func<T, R, R> okHandler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(okHandler);

            return IsOk ? okHandler(_value, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Try value if the Try has error.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="errorHandler">Function for applying, if try has error</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldError<R>(R init, Func<Exception, R, R> errorHandler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(errorHandler);

            return IsError ? errorHandler(_error, init) : init;
        }
        
        /// <summary>
        /// Returns the result of applying handler to this Try value if
        /// this Try contain ok.
        /// Returns Try without apply handler if this Try has error.
        /// Slightly different from `map` in that handler is expected to
        /// return an Try
        /// </summary>
        /// <param name="handler">Applying func</param>
        /// <typeparam name="R">New hold value type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try<R, TOthCheckPolicy> FlatMap<R, TOthCheckPolicy>(Func<T, Try<R, TOthCheckPolicy>> handler)
            where TOthCheckPolicy : struct, ICheckPolicy
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return IsOk ? handler(_value) : new Try<R, TOthCheckPolicy>(_error);
        } 

        /// <summary>
        /// Apply handler for value if Try has ok, then apply ok handler,
        /// otherwise - error
        /// </summary>
        /// <param name="ok">Function for ok branch applying</param>
        /// <typeparam name="L">New hold value type</typeparam>
        /// <returns>Return Either with new type, who contain old branch(left or right).</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try<L, TCheckPolicy> Map<L>(Func<T, L> ok)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(ok);

            return IsOk ? new Try<L, TCheckPolicy>(ok(_value)) : new Try<L, TCheckPolicy>(_error);
        }
        
        /// <summary>
        /// Returns true if this try has ok branch and the
        /// predicate returns true when applied to this ok value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Exist(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return IsOk && predicate(_value);
        }
        
        /// <summary>
        /// Returns true if this try hasn't ok status or the
        /// predicate returns true when applied to this ok value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Forall(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return !IsOk || predicate(_value);
        }
        
        /// <summary>
        /// Returns true if this try has ok and the
        /// compare returns 0 when compare hold value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Contain(T val) => IsOk && Comparer<T>.Default.Compare(_value, val) == 0;
        
        /// <summary>
        /// Returns Some Option if it Try has ok branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<T, TCheckPolicy> Filter(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return IsOk && predicate(_value) ? _value.ToOption<T, TCheckPolicy>() : Optional.None<T, TCheckPolicy>();
        }

        /// <summary>
        /// If try - ok, return _value.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if try - error</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T ValueOr(T defaultVal) => IsOk ? _value : defaultVal;
        
        /// <summary>
        /// If try - ok, return _value.
        /// Otherwise - default value for type. May return null, if T - class
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T ValueOrDefault() => IsOk ? _value : default(T);

        /// <summary>
        /// If try - ok, return _value.
        /// Otherwise use factory function, for create return value.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T ValueOr(Func<T> defaultValFactory)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(defaultValFactory);

            return IsOk ? _value : defaultValFactory();
        }

        /// <summary>
        /// If try - ok, return ok value.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
        public T ValueOrThrow()
        {
            if(IsError)
                TryExceptions.NotFount();
            
            return _value;
        }
        
        #endregion

        #region Pattern matching

        /// <summary>
        /// Pattern matching for try
        /// </summary>
        /// <param name="ok">Action run, if try has ok branch</param>
        /// <param name="error">Action run, if try has error branch</param>
        public Unit Match(Action<T> ok, Action<Exception> error)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(ok);
                ExceptionUtility.NullHandlerCheck(error);
            }

            if (IsOk)
                    ok(_value);
            else
                    error(_error);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for try
        /// </summary>
        /// <param name="ok">Func run, if try has ok branch</param>
        /// <param name="error">Func run, if try has error branch</param>
        /// <returns>Return value from ok, if try has ok branch, otherwise from error handler</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<T, R> ok, Func<Exception, R> error)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(ok);
                ExceptionUtility.NullHandlerCheck(error);
            }

            return IsOk ? ok(_value) : error(_error);
        }
                
        /// <summary>
        /// Pattern matching for ok try
        /// </summary>
        /// <param name="ok">Func run, if try has ok branch</param>
        /// <param name="error">Return if try - error</param>
        /// <returns>If try - ok, return value from left, otherwise - nonLeft value</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchOk<R>(Func<T, R> ok, R error)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(ok);

            return IsOk ? ok(_value) : error;
        }
        
        /// <summary>
        /// Pattern matching for ok try
        /// </summary>
        /// <param name="ok">Func run, if try has ok branch</param>
        /// <param name="error">Run this factory, for create return value, if try - error</param>
        /// <returns>If try - ok, return value from ok, otherwise create value from error function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchOk<R>(Func<T, R> ok, Func<R> error)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(ok);
                ExceptionUtility.NullHandlerCheck(error);
            }

            return IsOk ? ok(_value) : error();
        }

        /// <summary>
        /// Pattern matching for ok try
        /// </summary>
        /// <param name="left">Action run, if try has ok branch</param>
        public Unit MatchOk(Action<T> left)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            if (IsOk)
                    left(_value);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for error Try
        /// </summary>
        /// <param name="error">Action run, if try has error branch</param>
        public Unit MatchError(Action<Exception> error)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(error);

            if (IsError)
                    error(_error);
            
            return Unit.Def;
        }
        
        /// <summary>
        /// Pattern matching for error try
        /// </summary>
        /// <param name="error">Func run, if try has error branch</param>
        /// <param name="ok">Return if try - ok</param>
        /// <returns>If try - error, return value from error, otherwise - ok value</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchError<R>(Func<Exception, R> error, R ok)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(error);

            return IsError ? error(_error) : ok;
        }

        /// <summary>
        /// Pattern matching for error try
        /// </summary>
        /// <param name="error">Func run, if try has error branch</param>
        /// <param name="ok">Run this factory, for create return value, if try - ok</param>
        /// <returns>If Try - error, return value exception value, otherwise create value from ok function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchError<R>(Func<Exception, R> error, Func<R> ok)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(error);
                ExceptionUtility.NullHandlerCheck(ok);
            }
            
            return IsError ? error(_error) : ok();
        }
        
        #endregion
        
        #region Try<T> equal Try<T>

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => !left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => left.CompareTo(right) == -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => left.CompareTo(right) == 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => left.CompareTo(right) != 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Try<T, TCheckPolicy> left, Try<T, TCheckPolicy> right) => left.CompareTo(right) != -1;

        #endregion

        #region Convert To Other
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<Exception, T, TCheckPolicy> ToEither()
        {
            return _error != null ? 
                new Either<Exception, T, TCheckPolicy>(_error) :
                new Either<Exception, T, TCheckPolicy>(_value);
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<T, TCheckPolicy> ToOption() => _error != null ? Optional.None<T, TCheckPolicy>() : Optional.Some<T, TCheckPolicy>(_value);

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Try<T, TOtherPolicy> ToTry<TOtherPolicy>()
            where TOtherPolicy : struct, ICheckPolicy => new Try<T, TOtherPolicy>(_value, _error);
        
        #endregion
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Try<T, TCheckPolicy>(T value) => new Try<T, TCheckPolicy>(value);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Option<T, TCheckPolicy>(Try<T, TCheckPolicy> tryVal) => tryVal.ToOption();
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Try<T, TCheckPolicy>(Option<T, TCheckPolicy> optionVal) => optionVal.ToTry();
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Try<T, TCheckPolicy> other)
        {
            if (_error != null)
                return other._error == null ? 1 : Comparer<T>.Default.Compare(_value, other._value);

            return other._error != null ? -1 : 0;
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other) => _error != null ? Comparer<T>.Default.Compare(_value, other) : -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is T)
                return CompareTo((T) obj);

            if (obj is Try<T, TCheckPolicy>)
                return CompareTo((Try<T, TCheckPolicy>) obj);

            return -1;
        }
    }

    public static class Try
    {
        /// <summary>
        /// Create Try who hold TError
        /// </summary>
        /// <typeparam name="TCheckPolicy">Check policy for try</typeparam>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, TCheckPolicy> From<T, TError, TCheckPolicy>() 
            where TError : Exception, new()
            where TCheckPolicy : struct, ICheckPolicy => new Try<T, TCheckPolicy>(new TError());
        
        /// <summary>
        /// Return try, who run func and if result - success, then status Ok, otherwise - Error
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, TCheckPolicy> From<T, TCheckPolicy>(Func<T> factory) 
            where TCheckPolicy : struct, ICheckPolicy => new Try<T, TCheckPolicy>(factory);
        
        /// <summary>
        /// Return Ok Try with value
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, TCheckPolicy> From<T, TCheckPolicy>(T val) 
            where TCheckPolicy : struct, ICheckPolicy => new Try<T, TCheckPolicy>(val);
        
        /// <summary>
        /// Create Try who hold TError with &SafePolicy
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, SafePolicy> FromSafe<T, TError>() 
            where TError : Exception, new() => new Try<T, SafePolicy>(new TError());
        
        /// <summary>
        /// Return try with &SafePolicy,
        /// who run func and if result - success, then status Ok, otherwise - Error
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, SafePolicy> FromSafe<T>(Func<T> factory) => new Try<T, SafePolicy>(factory);
        
        /// <summary>
        /// Return Ok Try with value and &SafePolicy
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, SafePolicy> FromSafe<T>(T val) => new Try<T, SafePolicy>(val);
        
        /// <summary>
        /// Create Try who hold TError with &UnsafePolicy
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, UnsafePolicy> FromUnsafe<T, TError>() 
            where TError : Exception, new() => new Try<T, UnsafePolicy>(new TError());
        
        /// <summary>
        /// Return try with &UnsafePolicy,
        /// who run func and if result - success, then status Ok, otherwise - Error
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, UnsafePolicy> FromUnsafe<T>(Func<T> factory) => new Try<T, UnsafePolicy>(factory);
        
        /// <summary>
        /// Return Ok Try with value and &UnsafePolicy
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Try<T, UnsafePolicy> FromUnsafe<T>(T val) => new Try<T, UnsafePolicy>(val);
    }
}