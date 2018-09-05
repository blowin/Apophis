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
    public struct EitherRef<L, R, TCheckPolicy> : 
        IEquatable<EitherRef<L, R, TCheckPolicy>>,
        IComparable<EitherRef<L, R, TCheckPolicy>>,
        IComparable,
        IEnumerable<EitherRef<L, R, TCheckPolicy>>,
        ITypeClass<EitherType>
        where L : class
        where R : class
        where TCheckPolicy : struct, ICheckPolicy
    {
        private Union _data;
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

        public Option<L, TCheckPolicy> Left
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft ? _data.Left.ToOption<L, TCheckPolicy>() : Optional.None<L, TCheckPolicy>();
            }
        }
        
        public Option<R, TCheckPolicy> Right
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return _hasLeft == false ? _data.Right.ToOption<R, TCheckPolicy>() : Optional.None<R, TCheckPolicy>();
            }
        }
        
        public EitherRef<R, L, TCheckPolicy> Swap
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {
                return new EitherRef<R, L, TCheckPolicy>(_data.Right, _data.Left, !_hasLeft);
            }
        }
        
        #region Constructors
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef(L left)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(left == null);

            _data = new Union(left);
            _hasLeft = true;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef(R right)
        {
            ExceptionUtility.ThrowIfTrue<EitherExceptions.EitherNullRef>(right == null);
            
            _data = new Union(right);
            _hasLeft = false;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EitherRef(EitherRef<L, R, TCheckPolicy> oth)
        {
            _data = oth._data;
            _hasLeft = oth._hasLeft;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EitherRef(L left, R right, bool hasLeft)
        {
            _data = hasLeft ? new Union(left) : new Union(right);
            _hasLeft = hasLeft;
        }

        #endregion

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _hasLeft ? _data.Left.GetHashCode() : _data.Right.GetHashCode();

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            if (obj is EitherRef<L, R, TCheckPolicy>)
                return Equals((EitherRef<L, R, TCheckPolicy>) obj);
            
            if (obj is EitherRef<R, L, TCheckPolicy>)
                return Equals((EitherRef<R, L, TCheckPolicy>) obj);

            if (_hasLeft && obj is L)
                return _data.Left.Equals((L) obj);

            if (!_hasLeft && obj is R)
                return _data.Right.Equals((R) obj);
            
            return false;
        }
        
        public bool Equals(EitherRef<L, R, TCheckPolicy> other)
        {
            if (_hasLeft)
                return other._hasLeft && _data.Left.Equals(other._data.Left);
            
            return !other._hasLeft && _data.Right.Equals(other._data.Right);
        }

        public bool Equals(EitherRef<R, L, TCheckPolicy> other)
        {
            if (_hasLeft)
                return !other._hasLeft && _data.Left.Equals(other._data.Right);

            return other._hasLeft && _data.Right.Equals(other._data.Left);
        }

        #endregion

        #region CompareTo

        public int CompareTo(EitherRef<L, R, TCheckPolicy> other)
        {
            if (_hasLeft)
                return other._hasLeft ? Comparer<L>.Default.Compare(_data.Left, other._data.Left) : 1;

            return !other._hasLeft ? Comparer<R>.Default.Compare(_data.Right, other._data.Right) : -1;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            
            if (obj is EitherRef<L, R, TCheckPolicy>)
                return CompareTo((EitherRef<L, R, TCheckPolicy>) obj);

            if (obj is EitherRef<R, L, TCheckPolicy>)
                return CompareTo(((EitherRef<R, L, TCheckPolicy>) obj).Swap);

            if (_hasLeft && obj is L)
                return Comparer<L>.Default.Compare(_data.Left, (L) obj);

            if (!_hasLeft && obj is R)
                return Comparer<R>.Default.Compare(_data.Right, (R) obj);

            return -1;
        }

        #endregion

        #region Equality operators

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right) 
            => left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right)
            => !left.Equals(right);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right)
         => left.CompareTo(right) == -1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right)
         => left.CompareTo(right) == 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right)
         => left.CompareTo(right) != 1;

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(EitherRef<L, R, TCheckPolicy> left, EitherRef<L, R, TCheckPolicy> right)
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
        public Ret FoldLeft<Ret>(Ret init, Func<L, Ret, Ret> left)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_data.Left, init) : init;
        }

        /// <summary>
        /// Returns the result of applying handler to this Either value if the Either has right branch.
        /// Otherwise, return init.
        /// </summary>
        /// <param name="init">Initial value</param>
        /// <param name="right">Function for applying, if either has Right</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Ret FoldRight<Ret>(Ret init, Func<R, Ret, Ret> right)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            return _hasLeft == false ? right(_data.Right, init) : init;
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
        public Ret Fold<Ret>(Ret init, Func<L, Ret, Ret> left, Func<R, Ret, Ret> right)
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? left(_data.Left, init) : right(_data.Right, init);
        }
        
        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain left branch.
        /// Returns Either without apply handler if this Either has right branch.
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="left">Applying func</param>
        /// <typeparam name="Ret">New left type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<Ret, R, TOthCheckPolicy> FlatMapLeft<Ret, TOthCheckPolicy>(Func<L, EitherRef<Ret, R, TOthCheckPolicy>> left)
            where TOthCheckPolicy : struct, ICheckPolicy 
            where Ret : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_data.Left) : new EitherRef<Ret, R, TOthCheckPolicy>(default(Ret), _data.Right, _hasLeft);
        } 

        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain right branch.
        /// Returns Either without apply handler if this Either has left branch.
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="right">Applying func</param>
        /// <typeparam name="Ret">New right type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<L, Ret, TOthCheckPolicy> FlatMapRight<Ret, TOthCheckPolicy>(Func<R, EitherRef<L, Ret, TOthCheckPolicy>> right)
            where TOthCheckPolicy : struct, ICheckPolicy 
            where Ret : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            return _hasLeft == false ? right(_data.Right) : new EitherRef<L, Ret, TOthCheckPolicy>(_data.Left, default(Ret), _hasLeft);
        } 

        /// <summary>
        /// Returns the result of applying handler to this Either value if
        /// this Either contain left branch, then apply left handler, otherwise - right
        /// Slightly different from `map` in that handler is expected to
        /// return an Either
        /// </summary>
        /// <param name="left">Applying func for left branch</param>
        /// <param name="right">Applying func for right branch</param>
        /// <typeparam name="TNL">New left type</typeparam>
        /// <typeparam name="TNR">New right type</typeparam>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<TNL, TNR, TOthCheckPolicy> FlatMap<TNL, TNR, TOthCheckPolicy>(Func<L, EitherRef<TNL, TNR, TOthCheckPolicy>> left, Func<R, EitherRef<TNL, TNR, TOthCheckPolicy>> right)
            where TOthCheckPolicy : struct, ICheckPolicy 
            where TNL : class 
            where TNR : class
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? left(_data.Left) : right(_data.Right);
        }
        
        /// <summary>
        /// Apply handler for value if Either has left branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="TNL">New hold left type</typeparam>
        /// <returns>Return right if has right branch.</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<TNL, R, TCheckPolicy> MapLeft<TNL>(Func<L, TNL> handler) 
            where TNL : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);
            return _hasLeft ? 
                new EitherRef<TNL, R, TCheckPolicy>(handler(_data.Left)) : 
                new EitherRef<TNL, R, TCheckPolicy>(_data.Right);
        }

        /// <summary>
        /// Apply handler for value if Either has right branch
        /// </summary>
        /// <param name="handler">Function for applying</param>
        /// <typeparam name="TNR">New hold right type</typeparam>
        /// <returns>Return right if has left branch.</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<L, TNR, TCheckPolicy> MapRight<TNR>(Func<R, TNR> handler) 
            where TNR : class
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(handler);

            return _hasLeft == false ? 
                new EitherRef<L, TNR, TCheckPolicy>(handler(_data.Right)) : 
                new EitherRef<L, TNR, TCheckPolicy>(_data.Left);
        }

        /// <summary>
        /// Apply handler for value if Either has left branch, then apply left handler,
        /// otherwise - right
        /// </summary>
        /// <param name="left">Function for left branch applying</param>
        /// <param name="right">Function for right branch applying</param>
        /// <typeparam name="TNL">New hold left type</typeparam>
        /// <typeparam name="TNR">New hold right type</typeparam>
        /// <returns>Return Either with new type, who contain old branch(left or right).</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public EitherRef<TNL, TNR, TCheckPolicy> Map<TNL, TNR>(Func<L, TNL> left, Func<R, TNR> right) 
            where TNR : class 
            where TNL : class
        {
            if (default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? 
                new EitherRef<TNL, TNR, TCheckPolicy>(left(_data.Left)) : 
                new EitherRef<TNL, TNR, TCheckPolicy>(right(_data.Right));
        }
        
        /// <summary>
        /// Returns true if this either has left branch and the
        /// predicate returns true when applied to this left value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ExistLeft(Func<L, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft && predicate(_data.Left);
        }

        /// <summary>
        /// Returns true if this either has right branch and the
        /// predicate returns true when applied to this right value.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ExistRight(Func<R, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft == false && predicate(_data.Right);
        }

        /// <summary>
        /// Returns true if right or left predicate return true.
        /// If Either contain left, then check predicateLeft, otherwise - predicatRight
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Exist(Func<L, bool> predicateLeft, Func<R, bool> predicateRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullPredicatCheck(predicateLeft);
                ExceptionUtility.NullPredicatCheck(predicateRight);
            }

            return _hasLeft ? predicateLeft(_data.Left) : predicateRight(_data.Right);
        }
        
        /// <summary>
        /// Returns true if this either hasn't left or the
        /// predicate returns true when applied to this left value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ForallLeft(Func<L, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return !_hasLeft || predicate(_data.Left);
        }
        
        /// <summary>
        /// Returns true if this either hasn't right or the
        /// predicate returns true when applied to this right value.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ForallRight(Func<R, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);
            
            return _hasLeft || predicate(_data.Right);
        }
        
        /// <summary>
        /// Returns true if this either has right and the
        /// compare returns 0 when compare right value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ContainRight(R val) => _hasLeft  == false && Comparer<R>.Default.Compare(_data.Right, val) == 0;

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with val.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="val">Value for compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ContainLeft(L val) => _hasLeft && Comparer<L>.Default.Compare(_data.Left, val) == 0;

        /// <summary>
        /// Returns true if this either has left and the
        /// compare returns 0 when compare left value with leftVal,
        /// or compare return 0 when compare right with rightVal
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="leftVal">Value for left branch compare</param>
        /// <param name="rightVal">Value for right branch compare</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Contain(L leftVal, R rightVal)
        {
            if(_hasLeft)
                return Comparer<L>.Default.Compare(_data.Left, leftVal) == 0;
            
            return Comparer<R>.Default.Compare(_data.Right, rightVal) == 0;
        }
        
        /// <summary>
        /// Returns Some Option if it Either has left branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<L, TCheckPolicy> FilterLeft(Func<L, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);

            return _hasLeft && predicate(_data.Left) ? _data.Left.ToOption<L, TCheckPolicy>() : Optional.None<L, TCheckPolicy>();
        }

        /// <summary>
        /// Returns Some Option if it Either has right branch and applying the predicate to
        /// this value returns true. Otherwise, return None.
        /// </summary>
        /// <param name="predicate">Predicate for testing</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Option<R, TCheckPolicy> FilterRight(Func<R, bool> predicate)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullPredicatCheck(predicate);
            
            return !_hasLeft && predicate(_data.Right) ? _data.Right.ToOption<R, TCheckPolicy>(): Optional.None<R, TCheckPolicy>();
        }
        
        /// <summary>
        /// Returns Some Option if it Either has left branch and applying the predicateLeft to
        /// this value returns true, or predicateRight return true for right branch. Otherwise, return None.
        /// </summary>
        /// <param name="predicateLeft">Predicate for left branch testing</param>
        /// <param name="predicateRight">Predicate for right branch testing</param>
        public Option<EitherRef<L, R, TCheckPolicy>, TCheckPolicy> Filter(Func<L, bool> predicateLeft, Func<R, bool> predicateRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullPredicatCheck(predicateLeft);
                ExceptionUtility.NullPredicatCheck(predicateRight);
            }

            if (_hasLeft)
            {
                if (predicateLeft(_data.Left))
                    return this.ToOption<EitherRef<L, R, TCheckPolicy>, TCheckPolicy>();
            }
            else if (predicateRight(_data.Right))
            {
                return this.ToOption<EitherRef<L, R, TCheckPolicy>, TCheckPolicy>();
            }
            
            return Optional.None<EitherRef<L, R, TCheckPolicy>, TCheckPolicy>();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if either - right</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public L LeftOr(L defaultVal) => _hasLeft ? _data.Left : defaultVal;
        
        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise - default value for type. May return null, if TLeft - class
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public L LeftOrDefault() => _hasLeft ? _data.Left : default(L);

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise use factory function, for create return value.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public L LeftOr(Func<L> defaultValFactory)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(defaultValFactory);

            return _hasLeft ? _data.Left : defaultValFactory();
        }

        /// <summary>
        /// If either - Left, return _left.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
        public L LeftOrThrow()
        {
            if(!_hasLeft)
                EitherExceptions.NotFound();
            
            return _data.Left;
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise - defaultValue
        /// </summary>
        /// <param name="defaultVal">Default value, if either - left</param>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R RightOr(R defaultVal) => _hasLeft == false ? _data.Right : defaultVal;

        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise - default value for type. May return null, if TRight - class
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R RightOrDefault() => _hasLeft == false ? _data.Right : default(R);
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise use factory function, for create return value.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public R RightOr(Func<R> defaultValFactory)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(defaultValFactory);

            return _hasLeft == false ? _data.Right : defaultValFactory();
        }
        
        /// <summary>
        /// If either - Right, return _right.
        /// Otherwise throw not found exception.
        /// </summary>
        /// <exception cref="EitherExceptions.EitherNotFound"></exception>
        public R RightOrThrow()
        {
            if(_hasLeft)
                EitherExceptions.NotFound();
            
            return _data.Right;
        }    
        
        #endregion

        #region Pattern matching

        /// <summary>
        /// Pattern matching for either
        /// </summary>
        /// <param name="left">Action run, if either has left branch</param>
        /// <param name="right">Action run, if either has right branch</param>
        public Unit Match(Action<L> left, Action<R> right)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            if (_hasLeft)
                    left(_data.Left);
            else
                    right(_data.Right);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="right">Func run, if either has right branch</param>
        /// <returns>Return value from left, if either has left branch, otherwise from right handler</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TNR Match<TNR>(Func<L, TNR> left, Func<R, TNR> right)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(right);
            }

            return _hasLeft ? left(_data.Left) : right(_data.Right);
        }
                
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Return if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise - nonLeft value</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TNR MatchLeft<TNR>(Func<L, TNR> left, TNR nonLeft)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            return _hasLeft ? left(_data.Left) : nonLeft;
        }
        
        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Func run, if either has left branch</param>
        /// <param name="nonLeft">Run this factory, for create return value, if either - Right</param>
        /// <returns>If either - left, return value from left, otherwise create value from nonLeft function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TNR MatchLeft<TNR>(Func<L, TNR> left, Func<TNR> nonLeft)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(left);
                ExceptionUtility.NullHandlerCheck(nonLeft);
            }

            return _hasLeft ? left(_data.Left) : nonLeft();
        }

        /// <summary>
        /// Pattern matching for left either
        /// </summary>
        /// <param name="left">Action run, if either has left branch</param>
        public Unit MatchLeft(Action<L> left)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(left);

            if (_hasLeft)
                    left(_data.Left);
            
            return Unit.Def;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Action run, if either has right branch</param>
        public Unit MatchRight(Action<R> right)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            if (!_hasLeft)
                    right(_data.Right);
            
            return Unit.Def;
        }
        
        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Func run, if either has right branch</param>
        /// <param name="nonRight">Return if either - Left</param>
        /// <returns>If either - right, return value from right, otherwise - nonRight value</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TNR MatchRight<TNR>(Func<R, TNR> right, TNR nonRight)
        {
            if(default(TCheckPolicy).NeedCheck)
                ExceptionUtility.NullHandlerCheck(right);

            return _hasLeft == false ? right(_data.Right) : nonRight;
        }

        /// <summary>
        /// Pattern matching for right either
        /// </summary>
        /// <param name="right">Func run, if either has right branch</param>
        /// <param name="nonRight">Run this factory, for create return value, if either - Left</param>
        /// <returns>If either - right, return value from right, otherwise create value from nonRight function</returns>
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public TNR MatchRight<TNR>(Func<R, TNR> right, Func<TNR> nonRight)
        {
            if(default(TCheckPolicy).NeedCheck)
            {
                ExceptionUtility.NullHandlerCheck(right);
                ExceptionUtility.NullHandlerCheck(nonRight);
            }
            
            return _hasLeft == false ? right(_data.Right) : nonRight();
        }
        
        #endregion
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator EitherRef<L, R, TCheckPolicy>(L left) => new EitherRef<L, R, TCheckPolicy>(left);

        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator EitherRef<L, R, TCheckPolicy>(R right) => new EitherRef<L, R, TCheckPolicy>(right);

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Either<L, R, TCheckPolicy>(EitherRef<L, R, TCheckPolicy> right)
            => right._hasLeft
                ? new Either<L, R, TCheckPolicy>(right._data.Left)
                : new Either<L, R, TCheckPolicy>(right._data.Right);

        public IEnumerator<EitherRef<L, R, TCheckPolicy>> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerable<L> GetLeftEnumerator()
        {
            if (_hasLeft)
                yield return _data.Left;
        }
        
        IEnumerable<R> GetRightEnumerator()
        {
            if (_hasLeft == false)
                yield return _data.Right;
        }
        
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _hasLeft ? 
            string.Concat("Left(", _data.Left.ToString(), ")") :
            string.Concat("Right(", _data.Right.ToString(), ")");
        
        #region Union

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        private struct Union
        {
            [FieldOffset(0)] private object Lft;
            [FieldOffset(0)] private object Rgt;

            public L Left
            {
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                set { Lft = value; }
                
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                get { return (L)Lft; }
            }
            
            public R Right
            {
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                set { Rgt = value; }
                
                [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                get { return (R)Rgt; }
            }

            public Union(L left)
            {
                Rgt = null;
                Lft = left;
            }
            
            public Union(R right)
            {
                Lft = null;
                Rgt = right;
            }
        }

        #endregion
    }
}