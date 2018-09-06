using System;
using System.Collections;
using System.Collections.Generic;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Mixin;
using Apophis.Types.Monads.Either;
using Apophis.Types.Monads.Option;
using Apophis.Types.Monads.Try;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Monads.Eval
{
    public struct Eval<T, TCheckPolicy> :
        IEquatable<Eval<T, TCheckPolicy>>,
        IEquatable<T>,
        IEnumerable<T>,
        IComparable<Eval<T, TCheckPolicy>>,
        IComparable<T>,
        IComparable,
        ITypeClass<EvalType>
        where TCheckPolicy : struct, ICheckPolicy
    {
        private Func<T> _factory;
        private T _value;
        private EvalType _type;
        
        public EvalType Type
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return _type; }
        }

        public T Value
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (_type)
                {
                    case EvalType.Later:  return CalcIfNeed;
                    case EvalType.Always: return _factory();
                }

                return _value;
            }
        }
        
        private T CalcIfNeed
        {
            [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_factory == null)
                    return _value;

                _value = _factory();
                _factory = null;
                return _value;
            }
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Eval(Func<T> factory, EvalType type)
        {
            if(factory == null)
                throw new ArgumentNullException();
            
            _factory = factory;
            _value = default(T);
            _type = type;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Eval(T val, EvalType type)
        {
            _factory = null;
            _value = val;
            _type = type;
        }
        
        #region Operators

        /// <summary>
        /// Returns the result of applying handler to this Eval's
        /// return Eval from handler
        /// </summary>
        /// <param name="handler">Applying func</param>
        /// <typeparam name="R">Return type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Eval<R, TOtherCheckPolicy> FlatMap<R, TOtherCheckPolicy>(Func<T, Eval<R, TOtherCheckPolicy>> handler)
            where TOtherCheckPolicy : struct, ICheckPolicy
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return handler(Value);
        }

        /// <summary>
        /// Apply handler for value or calculate and apply
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="R">New hold Eval type</typeparam>
        /// <returns>Return none if empty. Otherwise return new Eval for value return from handler</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Eval<R, TCheckPolicy> Map<R>(Func<T, R> handler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);
            
            return Eval<R, TCheckPolicy>.Now(handler(Value));
        }

        /// <summary>
        /// Returns Some Option if predicat return true
        /// Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<T, TCheckPolicy> Filter(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            var val = Value;
            return predicate(val) ? val.ToOption<T, TCheckPolicy>() : new Option<T, TCheckPolicy>();
        }

        /// <summary>
        /// Returns Some Option if predicat return false
        /// Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<T, TCheckPolicy> FilterNot(Func<T, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            var val = Value;
            return !predicate(val) ? val.ToOption<T, TCheckPolicy>() : new Option<T, TCheckPolicy>();
        }

        /// <summary>
        /// Returns the result of applying handler to this Eval's value.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="handler">Function for applying, if Eval nonempty</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T Fold(T init, Func<T, T, T> handler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return handler(init, Value);
        }

        /// <summary>
        /// Returns true if default comparator returns 0 when compare &Value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Contain(T val) => Comparer<T>.Default.Compare(Value, val) == 0;

        /// <summary>
        /// Returns true if this comparator returns 0 when compare &Value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">value for compare</param>
        /// <param name="predicat">value comparator</param>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Contain<TOth>(TOth val, Func<T, TOth, bool> predicat)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicat);

            return predicat(Value, val);
        }
        
        #endregion

        #region Pattern matching

        /// <summary>
        /// Pattern matching for Eval
        /// </summary>
        /// <param name="now">Function if Now</param>
        /// <param name="later">Function if Later</param>
        /// <param name="always">Function if Always</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Unit Match(Action<T> now, Action<T> later, Action<T> always)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(now, "Function for match not be null");
                ExceptionUtility.NullHandlerCheck(later, "Function for match not be null");
                ExceptionUtility.NullHandlerCheck(always, "Function for match not be null");
            }

            switch (_type)
            {
                case EvalType.Now:
                    now(Value);
                    break;
                case EvalType.Later:
                    later(Value);
                    break;
                case EvalType.Always:
                    always(Value);
                    break;
            }
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for Eval with result
        /// </summary>
        /// <param name="now">Function if Now</param>
        /// <param name="later">Function if Later</param>
        /// <param name="always">Function if Always</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<T, R> now, Func<T, R> later, Func<T, R> always)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(now, "Function for match not be null");
                ExceptionUtility.NullHandlerCheck(later, "Function for match not be null");
                ExceptionUtility.NullHandlerCheck(always, "Function for match not be null");
            }

            switch (_type)
            {
                case EvalType.Now:     return now(Value);
                case EvalType.Later:   return later(Value);
                case EvalType.Always:  return always(Value);
                default:               return default(R);
            }
        }
        
        /// <summary>
        /// Math for value
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Unit Match(Action<T> matcher)
        {
            if (default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(matcher, "Function for match not be null");

            matcher(Value);
            
            return Unit.Def;
        }

        #endregion
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Eval<T, TCheckPolicy> && CompareTo((Eval<T, TCheckPolicy>)obj) == 0;
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            switch (_type)
            {
                case EvalType.Now:     return _value.GetHashCode();
                case EvalType.Later:   return _factory.GetHashCode() << 2;
                case EvalType.Always:  return _factory.GetHashCode();
            }

            return -1;
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            switch (_type)
            {
                case EvalType.Now:     return string.Concat("Now(", _value == null ? "null" : _value.ToString(), ")");
                case EvalType.Later:   return "Later";
                case EvalType.Always:  return "Always";
                default: return string.Empty;
            }
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Eval<T, TCheckPolicy> other) => CompareTo(other) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(T other) => _type == EvalType.Now && Comparer<T>.Default.Compare(_value, other) == 0;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            yield return Value;
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Eval<T, TCheckPolicy> other)
        {
            return Comparer<T>.Default.Compare(Value, other.Value);
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other)
        {
            return Comparer<T>.Default.Compare(Value, other);
        }

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is Eval<T, TCheckPolicy>)
                return CompareTo((Eval<T, TCheckPolicy>) obj);

            if (obj is T)
                return CompareTo((T) obj);

            return -1;
        }
        
        #region Eval<T> equal Eval<T>

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => !left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => left.CompareTo(right) == -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => left.CompareTo(right) == 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => left.CompareTo(right) != 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Eval<T, TCheckPolicy> left, Eval<T, TCheckPolicy> right) => left.CompareTo(right) != -1;

        #endregion

        #region Convert to other

        public Either<T, Unit, TCheckPolicy> ToLeft() => new Either<T, Unit, TCheckPolicy>(Value);
        
        public Either<Unit, T, TCheckPolicy> ToRight() => new Either<Unit, T, TCheckPolicy>(Value);

        public Option<T, TCheckPolicy> ToOption() => Value.ToOption<T, TCheckPolicy>();

        public Try<T, TCheckPolicy> ToTry()
        {
            switch (_type)
            {
                case EvalType.Now:    return Try.Try.From<T, TCheckPolicy>(_value);
                case EvalType.Later:  return _factory == null ? Try.Try.From<T, TCheckPolicy>(_value)
                    : Try.Try.From<T, TCheckPolicy>(_factory);
                case EvalType.Always: return Try.Try.From<T, TCheckPolicy>(_factory);
                default:              return Try.Try.From<T, InvalidOperationException, TCheckPolicy>();
            }
        }
        
        #endregion
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Now(Func<T> factory) => new Eval<T, TCheckPolicy>(factory(), EvalType.Now);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Now(T val) => new Eval<T, TCheckPolicy>(val, EvalType.Now);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Later(Func<T> factory) => new Eval<T, TCheckPolicy>(factory, EvalType.Later);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Always(Func<T> factory) => new Eval<T, TCheckPolicy>(factory, EvalType.Always);
    }

    public static class Eval
    {
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Now<T, TCheckPolicy>(Func<T> factory) 
            where TCheckPolicy : struct, ICheckPolicy => Eval<T, TCheckPolicy>.Now(factory);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Now<T, TCheckPolicy>(T val) 
            where TCheckPolicy : struct, ICheckPolicy => Eval<T, TCheckPolicy>.Now(val);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Later<T, TCheckPolicy>(Func<T> factory) 
            where TCheckPolicy : struct, ICheckPolicy => Eval<T, TCheckPolicy>.Later(factory);
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Eval<T, TCheckPolicy> Always<T, TCheckPolicy>(Func<T> factory) 
            where TCheckPolicy : struct, ICheckPolicy => Eval<T, TCheckPolicy>.Always(factory);
    }
}