using System;
using static System.Math;


namespace GeometRi
{
    /// <summary>
    /// 在全局或局部参考系中定义的 3D 空间中的旋转（内部由旋转矩阵表示）。<br></br>
    /// Rotation in 3D space defined in global or local reference frame (internally represented by rotation matrix).
    /// </summary>
#if NET20
    [Serializable]
#endif
    public class Rotation
    {
        private Matrix3d _r;
        private Coord3d _coord;
        private double _angle;
        private Vector3d _axis;
        private bool converted_to_axisangle = false;

        #region "Constructors"
        /// <summary>
        /// 默认构造函数，初始化单位矩阵（零旋转）。<br></br>
        /// Default constructor, initializes identity matrix (zero rotation).
        /// </summary>
        public Rotation()
        {
            _r = Matrix3d.Identity();
            _coord = Coord3d.GlobalCS;
        }

        /// <summary>
        /// 初始化旋转，等于从全局 CS 到“坐标”的旋转。<br></br>
        /// Initializes rotation, equal to the rotation from global CS to 'coord'.
        /// </summary>
        public Rotation(Coord3d coord)
        {
            if (coord == null)
            {
                _r = Matrix3d.Identity();
            }
            else
            {
                _r = coord.Axes.Transpose();
            }
            _coord = Coord3d.GlobalCS;
        }

        /// <summary>
        /// 使用旋转矩阵初始化旋转。<br></br>
        /// Initializes rotation using rotation matrix.
        /// </summary>
        /// <param name="m">旋转矩阵。<br></br>Rotation matrix.</param>
        /// <param name="coord">
        /// 参考坐标系（默认 - Coord3d.GlobalCS）。<br></br>
        /// Reference coordinate system (default - Coord3d.GlobalCS).
        /// </param>
        public Rotation(Matrix3d m, Coord3d coord = null)
        {
            _r = m.Copy();
            _coord = (coord == null) ? Coord3d.GlobalCS : coord;
        }

        /// <summary>
        /// 使用四元数初始化旋转。<br></br>
        /// Initializes rotation using quaternion.
        /// </summary>
        /// <param name="q"></param>
        public Rotation(Quaternion q)
        {
            _r = q.ToRotationMatrix();
            _coord = q.Coord;
        }

        /// <summary>
        /// 使用轴和旋转角度初始化旋转。<br></br>
        /// Initializes rotation using axis and angle of rotation.
        /// </summary>
        /// <param name="axis">旋转轴。<br></br>Rotation axis</param>
        /// <param name="alpha">
        /// 旋转角度（逆时针，弧度）<br></br>
        /// Angle of rotation (counterclockwise, radians)
        /// </param>
        public Rotation(Vector3d axis, double alpha)
        {
            Vector3d v = axis.Normalized;
            double c = Cos(alpha);
            double s = Sin(alpha);

            _r = new Matrix3d();
            _r[0, 0] = c + v.X * v.X * (1 - c);
            _r[0, 1] = v.X * v.Y * (1 - c) - v.Z * s;
            _r[0, 2] = v.X * v.Z * (1 - c) + v.Y * s;

            _r[1, 0] = v.Y * v.X * (1 - c) + v.Z * s;
            _r[1, 1] = c + v.Y * v.Y * (1 - c);
            _r[1, 2] = v.Y * v.Z * (1 - c) - v.X * s;

            _r[2, 0] = v.Z * v.X * (1 - c) - v.Y * s;
            _r[2, 1] = v.Z * v.Y * (1 - c) + v.X * s;
            _r[2, 2] = c + v.Z * v.Z * (1 - c);

            _coord = axis.Coord;
        }
        #endregion

        private double this[int i, int j]
        {
            get { return this._r[i, j]; }
            set { this._r[i, j] = value; }
        }

        /// <summary>
        /// 随机旋转<br></br>
        /// Random rotation
        /// </summary>
        public static Rotation Random()
        {
            return new Rotation(Vector3d.Random(), 2 * PI * GeometRi3D.rnd.NextDouble());
        }

