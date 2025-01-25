using System;

namespace GeometRi
{

    /// <summary>
    /// 一维对象接口（矢量、线、射线、线段）<br></br>
    /// Interface for 1D objects (vector, line, ray, segment)
    /// </summary>
    public interface ILinearObject
    {
        /// <summary>
        /// 方向
        /// </summary>
        Vector3d Direction { get; }
        /// <summary>
        /// 有方向
        /// </summary>
        bool IsOriented { get; }
        /// <summary>
        /// 转为线
        /// </summary>
        Line3d ToLine { get; }
    }

    /// <summary>
    /// 二维对象接口（平面、圆、椭圆、三角形）<br></br>
    /// Interface for 2D objects (plane, circle, ellipse, triangle)
    /// </summary>
    public interface IPlanarObject
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Vector3d Normal { get; }
        /// <summary>
        /// 有方向
        /// </summary>
        bool IsOriented { get; }
        /// <summary>
        /// 平面
        /// </summary>
        Plane3d ToPlane { get; }
    }

    /// <summary>
    /// 有限对象接口<br></br>
    /// Interface for finite objects
    /// </summary>
    public interface IFiniteObject
    {
        /// <summary>
        /// 边界框
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        Box3d BoundingBox(Coord3d coord);
        /// <summary>
        /// 最小边界框
        /// </summary>
        Box3d MinimumBoundingBox { get; }
        /// <summary>
        /// 边界球
        /// </summary>
        Sphere BoundingSphere { get; }
    }
}
