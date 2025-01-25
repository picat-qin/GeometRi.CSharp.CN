using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 单位四元数（W + X*i + Y*j + Z*k）。
    /// Unit quaternion (W + X*i + Y*j + Z*k).
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Quaternion
    {

        private double _w, _x, _y, _z;
        private Coord3d _coord;

        #region "Constructors"
        /// <summary>
        /// 默认构造函数，用零旋转（1,0,0,0）初始化四元数。<br></br>
        /// Default constructor, initializes quaternion with zero rotation (1,0,0,0).
        /// </summary>
        /// <param name="coord">
        /// 参考坐标系（默认 - Coord3d.GlobalCS）。<br></br>
        /// Reference coordinate system (default - Coord3d.GlobalCS).
        /// </param>
        public Quaternion(Coord3d coord = null)
        {
            _w = 1; _x = 0; _y = 0; _z = 0;
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 初始化四元数。<br></br>
        /// Initializes quaternion.
        /// </summary>
        /// <param name="coord">
        /// 参考坐标系（默认 - Coord3d.GlobalCS）。<br></br>
        /// Reference coordinate system (default - Coord3d.GlobalCS).
        /// </param>
        public Quaternion(double w, double x, double y, double z, Coord3d coord = null)
        {
            _w = w; _x = x; _y = y; _z = z;
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用双精度数组初始化四元数。<br></br>
        /// Initializes quaternion using double array.
        /// </summary>
        /// <param name="coord">
        /// 参考坐标系（默认 - Coord3d.GlobalCS）。<br></br>
        /// Reference coordinate system (default - Coord3d.GlobalCS).
        /// </param>
        public Quaternion(double[] q, Coord3d coord = null)
        {
            if (q.GetUpperBound(0) < 3)
                throw new Exception("Quaternion: Array size mismatch");
            _w = q[0];
            _x = q[1];
            _y = q[2];
            _z = q[3];
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用旋转轴和角度初始化四元数。<br></br>
        /// Initializes quaternion using axis of rotation and angle.
        /// </summary>
        public Quaternion(Vector3d axis, double angle)
        {
            Vector3d v = axis.Normalized;
            _w = Cos(angle / 2);
            _x = v.X * Sin(angle / 2);
            _y = v.Y * Sin(angle / 2);
            _z = v.Z * Sin(angle / 2);

            _coord = axis.Coord;
        }

        /// <summary>
        /// 使用旋转矩阵初始化四元数。<br></br>
        /// Initializes quaternion using rotation matrix.
        /// </summary>
        /// <param name="m">Rotation matrix.</param>
        /// <param name="coord">
        /// 参考坐标系（默认 - Coord3d.GlobalCS）。<br></br>
        /// Reference coordinate system (default - Coord3d.GlobalCS).
        /// </param>
        public Quaternion(Matrix3d m, Coord3d coord = null)
        {
            if (!m.IsOrthogonal)
            {
                throw new Exception("Matrix is not orthogonal");
            }
            double tr = m.Trace;
            if (tr > 0)
            {
                _w = 0.5 * Sqrt(tr + 1);
                _x = -(m[1, 2] - m[2, 1]) / (4 * _w);
                _y = -(m[2, 0] - m[0, 2]) / (4 * _w);
                _z = -(m[0, 1] - m[1, 0]) / (4 * _w);
            } else if (m[0, 0] >= m[1, 1] && m[0, 0] >= m[2, 2])
            {
                _x = Sqrt(m[0, 0] - m[1, 1] - m[2, 2] + 1) / 2;
                _w = -(m[1, 2] - m[2, 1]) / (4 * _x);
                _y = (m[0, 1] + m[1, 0]) / (4 * _x);
                _z = (m[0, 2] + m[2, 0]) / (4 * _x);
            } else if (m[1, 1] >= m[0, 0] && m[1, 1] >= m[2, 2])
            {
                _y = Sqrt(m[1, 1] - m[0, 0] - m[2, 2] + 1) / 2;
                _w = -(m[2, 0] - m[0, 2]) / (4 * _y);
                _x = (m[0, 1] + m[1, 0]) / (4 * _y);
                _z = (m[1, 2] + m[2, 1]) / (4 * _y);
            } else
            {
                _z = Sqrt(m[2, 2] - m[0, 0] - m[1, 1] + 1) / 2;
                _w = -(m[0, 1] - m[1, 0]) / (4 * _z);
                _x = (m[0, 2] + m[2, 0]) / (4 * _z);
                _y = (m[1, 2] + m[2, 1]) / (4 * _z);
            }

            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }
        #endregion


        /// <summary>
        /// Creates copy of the object
        /// </summary>
        public Quaternion Copy()
        {
            return new Quaternion(this.W, this.X, this.Y, this.Z, _coord);
        }

        #region "Properties"
        /// <summary>
        /// 参考坐标系中的 X 分量<br></br>
        /// X component in reference coordinate system
        /// </summary>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// 参考坐标系中的 Y 分量<br></br>
        /// Y component in reference coordinate system
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// 参考坐标系中的 Z 分量<br></br>
        /// Z component in reference coordinate system
        /// </summary>
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// 参考坐标系中的 W 分量<br></br>
        /// W component in reference coordinate system
        /// </summary>
        public double W
        {
            get { return _w; }
            set { _w = value; }
        }

        /// <summary>
        ///  参考坐标系<br></br>
        ///  Reference coordinate system
        /// </summary>
        public Coord3d Coord
        {
            get { return _coord; }
        }

        /// <summary>
        /// 四元数的范数<br></br>
        /// Norm of a quaternion
        /// </summary>
        public double Norm
        {
            get { return Sqrt(_w * _w + _x * _x + _y * _y + _z * _z); }
        }

        /// <summary>
        /// 四元数范数的平方<br></br>
        /// Square of the norm of a quaternion
        /// </summary>
        public double SquareNorm
        {
            get { return (_w * _w + _x * _x + _y * _y + _z * _z); }
        }

        /// <summary>
        /// 四元数的共轭<br></br>
        /// Conjugate of a quaternion
        /// </summary>
        public Quaternion Conjugate
        {
            get {
                Quaternion qq = this.Copy();
                qq.X = -this.X;
                qq.Y = -this.Y;
                qq.Z = -this.Z;
                return qq;
            }
        }

        /// <summary>
        /// 获取参考坐标系中的旋转轴。<br></br>
        /// Get axis of rotation in reference coordinate system.
        /// </summary>
        public Vector3d ToAxis
        {
            get
            {
                this.Normalize();
                if (GeometRi3D.AlmostEqual(Abs(_w), 1.0))
                {
                    // Zero rotation
                    // For robustness, return any unit vector
                    return new Vector3d(1, 0, 0, _coord);
                }
                else
                {
                    double tmp =  1.0 / Sqrt(1 - _w*_w);
                    return new Vector3d(_x, _y, _z, _coord).Mult(tmp);
                }
            }
        }

        /// <summary>
        /// 获取参考坐标系中的旋转角度。<br></br>
        /// Get rotation angle in reference coordinate system.
        /// </summary>
        public double ToAngle
        {
            get
            {
                this.Normalize();
                if (GeometRi3D.AlmostEqual(Abs(_w),1.0))
                {
                    return 0.0;
                } else
                {
                    return 2 * Acos(_w);
                }
            }
        }
        #endregion

        /// <summary>
        /// 返回规范化的四元数<br></br>
        /// Return normalized quaternion
        /// </summary>
        public Quaternion Normalized
        {
            get
            {
                Quaternion tmp = this.Copy();
                double tmp_norm = this.Norm;
                tmp.W = _w / tmp_norm;
                tmp.X = _x / tmp_norm;
                tmp.Y = _y / tmp_norm;
                tmp.Z = _z / tmp_norm;
                return tmp;
            }
        }

        /// <summary>
        /// 规范化当前四元数<br></br>
        /// Normalize the current quaternion
        /// </summary>
        public void Normalize()
        {
            double tmp = 1.0 / this.Norm;
            _w = _w * tmp;
            _x = _x * tmp;
            _y = _y * tmp;
            _z = _z * tmp;
        }

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Quaternion Add(Quaternion q)
        {
            if ((this._coord != q._coord))
                q = q.ConvertTo(this._coord);

            Quaternion m = new Quaternion(this.Coord);
            m.W = _w + q.W;
            m.X = _x + q.X;
            m.Y = _y + q.Y;
            m.Z = _z + q.Z;
            return m;
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Quaternion Subtract(Quaternion q)
        {
            if ((this._coord != q._coord))
                q = q.ConvertTo(this._coord);

            Quaternion m = new Quaternion(this.Coord);
            m.W = _w - q.W;
            m.X = _x - q.X;
            m.Y = _y - q.Y;
            m.Z = _z - q.Z;
            return m;
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Quaternion Mult (Quaternion q)
        {
            if ((this._coord != q._coord))
                q = q.ConvertTo(this._coord);

            Quaternion m = new Quaternion(this.Coord);
            m.W = W * q.W - X * q.X - Y * q.Y - Z * q.Z;
            m.X = W * q.X + X * q.W + Y * q.Z - Z * q.Y;
            m.Y = W * q.Y - X * q.Z + Y * q.W + Z * q.X;
            m.Z = W * q.Z + X * q.Y - Y * q.X + Z * q.W;

            return m;

        }

        /// <summary>
        /// 成比例放大缩小
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Quaternion Scale(double a)
        {
            return new Quaternion(_w*a, _x*a, _y*a, _z*a, this.Coord);
        }

        /// <summary>
        /// 逆
        /// </summary>
        /// <returns></returns>
        public Quaternion Inverse()
        {
            return this.Conjugate.Scale(1.0 / this.SquareNorm);
        }

        /// <summary>
        /// 将四元数转换为全局坐标系。<br></br>
        /// Convert quaternion to global coordinate system.
        /// </summary>
        public Quaternion ConvertToGlobal()
        {
            if (_coord == null || object.ReferenceEquals(_coord, Coord3d.GlobalCS))
            {
                return this.Copy();
            }
            else
            {
                Vector3d axis = this.ToAxis;
                double angle = this.ToAngle;
                axis = axis.ConvertToGlobal();
                return new Quaternion(axis, angle);
            }
        }

        /// <summary>
        /// 将四元数转换为参考坐标系。<br></br>
        /// Convert quaternion to reference coordinate system.
        /// </summary>
        public Quaternion ConvertTo(Coord3d coord)
        {
            if (_coord == null || object.ReferenceEquals(_coord, Coord3d.GlobalCS))
            {
                return this.Copy();
            }
            Vector3d axis = this.ToAxis;
            double angle = this.ToAngle;
            axis = axis.ConvertTo(coord);
            return new Quaternion(axis, angle);
        }

        /// <summary>
        /// 返回旋转矩阵（在当前参考坐标系中）。<br></br>
        /// Returns rotation matrix (in current reference coordinate system).
        /// </summary>
        public Matrix3d ToRotationMatrix()
        {
            Matrix3d m = new Matrix3d();
            this.Normalize();

            m[0, 0] = 1 - 2 * (_y * _y + _z * _z);
            m[0, 1] = 2 * (_x * _y - _w * _z);
            m[0, 2] = 2 * (_x * _z + _w * _y);

            m[1, 0] = 2 * (_x * _y + _w * _z);
            m[1, 1] = 1 - 2 * (_x * _x + _z * _z);
            m[1, 2] = 2 * (_y * _z - _w * _x);

            m[2, 0] = 2 * (_x * _z - _w * _y);
            m[2, 1] = 2 * (_y * _z + _w * _x);
            m[2, 2] = 1 - 2 * (_x * _x + _y * _y);
            return m;
        }

        /// <summary>
        /// 两个旋转的球面线性插值。<br></br>
        /// Spherical linear interpolation of two rotations.
        /// </summary>
        /// <param name="q1">初始旋转<br></br> Initial rotation</param>
        /// <param name="q2">最后旋转<br></br> Final rotation</param>
        /// <param name="t">
        /// 插值参数在范围 [0, 1] 内<br></br>
        /// Interpolation parameter within range [0, 1]
        /// </param>
        public static Quaternion SLERP(Quaternion q1, Quaternion q2, double t)
        {
            // Algorithm from https://en.wikipedia.org/wiki/Slerp

            Quaternion qq1 = q1.Copy().Normalized;
            Quaternion qq2 = q2.Copy().Normalized;
            if (qq2.Coord != qq1.Coord) qq2 = qq2.ConvertTo(qq1.Coord);

            double dot = qq1.W * qq2.W + qq1.X * qq2.X + qq1.Y * qq2.Y + qq1.Z * qq2.Z;

            const double threshold = 0.9995;
            if (Abs(dot) > threshold)
            {
                // Using linear interpolation if two rotations are close
                Quaternion res = qq1 + t * (qq2 - qq1);
                res.Normalize();
                return res;
            }

            //Make sure to choose shortest path
            if (dot < 0.0)
            {
                qq2 = -qq2;
                dot = -dot;
            }

            double theta_0 = Acos(dot);
            double theta = theta_0 * t;

            qq2 = qq2 - qq1 * dot;
            qq2.Normalize();
            return qq1 * Cos(theta) + qq2 * Sin(theta);
        }

        /// <summary>
        /// 确定两个对象是否相等。<br></br>
        /// Determines whether two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }
            Quaternion q = (Quaternion)obj;
            // No check for absolute tolerance since this is unit quaternion
            return (this - q).Norm <= GeometRi3D.Tolerance;
        }

        /// <summary>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_w.GetHashCode(), _x.GetHashCode(), _y.GetHashCode(), _z.GetHashCode());
        }

        /// <summary>
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override String ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }
        
        /// <summary>
                /// String representation of an object in reference coordinate system.
                /// </summary>
        public string ToString(Coord3d coord)
        {
            if (coord == null) { coord = Coord3d.GlobalCS; }
            Quaternion q = this.ConvertTo(coord);
            return string.Format("Quaternion -> ({0,10:g5}, {1,10:g5}, {2,10:g5}, {3,10:g5})", q.W, q.X, q.Y, q.Z);
        }

        // Operators overloads
        //-----------------------------------------------------------------
        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return q1.Add(q2);
        }

        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return q1.Subtract(q2);
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return q1.Mult(q2);
        }

        public static Quaternion operator *(Quaternion q1, double a)
        {
            return q1.Scale(a);
        }

        public static Quaternion operator /(Quaternion q1, double a)
        {
            return q1.Scale(1.0/a);
        }

        public static Quaternion operator *(double a, Quaternion q1)
        {
            return q1.Scale(a);
        }

        public static Quaternion operator -(Quaternion q1)
        {
            return q1.Scale(-1.0);
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            if (object.ReferenceEquals(q1, null))
                return object.ReferenceEquals(q2, null);
            return q1.Equals(q2);
        }
        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            if (object.ReferenceEquals(q1, null))
                return !object.ReferenceEquals(q2, null);
            return !q1.Equals(q2);
        }

    }
}