        /// <summary>
        /// Creates copy of the object
        /// </summary>
        public Rotation Copy()
        {
            return new Rotation(_r, _coord);
        }

        #region "Properties"
        /// <summary>
        /// 返回与当前旋转等效的旋转矩阵。<br></br>
        /// Returns rotation matrix equivalent to the current rotation.
        /// </summary>
        public Matrix3d ToRotationMatrix
        {
            get { return this._r; }
        }

        /// <summary>
        /// 返回反向旋转。<br></br>
        /// Returns inverse rotation.
        /// </summary>
        public Rotation Inverse
        {
            get { return new Rotation(this._r.Transpose()); }
        }

        /// <summary>
        /// 返回与当前旋转等价的四元数。<br></br>
        /// Returns quaternion equivalent to the current rotation.
        /// </summary>
        public Quaternion ToQuaternion
        {
            get { return new Quaternion(_r, _coord); }
        }

        /// <summary>
        /// 返回旋转轴（使用“ToAngle”属性获取旋转的角度）。<br></br>
        /// Returns axis of the rotation (use 'ToAngle' property to get angle of the rotation).
        /// </summary>
        public Vector3d ToAxis
        {
            get
            {

                if (converted_to_axisangle)
                {
                    return _axis;
                }

                Quaternion q = this.ToQuaternion;
                _angle = q.ToAngle;
                _axis = q.ToAxis;
                converted_to_axisangle = true;
                return _axis;
            }
        }

        /// <summary>
        /// 返回旋转的角度（使用‘ToAxis’属性来获取旋转的轴）。<br></br>
        /// Returns angle of the rotation (use 'ToAxis' property to get axis of the rotation).
        /// </summary>
        public double ToAngle
        {
            get
            {
                if (converted_to_axisangle)
                {
                    return _angle;
                }
                else
                {
                    Quaternion q = this.ToQuaternion;
                    _angle = q.ToAngle;
                    _axis = q.ToAxis;
                    converted_to_axisangle = true;
                    return _angle;
                }
            }
        }

        /// <summary>
        ///  参考坐标系<br></br>
        ///  Reference coordinate system
        /// </summary>
        public Coord3d Coord
        {
            get { return _coord; }
        }
        #endregion

