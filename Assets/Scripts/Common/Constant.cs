using UnityEngine;

namespace vFramework.Common
{
    public class Constant
    {

        #region static variate
        /// <summary>
        /// 设置标准分辨率为1134 x 750
        /// </summary>
        public static readonly Vector2 defaultResolution = new Vector2(1134, 750);
        /// <summary>
        /// 设置标准尺寸为2200 * 1200
        /// </summary>
        public static readonly Vector2 size = new Vector2(2200, 1200);
        /// <summary>
        /// 设置UI根节点标准缩放值，GearVR的标准缩放值是0.00175
        /// </summary>
        public static readonly Vector3 defaultScale = new Vector3(0.00175f, 0.00175f, 0.00175f);
        #endregion
    }
}