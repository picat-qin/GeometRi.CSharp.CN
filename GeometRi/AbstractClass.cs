using System;
using static System.Math;
using System.Collections.Generic;

namespace GeometRi
{
    /// <summary>
    /// 有限对象
    /// </summary>
#if NET20
    [Serializable]
#endif
    abstract public class FiniteObject
    {
        /// <summary>
        /// 点位置
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        abstract internal int _PointLocation(Point3d p);
    }
}
