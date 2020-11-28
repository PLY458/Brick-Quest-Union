using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game
{
        /// <summary>
        /// 寻路点类型上下文
        /// </summary>
        public class GameTileContent : MonoBehaviour
    {
        [SerializeField]
        GameTileContentType type = default;

        public GameTileContentType Type => type;

        GameTileContentFactory originFactory;

        

        public GameTileContentFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined origin factory!");
                originFactory = value;
            }
        }


        /// <summary>
        /// 呼叫源工厂将自己进行回收作业
        /// </summary>
        public void Recycle()
        {
            originFactory.Reclaim(this);
        }
    }

}
