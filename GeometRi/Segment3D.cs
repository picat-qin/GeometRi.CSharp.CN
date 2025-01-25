﻿using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 三维空间中由两个端点定义的线段。<br></br>
    /// Line segment in 3D space defined by two end points.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Segment3d : FiniteObject, ILinearObject, IFiniteObject
    {

        private Point3d _p1;
        private Point3d _p2;

        /// <summary>
        /// 使用两点初始化线段。<br></br>
        /// Initializes line segment using two points.
        /// </summary>
        public Segment3d(Point3d p1, Point3d p2)
        {
            _p1 = p1.Copy();
            _p2 = p2.ConvertTo(p1.Coord);
        }

        /// <summary>
        /// 创建对象的副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Segment3d Copy()
        {
            return new Segment3d(_p1,_p2);
        }

        /// <summary>
        /// 点1
        /// </summary>
        public Point3d P1
        {
            get { return _p1.Copy(); }
            set { _p1 = value.Copy(); }
        }

        /// <summary>
        /// 点2
        /// </summary>
        public Point3d P2
        {
            get { return _p2.Copy(); }
            set { _p2 = value.Copy(); }
        }

        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d Center
        {
            get { return (_p1 + _p2) / 2; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public double Length
        {
            get { return _p1.DistanceTo(_p2); }
        }

        /// <summary>
        /// 转换为矢量
        /// </summary>
        public Vector3d ToVector
        {
            get { return new Vector3d(_p1, _p2); }
        }

        /// <summary>
        /// 转换为射线
        /// </summary>
        public Ray3d ToRay
        {
            get { return new Ray3d(_p1, new Vector3d(_p1, _p2)); }
        }

        /// <summary>
        /// 转换为无限线
        /// </summary>
        public Line3d ToLine
        {
            get { return new Line3d(_p1, _p2); }
        }

        /// <summary>
        /// 线段的方向向量<br></br>
        /// Direction vector of the segment
        /// </summary>
        /// <returns></returns>
        public Vector3d Direction
        {
            get { return this.ToVector.Normalized; }
        }

        /// <inheritdoc/>
        public bool IsOriented
        {
            get { return false; }
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
        /// 返回从线段到点的最短距离<br></br>
        /// Returns shortest distance from segment to the point
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            return p.DistanceTo(this);
        }

        /// <summary>
        /// 线段上最接近目标点“p”的点。<br></br>
        /// Point on segment closest to target point "p".
        /// </summary>
        public Point3d ClosestPoint(Point3d p)
        {
            Point3d projection_point = p.ProjectionTo(this.ToLine);
            if (projection_point.BelongsTo(this))
            {
                return projection_point;
            }
            else
            {
                double dist1 = p.DistanceTo(this.P1);
                double dist2 = p.DistanceTo(this.P2);

                if (dist1 <= dist2)
                {
                    return this.P1;
                }
                else
                {
                    return this.P2;
                }
            }
        }

        /// <summary>
        /// 返回从线段到平面的最短距离<br></br>
        /// Returns shortest distance from segment to the plane
        /// </summary>
        public double DistanceTo(Plane3d s)
        {

            object obj = this.IntersectionWith(s);

            if (obj == null)
            {
                return Min(this.P1.DistanceTo(s), this.P2.DistanceTo(s));
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 返回从线段到线的最短距离<br></br>
        /// Returns shortest distance from segment to the line
        /// </summary>
        public double DistanceTo(Line3d l)
        {
            Point3d p = l.PerpendicularTo(this.ToLine);
            if (p != null && p.BelongsTo(this))
            {
                return l.DistanceTo(this.ToLine);
            }
            else
            {
                return Min(this.P1.DistanceTo(l), this.P2.DistanceTo(l));
            }
        }

        /// <summary>
        /// 返回两个线段之间的最短距离<br></br>
        /// Returns shortest distance between two segments
        /// </summary>
        public double DistanceTo(Segment3d s)
        {
            return _DistanceTo(s, out double sc, out double tc);
        }

        /// <summary>
        /// 返回两个线段之间的最短距离（包含最近点）<br></br>
        /// Returns shortest distance between two segments (with closest points)
        /// </summary>
        public double DistanceTo(Segment3d s, out Point3d point_on_this_segment, out Point3d point_on_target_segment)
        {
            double p1, p2;
            double dist = _DistanceTo(s, out p1, out p2);
            point_on_this_segment = this.P1.Translate(p1 * this.ToVector);
            point_on_target_segment = s.P1.Translate(p2 * s.ToVector);
            return dist;
        }
        internal double _DistanceTo(Segment3d s, out double p1, out double p2)
        {

            // Algorithm by Dan Sunday
            // http://geomalgorithms.com/a07-_distance.html

            double small = GeometRi3D.Tolerance;


            Vector3d u = this.ToVector;
            Vector3d v = s.ToVector;
            Vector3d w = new Vector3d(s.P1, this.P1);

            double a = u * u;
            double b = u * v;
            double c = v * v;
            double d = u * w;
            double e = v * w;

            double DD = a * c - b * b;
            double sc = 0;
            double sN = 0;
            double sD = 0;
            double tc = 0;
            double tN = 0;
            double tD = 0;
            sD = DD;
            tD = DD;

            if (DD < small)
            {
                // the lines are almost parallel, force using point Me.P1 to prevent possible division by 0.0 later
                sN = 0.0;
                sD = 1.0;
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if ((sN < 0.0))
                {
                    // sc < 0 => the s=0 edge Is visible
                    sN = 0.0;
                    tN = e;
                    tD = c;
                }
                else if ((sN > sD))
                {
                    // sc > 1  => the s=1 edge Is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if ((tN < 0.0))
            {
                // tc < 0 => the t=0 edge Is visible
                tN = 0.0;
                // recompute sc for this edge
                if ((-d < 0.0))
                {
                    sN = 0.0;
                }
                else if ((-d > a))
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if ((tN > tD))
            {
                // tc > 1  => the t=1 edge Is visible
                tN = tD;
                // recompute sc for this edge
                if (((-d + b) < 0.0))
                {
                    sN = 0;
                }
                else if (((-d + b) > a))
                {
                    sN = sD;
                }
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            // finally do the division to get sc And tc
            sc = Abs(sN) < small ? 0.0 : sN / sD;
            tc = Abs(tN) < small ? 0.0 : tN / tD;

            // get the difference of the two closest points
            Vector3d dP = w + (sc * u) - (tc * v);
            // =  S1(sc) - S2(tc)

            p1 = sc;
            p2 = tc;

            return dP.Norm;

        }

        /// <summary>
        /// 返回从线段到射线的最短距离<br></br>
        /// Returns shortest distance from segment to ray
        /// </summary>
        public double DistanceTo(Ray3d r)
        {


            Point3d p1 = this.ToLine.PerpendicularTo(r.ToLine);
            Point3d p2 = r.ToLine.PerpendicularTo(this.ToLine);

            if (p1 != null && p2 != null && p1.BelongsTo(r) && p2.BelongsTo(this))
            {
                return this.ToLine.DistanceTo(r.ToLine);
            }

            double d1 = double.PositiveInfinity;
            double d2 = double.PositiveInfinity;
            double d3 = double.PositiveInfinity;
            bool flag = false;

            if (r.Point.ProjectionTo(this.ToLine).BelongsTo(this))
            {
                d1 = r.Point.DistanceTo(this.ToLine);
                flag = true;
            }
            if (this.P1.ProjectionTo(r.ToLine).BelongsTo(r))
            {
                d2 = this.P1.DistanceTo(r.ToLine);
                flag = true;
            }
            if (this.P2.ProjectionTo(r.ToLine).BelongsTo(r))
            {
                d3 = this.P2.DistanceTo(r.ToLine);
                flag = true;
            }

            if (flag)
                return Min(d1, Min(d2, d3));

            return Min(this.P1.DistanceTo(r.Point), this.P2.DistanceTo(r.Point));

        }

        /// <summary>
        /// 线段与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between segment and circle (including interior points)
        /// </summary>
        public double DistanceTo(Circle3d c)
        {
            return c.DistanceTo(this);
        }

        /// <summary>
        /// 线段与球体之间的最短距离<br></br>
        /// Shortest distance between segment and sphere
        /// </summary>
        public double DistanceTo(Sphere s)
        {
            return s.DistanceTo(this);
        }

        /// <summary>
        /// 线段与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between segment and circle (including interior points)
        /// </summary>
        /// <param name="c">目标圆<br></br> Target circle</param>
        /// <param name="point_on_segment">线段上最近的点Closest point on segment</param>
        /// <param name="point_on_circle">圆上最近的点Closest point on circle</param>
        public double DistanceTo(Circle3d c, out Point3d point_on_segment, out Point3d point_on_circle)
        {
            return c.DistanceTo(this, out point_on_circle, out point_on_segment);
        }

        /// <summary>
        /// 线段与三角形之间的最短距离<br></br>
        /// Shortest distance between segment and triangle
        /// </summary>
        public double DistanceTo(Triangle t)
        {
            return t.DistanceTo(this);
        }

        /// <summary>
        /// 线段和三角形之间的最短距离（包含最近点）<br></br>
        /// Shortest distance between segment and triangle (with closest points)
        /// </summary>
        public double DistanceTo(Triangle t, out Point3d point_on_segment, out Point3d point_on_triangle)
        {
            return t.DistanceTo(this, out point_on_triangle, out point_on_segment);
        }
        #endregion

        /// <summary>
        /// <para>测试线段是否位于线的 epsilon 邻域内。</para> 
        /// <para>Epsilon 邻域由 Geomet Ri3D.Tolerance 属性定义。</para> 
        /// <para>对于相对公差测试，线段长度的一小部分用于定义 epsilon 邻域。</para>
        /// <para>Test if segment is located in the epsilon neighborhood of the line.</para>
        /// <para>Epsilon neighborhood is defined by a GeometRi3D.Tolerance property.</para>
        /// <para>For relative tolerance tests a fraction of the segment's length is used to define epsilon neighborhood.</para>
        /// </summary>
        public bool BelongsTo(Line3d l)
        {
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return _p1.BelongsTo(l) && _p2.BelongsTo(l);
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Length;
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = this.BelongsTo(l);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        #region "BoundingBox"
        /// <summary>
        /// 返回最小边界框。<br></br>
        /// Return minimum bounding box.
        /// </summary>
        public Box3d MinimumBoundingBox
        {
            get
            {
                Vector3d v1 = new Vector3d(_p1, _p2).Normalized;
                Vector3d v2 = v1.OrthogonalVector.Normalized;
                Vector3d v3 = v1.Cross(v2);
                Matrix3d m = new Matrix3d(v1, v2, v3);
                Rotation r = new Rotation(m.Transpose());
                return new Box3d(0.5 * (_p1 + _p2), Length, 0, 0, r);
            }
        }

        /// <summary>
        /// 返回给定坐标系中的轴对齐边界框（AABB）。<br></br>
        /// Return Axis Aligned Bounding Box (AABB) in given coordinate system.
        /// </summary>
        public Box3d BoundingBox(Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            Line3d l1 = new Line3d(coord.Origin, coord.Xaxis);
            Line3d l2 = new Line3d(coord.Origin, coord.Yaxis);
            Line3d l3 = new Line3d(coord.Origin, coord.Zaxis);
            object s1 = this.ProjectionTo(l1);
            object s2 = this.ProjectionTo(l2);
            object s3 = this.ProjectionTo(l3);
            double lx, ly, lz;
            if (s1.GetType() == typeof(Segment3d)) { lx = ((Segment3d)s1).Length; } else { lx = 0.0; }
            if (s2.GetType() == typeof(Segment3d)) { ly = ((Segment3d)s2).Length; } else { ly = 0.0; }
            if (s3.GetType() == typeof(Segment3d)) { lz = ((Segment3d)s3).Length; } else { lz = 0.0; }
            return new Box3d(0.5 * (_p1 + _p2), lx, ly, lz, coord);
        }

        /// <summary>
        /// 返回边界球。<br></br>
        /// Return bounding sphere.
        /// </summary>
        public Sphere BoundingSphere
        {
            get { return new Sphere(0.5*(_p1+_p2), 0.5*Length); }

        }
        #endregion

        /// <summary>
        /// 得到线段与平面的交点。<br></br>
        /// Get intersection of segment with plane.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Plane3d s)
        {

            object obj = this.ToRay.IntersectionWith(s);

            if (obj == null)
            {
                return null;
            }
            else
            {
                if (object.ReferenceEquals(obj.GetType(), typeof(Ray3d)))
                {
                    return this;
                }
                else
                {
                    Ray3d r = new Ray3d(this.P2, new Vector3d(this.P2, this.P1));
                    object obj2 = r.IntersectionWith(s);
                    if (obj2 == null)
                    {
                        return null;
                    }
                    else
                    {
                        return (Point3d)obj2;
                    }
                }
            }
        }

        /// <summary>
        /// 获得线段与线的交点。<br></br>
        /// Get intersection of segment with line.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {

            if (this.BelongsTo(l)) { return this.Copy(); }

            Point3d p = l.PerpendicularTo(this.ToLine);
            if (p != null && p.BelongsTo(this) && p.BelongsTo(l))
            {
                return p;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取某段与其他段的交点。<br></br>
        /// Get intersection of segment with other segment.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Segment3d s)
        {

            if (this == s) { return this.Copy(); }

            Line3d l1 = this.ToLine;
            Line3d l2 = s.ToLine;

            if (this.BelongsTo(l2) || s.BelongsTo(l1))
            {
                // Segments are collinear

                // Relative tolerance check ================================
                double tol = GeometRi3D.Tolerance;
                if (!GeometRi3D.UseAbsoluteTolerance)
                {
                    tol = GeometRi3D.Tolerance * Max(this.Length, s.Length);
                }
                //==========================================================

                // Create local CS with X-axis along segment 's'
                Vector3d v2 = s.ToVector.OrthogonalVector;
                Coord3d cs = new Coord3d(s.P1, s.ToVector, v2);
                double x1 = 0.0;
                double x2 = s.Length;

                double t3 = this.P1.ConvertTo(cs).X;
                double t4 = this.P2.ConvertTo(cs).X;
                double x3 = Min(t3, t4);
                double x4 = Max(t3, t4);

                // Segments do not overlap
                if (GeometRi3D.Smaller(x4, x1, tol) || GeometRi3D.Greater(x3, x2, tol)) { return null; }

                // One common point
                if (GeometRi3D.AlmostEqual(Max(x3, x4), x1, tol)) { return new Point3d(x1, 0, 0, cs); }
                if (GeometRi3D.AlmostEqual(Min(x3, x4), x2, tol)) { return new Point3d(x2, 0, 0, cs); }

                // Overlaping segments
                x1 = Max(x1, x3);
                x2 = Min(x2, x4);
                return new Segment3d(new Point3d(x1, 0, 0, cs), new Point3d(x2, 0, 0, cs));
            }
            else
            {
                Point3d p = l1.PerpendicularTo(l2);
                if (p != null && p.BelongsTo(this) && p.BelongsTo(s))
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
        /// 获取线段与球体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with sphere.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Sphere s)
        {
            return s.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与椭圆体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with ellipsoid.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ellipsoid e)
        {
            return e.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与椭圆的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with ellipse.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ellipse e)
        {
            return e.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与圆的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with circle.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Circle3d c)
        {
            return c.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与射线的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with ray.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ray3d r)
        {
            return r.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与三角形的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with triangle.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Triangle t)
        {
            return t.IntersectionWith(this);
        }

        /// <summary>
        /// 获取线段与框的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with box.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Box3d b)
        {
            return b.IntersectionWith(this);
        }

        /// <summary>
        /// 线段与圆的相交检查<br></br>
        /// Intersection check between segment and circle
        /// </summary>
        public bool Intersects(Circle3d c)
        {
            return this.IntersectionWith(c) != null;
        }

        /// <summary>
        /// 获取线段到线的正交投影。<br></br>
        /// 返回“Segment3d”或“Point3d”类型的对象<br></br>
        /// Get the orthogonal projection of a segment to the line.<br></br>
        /// Return object of type 'Segment3d' or 'Point3d'
        /// </summary>
        public object ProjectionTo(Line3d l)
        {
            if (this.ToVector.IsOrthogonalTo(l.Direction))
            {
                // Segment is perpendicular to the line
                return this.P1.ProjectionTo(l);
            }
            else
            {
                return new Segment3d(this.P1.ProjectionTo(l), this.P2.ProjectionTo(l));
            }
        }

        /// <summary>
        /// 获取线段在平面上的正交投影。<br></br>
        /// 返回“Segment3d”或“Point3d”类型的对象<br></br>
        /// Get the orthogonal projection of a segment to the plane.<br></br>
        /// Return object of type 'Segment3d' or 'Point3d'
        /// </summary>
        public object ProjectionTo(Plane3d s)
        {
            if (this.ToVector.IsParallelTo(s.Normal))
            {
                // Segment is perpendicular to the plane
                return this.P1.ProjectionTo(s);
            }
            else
            {
                return new Segment3d(this.P1.ProjectionTo(s), this.P2.ProjectionTo(s));
            }
        }

        internal override int _PointLocation(Point3d p)
        {
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                Point3d proj = p.ProjectionTo(this.ToLine);
                if (GeometRi3D.AlmostEqual(p.DistanceTo(proj),0))
                {
                    if (GeometRi3D.AlmostEqual(p.DistanceTo(this._p1), 0) || GeometRi3D.AlmostEqual(p.DistanceTo(this._p2), 0))
                    {
                        return 0; // Point is on boundary
                    }
                    else if (Abs(new Vector3d(proj, this._p1).Normalized.AngleTo(new Vector3d(proj, this._p2).Normalized)) < PI/2)
                    {
                        return -1; // Point is outside
                    }
                    else
                    {
                        return 1; // // Point is strictly inside
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
                GeometRi3D.Tolerance = tol * this.Length;
                GeometRi3D.UseAbsoluteTolerance = true;
                int result = this._PointLocation(p);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        internal int _AxialPointLocation(Point3d p)
        {
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                if (GeometRi3D.AlmostEqual(p.DistanceTo(this._p1), 0) || GeometRi3D.AlmostEqual(p.DistanceTo(this._p2), 0))
                {
                    return 0; // Point is on boundary
                }
                else if (Abs(new Vector3d(p, this._p1).Normalized.AngleTo(new Vector3d(p, this._p2).Normalized)) < PI / 2)
                {
                    return -1; // Point is outside
                }
                else
                {
                    return 1; // // Point is strictly inside
                }
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Length;
                GeometRi3D.UseAbsoluteTolerance = true;
                int result = this._AxialPointLocation(p);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
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
        /// 两个物体之间的角度（以度为单位） (0 &lt; angle &lt; 180)<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(IPlanarObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }
        #endregion

        #region "TranslateRotateReflect"
        /// <summary>
        /// 通过向量平移线段
        /// Translate segment by a vector
        /// </summary>
        public Segment3d Translate(Vector3d v)
        {
            return new Segment3d(P1.Translate(v), P2.Translate(v));
        }

        /// <summary>
        /// Rotate segment by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object and specify rotation center: this.Rotate(Rotation r, Point3d p)")]
        public virtual Segment3d Rotate(Matrix3d m)
        {
            return new Segment3d(P1.Rotate(m), P2.Rotate(m));
        }

        /// <summary>
        /// Rotate segment by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r, Point3d p)")]
        public virtual Segment3d Rotate(Matrix3d m, Point3d p)
        {
            return new Segment3d(P1.Rotate(m, p), P2.Rotate(m, p));
        }

        /// <summary>
        /// 以点 'p' 为中心旋转线段<br></br>
        /// Rotate segment around point 'p' as a rotation center
        /// </summary>
        public virtual Segment3d Rotate(Rotation r, Point3d p)
        {
            return new Segment3d(P1.Rotate(r, p), P2.Rotate(r, p));
        }

        /// <summary>
        /// 在给定点处反射线段<br></br>
        /// Reflect segment in given point
        /// </summary>
        public virtual Segment3d ReflectIn(Point3d p)
        {
            return new Segment3d(P1.ReflectIn(p), P2.ReflectIn(p));
        }

        /// <summary>
        /// 反射给定线段<br></br>
        /// Reflect segment in given line
        /// </summary>
        public virtual Segment3d ReflectIn(Line3d l)
        {
            return new Segment3d(P1.ReflectIn(l), P2.ReflectIn(l));
        }

        /// <summary>
        /// 在给定平面内反射线段<br></br>
        /// Reflect segment in given plane
        /// </summary>
        public virtual Segment3d ReflectIn(Plane3d s)
        {
            return new Segment3d(P1.ReflectIn(s), P2.ReflectIn(s));
        }

        /// <summary>
        /// 相对于给定点的缩放段<br></br>
        /// Scale segment relative to given point
        /// </summary>
        public virtual Segment3d Scale(double scale, Point3d scaling_center)
        {
            Point3d new_p1 = scaling_center + scale * (this._p1 - scaling_center);
            Point3d new_p2 = scaling_center + scale * (this._p2 - scaling_center);
            return new Segment3d(new_p1, new_p2);
        }

        /// <summary>
        /// 相对于其中心缩放片段<br></br>
        /// Scale segment relative to its center
        /// </summary>
        public virtual Segment3d Scale(double scale)
        {
            Point3d scaling_center = this.Center;
            Point3d new_p1 = scaling_center + scale * (this._p1 - scaling_center);
            Point3d new_p2 = scaling_center + scale * (this._p2 - scaling_center);
            return new Segment3d(new_p1, new_p2);
        }
        #endregion

        /// <summary>
        /// Determines whether two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }
            Segment3d s = (Segment3d)obj;
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return (this.P1 == s.P1 && this.P2 == s.P2) | (this.P1 == s.P2 && this.P2 == s.P1);
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Length;
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = (this.P1 == s.P1 && this.P2 == s.P2) | (this.P1 == s.P2 && this.P2 == s.P1);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        /// <summary>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_p1.GetHashCode(), _p2.GetHashCode());
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
        public String ToString(Coord3d coord)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d p1 = _p1.ConvertTo(coord);
            Point3d p2 = _p2.ConvertTo(coord);

            str.Append("Segment:" + nl);
            str.Append(string.Format("Point 1  -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p1.X, p1.Y, p1.Z) + nl);
            str.Append(string.Format("Point 2 -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p2.X, p2.Y, p2.Z));
            return str.ToString();
        }

        // Operators overloads
        //-----------------------------------------------------------------
        public static bool operator ==(Segment3d l1, Segment3d l2)
        {
            if (object.ReferenceEquals(l1, null))
                return object.ReferenceEquals(l2, null);
            return l1.Equals(l2);
        }
        public static bool operator !=(Segment3d l1, Segment3d l2)
        {
            if (object.ReferenceEquals(l1, null))
                return !object.ReferenceEquals(l2, null);
            return !l1.Equals(l2);
        }

    }
}


