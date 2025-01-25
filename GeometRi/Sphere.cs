﻿using System;
using static System.Math;

namespace GeometRi
{
    /// <summary>
    /// 由中心点和半径定义的球体物体。<br></br>
    /// Sphere object defined by center point and radius.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Sphere : FiniteObject, IFiniteObject
    {

        internal Point3d _point;
        private double _r;

        /// <summary>
        /// 使用中心点和半径初始化球体。<br></br>
        /// Initializes sphere using center point and radius.
        /// </summary>
        public Sphere(Point3d P, double R)
        {
            _point = P.Copy();
            _r = R;
        }

        /// <summary>
        /// 创建对象的副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Sphere Copy()
        {
            return new Sphere(_point,_r);
        }

        #region "Properties"
        /// <summary>
        /// 球体中心<br></br>
        /// Center of the sphere
        /// </summary>
        public Point3d Center
        {
            get { return _point.Copy(); }
            set { _point = value.Copy(); }
        }

        /// <summary>
        /// 球体中心的 X 分量<br></br>
        /// X component of the spheres' center
        /// </summary>
        private double X
        {
            get { return _point.X; }
            set { _point.X = value; }
        }

        /// <summary>
        /// 球体中心的 Y 分量<br></br>
        /// Y component of the spheres' center
        /// </summary>
        private double Y
        {
            get { return _point.Y; }
            set { _point.Y = value; }
        }

        /// <summary>
        /// 球体中心的 Z 分量<br></br>
        /// Z component of the spheres' center
        /// </summary>
        private double Z
        {
            get { return _point.Z; }
            set { _point.Z = value; }
        }

        /// <summary>
        /// 球体的半径<br></br>
        /// Radius of the sphere
        /// </summary>
        public double R
        {
            get { return _r; }
            set { _r = value; }
        }

        /// <summary>
        /// 表面积
        /// </summary>
        public double Area
        {
            get { return 4.0 * PI * Math.Pow(_r, 2); }
        }

        /// <summary>
        /// 体积
        /// </summary>
        public double Volume
        {
            get { return 4.0 / 3.0 * PI * Math.Pow(_r, 3); }
        }
        #endregion

        #region "DistanceTo"
        /// <summary>
        /// 点与球体（包括内部点）之间的最短距离。<br></br>
        /// Shortest distance between point and sphere (including interior points).
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            double d = p.DistanceTo(this._point);
            if (d > this.R)
            {
                return d - this.R;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 球体和线之间的最短距离<br></br>
        /// Shortest distance between sphere and line
        /// </summary>
        public double DistanceTo(Line3d l)
        {
            double d = l.DistanceTo(this._point);
            if (d > this.R)
            {
                return d - this.R;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 球体与射线之间的最短距离<br></br>
        /// Shortest distance between sphere and ray
        /// </summary>
        public double DistanceTo(Ray3d r)
        {
            if (this._point.ProjectionTo(r.ToLine).BelongsTo(r))
            {
                return this.DistanceTo(r.ToLine);
            }
            else
            {
                return this.DistanceTo(r.Point);
            }
        }

        /// <summary>
        /// 球体与线段之间的最短距离<br></br>
        /// Shortest distance between sphere and segment
        /// </summary>
        public double DistanceTo(Segment3d s)
        {
            if (this._point.ProjectionTo(s.ToLine).BelongsTo(s))
            {
                return this.DistanceTo(s.ToLine);
            }
            else
            {
                return Min(this.DistanceTo(s.P1), this.DistanceTo(s.P2));
            }
        }

        /// <summary>
        /// 球体与平面之间的最短距离<br></br>
        /// Shortest distance between sphere and plane
        /// </summary>
        public double DistanceTo(Plane3d s)
        {
            double d = this._point.DistanceTo(s);
            if (d > this.R)
            {
                return d - this.R;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 球与圆之间的最短距离（包括内部点）（近似解）<br></br>
        /// Shortest distance between sphere and circle (including interior points) (approximate solution)
        /// </summary>
        public double DistanceTo(Circle3d c)
        {
            return c.DistanceTo(this);
        }

        /// <summary>
        /// 球体与圆之间的最短距离（包括内部点）（近似解）。<br></br>
        /// Shortest distance between sphere and circle (including interior points) (approximate solution).
        /// <para> 如果物体相交，输出点可能不唯一。</para>
        /// <para> The output points may be not unique in case of intersecting objects.</para>
        /// </summary>
        /// <param name="c">目标圆<br></br> Target circle</param>
        /// <param name="p1">球上最近点<br></br> Closest point on sphere</param>
        /// <param name="p2">圆上最近点<br></br> Closest point on circle</param>
        public double DistanceTo(Circle3d c, out Point3d p1, out Point3d p2)
        {
            return c.DistanceTo(this, out p2, out p1);
        }

        /// <summary>
        /// 两个球体之间的最短距离。<br></br>
        /// Shortest distance between two spheres.
        /// <para> 如果一个球体位于另一个球体内部，则返回零距离。</para>
        /// <para> Zero distance is returned if one sphere located inside other.</para>
        /// </summary>
        public double DistanceTo(Sphere s)
        {
            double dist = this._point.DistanceTo(s._point);
            if (dist <= this.R + s.R)
            {
                return 0;
            }
            else
            {
                return dist - this.R - s.R;
            }
        }

        /// <summary>
        /// 两个球体之间的最短距离。
        /// Shortest distance between two spheres.
        /// <para> 如果一个球体位于另一个球体内部，则返回零距离。</para> 
        /// <para> 如果物体接触，输出点可能不唯一。</para>
        /// <para> Zero distance is returned if one sphere is located inside the other.</para>
        /// <para> The output points may be not unique in case of touching objects.</para>
        /// </summary>
        /// <param name="s">目标球Target sphere</param>
        /// <param name="p1">源球上最近点Closest point on source sphere</param>
        /// <param name="p2">目标球上最近点Closest point on target sphere</param>
        public double DistanceTo(Sphere s, out Point3d p1, out Point3d p2)
        {
            double dist = this._point.DistanceTo(s._point);
            if (dist <= this.R + s.R)
            {
                if (this._point == s._point)
                {
                    p1 = this._point.Translate(this.R * new Vector3d(1, 0, 0));
                    p2 = s._point.Translate(s.R * new Vector3d(1, 0, 0));
                }
                else
                {
                    p1 = this._point.Translate(this.R * new Vector3d(this._point, s._point).Normalized);
                    p2 = s._point.Translate(s.R * new Vector3d(s._point, this._point).Normalized);
                }
                return 0;
            }
            else
            {
                p1 = this._point.Translate(this.R * new Vector3d(this._point, s._point).Normalized);
                p2 = s._point.Translate(s.R * new Vector3d(s._point, this._point).Normalized);
                return dist - this.R - s.R;
            }
        }

        /// <summary>
        /// 从盒子到球体的最短距离<br></br>
        /// Shortest distance from box to sphere
        /// </summary>
        public double DistanceTo(Box3d box)
        {
            return box.DistanceTo(this);
        }

        #endregion

        #region "BoundingBox"
        /// <summary>
        /// 返回最小边界框。<br></br>
        /// Return minimum bounding box.
        /// </summary>
        public Box3d MinimumBoundingBox
        {
            get { return new Box3d(_point, 2.0 * _r, 2.0 * _r, 2.0 * _r); }
        }

        /// <summary>
        /// 返回给定坐标系中的边界框。<br></br>
        /// Return Bounding Box in given coordinate system.
        /// </summary>
        public Box3d BoundingBox(Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            return new Box3d(_point, 2.0 * _r, 2.0 * _r, 2.0 * _r, new Rotation(coord));
        }

        /// <summary>
        /// 返回轴对齐边界框（AABB）。<br></br>
        /// Return Axis Aligned Bounding Box (AABB).
        /// </summary>
        public AABB AABB()
        {
            return new AABB(_point, 2.0 * _r, 2.0 * _r, 2.0 * _r);
        }

        /// <summary>
        /// 返回边界球。<br></br>
        /// Return bounding sphere.
        /// </summary>
        public Sphere BoundingSphere
        {
            get { return this; }

        }

        /// <summary>
        /// 检查球体是否位于由全局公差属性（Geomet Ri3D.Tolerance）定义的公差的框内。<br></br>
        /// Check if sphere is located inside box with tolerance defined by global tolerance property (GeometRi3D.Tolerance).
        /// </summary>
        public bool IsInside(Box3d box)
        {
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
            Point3d p = _point.ConvertTo(local_coord);

            if (box.L1 / 2 - (Abs(p.X) + this._r) < GeometRi3D.Tolerance) return false;
            if (box.L2 / 2 - (Abs(p.Y) + this._r) < GeometRi3D.Tolerance) return false;
            if (box.L3 / 2 - (Abs(p.Z) + this._r) < GeometRi3D.Tolerance) return false;

            return true;
        }

        #endregion

        /// <summary>
        /// 球体表面上最接近目标点“p”的点。<br></br>
        /// Point on sphere's surface closest to target point "p".
        /// </summary>
        public Point3d ClosestPoint(Point3d p)
        {
            if (p == this.Center)
            {
                // return any point on surface
                return this.Center.Translate(this.R * new Vector3d(1, 0, 0));
            }
            else
            {
                return this.Center.Translate(this.R * new Vector3d(this.Center, p).Normalized);
            }
        }

        #region "Intersections"
        /// <summary>
        /// 获取线与球的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of line with sphere.<br></br>
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

            double d = l.Direction.Normalized * (l.Point.ToVector - this.Center.ToVector);
            double det = Math.Pow(d, 2) - Math.Pow(((l.Point.ToVector - this.Center.ToVector).Norm), 2) + Math.Pow(_r, 2);

            if (det < -GeometRi3D.Tolerance)
            {
                return null;
            }
            else if (det < GeometRi3D.Tolerance)
            {
                return l.Point - d * l.Direction.Normalized.ToPoint;
            }
            else
            {
                Point3d p1 = l.Point + (-d + Sqrt(det)) * l.Direction.Normalized.ToPoint;
                Point3d p2 = l.Point + (-d - Sqrt(det)) * l.Direction.Normalized.ToPoint;
                return new Segment3d(p1, p2);
            }

        }

        /// <summary>
        /// 获取线段与球体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of segment with sphere.<br></br>
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
        /// 获取射线与球体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Get intersection of ray with sphere.<br></br>
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
        /// 获取平面与球体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Circle3d”类型的对象。<br></br>
        /// Get intersection of plane with sphere.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Circle3d'.
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

            s.SetCoord(this.Center.Coord);
            double d1 = s.A * this.X + s.B * this.Y + s.C * this.Z + s.D;
            double d2 = Math.Pow(s.A, 2) + Math.Pow(s.B, 2) + Math.Pow(s.C, 2);
            double d = Abs(d1) / Sqrt(d2);

            if (d > this.R + GeometRi3D.Tolerance)
            {
                return null;
            }
            else
            {
                double Xc = this.X - s.A * d1 / d2;
                double Yc = this.Y - s.B * d1 / d2;
                double Zc = this.Z - s.C * d1 / d2;

                if (Abs(d - this.R) < GeometRi3D.Tolerance)
                {
                    return new Point3d(Xc, Yc, Zc, this.Center.Coord);
                }
                else
                {
                    double R = Sqrt(Math.Pow(this.R, 2) - Math.Pow(d, 2));
                    return new Circle3d(new Point3d(Xc, Yc, Zc, this.Center.Coord), R, s.Normal);
                }
            }
        }

        /// <summary>
        /// 获取两个球体的交点。<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Circle3d”类型的对象。<br></br>
        /// Get intersection of two spheres.<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Circle3d'.
        /// </summary>
        public object IntersectionWith(Sphere s)
        {
            
            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * Max(this.R, s.R);
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = this.IntersectionWith(s);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            Point3d p = s.Center.ConvertTo(this.Center.Coord);
            double Dist = Sqrt(Math.Pow((this.X - p.X), 2) + Math.Pow((this.Y - p.Y), 2) + Math.Pow((this.Z - p.Z), 2));

            // Separated spheres
            if (Dist > this.R + s.R + GeometRi3D.Tolerance)
                return null;

            // One sphere inside the other
            if (Dist < Abs(this.R - s.R) - GeometRi3D.Tolerance)
                return null;

            // Intersection plane
            double A = 2 * (p.X - this.X);
            double B = 2 * (p.Y - this.Y);
            double C = 2 * (p.Z - this.Z);
            double D = Math.Pow(this.X, 2) - Math.Pow(p.X, 2) + Math.Pow(this.Y, 2) - Math.Pow(p.Y, 2) + Math.Pow(this.Z, 2) - Math.Pow(p.Z, 2) - Math.Pow(this.R, 2) + Math.Pow(s.R, 2);

            // Intersection center
            double t = (this.X * A + this.Y * B + this.Z * C + D) / (A * (this.X - p.X) + B * (this.Y - p.Y) + C * (this.Z - p.Z));
            double x = this.X + t * (p.X - this.X);
            double y = this.Y + t * (p.Y - this.Y);
            double z = this.Z + t * (p.Z - this.Z);

            // Outer tangency
            if (Abs(this.R + s.R - D) < GeometRi3D.Tolerance)
                return new Point3d(x, y, z, this.Center.Coord);

            // Inner tangency
            if (Abs(Abs(this.R - s.R) - D) < GeometRi3D.Tolerance)
                return new Point3d(x, y, z, this.Center.Coord);

            // Intersection
            double alpha = Acos((Math.Pow(this.R, 2) + Math.Pow(Dist, 2) - Math.Pow(s.R, 2)) / (2 * this.R * Dist));
            double R = this.R * Sin(alpha);
            Vector3d v = new Vector3d(this.Center, s.Center);

            return new Circle3d(new Point3d(x, y, z, this.Center.Coord), R, v);

        }
        #endregion

        /// <summary>
        /// 圆与球的相交检查<br></br>
        /// Intersection check between circle and sphere
        /// </summary>
        public bool Intersects(Circle3d c)
        {
            return c.Intersects(this);
        }

        /// <summary>
        /// 球体与三角形的相交检查<br></br>
        /// Intersection check between sphere and triangle
        /// </summary>
        public bool Intersects(Triangle t)
        {
            return t.Intersects(this);
        }

        /// <summary>
        /// 球面到平面的正交投影<br></br>
        /// Orthogonal projection of the sphere to the plane
        /// </summary>
        public Circle3d ProjectionTo(Plane3d s)
        {
            Point3d p = this.Center.ProjectionTo(s);
            return new Circle3d(p, this.R, s.Normal);
        }

        /// <summary>
        /// 球面到直线的正交投影<br></br>
        /// Orthogonal projection of the sphere to the line
        /// </summary>
        public Segment3d ProjectionTo(Line3d l)
        {
            Point3d p = this.Center.ProjectionTo(l);
            return new Segment3d(p.Translate(this.R * l.Direction.Normalized), p.Translate(-this.R * l.Direction.Normalized));
        }

        internal override int _PointLocation(Point3d p)
        {

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                if (p.DistanceTo(this.Center) - this.R <= GeometRi3D.Tolerance )
                {
                    if (p.DistanceTo(this.Center) - this.R < -GeometRi3D.Tolerance )
                    {
                        return 1; // Point is strictly inside box
                    }
                    else
                    {
                        return 0; // Point is on boundary
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

        #region "TranslateRotateReflect"
        /// <summary>
        /// 通过矢量平移球体<br></br>
        /// Translate sphere by a vector
        /// </summary>
        public Sphere Translate(Vector3d v)
        {
            return new Sphere(this.Center.Translate(v), this.R);
        }

        /// <summary>
        /// 根据给定的旋转矩阵旋转球体<br></br>
        /// Rotate sphere by a given rotation matrix
        /// </summary>
        [System.Obsolete("use Rotation object and specify rotation center: this.Rotate(Rotation r, Point3d p)")]
        public Sphere Rotate(Matrix3d m)
        {
            return new Sphere(this.Center.Rotate(m), this.R);
        }

        /// <summary>
        /// 根据给定的旋转矩阵围绕点“p”作为旋转中心旋转球体<br></br>
        /// Rotate sphere by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        [System.Obsolete("use Rotation object: this.Rotate(Rotation r, Point3d p)")]
        public Sphere Rotate(Matrix3d m, Point3d p)
        {
            return new Sphere(this.Center.Rotate(m, p), this.R);
        }

        /// <summary>
        /// 以点“p”为旋转中心旋转球体<br></br>
        /// Rotate sphere around point 'p' as a rotation center
        /// </summary>
        public Sphere Rotate(Rotation r, Point3d p)
        {
            return new Sphere(this.Center.Rotate(r, p), this.R);
        }

        /// <summary>
        /// 在给定点反射球体<br></br>
        /// Reflect sphere in given point
        /// </summary>
        public Sphere ReflectIn(Point3d p)
        {
            return new Sphere(this.Center.ReflectIn(p), this.R);
        }

        /// <summary>
        /// 沿给定线反射球体<br></br>
        /// Reflect sphere in given line
        /// </summary>
        public Sphere ReflectIn(Line3d l)
        {
            return new Sphere(this.Center.ReflectIn(l), this.R);
        }

        /// <summary>
        /// 在给定平面内反射球体<br></br>
        /// Reflect sphere in given plane
        /// </summary>
        public Sphere ReflectIn(Plane3d s)
        {
            return new Sphere(this.Center.ReflectIn(s), this.R);
        }

        /// <summary>
        /// 相对于给定点的缩放球体<br></br>
        /// Scale sphere relative to given point
        /// </summary>
        public virtual Sphere Scale(double scale, Point3d scaling_center)
        {
            Point3d new_center = scaling_center + scale * (this.Center - scaling_center);
            return new Sphere(new_center, _r * scale);
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
            Sphere s = (Sphere)obj;
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return s.Center == this.Center && Abs(s.R - this.R) <= GeometRi3D.Tolerance;
            }
            else
            {
                return this.Center.DistanceTo(s.Center) <= GeometRi3D.Tolerance * this.R && 
                       Abs(s.R - this.R) <= GeometRi3D.Tolerance * this.R;
            }

        }

        /// <summary>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_point.GetHashCode(), _r.GetHashCode());
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
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d p = _point.ConvertTo(coord);

            string str = string.Format("Sphere: ") + nl;
            str += string.Format("  Center -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p.X, p.Y, p.Z) + nl;
            str += string.Format("  Radius -> {0,10:g5}", _r);
            return str;
        }

        // Operators overloads
        //-----------------------------------------------------------------

        public static bool operator ==(Sphere s1, Sphere s2)
        {
            if (object.ReferenceEquals(s1, null))
                return object.ReferenceEquals(s2, null);
            return s1.Equals(s2);
        }
        public static bool operator !=(Sphere s1, Sphere s2)
        {
            if (object.ReferenceEquals(s1, null))
                return !object.ReferenceEquals(s2, null);
            return !s1.Equals(s2);
        }

    }
}

