using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDG_game
{
    /// <summary>
    /// 导航点的类型区别
    /// </summary>
    public enum NaviTileContentType
    {
        Empty, Destination, Wall, SpawnPoint
    }


    /// <summary>
    /// 地图导航点生产工厂
    /// </summary>
    [CreateAssetMenu]
    public class NaviTileContentFactory : GameObjectFactory
    {
        #region 四种不同类型的导航点

        /// <summary>
        /// 敌人目标点
        /// </summary>
        [SerializeField]
        NaviTileContent destinationPrefab = default;

        /// <summary>
        /// 普通导航点
        /// </summary>
        [SerializeField]
        NaviTileContent emptyPrefab = default;

        /// <summary>
        /// 墙壁导航点
        /// </summary>
        [SerializeField]
        NaviTileContent wallPrefab = default;

        /// <summary>
        /// 敌人生成点
        /// </summary>
        [SerializeField]
        NaviTileContent spawnPointPrefab = default;

        #endregion
        /// <summary>
        /// 回收需要被撤下的导航点
        /// </summary>
        public void Reclaim(NaviTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }

        /// <summary>
        /// 根据传入的类型生产导航点
        /// </summary>
        public NaviTileContent GetContent(NaviTileContentType type)
        {
            switch (type)
            {
                case NaviTileContentType.Destination: return GetRealContent(destinationPrefab);
                case NaviTileContentType.Empty: return GetRealContent(emptyPrefab);
                case NaviTileContentType.Wall: return GetRealContent(wallPrefab);
                case NaviTileContentType.SpawnPoint: return GetRealContent(spawnPointPrefab);
            }
            Debug.Assert(false, "Unsupported type: " + type);
            return null;
        }

        /// <summary>
        /// 工厂内部实例化导航点
        /// </summary>
        NaviTileContent GetRealContent(NaviTileContent prefab)
        {
            NaviTileContent instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

    }


}
