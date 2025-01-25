﻿using System;
using static System.Math;
using System.Collections.Generic;

namespace GeometRi
{
    /// <summary>
    /// 由中心点、半径和法线向量定义的三维空间中的圆。<br></br>
    /// Circle in 3D space defined by center point, radius and normal vector.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Circle3d : FiniteObject, IPlanarObject, IFiniteObject
    {

        internal Point3d _point;
        private double _r;
        internal Vector3d _normal;

        /// <summary>
        /// 使用中心点、半径和法线向量初始化圆实例。<br></br>
        /// Initializes circle instance using center point, radius and normal vector.
        /// </summary>
        public Circle3d(Point3d Center, double Radius, Vector3d Normal)
        {
            _point = Center.Copy();
            _r = Radius;
            _normal = Normal.Normalized;
        }

        /// <summary>
        /// 初始化通过三点的圆。<br></br>
        /// Initializes circle passing through three points.
        /// </summary>
        public Circle3d(Point3d p1, Point3d p2, Point3d p3)
        {
            Vector3d v1 = new Vector3d(p1, p2);
            Vector3d v2 = new Vector3d(p1, p3);
            if (v1.Cross(v2).Norm < GeometRi3D.Tolerance)
            {
                throw new Exception("Collinear points");
            }

            Coord3d CS = new Coord3d(p1, v1, v2);
            Point3d a1 = p1.ConvertTo(CS);
            Point3d a2 = p2.ConvertTo(CS);
            Point3d a3 = p3.ConvertTo(CS);

            double d1 = Math.Pow(a1.X, 2) + Math.Pow(a1.Y, 2);
            double d2 = Math.Pow(a2.X, 2) + Math.Pow(a2.Y, 2);
            double d3 = Math.Pow(a3.X, 2) + Math.Pow(a3.Y, 2);
            double f = 2.0 * (a1.X * (a2.Y - a3.Y) - a1.Y * (a2.X - a3.X) + a2.X * a3.Y - a3.X * a2.Y);

            double X = (d1 * (a2.Y - a3.Y) + d2 * (a3.Y - a1.Y) + d3 * (a1.Y - a2.Y)) / f;
            double Y = (d1 * (a3.X - a2.X) + d2 * (a1.X - a3.X) + d3 * (a2.X - a1.X)) / f;
            //_point = (new Point3d(X, Y, 0, CS)).ConvertTo(p1.Coord);
            _point = new Point3d(X, Y, 0, CS);
            _point = _point.ConvertTo(p1.Coord);
            _r = Sqrt((X - a1.X) * (X - a1.X) + (Y - a1.Y) * (Y - a1.Y));
            _normal = v1.Cross(v2).Normalized;

        }

        /// <summary>
        /// 创建对象的副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Circle3d Copy()
        {
            return new Circle3d(_point, _r, _normal);
        }

        /// <summary>
        /// 圆心<br></br>
        /// Center of the circle
        /// </summary>
        public Point3d Center
        {
            get { return _point.Copy(); }
            set { _point = value.Copy(); }
        }

        /// <summary>
        /// 圆的半径<br></br>
        /// Radius of the circle
        /// </summary>
        public double R
        {
            get { return _r; }
            set { _r = value; }
        }

        /// <summary>
        /// 圆的法线
        /// Normal of the circle
        /// </summary>
        public Vector3d Normal
        {
            get { return _normal.Copy(); }
            set { _normal = value.Copy(); }
        }

        /// <inheritdoc/>
        public bool IsOriented
        {
            get { return false; }
        }

        /// <summary>
        /// 圆的周长<br></br>
        /// Perimeter of the circle.
        /// </summary>
        public double Perimeter
        {
            get { return 2 * PI * _r; }
        }

        /// <summary>
        /// 圆的面积。<br></br>
        /// Area of the circle.
        /// </summary>
        public double Area
        {
            get { return PI * Math.Pow(_r, 2); }
        }

        /// <summary>
        /// 将圆转换为椭圆对象。<br></br>
        /// Convert circle to ellipse object.
        /// </summary>
        public Ellipse ToEllipse
        {
            get
            {
                Vector3d v1 = _r * _normal.OrthogonalVector.Normalized;
                Vector3d v2 = _r * (_normal.Cross(v1)).Normalized;
                return new Ellipse(_point, v1, v2);
            }
        }

        /// <summary>
        /// 将圆形转换为平面对象。<br></br>
        /// Convert circle to plane object.
        /// </summary>
        public Plane3d ToPlane
        {
            get
            {
                return new Plane3d(_point, _normal);
            }
        }

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
        /// 检查两个物体是否共面
        /// Check if two objects are coplanar
        /// </summary>
        public bool IsCoplanarTo(ILinearObject obj)
        {
            return GeometRi3D._coplanar(this, obj);
        }
        #endregion

        #region "边界框 BoundingBox"
        /// <summary>
        /// 返回最小边界框。<br></br>
        /// Return minimum bounding box.
        /// </summary>
        public Box3d MinimumBoundingBox
        {
            get
            {
                Vector3d v1 = this.Normal.OrthogonalVector.Normalized;
                Vector3d v2 = this.Normal.Cross(v1).Normalized;
                Vector3d v3 = this.Normal;
                Matrix3d m = new Matrix3d(v1, v2, v3);
                Rotation r = new Rotation(m.Transpose());
                return new Box3d(_point, 2.0 * _r, 2.0 * _r, 0, r);
            }
        }

        /// <summary>
        /// 返回给定坐标系中的边界框。<br></br>
        /// Return Bounding Box in given coordinate system.
        /// </summary>
        public Box3d BoundingBox(Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;

            double s1 = _r * Cos(coord.Xaxis.AngleTo(this));
            double s2 = _r * Cos(coord.Yaxis.AngleTo(this));
            double s3 = _r * Cos(coord.Zaxis.AngleTo(this));

            return new Box3d(_point, 2 * s1, 2 * s2, 2 * s3, coord);
        }

        /// <summary>
        /// 返回轴对齐边界框（AABB）。<br></br>
        /// Return Axis Aligned Bounding Box (AABB).
        /// </summary>
        public AABB AABB()
        {
            double s1 = _r * Cos(Coord3d.GlobalCS.Xaxis.AngleTo(this));
            double s2 = _r * Cos(Coord3d.GlobalCS.Yaxis.AngleTo(this));
            double s3 = _r * Cos(Coord3d.GlobalCS.Zaxis.AngleTo(this));

            return new AABB(_point, 2 * s1, 2 * s2, 2 * s3);
        }

        /// <summary>
        /// 返回边界球。<br></br>
        /// Return bounding sphere.
        /// </summary>
        public Sphere BoundingSphere
        {
            get { return new Sphere(_point, _r); }

        }

        /// <summary>
        /// 检查圆是否位于由全局公差属性（Geomet Ri3D.Tolerance）定义的公差的框内。<br></br>
        /// Check if circle is located inside box with tolerance defined by global tolerance property (GeometRi3D.Tolerance).
        /// </summary>
        public bool IsInside(Box3d box)
        {
            // 相对公差 ================================
            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.R;
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = this.IsInside(box);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            if (!this._point.IsInside(box)) return false;

            Coord3d local_coord = box.LocalCoord();
            Box3d circle_box = this.BoundingBox(local_coord);
            Point3d p = _point.ConvertTo(local_coord);

            if (box.L1 / 2 - (Abs(p.X) + circle_box.L1 / 2) < GeometRi3D.Tolerance) return false;
            if (box.L2 / 2 - (Abs(p.Y) + circle_box.L2 / 2) < GeometRi3D.Tolerance) return false;
            if (box.L3 / 2 - (Abs(p.Z) + circle_box.L3 / 2) < GeometRi3D.Tolerance) return false;

            return true;
        }

        #endregion

        /// <summary>
        /// 返回给定参数“t”的圆上的点（0 &lt; t &lt; 2Pi）<br></br>
        /// Returns point on circle for given parameter 't' (0 &lt;= t &lt; 2Pi)
        /// </summary>
        public Point3d ParametricForm(double t)
        {

            // Get two orthogonal coplanar vectors
            Vector3d v1 = _r * _normal.OrthogonalVector.Normalized;
            Vector3d v2 = _r * (_normal.Cross(v1)).Normalized;
            return _point + v1.ToPoint * Cos(t) + v2.ToPoint * Sin(t);

        }

        #region "DistanceMethods"
        /// <summary>
        /// 圆到平面的距离<br></br>
        /// Distance from circle to plane
        /// </summary>
        public double DistanceTo(Plane3d s)
        {
            double center_distance = this._point.DistanceTo(s);
            double sin_angle = this._normal.Cross(s._normal).Norm / this._normal.Norm / s._normal.Norm;
            double delta = Abs(this.R * sin_angle);

            if (delta < center_distance)
            {
                return center_distance - delta;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 平面与圆之间的最短距离
        /// Shortest distance between plane and circle
        /// <para> 
        /// 如果物体平行或相交，输出点可能不唯一。<br></br>
        /// The output points may be not unique in case of parallel or intersecting objects.
        /// </para>
        /// </summary>
        /// <param name="p">目标平面<br></br>Target plane</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_plane">平面上最近的点<br></br> Closest point on plane</param>
        public double DistanceTo(Plane3d p, out Point3d point_on_circle, out Point3d point_on_plane)
        {
            if (this.IsParallelTo(p))
            {
                point_on_circle = this.Center;
                point_on_plane = point_on_circle.ProjectionTo(p);
                return point_on_circle.DistanceTo(point_on_plane);
            }

            Vector3d v1 = this._normal.Cross(p.Normal);
            Vector3d v2 = this._normal.Cross(v1);
            Line3d l = new Line3d(this._point, v2);
            Point3d intersection_point = (Point3d)l.IntersectionWith(p);

            if (intersection_point.DistanceTo(this) <= GeometRi3D.DefaultTolerance)
            {
                point_on_circle = intersection_point;
                point_on_plane = intersection_point;
                return 0;
            }
            else
            {
                v1 = new Vector3d(this._point, intersection_point).Normalized;
                point_on_circle = this._point.Translate(this.R * v1);
                point_on_plane = point_on_circle.ProjectionTo(p);
                return point_on_circle.DistanceTo(point_on_plane);
            }
        }

        /// <summary>
        /// 点到圆的最短距离（包括内部点）<br></br>
        /// Shortest distance from point to circle (including interior points)
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            Point3d projection = p.ProjectionTo(this.ToPlane);
            if (projection.Coord != this._point.Coord)
            {
                projection = projection.ConvertTo(this._point.Coord);
            }
            double proj_dist_cent = projection.DistanceTo(this._point);

            if (proj_dist_cent <= this.R)
            {
                return projection.DistanceTo(p);
            }
            else
            {
                // 找到圆边界上的最近点
                // find closest point on circle's boundary
                double x = this._point.X + this.R / proj_dist_cent * (projection.X - this._point.X);
                double y = this._point.Y + this.R / proj_dist_cent * (projection.Y - this._point.Y);
                double z = this._point.Z + this.R / proj_dist_cent * (projection.Z - this._point.Z);

                return Sqrt((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y) + (z - p.Z) * (z - p.Z));
            }
        }

        /// <summary>
        /// 圆上最接近目标点“p”的点（包括内部点）。<br></br>
        /// Point on circle (including interior points) closest to target point "p".
        /// </summary>
        public Point3d ClosestPoint(Point3d p)
        {
            Point3d projection = p.ProjectionTo(this.ToPlane);
            if (projection.Coord != this._point.Coord)
            {
                projection = projection.ConvertTo(this._point.Coord);
            }
            double proj_dist_cent = projection.DistanceTo(this._point);

            if (proj_dist_cent <= this.R)
            {
                return projection;
            }
            else
            {
                // 找到圆边界上的最近点
                // find closest point on circle's boundary
                double x = this._point.X + this.R / proj_dist_cent * (projection.X - this._point.X);
                double y = this._point.Y + this.R / proj_dist_cent * (projection.Y - this._point.Y);
                double z = this._point.Z + this.R / proj_dist_cent * (projection.Z - this._point.Z);

                return new Point3d(x, y, z, this._point.Coord);
            }
        }

        /// <summary>
        /// 两个圆之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between two circles (including interior points) (approximate solution)
        /// <para> 
        /// 数值解的默认公差：Geomet Ri3D.Default Tolerance。<br></br>
        /// Default tolerance for numerical solution: GeometRi3D.DefaultTolerance.
        /// </para>
        /// </summary>
        /// <param name="c">Target circle</param>
        public double DistanceTo(Circle3d c)
        {
            return DistanceTo(c, GeometRi3D.DefaultTolerance);
        }

        /// <summary>
        /// 两个圆之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between two circles (including interior points) (approximate solution)
        /// </summary>
        /// <param name="c">目标圆<br></br> Target circle</param>
        /// <param name="tolerance">
        /// 数值解的公差，默认 Geomet Ri3D.Default Tolerance<br></br>
        /// Tolerance for numerical solution, default GeometRi3D.DefaultTolerance
        /// </param>
        public double DistanceTo(Circle3d c, double tolerance)
        {
            double dist;
            if (this._normal.IsParallelTo(c._normal))
            {
                Point3d projection = c._point.ProjectionTo(this.ToPlane);
                dist = projection.DistanceTo(this._point);
                double vdist = projection.DistanceTo(c._point);
                if (dist < this.R + c.R)
                {
                    return vdist;
                }
                else
                {
                    return Sqrt((dist - this.R - c.R) * (dist - this.R - c.R) + vdist * vdist);
                }
            }

            double tol = GeometRi3D.Tolerance;
            bool mode = GeometRi3D.UseAbsoluteTolerance;
            GeometRi3D.Tolerance = GeometRi3D.DefaultTolerance;
            GeometRi3D.UseAbsoluteTolerance = true;

            object obj = this.IntersectionWith(c);
            if (obj != null)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;

                return 0;
            }

            Point3d p_on_circle, p_on_plane;
            dist = this.DistanceTo(c.ToPlane, out p_on_circle, out p_on_plane);
            if (p_on_plane.DistanceTo(c._point) <= c.R)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;
                return dist;
            }

            dist = c.DistanceTo(this.ToPlane, out p_on_circle, out p_on_plane);
            if (p_on_plane.DistanceTo(this._point) <= this.R)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;
                return dist;
            }