        /// <summary>
        /// 通过组合三个基本旋转（即绕坐标系轴的旋转）来创建旋转对象。<br></br>
        /// Creates rotation object by composing three elemental rotations, i.e. rotations about the axes of a coordinate system.
        /// <para>允许使用适当的欧拉角 ("xyx"、"zxz" 等) 或 Tait–Bryan 角 ("xyz"、"yzx")。</para>
        /// 外部旋转（固定框架内的旋转）应以小写形式书写（“xyz”、“zxz”等）。
        /// <para>固有旋转（移动框架中的旋转）应该用大写字母书写（“XYZ”、“ZXZ”等）。</para>
        /// <para>Both proper Euler angles ("xyx", "zxz", etc.) or Tait–Bryan angles ("xyz", "yzx") are allowed.</para>
        /// Extrinsic rotations (rotations in fixed frame) should be written in lower case ("xyz", zxz", etc.).
        /// <para>Intrinsic rotations (rotations in moving frame) should be written in upper case ("XYZ", "ZXZ", etc.).</para>
        /// </summary>
        /// <param name="alpha">第一个旋转角度。<br></br> First rotation angle.</param>
        /// <param name="beta">第二个旋转角度。<br></br> Second rotation angle.</param>
        /// <param name="gamma">第三个旋转角度。<br></br> Third rotation angle.</param>
        /// <param name="RotationOrder">
        /// 字符串，表示以“xyz”（外部旋转，固定框架）或“XYZ”（内部旋转，移动框架）形式表示的旋转轴。<br></br>
        /// String, representing rotation axes in the form "xyz" (extrinsic rotations, fixed frame) or "XYZ" (intrinsic rotations, moving frame).
        /// </param>
        /// <param name="coord">
        /// 参考坐标系，默认-Coord3d.GlobalCS。<br></br>
        /// Reference coordinate system, default - Coord3d.GlobalCS.
        /// </param>
        /// <returns></returns>
        public static Rotation FromEulerAngles(double alpha, double beta, double gamma, string RotationOrder, Coord3d coord = null)
        {
            if (string.IsNullOrEmpty(RotationOrder) || RotationOrder.Length < 3)
            {
                throw new ArgumentException("Invalid parameter: RotationOrder");
            }

            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            Vector3d v1 = CharToVector(RotationOrder[0], coord);
            Vector3d v2 = CharToVector(RotationOrder[1], coord);
            Vector3d v3 = CharToVector(RotationOrder[2], coord);

            Rotation r1 = new Rotation(v1, alpha);
            Rotation r2 = new Rotation(v2, beta);
            Rotation r3 = new Rotation(v3, gamma);

            if (RotationOrder[0] == 'x' || RotationOrder[0] == 'y' || RotationOrder[0] == 'z')
            {
                // Rotation in fixed frame
                return r3 * r2 * r1;
            }
            else
            {
                // Rotation in moving frame
                return r1 * r2 * r3;
            }
        }
        private static Vector3d CharToVector(char c, Coord3d coord)
        {
            if (c == 'x' || c == 'X') return new Vector3d(1, 0, 0, coord);
            if (c == 'y' || c == 'Y') return new Vector3d(0, 1, 0, coord);
            if (c == 'z' || c == 'Z') return new Vector3d(0, 0, 1, coord);

            throw new ArgumentException("Invalid parameter: RotationOrder");
        }

