﻿using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 三维空间中的椭圆，由中心点和两个正交向量（长半轴和短半轴）定义。<br></br>
    /// Ellipse in 3D space, defined by center point and two orthogonal vectors, major and minor semiaxes.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Ellipse : FiniteObject, IPlanarObject, IFiniteObject
    {

        private Point3d _point;
        private Vector3d _v1;
        private Vector3d _v2;

        /// <summary>
        /// 使用中心点和两个正交向量初始化椭圆实例。<br></br>
        /// Initializes ellipse instance using center point and two orthogonal vectors.
        /// </summary>
        /// <param name="Center">中心点<br></br> Center point.</param>
        /// <param name="v1">第一个矢量<br></br> First semiaxis.</param>
        /// <param name="v2">第二个矢量<br></br> Second semiaxis.</param>
        public Ellipse(Point3d Center, Vector3d v1, Vector3d v2)
        {
            if (!v1.IsOrthogonalTo(v2))
            {
                throw new Exception("Semiaxes are not orthogonal");
            }
            _point = Center.Copy();
            if (v1.Norm >= v2.Norm)
            {
                _v1 = v1.Copy();
                _v2 = v2.Copy();
            }
            else
            {
                _v1 = v2.Copy();
                _v2 = v1.Copy();
            }

        }

        /// <summary>
        /// 创建对象副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Ellipse Copy()
        {
            return new Ellipse(_point.Copy(), _v1.Copy(), _v2.Copy());
        }

        #region "Properties"
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d Center
        {
            get { return _point.Copy(); }
        }

        /// <summary>
        /// 长半轴
        /// </summary>
        public Vector3d MajorSemiaxis
        {
            get { return _v1.Copy(); }
        }

        /// <summary>
        /// 短半轴
        /// </summary>
        public Vector3d MinorSemiaxis
        {
            get { return _v2.Copy(); }
        }

        /// <summary>
        /// 普通的
        /// </summary>
        public Vector3d Normal
        {
            get { return _v1.Cross(_v2).Normalized; }
        }

        /// <inheritdoc/>
        public bool IsOriented
        {
            get { return false; }
        }

        /// <summary>
        /// 长半轴长度<br></br>
        /// Length of the major semiaxis
        /// </summary>
        public double A
        {
            get { return _v1.Norm; }
        }

        /// <summary>
        /// 短半轴的长度<br></br>
        /// Length of the minor semiaxis
        /// </summary>
        public double B
        {
            get { return _v2.Norm; }
        }

        /// <summary>
        /// 中心到焦点的距离<br></br>
        /// Distance from center to focus
        /// </summary>
        public double F
        {
            get { return Sqrt(Math.Pow(_v1.Norm, 2) - Math.Pow(_v2.Norm, 2)); }
        }

        /// <summary>
        /// 第一个焦点<br></br>
        /// First focus
        /// </summary>
        public Point3d F1
        {
            get { return _point.Translate(F * _v1.Normalized); }
        }

        /// <summary>
        /// 第二个焦点<br></br>
        /// Second focus
        /// </summary>
        public Point3d F2
        {
            get { return _point.Translate(-F * _v1.Normalized); }
        }

        /// <summary>
        /// 椭圆的偏心率<br></br>
        /// Eccentricity of the ellipse
        /// </summary>
        public double E
        {
            get { return Sqrt(1 - Math.Pow(_v2.Norm, 2) / Math.Pow(_v1.Norm, 2)); }
        }

        /// <summary>
        /// 面积
        /// </summary>
        public double Area
        {
            get { return PI * A * B; }
        }

        /// <summary>
        /// 椭圆的近似周长<br></br>
        /// Approximate circumference of the ellipse
        /// </summary>
        public double Perimeter
        {
            get
            {
                double a = _v1.Norm;
                double b = _v2.Norm;
                double h = Math.Pow((a - b), 2) / Math.Pow((a + b), 2);
                return PI * (a + b) * (1 + 3 * h / (10 + Sqrt(4 - 3 * h)));
            }
        }

        /// <summary>
        /// 将椭圆转换为平面物体。<br></br>
        /// Convert ellipse to plane object.
        /// </summary>
        public Plane3d ToPlane
        {
            get
            {
                return new Plane3d(_point, Normal);
            }
        }
        #endregion

        #region "平行方法 ParallelMethods"
        /// <summary>
        /// 检查两个物体是否平行<br></br>
        /// Check if two objects are parallel
        /// </summary>
        public bool IsParallelTo(ILinearObject obj)
        {
            return this.Normal.IsOrthogonalTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否不平行<br></br>
        /// Check if two objects are NOT parallel
        /// </summary>
        public bool IsNotParallelTo(ILinearObject obj)
        {
            return !this.Normal.IsOrthogonalTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否正交<br></br>
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(ILinearObject obj)
        {
            return this.Normal.IsParallelTo(obj.Direction);
        }

        /// <summary>
        /// 检查两个物体是否平行<br></br>
        /// Check if two objects are parallel
        /// </summary>
        public bool IsParallelTo(IPlanarObject obj)
        {
            return this.Normal.IsParallelTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否不平行<br></br>
        /// Check if two objects are NOT parallel
        /// </summary>
        public bool IsNotParallelTo(IPlanarObject obj)
        {
            return this.Normal.IsNotParallelTo(obj.Normal);
        }

        /// <summary>
        /// 检查两个物体是否正交<br></br>
        /// Check if two objects are orthogonal
        /// </summary>
        public bool IsOrthogonalTo(IPlanarObject obj)
        {
            return this.Normal.IsOrthogonalTo(obj.Normal);
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

        #region "BoundingBox"
        /// <summary>
        /// 返回最小边界框。<br></br>
        /// Return minimum bounding box.
        /// </summary>
        public Box3d MinimumBoundingBox
        {
            get
            {
                Vector3d v1 = _v1.Normalized;
                Vector3d v2 = _v2.Normalized;
                Vector3d v3 = v1.Cross(v2).Normalized;
                Matrix3d m = new Matrix3d(v1, v2, v3);
                Rotation r = new Rotation(m.Transpose());
                return new Box3d(_point, 2.0 * this.A, 2.0 * this.B, 0, r);
            }
        }

        /// <summary>
        /// 返回给定坐标系中的边界框。<br></br>
        /// Return Bounding Box in given coordinate system.
        /// </summary>
        public Box3d BoundingBox(Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            Line3d l1 = new Line3d(coord.Origin, coord.Xaxis);
            Line3d l2 = new Line3d(coord.Origin, coord.Yaxis);
            Line3d l3 = new Line3d(coord.Origin, coord.Zaxis);
            Segment3d s1 = this.ProjectionTo(l1);
            Segment3d s2 = this.ProjectionTo(l2);
            Segment3d s3 = this.ProjectionTo(l3);
            return new Box3d(_point, s1.Length, s2.Length, s3.Length, coord);
        }

        /// <summary>
        /// 返回轴对齐边界框（AABB）。<br></br>
        /// Return Axis Aligned Bounding Box (AABB).
        /// </summary>
        public AABB AABB()
        {
            Line3d l1 = new Line3d(_point, Coord3d.GlobalCS.Xaxis);
            Line3d l2 = new Line3d(_point, Coord3d.GlobalCS.Yaxis);
            Line3d l3 = new Line3d(_point, Coord3d.GlobalCS.Zaxis);
            Segment3d s1 = this.ProjectionTo(l1);
            Segment3d s2 = this.ProjectionTo(l2);
            Segment3d s3 = this.ProjectionTo(l3);
            return new AABB(_point, s1.Length, s2.Length, s3.Length);
        }

        /// <summary>
        /// 返回边界球。<br></br>
        /// Return bounding sphere.
        /// </summary>
        public Sphere BoundingSphere
        {
            get { return new Sphere(_point, this.A); }

        }
        #endregion

        /// <summary>
        /// 返回给定参数“t”的椭圆上的点 (0 &lt;= t &lt; 2Pi)<br></br>
        /// Returns point on ellipse for given parameter 't' (0 &lt;= t &lt; 2Pi)
        /// </summary>
        public Point3d ParametricForm(double t)
        {

            return _point + _v1.ToPoint * Cos(t) + _v2.ToPoint * Sin(t);

        }

        /// <summary>
        /// 椭圆到线的正交投影。<br></br>
        /// Orthogonal projection of ellipse to line.
        /// </summary>
        public Segment3d ProjectionTo(Line3d l)
        {
            // Using algorithm for ellipsoid with third semiaxis -> 0
            // Stephen B. Pope "Algorithms for Ellipsoids"
            // https://tcg.mae.cornell.edu/pubs/Pope_FDA_08.pdf

            Coord3d lc = new Coord3d(_point, _v1, _v2);
            Point3d x0 = l.Point.ConvertTo(lc);
            Vector3d v = l.Direction.ConvertTo(lc);

            Matrix3d L_T = Matrix3d.DiagonalMatrix(this.A, this.B, 0);
            Vector3d c = new Vector3d(0.0, 0.0, 0.0, lc);
            double s0 = v * (c - x0.ToVector) / (v * v);
            Vector3d w = L_T * v / (v * v);
            Point3d P1 = x0.Translate((s0 + w.Norm) * v);
            Point3d P2 = x0.Translate((s0 - w.Norm) * v);
            return new Segment3d(P1, P2);
        }

        /// <summary>
        /// 椭圆到平面的正交投影<br></br>
        /// Orthogonal projection of the ellipse to plane
        /// </summary>
        public Ellipse ProjectionTo(Plane3d s)
        {

            Point3d c = _point.ProjectionTo(s);
            Point3d q = _point.Translate(_v1).ProjectionTo(s);
            Point3d p = _point.Translate(_v2).ProjectionTo(s);

            Vector3d f1 = new Vector3d(c, p);
            Vector3d f2 = new Vector3d(c, q);

            double t0 = 0.5 * Atan2(2 * f1 * f2, f1 * f1 - f2 * f2);
            Vector3d v1 = f1 * Cos(t0) + f2 * Sin(t0);
            Vector3d v2 = f1 * Cos(t0 + PI / 2) + f2 * Sin(t0 + PI / 2);

            return new Ellipse(c, v1, v2);
        }

        /// <summary>
        /// 椭圆与直线的交点。<br></br>
        /// Intersection of ellipse with line.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.A;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(l);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================


            if (l.Direction.IsOrthogonalTo(this.Normal))
            {
                if (l.Point.BelongsTo(new Plane3d(this.Center, this.Normal)))
                {
                    // coplanar objects
                    // Find intersection of line and ellipse (2D)
                    // Solution from: http://www.ambrsoft.com/TrigoCalc/Circles2/Ellipse/EllipseLine.htm

                    Coord3d local_coord = new Coord3d(this.Center, this._v1, this._v2);
                    Point3d p = l.Point.ConvertTo(local_coord);
                    Vector3d v = l.Direction.ConvertTo(local_coord);
                    double a = this.A;
                    double b = this.B;

                    if (Abs(v.Y / v.X) > 100)
                    {
                        // line is almost vertical, rotate local coord
                        local_coord = new Coord3d(this.Center, this._v2, this._v1);
                        p = l.Point.ConvertTo(local_coord);
                        v = l.Direction.ConvertTo(local_coord);
                        a = this.B;
                        b = this.A;
                    }

                    // Line equation in form: y = mx + c
                    double m = v.Y / v.X;
                    double c = p.Y - m * p.X;

                    double amb = Math.Pow(a, 2) * Math.Pow(m, 2) + Math.Pow(b, 2);
                    double det = amb - Math.Pow(c, 2);
                    if (det < -GeometRi3D.Tolerance)
                    {
                        return null;
                    }
                    else if (det > 1e-12)
                    {
                        double x1 = (-Math.Pow(a, 2) * m * c + a * b * Sqrt(det)) / amb;
                        double x2 = (-Math.Pow(a, 2) * m * c - a * b * Sqrt(det)) / amb;
                        double y1 = (Math.Pow(b, 2) * c + a * b * m * Sqrt(det)) / amb;
                        double y2 = (Math.Pow(b, 2) * c - a * b * m * Sqrt(det)) / amb;
                        return new Segment3d(new Point3d(x1, y1, 0, local_coord), new Point3d(x2, y2, 0, local_coord));
                    }
                    else
                    {
                        double x = -Math.Pow(a, 2) * m * c / amb;
                        double y = Math.Pow(b, 2) * c / amb;
                        return new Point3d(x, y, 0, local_coord);
                    }

                }
                else
                {
                    // parallel objects
                    return null;
                }
            }
            else
            {
                // Line intersects ellipse' plane
                Point3d p = (Point3d)l.IntersectionWith(new Plane3d(this.Center, this.Normal));
                if (p.BelongsTo(this))
                {
                    return p;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 椭圆与平面的交点。<br></br>
        /// Intersection of ellipse with plane.<br></br>
        /// 返回“null”（无交点）或“Ellipse”、“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Ellipse', 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Plane3d s)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.A;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(s);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            if (this.Normal.IsParallelTo(s.Normal))
            {
                if (this.Center.BelongsTo(s))
                {
                    // coplanar objects
                    return this.Copy();
                }
                else
                {
                    // parallel objects
                    return null;
                }
            }
            else
            {
                Line3d l = (Line3d)s.IntersectionWith(new Plane3d(this.Center, this.Normal));
                return this.IntersectionWith(l);
            }

        }

        /// <summary>
        /// 椭圆与线段的交点。<br></br>
        /// Intersection of ellipse with segment.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Segment3d s)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.A;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(s);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            object obj = this.IntersectionWith(s.ToLine);

            if (obj == null)
            {
                return null;
            }
            else if (obj.GetType() == typeof(Point3d))
            {
                Point3d p = (Point3d)obj;
                if (p.BelongsTo(s))
                {
                    return p;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return s.IntersectionWith((Segment3d)obj);
            }
        }

        /// <summary>
        /// 椭圆与射线的交点。<br></br>
        /// Intersection of ellipse with ray.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ray3d r)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.A;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(r);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            object obj = this.IntersectionWith(r.ToLine);

            if (obj == null)
            {
                return null;
            }
            else if (obj.GetType() == typeof(Point3d))
            {
                Point3d p = (Point3d)obj;
                if (p.BelongsTo(r))
                {
                    return p;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return r.IntersectionWith((Segment3d)obj);
            }
        }

        internal override int _PointLocation(Point3d p)
        {
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                Plane3d s = new Plane3d(this.Center, this.Normal);
                Point3d proj = p.ProjectionTo(s);
                if (GeometRi3D.AlmostEqual(p.DistanceTo(proj), 0))
                {
                    if (GeometRi3D.AlmostEqual(p.DistanceTo(this.ClosestPoint(proj)), 0))
                    {
                        return 0; // Point is on boundary
                    }
                    else if (GeometRi3D.Smaller(proj.DistanceTo(this.F1) + proj.DistanceTo(this.F2), 2 * this.A))
                    {
                        return 1; // Point is strictly inside
                    }
                    else
                    {
                        return -1; // Point is outside
                    }
                }
                else
                {
                    return -1; // Point is outside
                }
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.A;
                GeometRi3D.UseAbsoluteTolerance = true;
                int result = this._PointLocation(p);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        /// <summary>
        /// 计算椭圆边界上​​距离给定点最近的点。<br></br>
        /// Calculates the point on the ellipse's boundary closest to given point.
        /// </summary>
        public Point3d ClosestPoint(Point3d p)
        {

            // Algorithm by Dr. Robert Nurnberg
            // http://wwwf.imperial.ac.uk/~rn/distance2ellipse.pdf
            // Does not work for interior points

            Coord3d local_coord = new Coord3d(this.Center, this._v1, this._v2);
            p = p.ConvertTo(local_coord);

            if (GeometRi3D.AlmostEqual(p.X, 0) && GeometRi3D.AlmostEqual(p.Y, 0))
            {
                // Center point, choose any minor-axis
                return new Point3d(0, this.B, 0, local_coord);
            }

            double theta = Atan2(this.A * p.Y, this.B * p.X);
            int iter = 0;
            int max_iter = 100;
            Point3d n0 = p.Copy();

            while (iter < max_iter)
            {
                iter += 1;
                double f = (A * A - B * B) * Cos(theta) * Sin(theta) - p.X * A * Sin(theta) + p.Y * B * Cos(theta);
                double f_prim = (A * A - B * B) * (Cos(theta) * Cos(theta) - Sin(theta) * Sin(theta))
                                - p.X * A * Cos(theta) - p.Y * B * Sin(theta);
                theta = theta - f / f_prim;
                Point3d n = new Point3d(A * Cos(theta), B * Sin(theta), 0, local_coord);

                if (n0.DistanceTo(n) < GeometRi3D.Tolerance)
                {
                    return n;
                }
                n0 = n.Copy();
            }

            return n0;
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
        /// 通过矢量平移椭圆<br></br>
        /// Translate ellipse by a vector
        /// </summary>
        public Ellipse Translate(Vector3d v)
        {
            return new Ellipse(this.Center.Translate(v), _v1, _v2);
        }

        /// <summary>
        /// 根据给定的旋转矩阵旋转椭圆<br></br>
        /// Rotate ellipse by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object and specify rotation center: this.Rotate(Rotation r, Point3d p)")]
        public Ellipse Rotate(Matrix3d m)
        {
            return new Ellipse(this.Center.Rotate(m), _v1.Rotate(m), _v2.Rotate(m));
        }

        /// <summary>
        /// 以点“p”为旋转中心，按照给定的旋转矩阵旋转椭圆<br></br>
        /// Rotate ellipse by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r, Point3d p)")]
        public Ellipse Rotate(Matrix3d m, Point3d p)
        {
            return new Ellipse(this.Center.Rotate(m, p), _v1.Rotate(m), _v2.Rotate(m));
        }

        /// <summary>
        /// 围绕点“p”作为旋转中心旋转椭圆。<br></br>
        /// Rotate ellipse around point 'p' as a rotation center.
        /// </summary>
        public Ellipse Rotate(Rotation r, Point3d p)
        {
            return new Ellipse(this.Center.Rotate(r, p), _v1.Rotate(r), _v2.Rotate(r));
        }

        /// <summary>
        /// 在给定点反射椭圆<br></br>
        /// Reflect ellipse in given point
        /// </summary>
        public Ellipse ReflectIn(Point3d p)
        {
            return new Ellipse(this.Center.ReflectIn(p), _v1.ReflectIn(p), _v2.ReflectIn(p));
        }

        /// <summary>
        /// 在给定的线上反射椭圆<br></br>
        /// Reflect ellipse in given line
        /// </summary>
        public Ellipse ReflectIn(Line3d l)
        {
            return new Ellipse(this.Center.ReflectIn(l), _v1.ReflectIn(l), _v2.ReflectIn(l));
        }

        /// <summary>
        /// 在给定平面上反射椭圆<br></br>
        /// Reflect ellipse in given plane
        /// </summary>
        public Ellipse ReflectIn(Plane3d s)
        {
            return new Ellipse(this.Center.ReflectIn(s), _v1.ReflectIn(s), _v2.ReflectIn(s));
        }

        /// <summary>
        /// 相对于给定点缩放椭圆<br></br>
        /// Scale ellipse relative to given point
        /// </summary>
        public virtual Ellipse Scale(double scale, Point3d scaling_center)
        {
            Point3d new_center = scaling_center + scale * (this.Center - scaling_center);
            return new Ellipse(new_center, _v1 * scale, _v2 * scale);
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
            Ellipse e = (Ellipse)obj;

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                if (GeometRi3D.AlmostEqual(this.A, this.B))
                {
                    // Ellipse is circle
                    if (GeometRi3D.AlmostEqual(e.A, e.B))
                    {
                        // Second ellipse also circle
                        return this.Center == e.Center && GeometRi3D.AlmostEqual(this.A, e.A) && e.Normal.IsParallelTo(this.Normal);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return this.Center == e.Center && GeometRi3D.AlmostEqual(this.A, e.A) && GeometRi3D.AlmostEqual(this.B, e.B) &&
                           e.MajorSemiaxis.IsParallelTo(this.MajorSemiaxis) && e.MinorSemiaxis.IsParallelTo(this.MinorSemiaxis);
                }
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * e.MajorSemiaxis.Norm;
                GeometRi3D.UseAbsoluteTolerance = true;

                if (GeometRi3D.AlmostEqual(this.A, this.B))
                {
                    // Ellipse is circle
                    if (GeometRi3D.AlmostEqual(e.A, e.B))
                    {
                        // Second ellipse also circle
                        bool res1 = this.Center == e.Center && GeometRi3D.AlmostEqual(this.A, e.A);
                        GeometRi3D.UseAbsoluteTolerance = false;
                        GeometRi3D.Tolerance = tol;
                        bool res2 = e.Normal.IsParallelTo(this.Normal);
                        return res1 && res2;
                    }
                    else
                    {
                        GeometRi3D.UseAbsoluteTolerance = false;
                        GeometRi3D.Tolerance = tol;
                        return false;
                    }
                }
                else
                {
                    bool res1 = this.Center == e.Center && GeometRi3D.AlmostEqual(this.A, e.A) && GeometRi3D.AlmostEqual(this.B, e.B);
                    GeometRi3D.UseAbsoluteTolerance = false;
                    GeometRi3D.Tolerance = tol;
                    bool res2 = e.MajorSemiaxis.IsParallelTo(this.MajorSemiaxis) && e.MinorSemiaxis.IsParallelTo(this.MinorSemiaxis);
                    return res1 && res2;
                }
            }


        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_point.GetHashCode(), _v1.GetHashCode(), _v2.GetHashCode());
        }

        /// <summary>
        /// 全局坐标系中对象的字符串表示形式。
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override String ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }

        /// <summary>
        /// 参考坐标系中对象的字符串表示。
        /// String representation of an object in reference coordinate system.
        /// </summary>
        public String ToString(Coord3d coord)
        {
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d P = _point.ConvertTo(coord);
            Vector3d v1 = _v1.ConvertTo(coord);
            Vector3d v2 = _v2.ConvertTo(coord);

            string str = string.Format("Ellipse: ") + nl;
            str += string.Format("  Center -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", P.X, P.Y, P.Z) + nl;
            str += string.Format("  Semiaxis A -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", v1.X, v1.Y, v1.Z) + nl;
            str += string.Format("  Semiaxis B -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", v2.X, v2.Y, v2.Z) + nl;
            return str;
        }

        // Operators overloads
        //-----------------------------------------------------------------

        public static bool operator ==(Ellipse c1, Ellipse c2)
        {
            if (object.ReferenceEquals(c1, null))
                return object.ReferenceEquals(c2, null);
            return c1.Equals(c2);
        }
        public static bool operator !=(Ellipse c1, Ellipse c2)
        {
            if (object.ReferenceEquals(c1, null))
                return !object.ReferenceEquals(c2, null);
            return !c1.Equals(c2);
        }

    }
}

