using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 在全局或局部参考系中定义的 3D 空间中的矢量。<br></br>
    /// Vector in 3D space defined in global or local reference frame.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Vector3d : ILinearObject
    {

        private double[] val;
        private Coord3d _coord;

        #region "Constructors"
        /// <summary>
        /// 默认构造函数，初始化零向量。<br></br>
        /// Default constructor, initializes zero vector.
        /// </summary>
        /// <param name="coord">Reference coordinate system (default - Coord3d.GlobalCS).</param>
        public Vector3d(Coord3d coord = null)
        {
            this.val = new double[3];
            this.val[0] = 0.0;
            this.val[1] = 0.0;
            this.val[2] = 0.0;
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用参考坐标系中的组件初始化矢量对象。
        /// Initializes vector object using components in reference coordinate system.
        /// </summary>
        /// <param name="coord">Reference coordinate system (default - Coord3d.GlobalCS).</param>
        public Vector3d(double X, double Y, double Z, Coord3d coord = null)
        {
            this.val = new double[3];
            this.val[0] = X;
            this.val[1] = Y;
            this.val[2] = Z;
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 将矢量对象初始化为参考坐标系中某点的半径矢量。<br></br>
        /// Initializes vector object as radius vector of a point in reference coordinate system.
        /// </summary>
        public Vector3d(Point3d p, Coord3d coord = null)
        {
            p = p.ConvertTo(coord);
            this.val = new double[3];
            this.val[0] = p.X;
            this.val[1] = p.Y;
            this.val[2] = p.Z;
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用第一个点的参考坐标系中的两个点初始化矢量对象。<br></br>
        /// Initializes vector object using two points in reference coordinate system of the first point.
        /// </summary>
        /// <param name="p1">Start point.</param>
        /// <param name="p2">End point.</param>
        public Vector3d(Point3d p1, Point3d p2)
        {
            if (p1.Coord != p2.Coord)
                p2 = p2.ConvertTo(p1.Coord);
            this.val = new double[3];
            this.val[0] = p2.X - p1.X;
            this.val[1] = p2.Y - p1.Y;
            this.val[2] = p2.Z - p1.Z;
            _coord = p1.Coord;
        }

        /// <summary>
        /// 使用双精度数组初始化向量。<br></br>
        /// Initializes vector using double array.
        /// </summary>
        /// <param name="coord">Reference coordinate system (default - Coord3d.GlobalCS).</param>
        public Vector3d(double[] a, Coord3d coord = null)
        {
            if (a.GetUpperBound(0) < 2)
                throw new Exception("Vector3d: Array size mismatch");
            this.val = new double[3];
            this.val[0] = a[0];
            this.val[1] = a[1];
            this.val[2] = a[2];
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }
        #endregion

        /// <summary>
        /// 单位长度的均匀分布随机向量<br></br>
        /// Uniformly distrbuted random vector of unit length
        /// </summary>
        public static Vector3d Random()
        {
            if (GeometRi3D.rnd == null)
            {
                GeometRi3D.rnd = new Random();
            }
            
            double u = GeometRi3D.rnd.NextDouble();
            double v = GeometRi3D.rnd.NextDouble();
            double theta = Acos(2 * u - 1);
            double phi = 2 * PI * v;

            double x = Sin(theta) * Cos(phi);
            double y = Sin(theta) * Sin(phi);
            double z = Cos(theta);
            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// Creates copy of the object
        /// </summary>
        public Vector3d Copy()
        {
            return new Vector3d(val, _coord);
        }

        public double this[int i]
        {
            get { return val[i]; }
            set { val[i] = value; }
        }

        /// <summary>
        /// 参考坐标系中的 X 分量<br></br>
        /// X component in reference coordinate system
        /// </summary>
        public double X
        {
            get { return val[0]; }
            set { val[0] = value; }
        }

        /// <summary>
        /// 参考坐标系中的 Y 分量<br></br>
        /// Y component in reference coordinate system
        /// </summary>
        public double Y
        {
            get { return val[1]; }
            set { val[1] = value; }
        }

        /// <summary>
        /// 参考坐标系中的 Z 分量<br></br>
        /// Z component in reference coordinate system
        /// </summary>
        public double Z
        {
            get { return val[2]; }
            set { val[2] = value; }
        }

        /// <summary>
        /// 向量范数<br></br>
        /// Norm of a vector
        /// </summary>
        public double Norm
        {
            get { return Sqrt(val[0]*val[0] + val[1]*val[1] + val[2]*val[2]); }
        }

        /// <summary>
        /// 向量的最小元素 
        /// Minimal element of a vector
        /// </summary>
        public double MinElement
        {
            get { return Min(Min(val[0], val[1]), val[2]); }
        }

        /// <summary>
        /// 向量的最大元素<br></br>
        /// Maximal element of a vector
        /// </summary>
        public double MaxElement
        {
            get { return Max(Max(val[0], val[1]), val[2]); }
        }

        /// <summary>
        ///  参考坐标系<br></br>
        ///  Reference coordinate system
        /// </summary>
        public Coord3d Coord
        {
            get { return _coord; }
        }

        /// <inheritdoc/>
        public Vector3d Direction
        {
            get { return this.Normalized;  }
        }

        /// <inheritdoc/>
        public bool IsOriented
        {
            get {  return true;  }
        }

        /// <summary>
        /// 返回沿矢量方向通过原点的线<br></br>
        /// Returns line passing through origin in the direction of vector
        /// </summary>
        public Line3d ToLine
        {
            get { return new Line3d(new Point3d(), this.Direction); }
        }
        //////////////////////////////////////////

        #region "ParallelMethods"
        /// <summary>
        /// 检查两个物体是否平行<br></br>
        /// Check if two objects are parallel
        /// </summary>
        public bool IsParallelTo(ILinearObject obj)
        {
            Vector3d v = obj.Direction;
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);

            return GeometRi3D.AlmostEqual(this.Normalized.Cross(v.Normalized).Norm, 0.0);        
        }

        /// <summary>
        /// 检查两个物体是否不平行<br></br>
        /// Check if two objects are NOT parallel
        /// </summary>
        public bool IsNotParallelTo(ILinearObject obj)
        {
            return ! this.IsParallelTo(obj);
        }

        /// <summary>
        /// 检查两个物体是否正交
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(ILinearObject obj)
        {
            Vector3d v = obj.Direction;
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);

            double this_norm = this.Norm;
            double v_norm = v.Norm;
            return GeometRi3D.AlmostEqual(Abs(this * v) / (this_norm * v_norm), 0.0);

        }

        /// <summary>
        /// 检查两个物体是否平行
        /// Check if two objects are parallel
        /// </summary>
        public bool IsParallelTo(IPlanarObject obj)
        {
            return this.Direction.IsOrthogonalTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否不平行<br></br>
        /// Check if two objects are NOT parallel
        /// </summary>
        public bool IsNotParallelTo(IPlanarObject obj)
        {
            return ! this.Direction.IsOrthogonalTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否正交<br></br>
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(IPlanarObject obj)
        {
            return this.Direction.IsParallelTo(obj.Normal);
        }
        #endregion


        /// <summary>
        /// 点，由从原点开始的矢量表示<br></br>
        /// Point, represented by vector starting in origin
        /// </summary>
        public Point3d ToPoint
        {
            get { return new Point3d(val[0], val[1], val[2], _coord); }
        }

        /// <summary>
        /// 返回标准化向量<br></br>
        /// Return normalized vector
        /// </summary>
        public Vector3d Normalized
        {
            get
            {
                Vector3d tmp = this.Copy();
                double tmp_norm = this.Norm;
                tmp[0] = val[0] / tmp_norm;
                tmp[1] = val[1] / tmp_norm;
                tmp[2] = val[2] / tmp_norm;
                return tmp;
            }
        }

        /// <summary>
        /// 规范化当前向量<br></br>
        /// Normalize the current vector
        /// </summary>
        public void Normalize()
        {
            double tmp = 1.0 / this.Norm;
            val[0] = val[0] * tmp;
            val[1] = val[1] * tmp;
            val[2] = val[2] * tmp;
        }

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Vector3d Add(double a)
        {
            Vector3d tmp = this.Copy();
            tmp[0] += a;
            tmp[1] += a;
            tmp[2] += a;
            return tmp;
        }

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3d Add(Vector3d v)
        {
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);
            Vector3d tmp = this.Copy();
            tmp[0] += v.X;
            tmp[1] += v.Y;
            tmp[2] += v.Z;
            return tmp;
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Vector3d Subtract(double a)
        {
            Vector3d tmp = this.Copy();
            tmp[0] -= a;
            tmp[1] -= a;
            tmp[2] -= a;
            return tmp;
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3d Subtract(Vector3d v)
        {
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);
            Vector3d tmp = this.Copy();
            tmp[0] -= v.X;
            tmp[1] -= v.Y;
            tmp[2] -= v.Z;
            return tmp;
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Vector3d Mult(double a)
        {
            Vector3d tmp = this.Copy();
            tmp[0] *= a;
            tmp[1] *= a;
            tmp[2] *= a;
            return tmp;
        }

        /// <summary>
        /// 两个向量的点积<br></br>
        /// Dot product of two vectors
        /// </summary>
        public double Dot(Vector3d v)
        {
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);
            return this.val[0] * v.val[0] + this.val[1] * v.val[1] + this.val[2] * v.val[2];
        }

        /// <summary>
        /// 两个向量的叉积<br></br>
        /// Cross product of two vectors
        /// </summary>
        public Vector3d Cross(Vector3d v)
        {
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);
            double x = this.Y * v.Z - this.Z * v.Y;
            double y = this.Z * v.X - this.X * v.Z;
            double z = this.X * v.Y - this.Y * v.X;
            return new Vector3d(x, y, z, _coord); ;
        }

        /// <summary>
        /// 将矢量转换为参考坐标系。<br></br>
        /// Convert vector to reference coordinate system.
        /// </summary>
        public Vector3d ConvertTo(Coord3d coord)
        {
            Vector3d v1 = this.Copy();
            v1 = v1.ConvertToGlobal();
            if (coord != null && !object.ReferenceEquals(coord, Coord3d.GlobalCS))
            {
                v1 = coord.Axes * v1;
                v1._coord = coord;
            }
            return v1;
        }

        /// <summary>
        /// 将矢量转换为全局坐标系<br></br>
        /// Convert vector to global coordinate system
        /// </summary>
        public Vector3d ConvertToGlobal()
        {
            if (_coord == null || object.ReferenceEquals(_coord, Coord3d.GlobalCS))
            {
                return this.Copy();
            }
            else
            {
                Vector3d v = this.Copy();
                v = _coord.Axes.Transpose() * v;
                v._coord = Coord3d.GlobalCS;
                return v;
            }
        }

        #region "AngleTo"
        /// <summary>
        /// 两个物体之间的角度（以弧度为单位） (0 &lt; angle &lt; Pi)<br></br>
        /// Angle between two objects in radians (0 &lt; angle &lt; Pi)
        /// </summary>
        public double AngleTo(ILinearObject obj)
        {
            return GeometRi3D.GetAngle(this, obj);
        }
        /// <summary>
        /// 两个物体之间的角度（以度为单位） (0 &lt; angle &lt; 180)<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(ILinearObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }

        /// <summary>
        /// 两个物体之间的角度（以弧度为单位） (0 &lt; angle &lt; Pi)<br></br>
        /// Angle between two objects in radians (0 &lt; angle &lt; Pi)
        /// </summary>
        public double AngleTo(IPlanarObject obj)
        {
            return GeometRi3D.GetAngle(this, obj);
        }
        /// <summary>
        /// 两个物体之间的角度（以度为单位）(0 &lt; angle &lt; 180)<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(IPlanarObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }
        #endregion

        /// <summary>
        ///将当前向量的投影返回到第二个向量<br></br>
        /// Return projection of the current vector to the second vector
        /// </summary>
        public Vector3d ProjectionTo(Vector3d v)
        {
            if ((this._coord != v._coord))
                v = v.ConvertTo(this._coord);
            return (this * v) / (v * v) * v;
        }

        /// <summary>
        /// 返回与当前向量正交的任意向量<br></br>
        /// Return arbitrary vector, orthogonal to the current vector
        /// </summary>
        public Vector3d OrthogonalVector
        {
            get
            {
                if (Abs(this.X) <= Abs(this.Y) && Abs(this.X) <= Abs(this.Z))
                {
                    return new Vector3d(0, this.Z, -this.Y, this.Coord);
                }
                else if (Abs(this.Y) <= Abs(this.X) && Abs(this.Y) <= Abs(this.Z))
                {
                    return new Vector3d(this.Z, 0, -this.X, this.Coord);
                }
                else
                {
                    return new Vector3d(this.Y, -this.X, 0, this.Coord);
                }
            }
        }

        #region "RotateReflect"
        /// <summary>
        /// Rotate vector by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r)")]
        public Vector3d Rotate(Matrix3d m)
        {
            return m * this;
        }

        /// <summary>
        /// 旋转向量<br></br>
        /// Rotate vector
        /// </summary>
        public Vector3d Rotate(Rotation r)
        {
            if (this._coord != r.Coord) r = r.ConvertTo(this._coord);
            return r.ToRotationMatrix * this;
        }

        /// <summary>
        /// 在给定点反射向量<br></br>
        /// Reflect vector in given point
        /// </summary>
        public Vector3d ReflectIn(Point3d p)
        {
            return -this;
        }

        /// <summary>
        /// 在给定的线中反映矢量<br></br>
        /// Reflect vector in given line
        /// </summary>
        public Vector3d ReflectIn(Line3d l)
        {
            Point3d p1 = new Point3d(0, 0, 0, this._coord);
            Point3d p2 = p1.Translate(this);
            return new Vector3d(p1.ReflectIn(l), p2.ReflectIn(l));
        }

        /// <summary>
        /// 在给定平面内反射向量<br></br>
        /// Reflect vector in given plane
        /// </summary>
        public Vector3d ReflectIn(Plane3d s)
        {
            Point3d p1 = new Point3d(0, 0, 0, this._coord);
            Point3d p2 = p1.Translate(this);
            return new Vector3d(p1.ReflectIn(s), p2.ReflectIn(s));
        }
        #endregion

        /// <summary>
        /// Determines whether two objects are equal.
        /// Determines whether two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }
            Vector3d v = (Vector3d)obj;
            if ((this._coord != v.Coord))
                v = v.ConvertTo(_coord);

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return Abs(this.X - v.X) + Abs(this.Y - v.Y) + Abs(this.Z - v.Z) < GeometRi3D.Tolerance;
            }
            else
            {
                return (Abs(this.X - v.X) + Abs(this.Y - v.Y) + Abs(this.Z - v.Z)) / this.Norm < GeometRi3D.Tolerance;
            }

        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(val[0].GetHashCode(), val[1].GetHashCode(), val[2].GetHashCode(), _coord.GetHashCode());
        }

        /// <summary>
        /// 全局坐标系中对象的字符串表示。<br></br>
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override String ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }

        /// <summary>
        /// 参考坐标系中对象的字符串表示。<br></br>
        /// String representation of an object in reference coordinate system.
        /// </summary>
        public String ToString(Coord3d coord)
        {
            if (coord == null) { coord = Coord3d.GlobalCS; }
            Vector3d v = this.ConvertTo(coord);
            return string.Format("Vector3d -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", v.X, v.Y, v.Z);
        }

        // Operators overloads
        //-----------------------------------------------------------------
        // "+"
        public static Vector3d operator +(Vector3d v, Vector3d a)
        {
            return v.Add(a);
        }
        // "-"
        public static Vector3d operator -(Vector3d v)
        {
            return v.Mult(-1.0);
        }
        public static Vector3d operator -(Vector3d v, Vector3d a)
        {
            return v.Subtract(a);
        }
        // "*"
        public static Vector3d operator *(Vector3d v, double a)
        {
            return v.Mult(a);
        }
        public static Vector3d operator /(Vector3d v, double a)
        {
            return v.Mult(1.0/a);
        }
        public static Vector3d operator *(double a, Vector3d v)
        {
            return v.Mult(a);
        }
        /// <summary>
        /// Dot product of two vectors
        /// </summary>
        public static double operator *(Vector3d v, Vector3d a)
        {
            return v.Dot(a);
        }

        public static bool operator ==(Vector3d v1, Vector3d v2)
        {
            if (object.ReferenceEquals(v1, null))
                return object.ReferenceEquals(v2, null);
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector3d v1, Vector3d v2)
        {
            if (object.ReferenceEquals(v1, null))
                return !object.ReferenceEquals(v2, null);
            return !v1.Equals(v2);
        }

    }
}


