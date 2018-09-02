using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Apophis.Types.Core;
using Apophis.Types.Enums;
using Apophis.Types.Exceptions;
using Apophis.Types.Extensions;
using Apophis.Types.Mixin;
using Apophis.Types.Monads.Option;
using Apophis.Types.Policys.Check;

namespace Apophis.Types.Monads.Either
{
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct Either<TLeft, TRight, TCheckPolicy> : 
        IEquatable<Either<TLeft, TRight, TCheckPolicy>>,
        IComparable<Either<TLeft, TRight, TCheckPolicy>>,
        IComparable,
        IEnumerable<Either<TLeft, TRight, TCheckPolicy>>,
        ITypeClass<EitherType>
        where TCheckPolicy : struct, ICheckPolicy
    {
        private TLeft _left;
        private TRight _right;
        
        private bool _hasLeft;

        public EitherType Type
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft ? EitherType.Left : EitherType.Right;
            }
        }

        public bool IsLeft
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft;
            }
        }

        public bool IsRight
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft == false;
            }
        }

        public Option<TLeft, SafePolicy> Left
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft ? _left.ToSafeOption() : Optional.None<TLeft, SafePolicy>();
            }
        }
        
        public Option<TRight, SafePolicy> Right
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft == false ? _right.ToSafeOption() : Optional.None<TRight, SafePolicy>();
            }
        }
        
        public Either<TRight, TLeft, TCheckPolicy> Swap
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Either<TRight, TLeft, TCheckPolicy>(_right, _left, !_hasLeft);
            }
        }
        
        #region Constructors
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either(TLeft left)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(left == null);
            
            _left = left;
            _right = default(TRight);
            _hasLeft = true;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either(TRight right)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(right == null);
            
            _right = right;
            _left = default(TLeft);
            _hasLeft = false;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Either(Either<TLeft, TRight, TCheckPolicy> oth)
        {
            _left = oth._left;
            _right = oth._right;
            _hasLeft = oth._hasLeft;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Either(TLeft left, TRight right, bool hasLeft)
        {
            _left = left;
            _right = right;
            _hasLeft = hasLeft;
        }

        #endregion

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _hasLeft ? _left.GetHashCode() : _right.GetHashCode();

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            if (obj is Either<TLeft, TRight, TCheckPolicy>)
                return Equals((Either<TLeft, TRight, TCheckPolicy>) obj);
            
            if (obj is Either<TRight, TLeft, TCheckPolicy>)
                return Equals(((Either<TRight, TLeft, TCheckPolicy>) obj).Swap);

            if (_hasLeft && obj is TLeft)
                return _left.Equals((TLeft) obj);

            if (!_hasLeft && obj is TRight)
                return _right.Equals((TRight) obj);
            
            return false;
        }
        
        public bool Equals(Either<TLeft, TRight, TCheckPolicy> other)
        {
            if (_hasLeft)
                return other._hasLeft && _left.Equals(other._left);
            
            return !other._hasLeft && _right.Equals(other._right);
        }

        public bool Equals(Either<TRight, TLeft, TCheckPolicy> other)
        {
            if (_hasLeft)
                return !other._hasLeft && _left.Equals(other._right);

            return other._hasLeft && _right.Equals(other._left);
        }

        #endregion

        #region CompareTo

        public int CompareTo(Either<TLeft, TRight, TCheckPolicy> other)
        {
            if (_hasLeft)
                return other._hasLeft ? Comparer<TLeft>.Default.Compare(_left, other._left) : 1;

            return !other._hasLeft ? Comparer<TRight>.Default.Compare(_right, other._right) : -1;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            
            if (obj is Either<TLeft, TRight, TCheckPolicy>)
                return CompareTo((Either<TLeft, TRight, TCheckPolicy>) obj);

            if (obj is Either<TRight, TLeft, TCheckPolicy>)
                return CompareTo(((Either<TRight, TLeft, TCheckPolicy>) obj).Swap);

            if (_hasLeft && obj is TLeft)
                return Comparer<TLeft>.Default.Compare(_left, (TLeft) obj);

            if (!_hasLeft && obj is TRight)
                return Comparer<TRight>.Default.Compare(_right, (TRight) obj);

            return -1;
        }

        #endregion

        #region Equality operators

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right) 
            => left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right)
            => !left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right)
         => left.CompareTo(right) == -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right)
         => left.CompareTo(right) == 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right)
         => left.CompareTo(right) != 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Either<TLeft, TRight, TCheckPolicy> left, Either<TLeft, TRight, TCheckPolicy> right)
         => left.CompareTo(right) != -1;

        #endregion

        #region Operators
        
        /// <summary>
        /// Returns the result of applying handler to this Either value if the Either has left branch.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="left">Function for applying, if either has Left</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldLeft<R>(R init, Func<TLeft, R, R> left)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_left, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Either value if the Either has right branch.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="right">Function for applying, if either has Right</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R FoldRight<R>(R init, Func<TRight, R, R> right)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R Fold<R>(R init, Func<TLeft, R, R> left, Func<TRight, R, R> right)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<R, TRight, TOthCheckPolicy> FlatMapLeft<R, TOthCheckPolicy>(Func<TLeft, Either<R, TRight, TOthCheckPolicy>> left)
            where TOthCheckPolicy : struct, ICheckPolicy
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_left) : new Either<R, TRight, TOthCheckPolicy>(default(R), _right, _hasLeft);
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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<TLeft, R, TOthCheckPolicy> FlatMapRight<R, TOthCheckPolicy>(Func<TRight, Either<TLeft, R, TOthCheckPolicy>> right)
            where TOthCheckPolicy : struct, ICheckPolicy
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            return _hasLeft == false ? right(_right) : new Either<TLeft, R, TOthCheckPolicy>(_left, default(R), _hasLeft);
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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<L, R, TOthCheckPolicy> FlatMap<L, R, TOthCheckPolicy>(Func<TLeft, Either<L, R, TOthCheckPolicy>> left, Func<TRight, Either<L, R, TOthCheckPolicy>> right)
            where TOthCheckPolicy : struct, ICheckPolicy
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? left(_left) : right(_right);
        }
        
        /// <summary>
        /// Apply handler for value if Either has left branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="L">New hold left type</typeparam>
        /// <returns>Return right if has right branch.</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<L, TRight, TCheckPolicy> MapLeft<L>(Func<TLeft, L> handler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return _hasLeft ? 
                handler(_left).ToLeft<L, TRight, TCheckPolicy>() : 
                new Either<L, TRight, TCheckPolicy>(default(L), _right, _hasLeft);
        }

        /// <summary>
        /// Apply handler for value if Either has right branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="R">New hold right type</typeparam>
        /// <returns>Return right if has left branch.</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<TLeft, R, TCheckPolicy> MapRight<R>(Func<TRight, R> handler)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return _hasLeft == false ? 
                handler(_right).ToRight<TLeft, R, TCheckPolicy>() : 
                new Either<TLeft, R, TCheckPolicy>(_left, default(R), _hasLeft);
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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Either<L, R, TCheckPolicy> Map<L, R>(Func<TLeft, L> left, Func<TRight, R> right)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? new Either<L, R, TCheckPolicy>(left(_left)) : new Either<L, R, TCheckPolicy>(right(_right));
        }
        
        /// <summary>
        /// Returns true if this either has left branch and the
        /// predicate returns true when applied to this left value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ExistLeft(Func<TLeft, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft && predicate(_left);
        }

        /// <summary>
        /// Returns true if this either has right branch and the
        /// predicate returns true when applied to this right value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ExistRight(Func<TRight, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft == false && predicate(_right);
        }

        /// <summary>
        /// Returns true if right or left predicate return true.
        /// If Either contain left, then check predicateLeft, otherwise - predicatRight
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Exist(Func<TLeft, bool> predicateLeft, Func<TRight, bool> predicateRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullPredicatCheck(predicateLeft);
                ExceptionUtility.NullPredicatCheck(predicateRight);
            }

            return _hasLeft ? predicateLeft(_left) : predicateRight(_right);
        }
        
        /// <summary>
        /// Returns true if this either hasn't left or the
        /// predicate returns true when applied to this left value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ForallLeft(Func<TLeft, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return !_hasLeft || predicate(_left);
        }
        
        /// <summary>
        /// Returns true if this either hasn't right or the
        /// predicate returns true when applied to this right value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ForallRight(Func<TRight, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);
            
            return _hasLeft || predicate(_right);
        }
        
        /// <summary>
        /// Returns true if this either has right and the
        /// compare returns 0 when compare right value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ContainRight(TRight val) => _hasLeft  == false && Comparer<TRight>.Default.Compare(_right, val) == 0;

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ContainLeft(TLeft val) => _hasLeft && Comparer<TLeft>.Default.Compare(_left, val) == 0;

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with leftVal,
        /// or compare return 0 when compare right with rightVal
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="leftVal">Value for left branch compare</param>
        /// <param name="rightVal">Value for right branch compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<TLeft, SafePolicy> FilterLeft(Func<TLeft, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft && predicate(_left) ? _left.ToSafeOption() : Optional.None<TLeft, SafePolicy>();
        }

        /// <summary>
        /// Returns Some Option if it Either has right branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<TRight, SafePolicy> FilterRight(Func<TRight, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);
            
            return !_hasLeft && predicate(_right) ? _right.ToSafeOption() : Optional.None<TRight, SafePolicy>();
        }
        
        /// <summary>
        /// Returns Some Option if it Either has left branch and applying the predicateLeft to
        /// this value returns true, or predicateRight return true for right branch. Otherwise, return None.
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
        public Option<Either<TLeft, TRight, TCheckPolicy>, SafePolicy> Filter(Func<TLeft, bool> predicateLeft, Func<TRight, bool> predicateRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullPredicatCheck(predicateLeft);
                ExceptionUtility.NullPredicatCheck(predicateRight);
            }

            if (_hasLeft)
            {
                if (predicateLeft(_left))
                    return this.ToSafeOption();
            }
            else if (predicateRight(_right))
            {
                return this.ToSafeOption();
            }
            
            return Optional.None<Either<TLeft, TRight, TCheckPolicy>, SafePolicy>();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if either - right</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TLeft LeftOr(TLeft defaultVal) => _hasLeft ? _left : defaultVal;
        
        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - default value for type. May return null, if TLeft - class
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TLeft LeftOrDefault() => _hasLeft ? _left : default(TLeft);

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise use factory function, for create return value.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TLeft LeftOr(Func<TLeft> defaultValFactory)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(defaultValFactory);

            return _hasLeft ? _left : defaultValFactory();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TRight RightOr(TRight defaultVal) => _hasLeft == false ? _right : defaultVal;

        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise - default value for type. May return null, if TRight - class
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TRight RightOrDefault() => _hasLeft == false ? _right : default(TRight);
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise use factory function, for create return value.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TRight RightOr(Func<TRight> defaultValFactory)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(defaultValFactory);

            return _hasLeft == false ? _right : defaultValFactory();
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
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
        public Unit Match(Action<TLeft> left, Action<TRight> right)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<TLeft, R> left, Func<TRight, R> right)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? left(_left) : right(_right);
        }
                
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Return if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise - nonLeft value</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchLeft<R>(Func<TLeft, R> left, R nonLeft)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_left) : nonLeft;
        }
        
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Run this factory, for create return value, if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise create value from nonLeft function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchLeft<R>(Func<TLeft, R> left, Func<R> nonLeft)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(nonLeft);
            }

            return _hasLeft ? left(_left) : nonLeft();
        }

        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Action run, if either has left branch</param>
        public Unit MatchLeft(Action<TLeft> left)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            if (_hasLeft)
                    left(_left);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Action run, if either has right branch</param>
        public Unit MatchRight(Action<TRight> right)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

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
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchRight<R>(Func<TRight, R> right, R nonRight)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            return _hasLeft == false ? right(_right) : nonRight;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Func run, if either has right branch</param>
        /// <param name="nonRight">Run this factory, for create return value, if either - Left</param>
        /// <returns>If either - right, return value from left, otherwise create value from nonRight function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R MatchRight<R>(Func<TRight, R> right, Func<R> nonRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(right);
                ExceptionUtility.NullHandlerCheck(nonRight);
            }
            
            return _hasLeft == false ? right(_right) : nonRight();
        }
        
        #endregion
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Either<TLeft, TRight, TCheckPolicy>(TLeft left) => new Either<TLeft, TRight, TCheckPolicy>(left);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Either<TLeft, TRight, TCheckPolicy>(TRight right) => new Either<TLeft, TRight, TCheckPolicy>(right);

        public IEnumerator<Either<TLeft, TRight, TCheckPolicy>> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _hasLeft ? 
            string.Concat("Left(", _left.ToString(), ")") :
            string.Concat("Right(", _right.ToString(), ")");

        #region From Other Either

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckInPolicy> Of<TCheckOutPolicy, TCheckInPolicy>(Either<Either<TLeft, TRight, TCheckInPolicy>, TRight, TCheckOutPolicy> other)
            where TCheckOutPolicy : struct, ICheckPolicy 
            where TCheckInPolicy : struct, ICheckPolicy 
            => new Either<TLeft, TRight, TCheckInPolicy>(other._left._left, other._right, other._hasLeft && other._left._hasLeft);
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckInPolicy> Of<TCheckOutPolicy, TCheckInPolicy>(Either<TLeft, Either<TLeft, TRight, TCheckInPolicy>, TCheckOutPolicy> other)
        where TCheckOutPolicy : struct, ICheckPolicy 
        where TCheckInPolicy : struct, ICheckPolicy 
            => new Either<TLeft, TRight, TCheckInPolicy>(other._left, other._right._right, other._hasLeft || other._right._hasLeft);
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight, TCheckHoldPolicy> Of<TCheckLeftPolicy, TCheckRightPolicy, TCheckHoldPolicy>(Either<Either<TLeft, TRight, TCheckLeftPolicy>, Either<TLeft, TRight, TCheckRightPolicy>, TCheckHoldPolicy> other)
            where TCheckLeftPolicy : struct, ICheckPolicy 
            where TCheckRightPolicy : struct, ICheckPolicy 
            where TCheckHoldPolicy : struct, ICheckPolicy 
            => new Either<TLeft, TRight, TCheckHoldPolicy>(other._left._left, other._right._right, other._hasLeft ? other._left._hasLeft : other._right._hasLeft);

        #endregion
    }
}