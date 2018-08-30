// #define APOPHIS_CHECK
// #define NET_4_6

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Milxn;
using Apophis.Types.Monads.Option;

namespace Apophis.Types.Monads.Either
{
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct Either<TLeft, TRight> : 
        IEquatable<Either<TLeft, TRight>>,
        IComparable<Either<TLeft, TRight>>,
        IComparable,
        IEnumerable<Either<TLeft, TRight>>,
        ITypeClass<EitherType>
    {
        private TLeft _left;
        private TRight _right;
        
        private bool _hasLeft;

        public EitherType Type
        {
            get { return _hasLeft ? EitherType.Left : EitherType.Right; }
        }

        public bool IsLeft
        {
            get { return _hasLeft; }
        }

        public bool IsRight
        {
            get { return _hasLeft == false; }
        }

        public Option<TLeft> Left
        {
            get { return _hasLeft ? _left.ToOption() : Optional.None<TLeft>(); }
        }
        
        public Option<TRight> Right
        {
            get { return _hasLeft == false ? _right.ToOption() : Optional.None<TRight>(); }
        }
        
        public Either<TRight, TLeft> Swap
        {
            get
            {
                return new Either<TRight, TLeft>(_right, _left, !_hasLeft);
            }
        }
        
        #region Constructors
        
        public Either(TLeft left)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(left == null);
            
            _left = left;
            _right = default(TRight);
            _hasLeft = true;
        }

        public Either(TRight right)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(right == null);
            
            _right = right;
            _left = default(TLeft);
            _hasLeft = false;
        }
        
        public Either(Either<TLeft, TRight> other)
        {
            _right = other._right;
            _left = other._left;
            _hasLeft = other._hasLeft;
        }

        public Either(Either<Either<TLeft, TRight>, TRight> other)
        {
            _right = other._right;
            _left = other._left._left;
            _hasLeft = other._hasLeft;
        }
        
        public Either(Either<TLeft, Either<TLeft, TRight>> other)
        {
            _right = other._right._right;
            _left = other._left;
            _hasLeft = other._hasLeft;
        }
        
        public Either(Either<Either<TLeft, TRight>, Either<TLeft, TRight>> other)
        {
            _right = other._right._right;
            _left = other._left._left;
            _hasLeft = other._hasLeft;
        }
        
        private Either(TLeft left, TRight right, bool hasLeft)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(left == null);
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(right == null);
            
            _left = left;
            _right = right;
            _hasLeft = hasLeft;
        }

        #endregion

        public override int GetHashCode()
        {
            return _hasLeft ? _left.GetHashCode() : _right.GetHashCode();
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            if (obj is Either<TLeft, TRight>)
                return Equals((Either<TLeft, TRight>) obj);
            
            if (obj is Either<TRight, TLeft>)
                return Equals(((Either<TRight, TLeft>) obj).Swap);

            if (_hasLeft && obj is TLeft)
                return _left.Equals((TLeft) obj);

            if (!_hasLeft && obj is TRight)
                return _right.Equals((TRight) obj);
            
            return false;
        }
        
        public bool Equals(Either<TLeft, TRight> other)
        {
            if (_hasLeft)
                return other._hasLeft && _left.Equals(other._left);
            
            return !other._hasLeft && _right.Equals(other._right);
        }

        public bool Equals(Either<TRight, TLeft> other)
        {
            if (_hasLeft)
                return !other._hasLeft && _left.Equals(other._right);

            return other._hasLeft && _right.Equals(other._left);
        }

        #endregion

        #region CompareTo

        public int CompareTo(Either<TLeft, TRight> other)
        {
            if (_hasLeft)
                return other._hasLeft ? Comparer<TLeft>.Default.Compare(_left, other._left) : 1;

            return !other._hasLeft ? Comparer<TRight>.Default.Compare(_right, other._right) : -1;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            
            if (obj is Either<TLeft, TRight>)
                return CompareTo((Either<TLeft, TRight>) obj);

            if (obj is Either<TRight, TLeft>)
                return CompareTo(((Either<TRight, TLeft>) obj).Swap);

            if (_hasLeft && obj is TLeft)
                return Comparer<TLeft>.Default.Compare(_left, (TLeft) obj);

            if (!_hasLeft && obj is TRight)
                return Comparer<TRight>.Default.Compare(_right, (TRight) obj);

            return -1;
        }

        #endregion

        #region Equality operators

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.Equals(right);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return !left.Equals(right);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator <(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.CompareTo(right) == -1;
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator >(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.CompareTo(right) == 1;
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator <=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.CompareTo(right) != 1;
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool operator >=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.CompareTo(right) != -1;
        }

        #endregion

        #region Operators
        
        /// <summary>
        /// Returns the result of applying handler to this Either value if the Either has left branch.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="left">Function for applying, if either has Left</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R FoldLeft<R>(R init, Func<TLeft, R, R> left)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
#endif
            return _hasLeft ? left(_left, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Either value if the Either has right branch.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="right">Function for applying, if either has Right</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R FoldRight<R>(R init, Func<TRight, R, R> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft == false ? right(_right, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Either value,
        /// if the Either has left branch, then apply left handler for _left branch value
        /// Otherwise, return result, from apply func for right branch value.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="left">Function for applying, if either has Left</param>
        /// <param name="right">Function for applying, if either has Right</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R Fold<R>(R init, Func<TLeft, R, R> left, Func<TRight, R, R> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft ? left(_left, init) : right(_right, init);
        }
        
        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain left branch.
        /// Returns Either without apply handler if this Either has right branch.
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="left">Applying func</param>
        /// <typeparam name="R">New left type</typeparam>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<R, TRight> FlatMapLeft<R>(Func<TLeft, Either<R, TRight>> left)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
#endif
            return _hasLeft ? left(_left) : new Either<R, TRight>(default(R), _right, _hasLeft);
        } 

        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain right branch.
        /// Returns Either without apply handler if this Either has left branch.
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="right">Applying func</param>
        /// <typeparam name="R">New right type</typeparam>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<TLeft, R> FlatMapRight<R>(Func<TRight, Either<TLeft, R>> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft == false ? right(_right) : new Either<TLeft, R>(_left, default(R), _hasLeft);
        } 

        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain left branch, then apply left handler, otherwise - right
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="left">Applying func for left branch</param>
        /// <param name="right">Applying func for right branch</param>
        /// <typeparam name="L">New left type</typeparam>
        /// <typeparam name="R">New right type</typeparam>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<L, R> FlatMap<L, R>(Func<TLeft, Either<L, R>> left, Func<TRight, Either<L, R>> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft ? left(_left) : right(_right);
        }
        
        /// <summary>
        /// Apply handler for value if Either has left branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="L">New hold left type</typeparam>
        /// <returns>Return right if has right branch.</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<L, TRight> MapLeft<L>(Func<TLeft, L> handler)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(handler);