            dist = _distance_circle_to_circle(this, c, out Point3d p1, out Point3d p2, tolerance);
            // 恢复初始状态
            // Restore initial state
            GeometRi3D.UseAbsoluteTolerance = mode;
            GeometRi3D.Tolerance = tol;
            return dist;

        }

        /// <summary>
        /// 两个圆之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between two circles (including interior points) (approximate solution)
        /// <para> 
        /// 在平行或相交的圆的情况下，输出点可能不唯一。<br></br>
        /// The output points may be not unique in case of parallel or intersecting circles.
        /// </para>
        /// <para> 
        /// 数值解的默认公差：GeometRi3D.DefaultTolerance。
        /// Default tolerance for numerical solution: GeometRi3D.DefaultTolerance.
        /// </para>
        /// </summary>
        /// <param name="c">Target circle</param>
        /// <param name="p1">Closest point on source circle</param>
        /// <param name="p2">Closest point on target circle</param>
        public double DistanceTo(Circle3d c, out Point3d p1, out Point3d p2)
        {
            return DistanceTo(c, out p1, out p2, GeometRi3D.DefaultTolerance);
        }

        /// <summary>
        /// 两个圆之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between two circles (including interior points) (approximate solution)
        /// <para> 
        /// 如果是平行或相交的圆，输出点可能不唯一。<br></br>
        /// The output points may be not unique in case of parallel or intersecting circles.
        /// </para>
        /// </summary>
        /// <param name="c">目标圆<br></br>Target circle</param>
        /// <param name="p1">源圆上的最近点<br></br> Closest point on source circle</param>
        /// <param name="p2">目标圆上最近的点 Closest point on target circle</param>
        /// <param name="tolerance">
        /// 数值解的容差<br></br>
        /// Tolerance for numerical solution
        /// </param>
        public double DistanceTo(Circle3d c, out Point3d p1, out Point3d p2, double tolerance)
        {
            double dist;
            if (this._normal.IsParallelTo(c._normal))
            {
                Point3d projection = c._point.ProjectionTo(this.ToPlane);
                dist = projection.DistanceTo(this._point);
                double vdist = projection.DistanceTo(c._point);
                if (dist < this.R + c.R)
                {
                    if (projection.BelongsTo(this))
                    {
                        p1 = projection;
                        p2 = c.Center;
                    }
                    else
                    {
                        p1 = this._point.Translate(this.R * new Vector3d(this._point, projection).Normalized);
                        p2 = p1.ProjectionTo(c.ToPlane);
                    }
                    return vdist;
                }
                else
                {
                    Vector3d v = new Vector3d(this._point, projection).Normalized;
                    p1 = this._point.Translate(this.R * v);
                    p2 = c._point.Translate(-c.R * v);
                    return Sqrt((dist - this.R - c.R) * (dist - this.R - c.R) + vdist * vdist);
                }
            }

            double tol = GeometRi3D.Tolerance;
            bool mode = GeometRi3D.UseAbsoluteTolerance;
            GeometRi3D.Tolerance = GeometRi3D.DefaultTolerance;
            GeometRi3D.UseAbsoluteTolerance = true;

            object obj = this.IntersectionWith(c);
            if (obj != null)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;

                if (obj.GetType() == typeof(Point3d))
                {
                    p1 = (Point3d)obj;
                    p2 = (Point3d)obj;
                }
                else if (obj.GetType() == typeof(Segment3d))
                {
                    p1 = ((Segment3d)obj).P1;
                    p2 = ((Segment3d)obj).P1;
                }
                else
                {
                    p1 = ((Circle3d)obj).Center;
                    p2 = ((Circle3d)obj).Center;
                }

                return 0;
            }

            dist = this.DistanceTo(c.ToPlane, out p1, out p2);
            if (p2.DistanceTo(c._point) <= c.R)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;
                return dist;
            }

            dist = c.DistanceTo(this.ToPlane, out p2, out p1);
            if (p1.DistanceTo(this._point) <= this.R)
            {
                // 恢复初始状态
                // Restore initial state
                GeometRi3D.UseAbsoluteTolerance = mode;
                GeometRi3D.Tolerance = tol;
                return dist;
            }

            dist = _distance_circle_to_circle(this, c, out p1, out p2, tolerance);

            // 恢复初始状态
            // Restore initial state
            GeometRi3D.UseAbsoluteTolerance = mode;
            GeometRi3D.Tolerance = tol;

            return dist;

        }


        /// <summary>
        /// 检查两个圆之间的距离是否大于阈值<br></br>
        /// Check if distance between two circles is greater than threshold
        /// </summary>
        [Obsolete]
        public bool DistanceGreater(Circle3d c, double threshold, double tolerance)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * Max(this.R, c.R);
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = this.DistanceGreater(c, threshold, tolerance);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            // Early exit (separated circles)
            double d = this._point.DistanceTo(c._point);
            if (d > this.R + c.R + GeometRi3D.Tolerance + threshold)
                return true;

            if (this._normal.IsParallelTo(c._normal))
            {
                if (this._point.BelongsTo(new Plane3d(c._point, c._normal)))
                {
                    // Coplanar objects
                    if (d <= this.R + c.R + GeometRi3D.Tolerance + threshold)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // parallel objects
                    return this.DistanceTo(c, out Point3d p1, out Point3d p2, tolerance) > threshold;
                }
            }
            else
            {
                // Check 3D intersection
                Vector3d v = new Vector3d(this._point, c._point);
                double this_norm = this._normal.Norm;
                double c_norm = c._normal.Norm;

                double cos_angle1 = this._normal * v / this_norm / d;
                double delta1 = Abs(d * cos_angle1);

                double sin_angle2 = this._normal.Cross(c._normal).Norm / this_norm / c_norm;
                double delta2 = Abs(this.R * sin_angle2);

                if (delta1 > delta2 + threshold) return true;

                cos_angle1 = c._normal * v / c_norm / d;
                delta1 = Abs(d * cos_angle1);
                delta2 = Abs(c.R * sin_angle2);

                if (delta1 > delta2 + threshold) return true;

                return this.DistanceTo(c, out Point3d p1, out Point3d p2, tolerance) > threshold;

            }
        }

        /// <summary>
        /// 圆与圆之间的距离
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        private double _distance_circle_to_circle(Circle3d c1, Circle3d c2, out Point3d p1, out Point3d p2, double tol)
        // Use quadratic interpolation to find closest point on one circle to other
        // p1 and p2 - closest points on both circles
        {
            //double tol = GeometRi3D.DefaultTolerance;
            double d1 = 1e20;
            double t1 = 0;
            Point3d p;

            // Prepare data for parametric form for circle "c1".
            // _point + v1.ToPoint * Cos(t) + v2.ToPoint * Sin(t);
            // Get two orthogonal vectors coplanar "c1"
            Vector3d v1 = c1._r * c1._normal.OrthogonalVector.Normalized;
            Vector3d v2 = c1._r * (c1._normal.Cross(v1)).Normalized;
            Point3d pf1 = v1.ToPoint.ConvertTo(c1._point.Coord);
            Point3d pf2 = v2.ToPoint.ConvertTo(c1._point.Coord);

            for (int i = 0; i < 16; i++)
            {
                double t = i * Math.PI / 8;
                p = c1._point + pf1 * Cos(t) + pf2 * Sin(t);
                double dist = p.DistanceTo(c2);
                if (dist < d1)
                {
                    d1 = dist;
                    t1 = t;
                }
            }
            double t2 = t1 - Math.PI / 8;
            p = c1._point + pf1 * Cos(t2) + pf2 * Sin(t2);
            double d2 = p.DistanceTo(c2);
            double t3 = t1 + Math.PI / 8;
            p = c1._point + pf1 * Cos(t3) + pf2 * Sin(t3);
            double d3 = p.DistanceTo(c2);

            int iter = 0;
            bool flag = false;
            while ((d2 - d1 > tol || d3 - d1 > tol)  && d1 > tol)
            {
                if (++iter > 100) break;

                double ax = 2.0 * d1 / (t1 - t2) / (t1 - t3);
                double aa = 0.5 * ax * (t2 + t3);
                double bx = 2.0 * d2 / (t2 - t1) / (t2 - t3);
                double bb = 0.5 * bx * (t1 + t3);
                double cx = 2.0 * d3 / (t3 - t1) / (t3 - t2);
                double cc = 0.5 * cx * (t1 + t2);

                double t = (aa + bb + cc) / (ax + bx + cx);
                p = c1._point + pf1 * Cos(t) + pf2 * Sin(t);
                double d = p.DistanceTo(c2);

                if (d > d1)
                // Possible special case, non-smooth function ( f(t)=|t| )
                {
                    flag = true;
                    break;
                }

                if (t>t2 & t<t1)
                {
                    t3 = t1; d3 = d1;
                }
                else
                {
                    t2 = t1; d2 = d1;
                }
                t1 = t; d1 = d;
            }

            if (flag)
            // Possible special case, non-smooth function ( f(t)=|t| )
            {
                while ((d2 - d1 > tol || d3 - d1 > tol) && d1 > tol)
                {
                    if (++iter > 100) break;

                    double t = (t2+t1) / 2;
                    p = c1._point + pf1 * Cos(t) + pf2 * Sin(t);
                    double d = p.DistanceTo(c2);
                    if (d < d1)
                    {
                        t3 = t1; d3 = d1;
                        t1 = t;  d1 = d;
                    }
                    else
                    {
                        t2 = t; d2 = d;
                    }

                    t = (t3 + t1) / 2;
                    p = c1._point + pf1 * Cos(t) + pf2 * Sin(t);
                    d = p.DistanceTo(c2);
                    if (d < d1)
                    {
                        t2 = t1; d2 = d1;
                        t1 = t; d1 = d;
                    }
                    else
                    {
                        t3 = t; d3 = d;
                    }
                }
            }

            p1 = c1._point + pf1 * Cos(t1) + pf2 * Sin(t1);
            p2 = c2.ClosestPoint(p1);
            return d1;
        }

        /// <summary>
        /// 圆与球之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between circle and sphere (including interior points) (approximate solution)
        /// </summary>
        public double DistanceTo(Sphere s)
        {
            return DistanceTo(s, GeometRi3D.DefaultTolerance);
        }

        /// <summary>
        /// 圆与球之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between circle and sphere (including interior points) (approximate solution)
        /// </summary>
        /// <param name="s">目标球体 Target sphere</param>
        /// <param name="tolerance">
        /// 数值解的容差<br></br>
        /// Tolerance for numerical solution
        /// </param>
        public double DistanceTo(Sphere s, double tolerance)
        {
            Plane3d p = this.ToPlane;
            if (s.Center.ProjectionTo(p).BelongsTo(this))
            {
                return s.DistanceTo(p);
            }

            if (this.Intersects(s))
                return 0;

            Point3d p1, p2;
            double dist = _distance_circle_to_sphere(this, s, out p1, out p2, tolerance);

            return dist;
        }

        /// <summary>
        /// 圆与球之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between circle and sphere (including interior points) (approximate solution)
        /// <para> 
        /// 如果物体相交，输出点可能不唯一。<br></br>
        /// The output points may be not unique in case of intersecting objects.
        /// </para>
        /// <para> 
        /// 数值解的默认公差: GeometRi3D.DefaultTolerance.<br></br>
        /// Default tolerance for numerical solution: GeometRi3D.DefaultTolerance.
        /// </para>
        /// </summary>
        /// <param name="s">目标球<br></br>Target sphere</param>
        /// <param name="p1">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="p2">球体上最近的点<br></br> Closest point on sphere</param>
        public double DistanceTo(Sphere s, out Point3d p1, out Point3d p2)
        {
            return DistanceTo(s, out p1, out p2, GeometRi3D.DefaultTolerance);
        }

        /// <summary>
        /// 圆与球之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between circle and sphere (including interior points) (approximate solution)
        /// <para> 
        /// 如果物体相交，输出点可能不唯一。<br></br>
        /// The output points may be not unique in case of intersecting objects.
        /// </para>
        /// </summary>
        /// <param name="s">目标球<br></br> Target sphere</param>
        /// <param name="p1">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="p2">球体上最近的点<br></br> Closest point on sphere</param>
        /// <param name="tolerance">
        /// 数值解的容差<br></br>
        /// Tolerance for numerical solution
        /// </param>
        public double DistanceTo(Sphere s, out Point3d p1, out Point3d p2, double tolerance)
        {
            Plane3d p = this.ToPlane;
            if (s.Center.ProjectionTo(p).BelongsTo(this))
            {
                p1 = s.Center.ProjectionTo(p);
                if (s.Center == p1)
                {
                    p2 = s.Center.Translate(s.R * this.Normal);
                }
                else
                {
                    p2 = s.Center.Translate(s.R * new Vector3d(s.Center, p1).Normalized);
                }
                return s.DistanceTo(p);
            }

            if (this.Intersects(s))
            {
                Object obj = s.IntersectionWith(p);
                if (obj.GetType() == typeof(Point3d))
                {
                    p1 = (Point3d)obj;
                    p2 = p1;
                }
                else
                {
                    Circle3d c = (Circle3d)obj;
                    p1 = this._point.Translate(this.R * new Vector3d(this._point, c._point).Normalized);
                    p2 = c._point.Translate(c.R * new Vector3d(c._point, this._point).Normalized);
                }
                return 0;
            }

            double dist = _distance_circle_to_sphere(this, s, out p1, out p2, tolerance);

            return dist;
        }

        /// <summary>
        /// 圆到球之间的距离
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        private double _distance_circle_to_sphere(Circle3d c1, Sphere c2, out Point3d p1, out Point3d p2, double tol)
        // Use quadratic interpolation to find closest point on circle
        // p1 and p2 - closest points on circle and sphere respectively
        {
            double d1 = 1e20;
            double t1 = 0;
            Point3d p;

            for (int i = 0; i < 16; i++)
            {
                double t = i * Math.PI / 8;
                p = c1.ParametricForm(t);
                double dist = p.DistanceTo(c2);
                if (dist < d1)
                {
                    d1 = dist;
                    t1 = t;
                }
            }
            double t2 = t1 - Math.PI / 8;
            p = c1.ParametricForm(t2);
            double d2 = p.DistanceTo(c2);
            double t3 = t1 + Math.PI / 8;
            p = c1.ParametricForm(t3);
            double d3 = p.DistanceTo(c2);

            int iter = 0;
            while (d2 - d1 > 0.2 * tol && d1 > tol)
            {
                if (++iter > 100) break;

                double ax = 2.0 * d1 / (t1 - t2) / (t1 - t3);
                double aa = 0.5 * ax * (t2 + t3);
                double bx = 2.0 * d2 / (t2 - t1) / (t2 - t3);
                double bb = 0.5 * bx * (t1 + t3);
                double cx = 2.0 * d3 / (t3 - t1) / (t3 - t2);
                double cc = 0.5 * cx * (t1 + t2);

                double t = (aa + bb + cc) / (ax + bx + cx);
                p = c1.ParametricForm(t);
                double d = p.DistanceTo(c2);

                if (d < d1)
                {
                    if (t > t2 & t < t1)
                    {
                        t3 = t1; d3 = d1;
                    }
                    else
                    {
                        t2 = t1; d2 = d1;
                    }
                    t1 = t; d1 = d;
                }
                else
                {
                    if (t < t1)
                    {
                        t2 = t; d2 = d;
                    }
                    else
                    {
                        t3 = t; d3 = d;
                    }
                }


            }

            p1 = c1.ParametricForm(t1);
            p2 = c2.ClosestPoint(p1);
            return d1;
        }

        /// <summary>
        /// 线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between line and circle (including interior points)
        /// </summary>
        public double DistanceTo(Line3d l)
        {
            Point3d point_on_circle, point_on_line;
            double dist = _distance_circle_to_line(l, out point_on_circle, out point_on_line);
            return dist;
        }

        /// <summary>
        /// 线与圆之间的最短距离（不包括内部点）<br></br>
        /// Shortest distance between line and circle (excluding interior points)
        /// </summary>
        public double DistanceToBoundary(Line3d l)
        {
            Point3d point_on_circle, point_on_line;
            double dist = _distance_circle_boundary_to_line(l, out point_on_circle, out point_on_line);
            return dist;
        }

        /// <summary>
        /// 线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between line and circle (including interior points)
        /// </summary>
        /// <param name="l">目标线<br></br> Target line</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_line">线上最近的点<br></br> Closest point on line</param>
        public double DistanceTo(Line3d l, out Point3d point_on_circle, out Point3d point_on_line)
        {
            double dist = _distance_circle_to_line(l, out point_on_circle, out point_on_line);
            return dist;
        }

        /// <summary>
        /// 线与圆之间的最短距离（不包括内部点）<br></br>
        /// Shortest distance between line and circle (excluding interior points)
        /// </summary>
        /// <param name="l">目标线<br></br> Target line</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_line">线上最近的点<br></br> Closest point on line</param>
        public double DistanceToBoundary(Line3d l, out Point3d point_on_circle, out Point3d point_on_line)
        {
            double dist = _distance_circle_boundary_to_line(l, out point_on_circle, out point_on_line);
            return dist;
        }

        /// <summary>
        /// 射线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between ray and circle (including interior points)
        /// </summary>
        public double DistanceTo(Ray3d r)
        {
            Point3d point_on_circle, point_on_ray;
            return DistanceTo(r, out point_on_circle, out point_on_ray);
        }

        /// <summary>
        /// 射线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between ray and circle (including interior points)
        /// </summary>
        /// <param name="r">目标射线<br></br> Target ray</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_ray">射线上最近的点<br></br> Closest point on ray</param>
        public double DistanceTo(Ray3d r, out Point3d point_on_circle, out Point3d point_on_ray)
        {
            double dist = _distance_circle_to_line(r.ToLine, out point_on_circle, out point_on_ray);

            if (point_on_ray.BelongsTo(r)) return dist;

            point_on_ray = r.Point;
            point_on_circle = this.ClosestPoint(point_on_ray);
            return point_on_ray.DistanceTo(point_on_circle);
        }

        /// <summary>
        /// 线段与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between segment and circle (including interior points)
        /// </summary>
        public double DistanceTo(Segment3d s)
        {
            Point3d point_on_circle, point_on_ray;
            return DistanceTo(s, out point_on_circle, out point_on_ray);
        }

        /// <summary>
        /// 线段与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between segment and circle (including interior points)
        /// </summary>
        /// <param name="s">目标线段<br></br> Target segment</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_segment">线段上最近的点<br></br> Closest point on segment</param>
        public double DistanceTo(Segment3d s, out Point3d point_on_circle, out Point3d point_on_segment)
        {
            double dist = _distance_circle_to_line(s.ToLine, out point_on_circle, out point_on_segment);

            if (point_on_segment.BelongsTo(s)) return dist;

            Point3d point_on_circle1 = this.ClosestPoint(s.P1);
            Point3d point_on_circle2 = this.ClosestPoint(s.P2);
            double dist1 = point_on_circle1.DistanceTo(s.P1);
            double dist2 = point_on_circle2.DistanceTo(s.P2);

            if (dist1 < dist2)
            {
                point_on_circle = point_on_circle1;
                point_on_segment = s.P1;
                return dist1;
            }
            else
            {
                point_on_circle = point_on_circle2;
                point_on_segment = s.P2;
                return dist2;
            }
        }


        /// <summary>
        /// 线与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between line and circle (including interior points)
        /// </summary>
        /// <param name="l">Target line</param>
        /// <param name="p1">Closest point on circle</param>
        /// <param name="p2">Closest point on line</param>
        private double _distance_circle_to_line(Line3d l, out Point3d p1, out Point3d p2)
        {

            // Line is parallel
            if (l.IsParallelTo(this))
            {
                p2 = this.Center.ProjectionTo(l);
                p1 = this.ClosestPoint(p2);
                return p1.DistanceTo(p2);
            }

            // Intrsecting line
            object obj = l.IntersectionWith(this);
            if (obj != null)
            {
                p1 = (Point3d)obj;
                p2 = p1;
                return 0;
            }

            return _distance_circle_boundary_to_line(l, out p1, out p2);
        }

        /// <summary>
        /// 线与圆边界之间的最短距离（不包括内部点）<br></br>
        /// Shortest distance between line and circle's boundary (excluding interior points)<br></br>
        /// （对称情况仅返回一个点）<br></br>
        /// (only one point will be returned for symmetrical case)
        /// </summary>
        /// <param name="l">Target line</param>
        /// <param name="point_on_circle">Closest point on circle</param>
        /// <param name="point_on_line">Closest point on line</param>
        private double _distance_circle_boundary_to_line(Line3d l, out Point3d point_on_circle, out Point3d point_on_line)
        {

            // Line is parallel
            if (l.IsParallelTo(this))
            {
                Plane3d plane = this.ToPlane;
                Line3d line_proj = (Line3d)l.ProjectionTo(plane);

                object obj = line_proj.IntersectionWith(this);
                if (obj == null)
                {
                    // Non-intersecting objects
                    point_on_line = this.Center.ProjectionTo(l);
                    point_on_circle = this.ClosestPoint(point_on_line);
                    return point_on_line.DistanceTo(point_on_circle);
                }
                else if (obj.GetType() == typeof(Point3d))
                {
                    // Touching objects
                    point_on_circle = (Point3d)obj;
                    point_on_line = point_on_circle.ProjectionTo(l);
                    return point_on_line.DistanceTo(point_on_circle);
                }
                else
                {
                    // Intrsecting objects, only one point will be used
                    Segment3d segm = (Segment3d)obj;
                    point_on_circle = segm.P1;
                    point_on_line = point_on_circle.ProjectionTo(l);
                    return point_on_line.DistanceTo(point_on_circle);
                }
            }

            // Orthogonal line
            if (l.IsOrthogonalTo(this))
            {
                Plane3d plane = this.ToPlane;
                Point3d projection_point = (Point3d)l.IntersectionWith(plane);

                if (projection_point == this.Center)
                {
                    point_on_line = projection_point;
                    point_on_circle = this.ParametricForm(0);
                    return point_on_line.DistanceTo(point_on_circle);
                }
                else
                {
                    Vector3d v = new Vector3d(this.Center, projection_point).Normalized;
                    point_on_line = projection_point;
                    point_on_circle = this.Center.Translate(this.R * v);
                    return point_on_line.DistanceTo(point_on_circle);
                }
            }


            // General case

            double d1 = 1e20;
            double t1 = 0;
            Point3d p;

            for (int i = 0; i < 16; i++)
            {
                double t = i * Math.PI / 8;
                p = this.ParametricForm(t);
                double dist = p.DistanceTo(l);
                if (dist < d1)
                {
                    d1 = dist;
                    t1 = t;
                }
            }
            double t2 = t1 - Math.PI / 8;
            p = this.ParametricForm(t2);
            double d2 = p.DistanceTo(l);
            double t3 = t1 + Math.PI / 8;
            p = this.ParametricForm(t3);
            double d3 = p.DistanceTo(l);

            int iter = 0;
            while (d2 - d1 > 0.2 * GeometRi3D.DefaultTolerance && d1 > GeometRi3D.DefaultTolerance)
            {
                if (++iter > 100) break;

                double ax = 2.0 * d1 / (t1 - t2) / (t1 - t3);
                double aa = 0.5 * ax * (t2 + t3);
                double bx = 2.0 * d2 / (t2 - t1) / (t2 - t3);
                double bb = 0.5 * bx * (t1 + t3);
                double cx = 2.0 * d3 / (t3 - t1) / (t3 - t2);
                double cc = 0.5 * cx * (t1 + t2);

                double t = (aa + bb + cc) / (ax + bx + cx);
                p = this.ParametricForm(t);
                double d = p.DistanceTo(l);

                if (d < d1)
                {
                    if (t > t2 & t < t1)
                    {
                        t3 = t1; d3 = d1;
                    }
                    else
                    {
                        t2 = t1; d2 = d1;
                    }
                    t1 = t; d1 = d;
                }
                else
                {
                    if (t < t1)
                    {
                        t2 = t; d2 = d;
                    }
                    else
                    {
                        t3 = t; d3 = d;
                    }
                }


            }

            point_on_circle = this.ParametricForm(t1);
            point_on_line = point_on_circle.ProjectionTo(l);
            return point_on_line.DistanceTo(point_on_circle);

        }


        /// <summary>
        /// 三角形与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between triangle and circle (including interior points)
        /// </summary>
        public double DistanceTo(Triangle t)
        {
            Point3d point_on_circle, point_on_triangle;
            return DistanceTo(t, out point_on_circle, out point_on_triangle);
        }

        /// <summary>
        /// 三角形与圆之间的最短距离（包括内部点）<br></br>
        /// Shortest distance between triangle and circle (including interior points)
        /// </summary>
        /// <param name="t">目标三角形<br></br> Target triangle</param>
        /// <param name="point_on_circle">圆上最近的点<br></br> Closest point on circle</param>
        /// <param name="point_on_triangle">三角形上最近的点<br></br> Closest point on triangle</param>
        public double DistanceTo(Triangle t, out Point3d point_on_circle, out Point3d point_on_triangle)
        {
            double dist = this.DistanceTo(t.ToPlane, out point_on_circle, out point_on_triangle);
            if (t.DistanceTo(point_on_triangle) <= GeometRi3D.DefaultTolerance)
            {
                return dist;
            }

            Segment3d AB = new Segment3d(t.A, t.B);
            Segment3d BC = new Segment3d(t.B, t.C);
            Segment3d AC = new Segment3d(t.A, t.C);

            dist = this.DistanceTo(AB, out point_on_circle, out point_on_triangle);
            Point3d point_on_circle2, point_on_triangle2;
            double dist2 = this.DistanceTo(BC, out point_on_circle2, out point_on_triangle2);
            if (dist2 < dist)
            {
                dist = dist2;
                point_on_circle = point_on_circle2;
                point_on_triangle = point_on_triangle2;
            }
            dist2 = this.DistanceTo(AC, out point_on_circle2, out point_on_triangle2);
            if (dist2 < dist)
            {
                dist = dist2;
                point_on_circle = point_on_circle2;
                point_on_triangle = point_on_triangle2;
            }
            return dist;
        }

        /// <summary>
        /// 圆到方框的最短距离<br></br>
        /// Shortest distance from circle to box
        /// </summary>
        public double DistanceTo(Box3d box)
        {
            return box.DistanceTo(this);
        }
        #endregion

        /// <summary>
        /// 圆与球的相交检查<br></br>
        /// Intersection check between circle and sphere
        /// </summary>
        public bool Intersects(Sphere s)
        {
            if (this._point.DistanceTo(s.Center) <= this.R + s.R)
            {
                Object obj = s.IntersectionWith(this.ToPlane);
                if (obj != null && obj.GetType() == typeof(Circle3d))
                {
                    // Check for circle-circle intersection
                    if (this.IntersectionWith((Circle3d)obj) != null)
                        return true;
                }
                else if (obj != null && obj.GetType() == typeof(Point3d))
                {
                    return ((Point3d)obj).BelongsTo(this);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 两个圆之间的相交检查<br></br>
        /// Intersection check between two circles
        /// </summary>
        public bool Intersects(Circle3d c)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * Max(this.R, c.R);
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = this.Intersects(c);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            // Early exit (separated circles)
            double d = this._point.DistanceTo(c._point);
            if (d > this.R + c.R + GeometRi3D.Tolerance)
                return false;

            if (this._normal.IsParallelTo(c._normal))
            {
                if (this._point.BelongsTo(new Plane3d(c._point, c._normal)))
                {
                    // Coplanar objects
                    if (d <= this.R + c.R + GeometRi3D.Tolerance)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }  
                }
                else
                {
                    // parallel objects
                    return false;
                }
            }
            else
            {
                // Check 3D intersection
                Vector3d v = new Vector3d(this._point, c._point);
                double this_norm = this._normal.Norm;
                double c_norm = c._normal.Norm;

                double cos_angle1 = this._normal * v / this_norm / d;
                double delta1 = Abs(d * cos_angle1);

                double sin_angle2 = this._normal.Cross(c._normal).Norm / this_norm / c_norm;
                double delta2 = Abs(c.R * sin_angle2);

                if (delta1 > delta2) return false;

                cos_angle1 = c._normal * v / c_norm / d;
                delta1 = Abs(d * cos_angle1);
                delta2 = Abs(this.R * sin_angle2);

                if (delta1 > delta2) return false;



                Plane3d plane_this = new Plane3d(this._point, this._normal);

                Line3d l = (Line3d)plane_this.IntersectionWith(new Plane3d(c._point, c._normal));
                Coord3d local_coord = new Coord3d(this._point, l._dir, this._normal.Cross(l._dir));
                Point3d p = l._point.ConvertTo(local_coord);

                if (GeometRi3D.Greater(Abs(p.Y), this.R))
                {
                    // No intersection
                    return false;
                }
                else if (GeometRi3D.AlmostEqual(Abs(p.Y), this.R))
                {
                    // Intersection in one point
                    Point3d pp = new Point3d(0, p.Y, 0, local_coord);
                    if (pp.DistanceTo(c._point) <= c.R + GeometRi3D.Tolerance)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    double dd = Sqrt(this.R * this.R - p.Y * p.Y);
                    Point3d p1 = new Point3d(-dd, p.Y, 0, local_coord);
                    Point3d p2 = new Point3d(dd, p.Y, 0, local_coord);

                    // check if at least one point is outside circle "c"
                    if (p1.DistanceTo(c._point) <= c.R + GeometRi3D.Tolerance) return true;

                    // Now check if segment (p1,p2) intrsects circle "c"
                    // Use local coord with center in c.Point and X-axis aligned with segment
                    local_coord = new Coord3d(c._point, l._dir, c._normal.Cross(l._dir));
                    p1 = p1.ConvertTo(local_coord);
                    p2 = p2.ConvertTo(local_coord);

                    // use parametric form
                    // x=t*x1+(1-t)x2
                    // y=t*y1+(1-t)y2
                    // and take into account that y1=y2, x0=y0=0
                    double aa = (p1.X - p2.X) * (p1.X - p2.X);
                    double bb = 2 * p2.X * (p1.X - p2.X);
                    double cc = p2.X * p2.X + p2.Y * p2.Y - c.R * c.R;
                    double discr = bb * bb - 4 * aa * cc;

                    if (discr < 0)
                    {
                        return false;
                    }
                    else
                    {
                        discr = Sqrt(discr);
                        double t1 = (-bb + discr) / (2 * aa);
                        double t2 = (-bb - discr) / (2 * aa);
                        if ((t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

            }

        }



        /// <summary>
        /// 圆与三角形的相交检查<br></br>
        /// Intersection check between circle and triangle
        /// </summary>
        public bool Intersects(Triangle t)
        {
            Plane3d t_plane = t.ToPlane;
            if (this.DistanceTo(t_plane) > 0) return false;

            if (this.IsCoplanarTo(t))
            {
                if (t._a.DistanceTo(this._point) <= this._r) return true;
                if (t._b.DistanceTo(this._point) <= this._r) return true;
                if (t._c.DistanceTo(this._point) <= this._r) return true;

                if (this._point.BelongsTo(t)) return true;
                if (this.IntersectionWith(new Segment3d(t._a, t._b)) != null) return true;
                if (this.IntersectionWith(new Segment3d(t._b, t._c)) != null) return true;
                if (this.IntersectionWith(new Segment3d(t._c, t._a)) != null) return true;
            }

            object obj = this.IntersectionWith(t_plane);
            if (obj != null && obj.GetType() == typeof(Point3d))
            {
                return ((Point3d)obj).BelongsTo(t);
            }
            else if (obj != null && obj.GetType() == typeof(Segment3d))
            {
                return ((Segment3d)obj).IntersectionWith(t) != null;
            }

            return false;
        }

        /// <summary>
        /// 圆与盒子之间的相交检查<br></br>
        /// Intersection check between circle and box
        /// </summary>
        public bool Intersects(Box3d box)
        {
            return box.Intersects(this);
        }

        /// <summary>
        /// 圆与线段的相交检查<br></br>
        /// Intersection check between circle and segment
        /// </summary>
        public bool Intersects(Segment3d s)
        {
            return this.IntersectionWith(s) != null;
        }

        /// <summary>
        /// 圆到平面的正交投影<br></br>
        /// Orthogonal projection of the circle to plane
        /// </summary>
        public Ellipse ProjectionTo(Plane3d s)
        {
            return this.ToEllipse.ProjectionTo(s);
        }

        /// <summary>
        /// 圆到线的正交投影<br></br>
        /// Orthogonal projection of the circle to line
        /// </summary>
        public Segment3d ProjectionTo(Line3d l)
        {
            double s = _r * Cos(l.AngleTo(this));
            Vector3d v = l.Direction.Normalized;
            Point3d p = _point.ProjectionTo(l);
            return new Segment3d(p.Translate(-s * v), p.Translate(s * v));
        }

        /// <summary>
        /// 圆与线的交点。<br></br>
        /// Intersection of circle with line.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.R;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(l);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================


            if (l._dir.IsOrthogonalTo(this._normal))
            {
                if (l._point.BelongsTo(new Plane3d(this._point, this._normal)))
                {
                    // coplanar objects
                    // Find intersection of line and circle (2D)

                    // Local coord: X - line direction, Z - circle normal
                    Coord3d local_coord = new Coord3d(this._point, l._dir, this._normal.Cross(l._dir));
                    Point3d p = l._point.ConvertTo(local_coord);

                    double c = p.Y;

                    if (Abs(c) > this.R + GeometRi3D.Tolerance)
                    {
                        return null;
                    }
                    else if (Abs(c) < this.R)
                    {
                        double x1 = Sqrt(this.R * this.R - Abs(c) * Abs(c));
                        double x2 = -x1;
                        return new Segment3d(new Point3d(x1, c, 0, local_coord), new Point3d(x2, c, 0, local_coord));
                    }
                    else if (c > 0)
                    {
                        return new Point3d(0, this.R, 0, local_coord);
                    }
                    else
                    {
                        return new Point3d(0, -this.R, 0, local_coord);
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
                // Line intersects circle' plane
                Point3d p = (Point3d)l.IntersectionWith(new Plane3d(this._point, this._normal));
                if (p.DistanceTo(this._point) < this.R + GeometRi3D.Tolerance)
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
        /// 圆与线段的交点。<br></br>
        /// Intersection of circle with segment.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Segment3d s)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.R;
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
        /// 圆与射线的交点。<br></br>
        /// Intersection of circle with ray.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ray3d r)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.R;
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

        /// <summary>
        /// 圆与平面的交点。<br></br>
        /// Intersection of circle with plane.<br></br>
        /// 返回“null”（无交点）或“Circle3d”、“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Circle3d', 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Plane3d s)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.R;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(s);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            if (this._normal.IsParallelTo(s.Normal))
            {
                if (this._point.BelongsTo(s))
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
                Line3d l = (Line3d)s.IntersectionWith(new Plane3d(this._point, this._normal));
                Coord3d local_coord = new Coord3d(this._point, l._dir, this._normal.Cross(l._dir));
                Point3d p = l._point.ConvertTo(local_coord);

                if (GeometRi3D.Greater(Abs(p.Y), this.R))
                {
                    return null;
                }
                else if (GeometRi3D.AlmostEqual(p.Y, this.R))
                {
                    return new Point3d(0, this.R, 0, local_coord);
                }
                else if (GeometRi3D.AlmostEqual(p.Y, -this.R))
                {
                    return new Point3d(0, -this.R, 0, local_coord);
                }
                else
                {
                    double d = Sqrt(this.R * this.R - p.Y * p.Y);
                    Point3d p1 = new Point3d(-d, p.Y, 0, local_coord);
                    Point3d p2 = new Point3d(d, p.Y, 0, local_coord);
                    return new Segment3d(p1, p2);
                }
            }

        }

        /// <summary>
        /// 两个圆的交点。<br></br>
        /// Intersection of two circles.
        /// 返回“null”（无交点）或“Circle3d”、“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Circle3d', 'Point3d' or 'Segment3d'.<br></br>
        /// 在 2D（共面圆）中，线段将定义两个交点。<br></br>
        /// In 2D (coplanar circles) the segment will define two intersection points.
        /// </summary>
        public object IntersectionWith(Circle3d c)
        {

            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * Max(this.R, c.R);
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(c);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            // Early exit (separated circles)
            double d = this._point.DistanceTo(c._point);
            if (d > this.R  + c.R  +  GeometRi3D.Tolerance)
                return null;

            if (this._normal.IsParallelTo(c._normal))
            {
                if (this._point.BelongsTo(new Plane3d(c._point, c._normal)))
                {
                    // Coplanar objects
                    // Search 2D intersection of two circles

                    // Equal circles
                    if (GeometRi3D.AlmostEqual(d, 0) && GeometRi3D.AlmostEqual(this.R, c.R))
                    {
                        return this.Copy();
                    }

                    

                    // One circle inside the other
                    if (d < Abs(this.R - c.R) - GeometRi3D.Tolerance)
                    {
                        if (this.R > c.R)
                        {
                            return c.Copy();
                        }
                        else
                        {
                            return this.Copy();
                        }
                    }

                    // Outer tangency
                    if (GeometRi3D.AlmostEqual(d, this.R + c.R))
                    {
                        Vector3d vec = new Vector3d(this._point, c._point);
                        return this._point.Translate(this.R * vec.Normalized);
                    }

                    // Inner tangency
                    if (Abs(Abs(this.R - c.R) - d) < GeometRi3D.Tolerance)
                    {
                        Vector3d vec = new Vector3d(this._point, c._point);
                        if (this.R > c.R)
                        {
                            return this._point.Translate(this.R * vec.Normalized);
                        }
                        else
                        {
                            return this._point.Translate(-this.R * vec.Normalized);
                        }
                        
                    }

                    // intersecting circles
                    // Create local CS with origin in circle's center
                    Vector3d vec1 = new Vector3d(this._point, c._point);
                    Vector3d vec2 = vec1.Cross(this._normal);
                    Coord3d local_cs = new Coord3d(this._point, vec1, vec2);

                    double x = 0.5 * (d * d - c.R * c.R + this.R * this.R) / d;
                    double y = 0.5 * Sqrt((-d + c.R - this.R) * (-d - c.R + this.R) * (-d + c.R + this.R) * (d + c.R + this.R)) / d;
                    Point3d p1 = new Point3d(x, y, 0, local_cs);
                    Point3d p2 = new Point3d(x, -y, 0, local_cs);
                    return new Segment3d(p1, p2);
                }
                else
                {
                    // parallel objects
                    return null;
                }
            }
            else
            {
                // Check 3D intersection

                Vector3d v = new Vector3d(this._point, c._point);
                double this_norm = this._normal.Norm;
                double c_norm = c._normal.Norm;

                double cos_angle1 = this._normal * v / this_norm / d;
                double delta1 = Abs(d * cos_angle1);

                double sin_angle2 = this._normal.Cross(c._normal).Norm / this_norm / c_norm;
                double delta2 = Abs(c.R * sin_angle2);

                if (delta1 > delta2) return null;

                cos_angle1 = c._normal * v / c_norm / d;
                delta1 = Abs(d * cos_angle1);
                delta2 = Abs(this.R * sin_angle2);

                if (delta1 > delta2) return null;


                Plane3d plane_this = new Plane3d(this._point, this._normal);
                object obj = c.IntersectionWith(plane_this);

                if (obj == null)
                {
                    return null;
                }
                else if (obj.GetType() == typeof(Point3d))
                {
                    Point3d p = (Point3d)obj;
                    if (p.DistanceTo(this._point) < this.R + GeometRi3D.Tolerance)
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
                    Segment3d s = (Segment3d)obj;
                    return s.IntersectionWith(this);
                }

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
                    if ( GeometRi3D.Smaller(p.DistanceTo(this.Center), this.R))
                    {
                        return 1; // Point is strictly inside
                    }
                    else if (GeometRi3D.AlmostEqual(p.DistanceTo(this.Center), this.R) )
                    {
                        return 0; // Point is on boundary
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
                GeometRi3D.Tolerance = tol * this.R;
                GeometRi3D.UseAbsoluteTolerance = true;
                int result = this._PointLocation(p);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        #region "AngleTo"
        /// <summary>
        /// 两个物体之间的角度（以弧度表示）（0 &lt; angle &lt; Pi）<br></br>
        /// Angle between two objects in radians (0 &lt; angle &lt; Pi)
        /// </summary>
        public double AngleTo(ILinearObject obj)
        {
            return GeometRi3D.GetAngle(this, obj);
        }
        /// <summary>
        /// 两个物体之间的角度（以度为单位）（0 &lt; 角度 &lt; 180）<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(ILinearObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }

        /// <summary>
        /// 两个物体之间的角度（以弧度表示）（0 &lt; 角度 &lt; Pi）<br></br>
        /// Angle between two objects in radians (0 &lt; angle &lt; Pi)
        /// </summary>
        public double AngleTo(IPlanarObject obj)
        {
            return GeometRi3D.GetAngle(this, obj);
        }
        /// <summary>
        /// 两个物体之间的角度（以度为单位）（0 &lt; 角度 &lt; 180）<br></br>
        /// Angle between two objects in degrees (0 &lt; angle &lt; 180)
        /// </summary>
        public double AngleToDeg(IPlanarObject obj)
        {
            return AngleTo(obj) * 180 / PI;
        }
        #endregion

        #region "平移 旋转 反射 TranslateRotateReflect"
        /// <summary>
        /// 通过矢量平移圆<br></br>
        /// Translate circle by a vector
        /// </summary>
        public Circle3d Translate(Vector3d v)
        {
            return new Circle3d(this.Center.Translate(v), this.R, this.Normal);
        }

        /// <summary>
        /// 按给定的旋转矩阵旋转一圈<br></br>
        /// Rotate circle by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object and specify rotation center: this.Rotate(Rotation r, Point3d p)")]
        public Circle3d Rotate(Matrix3d m)
        {
            return new Circle3d(this.Center.Rotate(m), this.R, this.Normal.Rotate(m));
        }

        /// <summary>
        /// 以点 'p' 为旋转中心，按照给定的旋转矩阵旋转一圈<br></br>
        /// Rotate circle by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r, Point3d p)")]
        public Circle3d Rotate(Matrix3d m, Point3d p)
        {
            return new Circle3d(this.Center.Rotate(m, p), this.R, this.Normal.Rotate(m));
        }

        /// <summary>
        /// 以点 'p' 为旋转中心旋转圆<br></br>
        /// Rotate circle around point 'p' as a rotation center
        /// </summary>
        public Circle3d Rotate(Rotation r, Point3d p)
        {
            return new Circle3d(this.Center.Rotate(r, p), this.R, this.Normal.Rotate(r));
        }

        /// <summary>
        /// 在给定点处反射圆<br></br>
        /// Reflect circle in given point
        /// </summary>
        public Circle3d ReflectIn(Point3d p)
        {
            return new Circle3d(this.Center.ReflectIn(p), this.R, this.Normal.ReflectIn(p));
        }

        /// <summary>
        /// 在给定的线中反射圆<br></br>
        /// Reflect circle in given line
        /// </summary>
        public Circle3d ReflectIn(Line3d l)
        {
            return new Circle3d(this.Center.ReflectIn(l), this.R, this.Normal.ReflectIn(l));
        }

        /// <summary>
        /// 在给定平面内反射圆<br></br>
        /// Reflect circle in given plane
        /// </summary>
        public Circle3d ReflectIn(Plane3d s)
        {
            return new Circle3d(this.Center.ReflectIn(s), this.R, this.Normal.ReflectIn(s));
        }

        /// <summary>
        /// 相对于给定点的缩放圆<br></br>
        /// Scale circle relative to given point
        /// </summary>
        public virtual Circle3d Scale(double scale, Point3d scaling_center)
        {
            Point3d new_center = scaling_center + scale * (this.Center - scaling_center);
            return new Circle3d(new_center, this._r * scale, this._normal);
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
            Circle3d c = (Circle3d)obj;

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return c.Center == this.Center && Abs(c.R - this.R) <= GeometRi3D.Tolerance && c.Normal.IsParallelTo(this.Normal);
            }
            else
            {
                return Abs(c.Center.DistanceTo(this.Center)) / this.R <= GeometRi3D.Tolerance && 
                       Abs(c.R - this.R) / this.R  <= GeometRi3D.Tolerance && 
                       c.Normal.IsParallelTo(this.Normal);
            }
        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_point.GetHashCode(), _r.GetHashCode(), _normal.GetHashCode());
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
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d P = _point.ConvertTo(coord);
            Vector3d normal = _normal.ConvertTo(coord);

            string str = string.Format("Circle: ") + nl;
            str += string.Format("  Center -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", P.X, P.Y, P.Z) + nl;
            str += string.Format("  Radius -> {0,10:g5}", _r) + nl;
            str += string.Format("  Normal -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", normal.X, normal.Y, normal.Z);
            return str;
        }

        // 运算符重载
        // Operators overloads
        //-----------------------------------------------------------------

        public static bool operator ==(Circle3d c1, Circle3d c2)
        {
            if (object.ReferenceEquals(c1, null))
                return object.ReferenceEquals(c2, null);
            return c1.Equals(c2);
        }
        public static bool operator !=(Circle3d c1, Circle3d c2)
        {
            if (object.ReferenceEquals(c1, null))
                return !object.ReferenceEquals(c2, null);
            return !c1.Equals(c2);
        }

    }
}