        /// <summary>
        /// 将旋转矩阵分解为三个元素旋转的乘积，即绕坐标系轴的旋转。
        /// <para>允许使用适当的欧拉角（“xyx”、“zxz”等）或 Tait-Bryan 角（“xyz”、“yzx”等）。</para> 
        /// 外部旋转（固定框架中的旋转）应以小写形式书写（“xyz”、“zxz”等）。
        /// <para>内部旋转（移动框架中的旋转）应以大写形式书写（“XYZ”、“ZXZ”等）。</para> 
        /// 请注意，这种分解通常不是唯一的！<br></br>
        /// Factor a rotation matrix as product of three elemental rotations, i.e. rotations about the axes of a coordinate system.
        /// <para>Both proper Euler angles ("xyx", "zxz", etc.) or Tait–Bryan angles ("xyz", "yzx") are allowed.</para>
        /// Extrinsic rotations (rotations in fixed frame) should be written in lower case ("xyz", zxz", etc.).
        /// <para>Intrinsic rotations (rotations in moving frame) should be written in upper case ("XYZ", "ZXZ", etc.).</para>
        /// Note that such factorization generally is not unique!
        /// </summary>
        /// <param name="RotationOrder">
        /// 字符串，表示以“xyz”（外部旋转，固定框架）或“XYZ”（内部旋转，移动框架）形式表示的旋转轴。<br></br>
        /// String, representing rotation axes in the form "xyz" (extrinsic rotations, fixed frame) or "XYZ" (intrinsic rotations, moving frame).
        /// </param>
        /// <param name="coord">
        /// 参考坐标系，默认-Coord3d.GlobalCS。<br></br>
        /// Reference coordinate system, default - Coord3d.GlobalCS.
        /// </param>
        /// <returns>
        /// 具有第一、第二和第三旋转角度的双阵列<br></br>
        /// Double array with first, second and third rotation angles
        /// </returns>
        public double[] ToEulerAngles(string RotationOrder, Coord3d coord = null)
        {
            if (string.IsNullOrEmpty(RotationOrder) || RotationOrder.Length < 3)
            {
                throw new ArgumentException("Invalid parameter: RotationOrder");
            }

            coord = (coord == null) ? Coord3d.GlobalCS : coord;
            Rotation r = this.ConvertTo(coord);

            // Portions of the code were derived from article
            // https://www.geometrictools.com/Documentation/EulerAngles.pdf
            // published under Boost license
            //============================================================================
            // Boost Software License - Version 1.0 - August 17th, 2003

            // Permission is hereby granted, free of charge, to any person or organization
            //obtaining a copy of the software and accompanying documentation covered by
            //this license(the "Software") to use, reproduce, display, distribute,
            //execute, and transmit the Software, and to prepare derivative works of the
            //Software, and to permit third - parties to whom the Software is furnished to
            //do so, all subject to the following:

            // The copyright notices in the Software and this entire statement, including
            //the above license grant, this restriction and the following disclaimer,
            //must be included in all copies of the Software, in whole or in part, and
            //all derivative works of the Software, unless such copies or derivative
            //works are solely in the form of machine-executable object code generated by
            //a source language processor.

            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            //FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON - INFRINGEMENT.IN NO EVENT
            //SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
            //FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
            //ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
            //DEALINGS IN THE SOFTWARE.
            //============================================================================

            double ax, ay, az;
            if (RotationOrder == "XYZ" || RotationOrder == "zyx")
            {
                if (GeometRi3D.Smaller(r[0,2],1))
                {
                    if (GeometRi3D.Greater(r[0, 2], -1))
                    {
                        ay = Asin(r[0, 2]);
                        ax = Atan2(-r[1, 2], r[2, 2]);
                        az = Atan2(-r[0, 1], r[0, 0]);
                    }
                    else
                    {
                        ay = -PI / 2.0;
                        ax = -Atan2(r[1, 0], r[1, 1]);
                        az = 0;
                    }
                }
                else
                {
                    ay = PI / 2.0;
                    ax = Atan2(r[1, 0], r[1, 1]);
                    az = 0;
                }
                if (RotationOrder == "XYZ")
                {
                    return new[] { ax, ay, az };
                }
                else
                {
                    return new[] { az, ay, ax };
                }
            }

            if (RotationOrder == "XZY" || RotationOrder == "yzx")
            {
                if (GeometRi3D.Smaller(r[0, 1], 1))
                {
                    if (GeometRi3D.Greater(r[0, 1], -1))
                    {
                        az = Asin(-r[0, 1]);
                        ax = Atan2(r[2, 1], r[1, 1]);
                        ay = Atan2(r[0, 2], r[0, 0]);
                    }
                    else
                    {
                        az = PI / 2.0;
                        ax = -Atan2(-r[2, 0], r[2, 2]);
                        ay = 0;
                    }
                }
                else
                {
                    az = -PI / 2.0;
                    ax = Atan2(-r[2, 0], r[2, 2]);
                    ay = 0;
                }
                if (RotationOrder == "XZY")
                {
                    return new[] { ax, az, ay };
                }
                else
                {
                    return new[] { ay, az, ax };
                }
            }

            if (RotationOrder == "YXZ" || RotationOrder == "zxy")
            {
                if (GeometRi3D.Smaller(r[1, 2], 1))
                {
                    if (GeometRi3D.Greater(r[1, 2], -1))
                    {
                        ax = Asin(-r[1, 2]);
                        ay = Atan2(r[0, 2], r[2, 2]);
                        az = Atan2(r[1, 0], r[1, 1]);
                    }
                    else
                    {
                        ax = PI / 2.0;
                        ay = -Atan2(-r[0, 2], r[0, 0]);
                        az = 0;
                    }
                }
                else
                {
                    ax = -PI / 2.0;
                    ay = Atan2(-r[0, 1], r[0, 0]);
                    az = 0;
                }
                if (RotationOrder == "YXZ")
                {
                    return new[] { ay, ax, az };
                }
                else
                {
                    return new[] { az, ax, ay };
                }
            }

            if (RotationOrder == "YZX" || RotationOrder == "xzy")
            {
                if (GeometRi3D.Smaller(r[1, 0], 1))
                {
                    if (GeometRi3D.Greater(r[1, 0], -1))
                    {
                        az = Asin(r[1, 0]);
                        ay = Atan2(-r[2, 0], r[0, 0]);
                        ax = Atan2(-r[1, 2], r[1, 1]);
                    }
                    else
                    {
                        az = -PI / 2.0;
                        ay = -Atan2(r[2, 1], r[2, 2]);
                        ax = 0;
                    }
                }
                else
                {
                    az = PI / 2.0;
                    ay = Atan2(r[2, 1], r[2, 2]);
                    ax = 0;
                }
                if (RotationOrder == "YZX")
                {
                    return new[] { ay, az, ax };
                }
                else
                {
                    return new[] { ax, az, ay };
                }
            }

            if (RotationOrder == "ZXY" || RotationOrder == "yxz")
            {
                if (GeometRi3D.Smaller(r[2, 1], 1))
                {
                    if (GeometRi3D.Greater(r[2, 1], -1))
                    {
                        ax = Asin(r[2, 1]);
                        az = Atan2(-r[0, 1], r[1, 1]);
                        ay = Atan2(-r[2, 0], r[2, 2]);
                    }
                    else
                    {
                        ax = -PI / 2.0;
                        az = -Atan2(r[0, 2], r[0, 0]);
                        ay = 0;
                    }
                }
                else
                {
                    ax = PI / 2.0;
                    az = Atan2(r[0, 2], r[0, 0]);
                    ay = 0;
                }
                if (RotationOrder == "ZXY")
                {
                    return new[] { az, ax, ay };
                }
                else
                {
                    return new[] { ay, ax, az };
                }
            }

            if (RotationOrder == "ZYX" || RotationOrder == "xyz")
            {
                if (GeometRi3D.Smaller(r[2, 0], 1))
                {
                    if (GeometRi3D.Greater(r[2, 0], -1))
                    {
                        ay = Asin(-r[2, 0]);
                        az = Atan2(r[1, 0], r[0, 0]);
                        ax = Atan2(r[2, 1], r[2, 2]);
                    }
                    else
                    {
                        ay = PI / 2.0;
                        az = -Atan2(-r[1, 2], r[1, 1]);
                        ax = 0;
                    }
                }
                else
                {
                    ay = -PI / 2.0;
                    az = Atan2(-r[1, 2], r[1, 1]);
                    ax = 0;
                }
                if (RotationOrder == "ZYX")
                {
                    return new[] { az, ay, ax };
                }
                else
                {
                    return new[] { ax, ay, az };
                }
            }


            double a1, a2, a3;
            if (RotationOrder == "XYX" || RotationOrder == "xyx")
            {
                if (GeometRi3D.Smaller(r[0, 0], 1))
                {
                    if (GeometRi3D.Greater(r[0, 0], -1))
                    {
                        a2 = Acos(r[0, 0]);
                        a1 = Atan2(r[1, 0], -r[2, 0]);
                        a3 = Atan2(r[0, 1], r[0, 2]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(-r[1, 2], r[1, 1]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(-r[1, 2], r[1, 1]);
                    a3 = 0;
                }
                if (RotationOrder == "XYX")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            if (RotationOrder == "XZX" || RotationOrder == "xzx")
            {
                if (GeometRi3D.Smaller(r[0, 0], 1))
                {
                    if (GeometRi3D.Greater(r[0, 0], -1))
                    {
                        a2 = Acos(r[0, 0]);
                        a1 = Atan2(r[2, 0], r[1, 0]);
                        a3 = Atan2(r[0, 2], -r[0, 1]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(-r[2, 1], r[2, 2]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(r[2, 1], r[2, 2]);
                    a3 = 0;
                }
                if (RotationOrder == "XZX")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            if (RotationOrder == "YXY" || RotationOrder == "yxy")
            {
                if (GeometRi3D.Smaller(r[1, 1], 1))
                {
                    if (GeometRi3D.Greater(r[1, 1], -1))
                    {
                        a2 = Acos(r[1, 1]);
                        a1 = Atan2(r[0, 1], r[2, 1]);
                        a3 = Atan2(r[1, 0], -r[1, 2]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(r[0, 2], r[0, 0]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(r[0, 2], r[0, 0]);
                    a3 = 0;
                }
                if (RotationOrder == "YXY")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            if (RotationOrder == "YZY" || RotationOrder == "yzy")
            {
                if (GeometRi3D.Smaller(r[1, 1], 1))
                {
                    if (GeometRi3D.Greater(r[1, 1], -1))
                    {
                        a2 = Acos(r[1, 1]);
                        a1 = Atan2(r[2, 1], -r[0, 1]);
                        a3 = Atan2(r[1, 2], r[1, 0]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(-r[2, 0], r[2, 2]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(-r[2, 0], r[2, 2]);
                    a3 = 0;
                }
                if (RotationOrder == "YZY")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            if (RotationOrder == "ZXZ" || RotationOrder == "zxz")
            {
                if (GeometRi3D.Smaller(r[2, 2], 1))
                {
                    if (GeometRi3D.Greater(r[2, 2], -1))
                    {
                        a2 = Acos(r[2, 2]);
                        a1 = Atan2(r[0, 2], -r[1, 2]);
                        a3 = Atan2(r[2, 0], r[2, 1]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(-r[0, 1], r[0, 0]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(-r[0, 1], r[0, 0]);
                    a3 = 0;
                }
                if (RotationOrder == "ZXZ")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            if (RotationOrder == "ZYZ" || RotationOrder == "zyz")
            {
                if (GeometRi3D.Smaller(r[2, 2], 1))
                {
                    if (GeometRi3D.Greater(r[2, 2], -1))
                    {
                        a2 = Acos(r[2, 2]);
                        a1 = Atan2(r[1, 2], r[0, 2]);
                        a3 = Atan2(r[2, 1], -r[2, 0]);
                    }
                    else
                    {
                        a2 = PI;
                        a1 = -Atan2(r[1, 0], r[1, 1]);
                        a3 = 0;
                    }
                }
                else
                {
                    a2 = 0;
                    a1 = Atan2(r[1, 0], r[1, 1]);
                    a3 = 0;
                }
                if (RotationOrder == "ZYZ")
                {
                    return new[] { a1, a2, a3 };
                }
                else
                {
                    return new[] { a3, a2, a1 };
                }
            }

            throw new ArgumentException("Invalid parameter: RotationOrder");
        }

        /// <summary>
        /// 两个旋转的球面线性插值。<br></br>
        /// Spherical linear interpolation of two rotations.
        /// </summary>
        /// <param name="r1">初始旋转<br></br> Initial rotation</param>
        /// <param name="r2">最后旋转<br></br> Final rotation</param>
        /// <param name="t">
        /// 插值参数在范围 [0, 1] 内<br></br>
        /// Interpolation parameter within range [0, 1]
        /// </param>
        public static Rotation SLERP(Rotation r1, Rotation r2, double t)
        {
            return new Rotation(Quaternion.SLERP(r1.ToQuaternion, r2.ToQuaternion, t));
        }

        /// <summary>
        /// 合并两次旋转。<br></br>
        /// Combine two rotations.
        /// </summary>
        public Rotation Mult(Rotation r)
        {
            Matrix3d m = this.ToRotationMatrix * r.ConvertTo(this.Coord).ToRotationMatrix;
            return new Rotation(m, this.Coord);
        }

        /// <summary>
        /// 将旋转矩阵乘以向量。<br></br>
        /// Multiply rotation matrix by vector.
        /// <para>
        /// 首先将旋转矩阵变换到矢量的参考坐标系中。<br></br>
        /// The rotation matrix is first transformed into reference coordinate system of vector.
        /// </para>
        /// </summary>
        public Vector3d Mult(Vector3d v)
        {
            return this.ConvertTo(v.Coord).ToRotationMatrix * v;
        }

        /// <summary>
        /// 将旋转矩阵与点相乘。<br></br>
        /// Multiply rotation matrix by point.
        /// <para>
        /// 首先将旋转矩阵变换到点的参考坐标系中。<br></br>
        /// The rotation matrix is first transformed into reference coordinate system of point.
        /// </para>
        /// </summary>
        public Point3d Mult(Point3d p)
        {
            return this.ConvertTo(p.Coord).ToRotationMatrix * p;
        }

        /// <summary>
        /// 将旋转对象转换至全局坐标系。<br></br>
        /// Convert rotation object to global coordinate system.
        /// </summary>
        public Rotation ConvertToGlobal()
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
                return new Rotation(axis, angle);
            }
        }

        /// <summary>
        /// 将旋转对象转换为参考坐标系。<br></br>
        /// Convert rotation object to reference coordinate system.
        /// </summary>
        public Rotation ConvertTo(Coord3d coord)
        {
            if (this._coord == coord)
            {
                return this.Copy();
            } else
            {
                Vector3d axis = this.ToAxis;
                double angle = this.ToAngle;
                axis = axis.ConvertTo(coord);
                return new Rotation(axis, angle);
            }
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
            Rotation r = (Rotation)obj;
            if (GeometRi3D.UseAbsoluteTolerance)
            {
                return (this.ToRotationMatrix - r.ConvertTo(this.Coord).ToRotationMatrix).MaxNorm < GeometRi3D.Tolerance;
            }
            else
            {
                return (this.ToRotationMatrix - r.ConvertTo(this.Coord).ToRotationMatrix).MaxNorm / this.ToRotationMatrix.MaxNorm < GeometRi3D.Tolerance;
            }

        }

        /// <summary>
        /// 返回对象的哈希码。<br></br>
        /// Returns the hashcode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_r.Row1.GetHashCode(), 
                                           _r.Row2.GetHashCode(), 
                                           _r.Row3.GetHashCode());
        }

        /// <summary>
        /// 全局坐标系中对象的字符串表示。<br></br>
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
            Rotation r = this.ConvertTo(coord);
            string str = "Rotation (reference coord.sys. " + coord.Name + "):" + nl;
            str += string.Format("Row1 -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", r[0, 0], r[0, 1], r[0, 2]) + nl;
            str += string.Format("Row2 -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", r[1, 0], r[1, 1], r[1, 2]) + nl;
            str += string.Format("Row3 -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", r[2, 0], r[2, 1], r[2, 2]);
            return str;
        }

        // Operators overloads
        //-----------------------------------------------------------------
        public static bool operator ==(Rotation m1, Rotation m2)
        {
            if (object.ReferenceEquals(m1, null))
                return object.ReferenceEquals(m2, null);
            return m1.Equals(m2);
        }
        public static bool operator !=(Rotation m1, Rotation m2)
        {
            if (object.ReferenceEquals(m1, null))
                return !object.ReferenceEquals(m2, null);
            return !m1.Equals(m2);
        }

        /// <summary>
        /// Combine two rotations.
        /// </summary>
        public static Rotation operator *(Rotation r1, Rotation r2)
        {
            return r1.Mult(r2);
        }

        /// <summary>
        /// Multiply rotation matrix by vector.
        /// <para>The rotation matrix is first transformed into reference coordinate system of vector.</para>
        /// </summary>
        public static Vector3d operator *(Rotation r, Vector3d v)
        {
            return r.Mult(v);
        }

        /// <summary>
        /// Multiply rotation matrix by point.
        /// <para>The rotation matrix is first transformed into reference coordinate system of point.</para>
        /// </summary>
        public static Point3d operator *(Rotation r, Point3d p)
        {
            return r.Mult(p);
        }
    }
}