#endif
            return _hasLeft ? 
                handler(_left).ToLeft<L, TRight>() : 
                new Either<L, TRight>(default(L), _right, _hasLeft);
        }

        /// <summary>
        /// Apply handler for value if Either has right branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="R">New hold right type</typeparam>
        /// <returns>Return right if has left branch.</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<TLeft, R> MapRight<R>(Func<TRight, R> handler)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(handler);
#endif
            return _hasLeft == false ? 
                handler(_right).ToRight<TLeft, R>() : 
                new Either<TLeft, R>(_left, default(R), _hasLeft);
        }

        /// <summary>
        /// Apply handler for value if Either has left branch, then apply left handler,
        /// otherwise - right
        /// </summary>
        /// <param name="left">Function for left branch applying</param>
        /// <param name="right">Function for right branch applying</param>
        /// <typeparam name="L">New hold left type</typeparam>
        /// <typeparam name="R">New hold right type</typeparam>
        /// <returns>Return Either with new type, who contain old branch(left or right).</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Either<L, R> Map<L, R>(Func<TLeft, L> left, Func<TRight, R> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft ? new Either<L, R>(left(_left)) : new Either<L, R>(right(_right));
        }
        
        /// <summary>
        /// Returns true if this either has left branch and the
        /// predicate returns true when applied to this left value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ExistLeft(Func<TLeft, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return _hasLeft && predicate(_left);
        }

        /// <summary>
        /// Returns true if this either has right branch and the
        /// predicate returns true when applied to this right value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ExistRight(Func<TRight, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return _hasLeft == false && predicate(_right);
        }

        /// <summary>
        /// Returns true if right or left predicate return true.
        /// If Either contain left, then check predicateLeft, otherwise - predicatRight
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool Exist(Func<TLeft, bool> predicateLeft, Func<TRight, bool> predicateRight)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicateLeft);
            ExceptionUtility.NullPredicatCheck(predicateRight);
#endif
            return _hasLeft ? predicateLeft(_left) : predicateRight(_right);
        }
        
        /// <summary>
        /// Returns true if this either hasn't left or the
        /// predicate returns true when applied to this left value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ForallLeft(Func<TLeft, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return !_hasLeft || predicate(_left);
        }
        
        /// <summary>
        /// Returns true if this either hasn't right or the
        /// predicate returns true when applied to this right value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ForallRight(Func<TRight, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return _hasLeft || predicate(_right);
        }
        
        /// <summary>
        /// Returns true if this either has right and the
        /// compare returns 0 when compare right value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ContainRight(TRight val)
        {
            return _hasLeft  == false && Comparer<TRight>.Default.Compare(_right, val) == 0;
        }

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ContainLeft(TLeft val)
        {
            return _hasLeft && Comparer<TLeft>.Default.Compare(_left, val) == 0;
        }

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with leftVal,
        /// or compare return 0 when compare right with rightVal
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="leftVal">Value for left branch compare</param>
        /// <param name="rightVal">Value for right branch compare</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contain(TLeft leftVal, TRight rightVal)
        {
            if(_hasLeft)
                return Comparer<TLeft>.Default.Compare(_left, leftVal) == 0;
            
            return Comparer<TRight>.Default.Compare(_right, rightVal) == 0;
        }
        
        /// <summary>
        /// Returns Some Option if it Either has left branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Option<TLeft> FilterLeft(Func<TLeft, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return _hasLeft && predicate(_left) ? _left.ToOption() : Optional.None<TLeft>();
        }

        /// <summary>
        /// Returns Some Option if it Either has right branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Option<TRight> FilterRight(Func<TRight, bool> predicate)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicate);
