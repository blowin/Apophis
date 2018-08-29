//#define APOPHIS_CHECK
//#define NET_4_6

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Apophis.Types.EnumTypes;
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
        
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ExistLeft(Func<TLeft, bool> predicat)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicat);
#endif
            return _hasLeft && predicat(_left);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ExistRight(Func<TRight, bool> predicat)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicat);
#endif
            return _hasLeft == false && predicat(_right);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool Exist(Func<TLeft, bool> predicatLeft, Func<TRight, bool> predicatRight)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicatLeft);
            ExceptionUtility.NullPredicatCheck(predicatRight);
#endif
            return _hasLeft ? predicatLeft(_left) : predicatRight(_right);
        }
        
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ForallLeft(Func<TLeft, bool> predicat)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicat);
#endif
            return !_hasLeft || predicat(_left);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public bool ForallRight(Func<TRight, bool> predicat)
        {
#if APOPHIS_CHECK
            ExceptionUtility.NullPredicatCheck(predicat);
#endif
            return _hasLeft || predicat(_right);
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOr(TLeft defaultVal)
        {
            return _hasLeft ? _left : defaultVal;
        }
        
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOrDefault()
        {
            return _hasLeft ? _left : default(TLeft);
        }

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

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TLeft LeftOrThrow()
        {
            if(!_hasLeft)
                EitherExceptions.NotFound();
            
            return _left;
        }
        
#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOr(TRight defaultVal)
        {
            return _hasLeft == false ? _right : defaultVal;
        }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public TRight RightOrDefault()
        {
            return _hasLeft == false ? _right : default(TRight);
        }
        
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

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public void Match(Action<TLeft> left, Action<TRight> right)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(left);
                        ExceptionUtility.NullHandlerCheck(right);
#endif
                        if (_hasLeft)
                                left(_left);
                        else
                                right(_right);
                }

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

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public void MatchLeft(Action<TLeft> left)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(left);
#endif
                        if (_hasLeft)
                                left(_left);
                }

#if NET_4_6 || NET_STANDARD_2_0
        [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
                public void MatchRight(Action<TRight> right)
                {
#if APOPHIS_CHECK
                        ExceptionUtility.NullHandlerCheck(right);
#endif
                        if (!_hasLeft)
                                right(_right);
                }
        
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
        
        #region Convert To Other

        public Option<TLeft> LeftOption
        {
            get
            {
                return new Option<TLeft>(_left);
            }
        }

        public Option<TRight> RightOption
        {
            get
            {
                return new Option<TRight>(_right);
            }
        }
        
        #endregion
        
        public override string ToString()
        {
            return _hasLeft ? 
                string.Concat("Left(", _left.ToString(), ")") :
                string.Concat("Right(", _right.ToString(), ")");
        }
    }
}