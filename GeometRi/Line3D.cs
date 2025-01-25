using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 三维空间中的无限线，由线上的任意点和方向向量定义。<br></br>
    /// Infinite line  in 3D space and defined by any point lying on the line and a direction vector.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Line3d : ILinearObject
    {

        internal Point3d _point;
        internal Vector3d _dir;

        #region "Constructors"
        /// <summary>
        /// 默认构造函数，初始化与全局坐标系中的 X 轴对齐的线。<br></br>
        /// Default constructor, initializes line aligned with X-axis in global coordinate system.
        /// </summary>
        public Line3d()
        {
            _point = new Point3d();
            _dir = new Vector3d(1, 0, 0);
        }

        /// <summary>
        /// 使用点和方向初始化线。<br></br>
        /// Initializes line using point and direction.
        /// </summary>
        /// <param name="p">线上的点<br></br> Point on the line.</param>
        /// <param name="v">方向向量<br></br> Direction vector.</param>
        public Line3d(Point3d p, Vector3d v)
        {
            _point = p.Copy();
            _dir = v.Normalized;
        }

        /// <summary>
        /// 使用两点初始化线。<br></br>
        /// Initializes line using two points.
        /// </summary>
        /// <param name="p1">第一点<br></br>First point.</param>
        /// <param name="p2">第二点<br></br>Second point.</param>
        public Line3d(Point3d p1, Point3d p2)
        {
            _point = p1.Copy();
            _dir = new Vector3d(p1, p2).Normalized;
        }
        #endregion

        /// <summary>
        /// 创建对象的副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Line3d Copy()
        {
            return new Line3d(_point, _dir);
        }

        /// <summary>
        /// 线的基点<br></br>
        /// Base point of the line
        /// </summary>
        public Point3d Point
        {
            get { return _point.Copy(); }
            set { _point = value.Copy(); }
        }

        /// <summary>
        /// 线的方向向量<br></br>
        /// Direction vector of the line
        /// </summary>
        public Vector3d Direction
        {
            get { return _dir.Copy(); }
            set { _dir = value.Copy(); }
        }

        public bool IsOriented
        {
            get { return false; }
        }

        /// <summary>
        /// 返回对象的副本<br></br>
        /// Returns copy the object
        /// </summary>
        public Line3d ToLine
        {
            get { return this.Copy(); }
        }

        #region "ParallelMethods"
        /// <summary>
        /// 检查两个物体是否平行<br></br>
        /// Check if two objects are parallel
        /// </summary>
        public bool IsParallelTo(ILinearObject obj)
        {
            return this.Direction.IsParallelTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否不平行<br></br>
        /// Check if two objects are NOT parallel
        /// </summary>
        public bool IsNotParallelTo(ILinearObject obj)
        {
            return this.Direction.IsNotParallelTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否正交<br></br>
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(ILinearObject obj)
        {
            return this.Direction.IsOrthogonalTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否平行<br></br>
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
            return !this.Direction.IsOrthogonalTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否正交<br></br>
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(IPlanarObject obj)
        {
            return this.Direction.IsParallelTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否共面<br></br>
        /// Check if two objects are coplanar
        /// </summary>
        public bool IsCoplanarTo(IPlanarObject obj)
        {
            return GeometRi3D._coplanar(this, obj);
        }

        /// <summary>
        /// 检查两个物体是否共面<br></br>
        /// Check if two objects are coplanar
        /// </summary>
        public bool IsCoplanarTo(ILinearObject obj)
        {
            return GeometRi3D._coplanar(this, obj);
        }
        #endregion

        #region "DistanceTo"
        /// <summary>
        /// 线与点之间的最短距离<br></br>
        /// Shortest distance between line and point
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            return p.DistanceTo(this);
        }

        /// <summary>
        /// 线与射线之间的最短距离<br></br>
        /// Shortest distance between line and ray
        /// </summary>
        public double DistanceTo(Ray3d r)
        {
            return r.DistanceTo(this);
        }

        /// <summary>
        /// 线与线段之间的最短距离<br></br>
        /// Shortest distance between line and segment
        /// </summary>
        public double DistanceTo(Segment3d s)
        {
            return s.DistanceTo(this);
        }

        /// <summary>
        /// 两条线之间的最短距离<br></br>
        /// Shortest distance between two lines
        /// </summary>
        public virtual double DistanceTo(Line3d l)
        {
            Vector3d r1 = this.Point.ToVector;
            Vector3d r2 = l.Point.ToVector;
            Vector3d s1 = this.Direction;
            Vector3d s2 = l.Direction;
            if (s1.Cross(s2).Norm > GeometRi3D.Tolerance)
            {
                // Crossing lines
                return Abs((r2 - r1) * s1.Cross(s2)) / s1.Cross(s2).Norm;
            }
            else
            {
                // Parallel lines
                return (r2 - r1).Cross(s1).Norm / s1.Norm;
            }
        }

        /// <summary>
        /// 线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between line and circle (including interior points)
        /// </summary>
        public double DistanceTo(Circle3d c)
        {
            return c.DistanceTo(this);
        }

        /// <summary>
        /// 线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between line and circle (including interior points)
        /// </summary>
        /// <param name="c">目标圆<br></br> Target circle</param>
        /// <param name="point_on_line">线上最近的点<br></br> Closest point on line</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        public double DistanceTo(Circle3d c, out Point3d point_on_line, out Point3d point_on_circle)
        {
            return c.DistanceTo(this, out point_on_circle, out point_on_line);
        }
        #endregion


        /// <summary>
        /// 垂直于第二条线的点（平行线为空）<br></br>
        /// Point on the perpendicular to the second line (null for parallel lines)
        /// </summary>
        public virtual Point3d PerpendicularTo(Line3d l)
        {
            Vector3d r1 = this.Point.ToVector;
            Vector3d r2 = l.Point.ToVector;
            Vector3d s1 = this.Direction;
            Vector3d s2 = l.Direction;
            if (s1.Cross(s2).Norm > GeometRi3D.Tolerance)
            {
                r1 = r2 + (r2 - r1) * s1.Cross(s1.Cross(s2)) / (s1 * s2.Cross(s1.Cross(s2))) * s2;
                return r1.ToPoint;
            }
            else
            {
                // Lineas are parallel
                // return null
                return null;
                //throw new Exception("Lines are parallel");
            }
        }

        /// <summary>
        /// 获取线与其他线的交点。<br></br>
        /// Get intersection of line with other line.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Line3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Line3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {

            if (l.IsParallelTo(this) && l.Point.BelongsTo(this))
            {
                return this.Copy();
            }

            Point3d p = l.PerpendicularTo(this);
            if (p == null)
            {
                return null;
            }
            else if (p.BelongsTo(l))
            {
                return p;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到线与平面的交点。<br></br>
        /// Get intersection of line with plane.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Line3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Line3d'.
        /// </summary>
        public virtual object IntersectionWith(Plane3d s)
        {
            return s.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与球的交点。<br></br>
        /// Get intersection of line with sphere.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Sphere s)
        {
            return s.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与椭圆体的交点。<br></br>
        /// Get intersection of line with ellipsoid.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ellipsoid e)
        {
            return e.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与椭圆的交点。<br></br>
        /// Get intersection of line with ellipse.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ellipse e)
        {
            return e.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与圆的交点。<br></br>
        /// Get intersection of line with circle.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Circle3d c)
        {
            return c.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与线段的交点。<br></br>
        /// Get intersection of line with segment.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Segment3d s)
        {
            return s.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与三角形的交点。<br></br>
        /// Get intersection of line with triangle.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Triangle  t)
        {
            return t.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线与框的交点。<br></br>
        /// Get intersection of line with box.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Box3d b)
        {
            return b.IntersectionWith(this);
        }

        /// <summary>
        /// 获取直线到平面的正交投影。<br></br>
        /// 返回“Line3d”或“Point3d”类型的对象<br></br>
        /// Get the orthogonal projection of a line to the plane.<br></br>
        /// Return object of type 'Line3d' or 'Point3d'
        /// </summary>
        public virtual object ProjectionTo(Plane3d s)
        {
            Vector3d n1 = s.Normal;
            Vector3d n2 = this.Direction.Cross(n1);
            if (n2.Norm < GeometRi3D.Tolerance)
            {
                // Line is perpendicular to the plane
                return this.Point.ProjectionTo(s);
            }
            else
            {
                return new Line3d(this.Point.ProjectionTo(s), n1.Cross(n2));
            }
        }

        #region "AngleTo"
        /// <summary>
        /// 两个物体之间的角度（以弧度表示）(0 &lt; angle &lt; Pi)<br></br>
        /// Angle between two objects in radians (0 &lt; angle &lt; Pi)
        /// </summary>
        public double AngleTo(ILinearObject obj)
        {
            return GeometRi3D.GetAngle(this, obj);
        }
        /// <summary>
        /// 两个物体之间的角度（以度为单位）(0 &lt; angle &lt; 180)<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(ILinearObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }

        /// <summary>
        /// 两个物体之间的角度（以弧度表示）(0 &lt; angle &lt; Pi)<br></br>
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


        #region "TranslateRotateReflect"
        /// <summary>
        /// 通过向量平移线<br></br>
        /// Translate line by a vector
        /// </summary>
        public virtual Line3d Translate(Vector3d v)
        {
            Line3d l = this.Copy();
            l.Point = l.Point.Translate(v);
            return l;
        }

        /// <summary>
        /// 根据给定的旋转矩阵旋转线<br></br>
        /// Rotate line by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object and specify rotation center: this.Rotate(Rotation r, Point3d p)")]
        public virtual Line3d Rotate(Matrix3d m)
        {
            Line3d l = this.Copy();
            l.Point = l.Point.Rotate(m);
            l.Direction = l.Direction.Rotate(m);
            return l;
        }

        /// <summary>
        /// 以点“p”为旋转中心，按照给定的旋转矩阵旋转直线<br></br>
        /// Rotate line by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r, Point3d p)")]
        public virtual Line3d Rotate(Matrix3d m, Point3d p)
        {
            Line3d l = this.Copy();
            l.Point = l.Point.Rotate(m, p);
            l.Direction = l.Direction.Rotate(m);
            return l;
        }

        /// <summary>
        /// 以点“p”为旋转中心旋转线。<br></br>
        /// Rotate line around point 'p' as a rotation center.
        /// </summary>
        public virtual Line3d Rotate(Rotation r, Point3d p)
        {
            return new Line3d(this.Point.Rotate(r, p), this.Direction.Rotate(r));
        }

        /// <summary>
        /// 在给定点处反射线<br></br>
        /// Reflect line in given point
        /// </summary>
        public virtual Line3d ReflectIn(Point3d p)
        {
            return new Line3d(this.Point.ReflectIn(p), this.Direction.ReflectIn(p));
        }

        /// <summary>
        /// 在给定的线中反射线<br></br>
        /// Reflect line in given line
        /// </summary>
        public virtual Line3d ReflectIn(Line3d l)
        {
            return new Line3d(this.Point.ReflectIn(l), this.Direction.ReflectIn(l));
        }

        /// <summary>
        /// 在给定平面上反射线<br></br>
        /// Reflect line in given plane
        /// </summary>
        public virtual Line3d ReflectIn(Plane3d s)
        {
            return new Line3d(this.Point.ReflectIn(s), this.Direction.ReflectIn(s));
        }
        #endregion

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
            Line3d l = (Line3d)obj;

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return this.Point.BelongsTo(l) && this.Direction.IsParallelTo(l.Direction);
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                if (this.Point == l.Point)
                {
                    // Both lines have the same reference point
                    // Use distance to Origin
                    GeometRi3D.Tolerance = tol * this.Point.DistanceTo(new Point3d(0,0,0));
                } else
                {
                    // Use max of distance to origin or distance between two points
                    GeometRi3D.Tolerance = tol * Math.Max(this.Point.DistanceTo(new Point3d(0, 0, 0)), this.Point.DistanceTo(l.Point));
                }
                GeometRi3D.UseAbsoluteTolerance = true;
                bool res1 = this.Point.BelongsTo(l);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                bool res2 = this.Direction.IsParallelTo(l.Direction);
                return res1 && res2;
            }
        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_point.GetHashCode(), _dir.GetHashCode());
        }

        /// <summary>
        /// 全局坐标系中对象的字符串表示形式。<br></br>
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
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d P = _point.ConvertTo(coord);
            Vector3d dir = _dir.ConvertTo(coord);

            str.Append("Line:" + nl);
            str.Append(string.Format("Point  -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", P.X, P.Y, P.Z) + nl);
            str.Append(string.Format("Direction -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", dir.X, dir.Y, dir.Z));
            return str.ToString();
        }

        // Operators overloads
        //-----------------------------------------------------------------
        public static bool operator ==(Line3d l1, Line3d l2)
        {
            if (object.ReferenceEquals(l1, null))
                return object.ReferenceEquals(l2, null);
            return l1.Equals(l2);
        }
        public static bool operator !=(Line3d l1, Line3d l2)
        {
            if (object.ReferenceEquals(l1, null))
                return !object.ReferenceEquals(l2, null);
            return !l1.Equals(l2);
        }

    }
}