#endif
            return !_hasLeft && predicate(_right) ? _right.ToOption() : Optional.None<TRight>();
        }
        
        /// <summary>
        /// Returns Some Option if it Either has left branch and applying the predicateLeft to
        /// this value returns true, or predicateRight return true for right branch. Otherwise, return None.
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Option<Either<TLeft, TRight>> Filter(Func<TLeft, bool> predicateLeft, Func<TRight, bool> predicateRight)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicateLeft);
            ExceptionUtility.NullPredicatCheck(predicateRight);
#endif
            if (_hasLeft)
            {
                if (predicateLeft(_left))
                    return this.ToOption();
            }
            else if (predicateRight(_right))
            {
                return this.ToOption();
            }
            
            return Optional.None<Either<TLeft, TRight>>();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if either - right</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOr(TLeft defaultVal)
        {
            return _hasLeft ? _left : defaultVal;
        }
        
        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - default value for type. May return null, if TLeft - class
        /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOrDefault()
        {
            return _hasLeft ? _left : default(TLeft);
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise use factory function, for create return value.
        /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOr(Func<TLeft> defaultValFactory)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(defaultValFactory);
#endif
            return _hasLeft ? _left : defaultValFactory();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOrThrow()
        {
            if(!_hasLeft)
                EitherExceptions.NotFound();
            
            return _left;
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if either - left</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOr(TRight defaultVal)
        {
            return _hasLeft == false ? _right : defaultVal;
        }

        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise - default value for type. May return null, if TRight - class
        /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOrDefault()
        {
            return _hasLeft == false ? _right : default(TRight);
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise use factory function, for create return value.
        /// </summary>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOr(Func<TRight> defaultValFactory)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(defaultValFactory);
#endif
            return _hasLeft == false ? _right : defaultValFactory();
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOrThrow()
        {
            if(_hasLeft)
                EitherExceptions.NotFound();
            
            return _right;
        }    
        
        #endregion

        #region Pattern matching

        /// <summary>
        /// Pattern matching for either
        /// </summary>
        /// <param name="left">Action run, if either has left branch</param>
        /// <param name="right">Action run, if either has right branch</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Unit Match(Action<TLeft> left, Action<TRight> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(right);
#endif
            if (_hasLeft)
                    left(_left);
            else
                    right(_right);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="right">Func run, if either has right branch</param>
        /// <returns>Return value from left, if either has left branch, otherwise from right handler</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R Match<R>(Func<TLeft, R> left, Func<TRight, R> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft ? left(_left) : right(_right);
        }
                
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Return if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise - nonLeft value</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R MatchLeft<R>(Func<TLeft, R> left, R nonLeft)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
#endif
            return _hasLeft ? left(_left) : nonLeft;
        }
        
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Run this factory, for create return value, if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise create value from nonLeft function</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R MatchLeft<R>(Func<TLeft, R> left, Func<R> nonLeft)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
            ExceptionUtility.NullHandlerCheck(nonLeft);
#endif
            return _hasLeft ? left(_left) : nonLeft();
        }

        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Action run, if either has left branch</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Unit MatchLeft(Action<TLeft> left)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(left);
#endif
            if (_hasLeft)
                    left(_left);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Action run, if either has right branch</param>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public Unit MatchRight(Action<TRight> right)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(right);
#endif
            if (!_hasLeft)
                    right(_right);
            
            return Unit.Def;
        }
        
        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Func run, if either has right branch</param>
        /// <param name="nonRight">Return if either - Left</param>
        /// <returns>If either - right, return value from right, otherwise - nonRight value</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R MatchRight<R>(Func<TRight, R> right, R nonRight)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(right);
#endif
            return _hasLeft == false ? right(_right) : nonRight;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Func run, if either has right branch</param>
        /// <param name="nonRight">Run this factory, for create return value, if either - Left</param>
        /// <returns>If either - right, return value from left, otherwise create value from nonRight function</returns>
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public R MatchRight<R>(Func<TRight, R> right, Func<R> nonRight)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullHandlerCheck(right);
            ExceptionUtility.NullHandlerCheck(nonRight);
#endif
            return _hasLeft == false ? right(_right) : nonRight();
        }
        
        #endregion
        
        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }

        public IEnumerator<Either<TLeft, TRight>> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerable<TLeft> GetLeftEnumerator()
        {
            if (_hasLeft)
                yield return _left;
        }
        
        IEnumerable<TRight> GetRightEnumerator()
        {
            if (_hasLeft == false)
                yield return _right;
        }
        
        public override string ToString()
        {
            return _hasLeft ? 
                string.Concat("Left(", _left.ToString(), ")") :
                string.Concat("Right(", _right.ToString(), ")");
        }
    }
}