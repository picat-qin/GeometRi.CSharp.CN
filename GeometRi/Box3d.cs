﻿using System;
using static System.Math;
using System.Collections.Generic;

namespace GeometRi
{
    /// <summary>
    /// 任意方向的 3D 框，可以退化为一个或多个等于 0 的维度。<br></br>
    /// Arbitrary oriented 3D box, can be degenerated with one or more dimensions equal 0.
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Box3d : FiniteObject, IFiniteObject
    {

        private Point3d _center;
        private double _lx, _ly, _lz;
        private Rotation _r;
        private Coord3d _local_coord = null;

        private List<Point3d> _list_p = null;
        private List<Triangle> _list_t = null;
        private List<Segment3d> _list_e = null;
        private List<Plane3d> _list_plane = null;

        #region "Constructors"

        /// <summary>
        /// 默认构造函数，在与坐标轴对齐的全局坐标系的原点中初始化盒子。<br></br>
        /// Default constructor, initializes box in the origin of the global coordinate system aligned with coordinate axes.
        /// </summary>
        public Box3d(Point3d center, double lx, double ly, double lz)
        {
            _center = center.Copy();
            _lx = lx;
            _ly = ly;
            _lz = lz;
            _r = new Rotation();
            _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
        }

        /// <summary>
        /// 在与坐标轴对齐的参考坐标系的原点初始化单位框。<br></br>
        /// Initializes unit box in the origin of the reference coordinate system aligned with coordinate axes.
        /// </summary>
        /// <param name="coord">Reference coordinate system.</param>
        public Box3d(Coord3d coord = null)
        {
            _center = new Point3d(coord);
            _lx = _ly = _lz = 1.0;
            _r = new Rotation(coord);
            if (coord != null)
            {
                // 不要设置与全局 CS 对齐的框的本地坐标
                // do not set local_coord for box aligned with global CS
                _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
            }
            coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用旋转对象定义的指定尺寸和方向初始化盒子。<br></br>
        /// Initializes box with specified dimensions and orientation defined by rotation object.
        /// </summary>
        /// <param name="center">盒子的中心点。<br></br>Center point of the box.</param>
        /// <param name="lx">第一维度。<br></br>Center point of the box.</param>
        /// <param name="ly">第二维度。<br></br>Second dimension.</param>
        /// <param name="lz">第三维度。<br></br>Third dimension.</param>
        /// <param name="r">盒子的方向，定义为从轴对齐位置（在全局 CS 中）到最终位置的旋转。<br></br>Orientation of the box, defined as rotation from axis aligned position (in global CS) to final position.</param>
        public Box3d(Point3d center, double lx, double ly, double lz, Rotation r)
        {
            _center = center.Copy();
            _lx = lx;
            _ly = ly;
            _lz = lz;
            _r = r.Copy();
            _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
        }

        /// <summary>
        /// 在局部坐标系中初始化轴对齐框。
        /// Initializes axis aligned box in local coordinate system.
        /// </summary>
        /// <param name="center">盒子的中心点。<br></br>Center point of the box.</param>
        /// <param name="lx">第一维度。<br></br>First dimension.</param>
        /// <param name="ly">第二维度。<br></br>Second dimension.</param>
        /// <param name="lz">第三维度。<br></br>Third dimension.</param>
        /// <param name="coord">Local coordinate system.</param>
        public Box3d(Point3d center, double lx, double ly, double lz, Coord3d coord)
        {
            _center = center.Copy();
            _lx = lx;
            _ly = ly;
            _lz = lz;
            _r = new Rotation(coord);
            _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
        }
        #endregion

        /// <summary>
        /// 创建对象的副本<br></br>
        /// Creates copy of the object
        /// </summary>
        public Box3d Copy()
        {
            return new Box3d(_center, _lx, _ly, _lz, _r);
        }

        #region "Properties"
        /// <summary>
        /// 盒子的中心点。<br></br>
        /// Center point of the box.
        /// </summary>
        public Point3d Center
        {
            get { return _center.Copy(); }
            set {
                _center = value.Copy();
                _list_p = null;
                _list_t = null;
                _list_e = null;
                _list_plane = null;
                _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
            }
        }

        /// <summary>
        /// 第一维度。<br></br>
        /// First dimension.
        /// </summary>
        public double L1
        {
            get { return _lx; }
            set {
                _lx = value;
                _list_p = null;
                _list_t = null;
                _list_e = null;
                _list_plane = null;
            }
        }

        /// <summary>
        /// 第二维度。
        /// Second dimension.
        /// </summary>
        public double L2
        {
            get { return _ly; }
            set {
                _ly = value;
                _list_p = null;
                _list_t = null;
                _list_e = null;
                _list_plane = null;
            }
        }

        /// <summary>
        /// 第三维度。
        /// Third dimension.
        /// </summary>
        public double L3
        {
            get { return _lz; }
            set {
                _lz = value;
                _list_p = null;
                _list_t = null;
                _list_e = null;
                _list_plane = null;
            }
        }

        /// <summary>
        /// 盒子第一维的方向。<br></br>
        /// Orientation of the first dimension of the box.
        /// </summary>
        public Vector3d V1
        {
            get { return _r.ConvertToGlobal().ToRotationMatrix.Column1; }
        }

        /// <summary>
        /// 盒子第二维的方向。<br></br>
        /// Orientation of the second dimension of the box.
        /// </summary>
        public Vector3d V2
        {
            get { return _r.ConvertToGlobal().ToRotationMatrix.Column2; }
        }

        /// <summary>
        /// 盒子第三维度的方向。
        /// Orientation of the third dimension of the box.
        /// </summary>
        public Vector3d V3
        {
            get { return _r.ConvertToGlobal().ToRotationMatrix.Column3; }
        }

        /// <summary>
        /// 盒子的方向，定义为从轴对齐位置（在全局 CS 中）到最终位置的旋转。<br></br>
        /// Orientation of the box, defined as rotation from axis aligned position (in global CS) to final position.
        /// </summary>
        public Rotation Orientation
        {
            get { return _r.Copy(); }
            set {
                _r = value.Copy();
                _list_p = null;
                _list_t = null;
                _list_e = null;
                _list_plane = null;
                _local_coord = new Coord3d(_center, _r.ConvertToGlobal().ToRotationMatrix.Transpose());
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P1
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    - _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    - _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    - _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P2
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    + _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    - _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    - _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P3
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    + _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    + _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    - _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P4
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    - _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    + _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    - _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P5
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    - _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    - _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    + _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P6
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    + _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    - _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    + _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P7
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    + _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    + _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    + _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 盒子的角点。<br></br>
        /// Corner point of the box.
        /// </summary>
        public Point3d P8
        {
            get
            {
                return _center.ConvertToGlobal() + 0.5 * (
                    - _lx * _r.ConvertToGlobal().ToRotationMatrix.Column1.ToPoint
                    + _ly * _r.ConvertToGlobal().ToRotationMatrix.Column2.ToPoint
                    + _lz * _r.ConvertToGlobal().ToRotationMatrix.Column3.ToPoint);
            }
        }

        /// <summary>
        /// 角点列表。<br></br>
        /// List of corner points.
        /// </summary>
        public List<Point3d> ListOfPoints
        {
            get
            {
                if (_list_p == null)
                {
                    _list_p = new List<Point3d> { P1, P2, P3, P4, P5, P6, P7, P8 };
                    return _list_p;
                }
                else
                {
                    return _list_p;
                }
            }
        }

        /// <summary>
        /// 构成盒子表面的三角形列表<br></br>
        /// List of triangles forming the box's surface
        /// </summary>
        public List<Triangle> ListOfTriangles
        {
            get
            {
                if (_list_t == null)
                {
                    Point3d P1 = this.P1;
                    Point3d P2 = this.P2;
                    Point3d P3 = this.P3;
                    Point3d P4 = this.P4;
                    Point3d P5 = this.P5;
                    Point3d P6 = this.P6;
                    Point3d P7 = this.P7;
                    Point3d P8 = this.P8;
                    _list_t = new List<Triangle> { };
                    _list_t.Add(new Triangle(P1, P3, P2));
                    _list_t.Add(new Triangle(P1, P4, P3));

                    _list_t.Add(new Triangle(P1, P2, P6));
                    _list_t.Add(new Triangle(P1, P6, P5));

                    _list_t.Add(new Triangle(P2, P3, P7));
                    _list_t.Add(new Triangle(P2, P7, P6));

                    _list_t.Add(new Triangle(P3, P4, P8));
                    _list_t.Add(new Triangle(P3, P8, P7));

                    _list_t.Add(new Triangle(P4, P1, P5));
                    _list_t.Add(new Triangle(P4, P5, P8));

                    _list_t.Add(new Triangle(P5, P6, P7));
                    _list_t.Add(new Triangle(P5, P7, P8));

                    return _list_t;
                }
                else
                {
                    return _list_t;
                }

            }
        }

        /// <summary>
        /// 构成盒子表面的平面列表<br></br>
        /// List of planes forming the box's surface
        /// </summary>
        public List<Plane3d> ListOfPlanes
        {
            get
            {
                if (_list_plane == null)
                {
                    _list_plane = new List<Plane3d> { };
                    _list_plane.Add(new Plane3d(P1, -V1));
                    _list_plane.Add(new Plane3d(P2, V1));
                    _list_plane.Add(new Plane3d(P1, -V2));
                    _list_plane.Add(new Plane3d(P4, V2));
                    _list_plane.Add(new Plane3d(P1, -V3));
                    _list_plane.Add(new Plane3d(P5, V3));

                    return _list_plane;
                }
                else
                {
                    return _list_plane;
                }
            }
        }

        /// <summary>
        /// 构成盒子的边列表<br></br>
        /// List of edges forming the box
        /// </summary>
        public List<Segment3d> ListOfEdges
        {
            get
            {
                if (_list_e ==  null)
                {
                    _list_e = new List<Segment3d> { };
                    _list_e.Add(new Segment3d(P1, P2));
                    _list_e.Add(new Segment3d(P2, P3));
                    _list_e.Add(new Segment3d(P3, P4));
                    _list_e.Add(new Segment3d(P4, P1));
                    _list_e.Add(new Segment3d(P5, P6));
                    _list_e.Add(new Segment3d(P6, P7));
                    _list_e.Add(new Segment3d(P7, P8));
                    _list_e.Add(new Segment3d(P8, P5));
                    _list_e.Add(new Segment3d(P1, P5));
                    _list_e.Add(new Segment3d(P2, P6));
                    _list_e.Add(new Segment3d(P3, P7));
                    _list_e.Add(new Segment3d(P4, P8));

                    return _list_e;
                }
                else
                {
                    return _list_e;
                }
            }
        }

        /// <summary>
        /// 盒子的体积。<br></br>
        /// Volume of the box.
        /// </summary>
        public double Volume
        {
            get { return _lx * _ly * _lz; }
        }

        /// <summary>
        /// 盒子的表面面积。<br></br>
        /// Surface area of the box.
        /// </summary>
        public double Area
        {
            get { return 2.0 * (_lx*_ly + _lx*_lz + _ly*_lz); }
        }

        /// <summary>
        /// 盒子对角线的长度。<br></br>
        /// Length of the box diagonal.
        /// </summary>
        public double Diagonal
        {
            get { return Sqrt(_lx * _lx + _ly * _ly + _lz * _lz); }
        }

        /// <summary>
        /// 如果框与轴对齐则为 True
        /// True if box is axis aligned
        /// </summary>
        public bool IsAxisAligned
        {
            get { return _r.ToRotationMatrix.IsIdentity; }
        }

        #endregion

        #region "边界盒子　BoundingBox"
        /// <summary>
        /// 返回最小边界框。<br></br>
        /// Return minimum bounding box.
        /// </summary>
        public Box3d MinimumBoundingBox
        {
            get { return this.Copy(); }
        }

        /// <summary>
        /// 返回轴对齐边界框（AABB）。<br></br>
        /// Return Axis Aligned Bounding Box (AABB).
        /// </summary>
        public AABB AABB
        {
            get { return GeometRi.AABB.BoundingBox(this.ListOfPoints); }
        }

        /// <summary>
        /// 返回给定坐标系中的边界框。<br></br>
        /// Return Bounding Box in given coordinate system.
        /// </summary>
        public Box3d BoundingBox(Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            Point3d c = _center.ConvertTo(coord);
            double mx = c.X;
            double my = c.Y;
            double mz = c.Z;

            foreach (Point3d p in this.ListOfPoints)
            {
                Point3d t = p.ConvertTo(coord);
                if (t.X < mx) mx = t.X;
                if (t.Y < my) my = t.Y;
                if (t.Z < mz) mz = t.Z;
            }

            return new Box3d(c, 2.0 * (c.X - mx), 2.0 * (c.Y - my), 2.0 * (c.Z - mz), coord);
        }

        /// <summary>
        /// 返回边界球。<br></br>
        /// Return bounding sphere.
        /// </summary>
        public Sphere BoundingSphere
        {
            get
            {
                double r = 0.5 * Sqrt(_lx * _lx + _ly * _ly + _lz * _lz);
                return new Sphere(this.Center, r);
            }
        }
        #endregion

        /// <summary>
        /// 返回点云的边界框。<br></br>
        /// Return Bounding Box for a cloud of points.
        /// </summary>
        public static Box3d BoundingBox(IEnumerable<Point3d> points, Coord3d coord = null)
        {
            coord = (coord == null) ? Coord3d.GlobalCS : coord;

            double maxx = double.NegativeInfinity;
            double maxy = double.NegativeInfinity;
            double maxz = double.NegativeInfinity;
            double minx = double.PositiveInfinity;
            double miny = double.PositiveInfinity;
            double minz = double.PositiveInfinity;

            foreach (Point3d p in points)
            {
                Point3d t = p.ConvertTo(coord);
                if (t.X > maxx) { maxx = t.X; }
                if (t.Y > maxy) { maxy = t.Y; }
                if (t.Z > maxz) { maxz = t.Z; }
                if (t.X < minx) { minx = t.X; }
                if (t.Y < miny) { miny = t.Y; }
                if (t.Z < minz) { minz = t.Z; }
            }

            return new Box3d(new Point3d(0.5*(maxx + minx), 0.5 * (maxy + miny), 0.5 * (maxz + minz)), maxx - minx, maxy - miny, maxz - minz, coord);
        }


        #region "Intersection"
        /// <summary>
        /// 获取线与框的交点。<br></br>
        /// Get intersection of line with box.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {
            return _line_intersection(l, double.NegativeInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// 获取射线与盒子的交点。<br></br>
        /// Get intersection of ray with box.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Ray3d r)
        {
            return _line_intersection(r.ToLine, 0.0, double.PositiveInfinity);
        }

        /// <summary>
        /// 获取线段与框的交点。<br></br>
        /// Get intersection of segment with box.<br></br>
        /// 返回“null”（无交点）或“Point3d”或“Segment3d”类型的对象。<br></br>
        /// Returns 'null' (no intersection) or object of type 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Segment3d s)
        {
            return _line_intersection(s.ToLine, 0.0, s.Length);
        }

        /// <summary>
        /// 检查方框与三角形的交点<br></br>
        /// Check intersection of box with triangle
        /// </summary>
        public bool Intersects(Triangle t)
        {
            if (t.A.BelongsTo(this) || t.B.BelongsTo(this) || t.C.BelongsTo(this))
            {
                return true;
            }

            foreach (Triangle bt in this.ListOfTriangles)
            {
                if (bt.Intersects(t)) return true;
            }

            return false;
        }

        /// <summary>
        /// 检查两个盒子的交集（仅适用于 AABB 盒子，不检查速度）<br></br>
        /// Check intersection of two boxes (only for AABB boxes, no check is performed for speed)
        /// </summary>
        public bool Intersects(Box3d box)
        {
            bool x = Abs(this.Center.X - box.Center.X) <= 0.5 * (this.L1 + box.L1) ? true : false;
            bool y = Abs(this.Center.Y - box.Center.Y) <= 0.5 * (this.L2 + box.L2) ? true : false;
            bool z = Abs(this.Center.Z - box.Center.Z) <= 0.5 * (this.L3 + box.L3) ? true : false;

            return x && y && z;
        }

        private object _line_intersection(Line3d l, double t0, double t1)
        {
            // Smith 算法：
            // Smith's algorithm:
            // “一种高效且稳健的射线盒相交算法”
            // "An Efficient and Robust Ray–Box Intersection Algorithm"
            // 艾米·威廉姆斯、史蒂夫·巴勒斯、R. 基思·莫利、彼得·雪莉
            // Amy Williams, Steve Barrus, R. Keith Morley, Peter Shirley
            // http://www.cs.utah.edu/~awilliam/box/box.pdf

            // 修改以允许基于公差的检查
            // Modified to allow tolerance based checks

            // 相对公差 ================================
            // Relative tolerance ================================
            if (!GeometRi3D.UseAbsoluteTolerance)
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Diagonal;
                GeometRi3D.UseAbsoluteTolerance = true;
                object result = _line_intersection(l, t0, t1);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
            //====================================================

            // 定义与框对齐的局部 CS
            // Define local CS aligned with box
            Coord3d local_CS = this.LocalCoord();

            Point3d Pmin = this.P1.ConvertTo(local_CS);
            Point3d Pmax = this.P7.ConvertTo(local_CS);

            l = new Line3d(l._point.ConvertTo(local_CS), l._dir.ConvertTo(local_CS).Normalized);

            double tmin, tmax, tymin, tymax, tzmin, tzmax;
            double divx = 1 / l._dir.X;
            if (divx >= 0)
            {
                tmin = (Pmin.X - l._point.X) * divx;
                tmax = (Pmax.X - l._point.X) * divx;
            }
            else
            {
                tmin = (Pmax.X - l._point.X) * divx;
                tmax = (Pmin.X - l._point.X) * divx;
            }

            double divy = 1 / l._dir.Y;
            if (divy >= 0)
            {
                tymin = (Pmin.Y - l._point.Y) * divy;
                tymax = (Pmax.Y - l._point.Y) * divy;
            }
            else
            {
                tymin = (Pmax.Y - l._point.Y) * divy;
                tymax = (Pmin.Y - l._point.Y) * divy;
            }

            if (GeometRi3D.Greater(tmin, tymax) || GeometRi3D.Greater(tymin, tmax))
                return null;
            if (GeometRi3D.Greater(tymin, tmin))
                tmin = tymin;
            if (GeometRi3D.Smaller(tymax, tmax))
                tmax = tymax;

            double divz = 1 / l._dir.Z;
            if (divz >= 0)
            {
                tzmin = (Pmin.Z - l._point.Z) * divz;
                tzmax = (Pmax.Z - l._point.Z) * divz;
            }
            else
            {
                tzmin = (Pmax.Z - l._point.Z) * divz;
                tzmax = (Pmin.Z - l._point.Z) * divz;
            }

            if (GeometRi3D.Greater(tmin, tzmax) || GeometRi3D.Greater(tzmin, tmax))
                return null;
            if (GeometRi3D.Greater(tzmin, tmin))
                tmin = tzmin;
            if (GeometRi3D.Smaller(tzmax, tmax))
                tmax = tzmax;

            // 现在检查片段的重叠部分
            // Now check the overlapping portion of the segments
            // 原始算法中缺少这一部分
            // This part is missing in the original algorithm
            if (GeometRi3D.Greater(tmin, t1))
                return null;
            if (GeometRi3D.Smaller(tmax, t0))
                return null;

            if (GeometRi3D.Smaller(tmin, t0))
                tmin = t0;
            if (GeometRi3D.Greater(tmax, t1))
                tmax = t1;

            if (GeometRi3D.AlmostEqual(tmin, tmax))
            {
                return l._point.Translate(tmin * l._dir);
            }
            else
            {
                return new Segment3d(l._point.Translate(tmin * l._dir), l._point.Translate(tmax * l._dir));
            }
        }
        #endregion


        /// <summary>
        /// 局部坐标系，原点位于框的中心并与框对齐<br></br>
        /// Local coordinate system with origin in box's center and aligned with box
        /// </summary>
        public Coord3d LocalCoord()
        {
            return _local_coord;
        }

        /// <summary>
        /// 盒子上最接近目标点“p”的点（包括内部点）。<br></br>
        /// Point on box (including interior points) closest to target point "p".
        /// </summary>
        public Point3d ClosestPoint(Point3d p)
        {
            Coord3d local_coord = this.LocalCoord();
            p = p.ConvertTo(local_coord);
            double x = GeometRi3D.Clamp(p.X, -_lx / 2, _lx / 2);
            double y = GeometRi3D.Clamp(p.Y, -_ly / 2, _ly / 2);
            double z = GeometRi3D.Clamp(p.Z, -_lz / 2, _lz / 2);

            return new Point3d(x, y, z, local_coord);
        }

        /// <summary>
        /// 轴对齐框（包括内部点）上最接近目标点“p”的点。<br></br>
        /// Point on axis aligned box (including interior points) closest to target point "p".
        /// </summary>
        public Point3d AABBClosestPoint(Point3d p)
        {
            p = p.ConvertToGlobal();
            double x = GeometRi3D.Clamp(p.X, this._center.X - _lx / 2, this._center.X + _lx / 2);
            double y = GeometRi3D.Clamp(p.Y, this._center.Y - _ly / 2, this._center.Y + _ly / 2);
            double z = GeometRi3D.Clamp(p.Z, this._center.Z - _lz / 2, this._center.Z + _lz / 2);

            return new Point3d(x, y, z);
        }

        /// <summary>
        /// 框到点的距离（对于位于框内的点将返回零）<br></br>
        /// Distance from box to point (zero will be returned for point located inside box)
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            return ClosestPoint(p).DistanceTo(p);
        }

        /// <summary>
        /// 从轴对齐框到点的距离（对于位于框内的点将返回零）<br></br>
        /// Distance from axis aligned box to point (zero will be returned for point located inside box)
        /// </summary>
        public double AABBDistanceTo(Point3d p)
        {
            return AABBClosestPoint(p).DistanceTo(p);
        }

        /// <summary>
        /// 从盒子到球体的最短距离<br></br>
        /// Shortest distance from box to sphere
        /// </summary>
        public double DistanceTo(Sphere s)
        {
            Point3d p = this.ClosestPoint(s._point);
            double dist = p.DistanceTo(s._point);
            return dist <= s.R ? 0.0 : dist - s.R;
        }

        /// <summary>
        /// 盒子到圆圈的最短距离<br></br>
        /// Shortest distance from box to circle
        /// </summary>
        public double DistanceTo(Circle3d c)
        {
            if (c._point.IsInside(this))
            {
                return 0;
            }
            double min_dist = Double.PositiveInfinity;
            foreach (Triangle triangle in ListOfTriangles)
            {
                double dist = c.DistanceTo(triangle);
                if (dist <= GeometRi3D.Tolerance) return 0;
                if (dist < min_dist) min_dist = dist;
            }
            return min_dist;
        }

        /// <summary>
        /// 圆与框之间的相交检查<br></br>
        /// Intersection check between circle and box
        /// </summary>
        public bool Intersects(Circle3d c)
        {
            //if (c.Center.IsInside(this)) return true;
            double dist = c._point.DistanceTo(this);
            if (dist > c.R) return false;
            if (dist < GeometRi3D.Tolerance) return true;

            foreach (Triangle triangle in ListOfTriangles)
            {
                if (c.Intersects(triangle)) return true;
            }
            return false;
        }

        internal override int _PointLocation(Point3d p)
        {
            Coord3d coord = this.LocalCoord();
            p = p.ConvertTo(coord);
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                if ( (Abs(p.X)-L1/2) <= GeometRi3D.Tolerance && (Abs(p.Y) - L2 / 2) <= GeometRi3D.Tolerance && (Abs(p.Z) - L3 / 2) <= GeometRi3D.Tolerance )
                {
                    if ( (Abs(p.X) - L1 / 2) < -GeometRi3D.Tolerance && (Abs(p.Y) - L2 / 2) < -GeometRi3D.Tolerance && (Abs(p.Z) - L3 / 2) < -GeometRi3D.Tolerance)
                    {
                        return 1; //点严格位于框内 Point is strictly inside box
                    } else
                    {
                        return 0; //点位于边界上 Point is on boundary
                    }
                } else
                {
                    return -1; //点在外面 Point is outside
                }
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Diagonal;
                GeometRi3D.UseAbsoluteTolerance = true;
                int result = this._PointLocation(p);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }
        }

        #region "平移 旋转 反射 TranslateRotateReflect"
        /// <summary>
        /// 通过向量平移盒子<br></br>
        /// Translate box by a vector
        /// </summary>
        public Box3d Translate(Vector3d v)
        {
            return new Box3d(_center.Translate(v), _lx, _ly, _lz);
        }

        /// <summary>
        /// 围绕点“p”作为旋转中心旋转盒子。<br></br>
        /// Rotate box around point 'p' as a rotation center.
        /// </summary>
        public Box3d Rotate(Rotation r, Point3d p)
        {
            Point3d new_center = r.ToRotationMatrix * (this._center - p) + p;
            Rotation new_rotation = r * this.Orientation;
            return new Box3d(new_center, _lx, _ly, _lz, new_rotation);
        }

        /// <summary>
        /// 给定点反射盒子<br></br>
        /// Reflect box in given point
        /// <para>The order of corner points will be changed during reflection operation.</para>
        /// </summary>
        public virtual Box3d ReflectIn(Point3d p)
        {
            Point3d new_center = this.Center.ReflectIn(p);
            return new Box3d(new_center, _lx, _ly, _lz, this._r);
        }

        /// <summary>
        /// 给定线反射盒子
        /// Reflect box in given line
        /// <para>
        /// 反射操作过程中角点的顺序将会改变。<br></br>
        /// The order of corner points will be changed during reflection operation.
        /// </para>
        /// </summary>
        public virtual Box3d ReflectIn(Line3d l)
        {
            Point3d new_center = this.Center.ReflectIn(l);
            Rotation r = new GeometRi.Rotation(l.Direction, PI);
            Rotation new_rotation = r * this.Orientation;
            return new Box3d(new_center, _lx, _ly, _lz, new_rotation);
        }

        /// <summary>
        /// 给定平面反射盒子
        /// Reflect box in given plane
        /// <para>The order of corner points will be changed during reflection operation.</para>
        /// </summary>
        public virtual Box3d ReflectIn(Plane3d s)
        {
            Point3d new_center = this.Center.ReflectIn(s);
            Vector3d nV1 = this.V1.ReflectIn(s);
            Vector3d nV2 = this.V2.ReflectIn(s);
            Coord3d coord = new Coord3d(new_center, nV1, nV2);
            Rotation new_rotation = new Rotation(coord);
            return new Box3d(new_center, _lx, _ly, _lz, new_rotation);
        }

        /// <summary>
        /// 相对于给定点的缩放框<br></br>
        /// Scale box relative to given point
        /// </summary>
        public virtual Box3d Scale(double scale, Point3d scaling_center)
        {
            Point3d new_center = scaling_center + scale * (this.Center - scaling_center);
            return new Box3d(new_center, scale * _lx, scale * _ly, scale * _lz, this._r);
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
            Box3d b = (Box3d)obj;

            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return this.Center == b.Center && _r == b.Orientation &&
                       GeometRi3D.AlmostEqual(L1, b.L1) &&
                       GeometRi3D.AlmostEqual(L2, b.L2) &&
                       GeometRi3D.AlmostEqual(L3, b.L3);
            }
            else
            {
                double tol = GeometRi3D.Tolerance;
                GeometRi3D.Tolerance = tol * this.Diagonal;
                GeometRi3D.UseAbsoluteTolerance = true;
                bool result = this.Equals(b);
                GeometRi3D.UseAbsoluteTolerance = false;
                GeometRi3D.Tolerance = tol;
                return result;
            }

        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            int hash_code = GeometRi3D.HashFunction(_lx.GetHashCode(), _ly.GetHashCode(), _lz.GetHashCode());
            return GeometRi3D.HashFunction(_center.GetHashCode(), _r.GetHashCode(), hash_code);
        }

        /// <summary>
        /// 全局坐标系中对象的字符串表示形式。<br></br>
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override string ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }

        /// <summary>
        /// 参考坐标系中对象的字符串表示。<br></br>
        /// String representation of an object in reference coordinate system.
        /// </summary>
        public string ToString(Coord3d coord)
        {
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d p = _center.ConvertTo(coord);

            string str = string.Format("Box3d (reference coord.sys. ") + coord.Name + "):" + nl;
            str += string.Format("Center -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p.X, p.Y, p.Z) + nl;
            str += string.Format("Lx, Ly, Lz -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", _lx, _ly, _lz) + nl;
            str += _r.ToString(coord);
            return str;
        }

        // 运算符重载
        // Operators overloads
        //-----------------------------------------------------------------
        public static bool operator ==(Box3d b1, Box3d b2)
        {
            if (object.ReferenceEquals(b1, null))
                return object.ReferenceEquals(b2, null);
            return b1.Equals(b2);
        }
        public static bool operator !=(Box3d b1, Box3d b2)
        {
            if (object.ReferenceEquals(b1, null))
                return !object.ReferenceEquals(b2, null);
            return !b1.Equals(b2);
        }
    }
}
