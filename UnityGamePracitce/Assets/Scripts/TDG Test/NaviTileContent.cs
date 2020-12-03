using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game
{

    /// <summary>
    /// 寻路点类型上下文
    /// </summary>
    public class NaviTileContent : MonoBehaviour
    {

        [SerializeField]
        NaviTileContentType type = default;

        NaviTileContentFactory Factory;

        /// <summary>
        /// 导航点类型标识
        /// </summary>
        public NaviTileContentType ContentType => type;

        /// <summary>
        /// 定位生产导航点的原工厂
        /// </summary>
        public NaviTileContentFactory OriginFactory
        {
            get => Factory;
            set
            {
                Debug.Assert(Factory == null, "Redefined origin factory!");
                Factory = value;
            }
        }

        /// <summary>
        /// 呼叫源工厂将自己进行回收作业
        /// </summary>
        public void Recycle()
        {
            Factory.Reclaim(this);
        }
    }

}
