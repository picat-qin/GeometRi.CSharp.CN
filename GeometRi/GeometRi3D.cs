﻿using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 静态类。实现全局公差属性和基于公差的相等方法。<br></br>
    /// Static class. Implements global tolerance property and tolerance based equality methods.
    /// </summary>
    public static class GeometRi3D
    {

        private static double _tolerance = 1E-12;
        private static double _default_tolerance = 1E-12;
        private static bool _absolute = true;
        internal static Random rnd;

        /// <summary>
        /// 用于比较操作的容差（默认 1e-12）<br></br>
        /// Tolerance used for comparison operations (default 1e-12)
        /// </summary>
        public static double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        /// <summary>
        /// 用于比较运算的默认容差<br></br>
        /// Default tolerance used for comparison operations
        /// </summary>
        public static double DefaultTolerance
        {
            get { return _default_tolerance; }
            set { _default_tolerance = value; }
        }

        /// <summary>
        /// 用于切换绝对（TRUE）至相对（FALSE）公差比较的标志。<br></br>
        /// Flag for switching absolute (TRUE) to relative (FALSE) tolerance comparison.
        /// </summary>
        public static bool UseAbsoluteTolerance
        {
            get { return _absolute; }
            set { _absolute = value; }
        }

        /// <summary>
        /// 基于容差的平等检查<br></br>
        /// Tolerance based equality check
        /// </summary>
        public static bool AlmostEqual(double a, double b)
        {
            if (Math.Abs(a - b) <= _tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于容差的平等检查<br></br>
        /// Tolerance based equality check
        /// </summary>
        public static bool AlmostEqual(double a, double b, double tolerance)
        {
            if (Math.Abs(a - b) <= tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的不平等检查<br></br>
        /// Tolerance based unequality check
        /// </summary>
        public static bool NotEqual(double a, double b)
        {
            if (Math.Abs(a - b) > _tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的不平等检查<br></br>
        /// Tolerance based unequality check
        /// </summary>
        public static bool NotEqual(double a, double b, double tolerance)
        {
            if (Math.Abs(a - b) > tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的比较<br></br>
        /// Tolerance based comparison
        /// </summary>
        public static bool Greater(double a, double b)
        {
            if ((a - b) > _tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的比较<br></br>
        /// Tolerance based comparison
        /// </summary>
        public static bool Greater(double a, double b, double tolerance)
        {
            if ((a - b) > tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的比较<br></br>
        /// Tolerance based comparison
        /// </summary>
        public static bool Smaller(double a, double b)
        {
            if ((a - b) < -_tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于公差的比较<br></br>
        /// Tolerance based comparison
        /// </summary>
        public static bool Smaller(double a, double b, double tolerance)
        {
            if ((a - b) < -tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static internal int HashFunction(int n1, int n2)
        {
            return (n1 << 4) ^ (n1 >> 28) ^ n2;
        }

        static internal int HashFunction(int n1, int n2, int n3)
        {
            n1 = (n1 << 4) ^ (n1 >> 28) ^ n2;
            return (n1 << 4) ^ (n1 >> 28) ^ n3;
        }

        static internal int HashFunction(int n1, int n2, int n3, int n4)
        {
            n1 = (n1 << 4) ^ (n1 >> 28) ^ n2;
            n1 = (n1 << 4) ^ (n1 >> 28) ^ n3;
            return (n1 << 4) ^ (n1 >> 28) ^ n4;
        }

        /// <summary>
        /// 将“值”限制在范围[最小值，最大值]内。<br></br>
        /// Restrict 'value' to a range [min, max].
        /// </summary>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value == null || min == null || max == null) throw new ArgumentNullException();
            // Check if min <= max
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            // Reverse min and max otherwise
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }

        #region "CalcAngle"
        static internal double GetAngle(ILinearObject obj1, ILinearObject obj2)
        {
            if (obj1.IsOriented && obj2.IsOriented)
            {
                double tmp = obj1.Direction.Dot(obj2.Direction);
                if (tmp > 1)
                {
                    tmp = 1;
                } else if (tmp < -1)
                {
                    tmp = -1;
                }
                return Acos(tmp);
            }
            else
            {
                // return smalest angle
                double ang = GetAngle(obj1.Direction, obj2.Direction);
                if (ang <= PI / 2)
                {
                    return ang;
                }
                else
                {
                    return PI - ang;
                }
            }
        }

        static internal double GetAngle(ILinearObject obj1, IPlanarObject obj2)
        {
            if (obj1.IsOriented && obj2.IsOriented)
            {
                throw new NotImplementedException();
            }
            else
            {
                double tmp = obj1.Direction.Dot(obj2.Normal);
                if (tmp > 1)
                {
                    tmp = 1;
                }
                else if (tmp < -1)
                {
                    tmp = -1;
                }
                double ang = Asin(tmp);
                return Abs(ang);
            }
        }

        static internal double GetAngle(IPlanarObject obj1, ILinearObject obj2)
        {
            return GetAngle(obj2, obj1);
        }

        static internal double GetAngle(IPlanarObject obj1, IPlanarObject obj2)
        {
            if (obj1.IsOriented && obj2.IsOriented)
            {
                throw new NotImplementedException();
            }
            else
            {
                // return smalest angle
                double ang = GetAngle(obj1.Normal, obj2.Normal);
                if (ang <= PI / 2)
                {
                    return ang;
                }
                else
                {
                    return PI - ang;
                }
            }
        }
        #endregion

        static internal bool _coplanar(IPlanarObject obj1, IPlanarObject obj2)
        {

            return obj1.ToPlane.Equals(obj2.ToPlane);

        }

        static internal bool _coplanar(ILinearObject obj1, ILinearObject obj2)
        {
            if (obj1.ToLine.IsParallelTo(obj2.ToLine))
            {
                return true;
            }
            return AlmostEqual(obj1.ToLine.DistanceTo(obj2.ToLine), 0.0);
        }

        static internal bool _coplanar(IPlanarObject obj1, ILinearObject obj2)
        {
            return obj1.Normal.IsOrthogonalTo(obj2.Direction) && obj2.ToLine.Point.BelongsTo(obj1.ToPlane);
        }

        static internal bool _coplanar(ILinearObject obj1, IPlanarObject obj2)
        {
            return _coplanar(obj2, obj1);
        }
    }
}
