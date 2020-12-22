using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDG_game
{
    /// <summary>
    /// 方块点的类型区别
    /// </summary>
    public enum NaviTileContentType
    {
        Empty, Destination, Wall, SpawnPoint, Tower
    }


    /// <summary>
    /// 地图方块点生产工厂
    /// </summary>
    [CreateAssetMenu]
    public class NaviTileContentFactory : GameObjectFactory
    {
        #region 不同类型的方块点

        /// <summary>
        /// 敌人目标点
        /// </summary>
        [SerializeField]
        NaviTileContent destinationPrefab = default;

        /// <summary>
        /// 普通方块点
        /// </summary>
        [SerializeField]
        NaviTileContent emptyPrefab = default;

        /// <summary>
        /// 墙壁方块点
        /// </summary>
        [SerializeField]
        NaviTileContent wallPrefab = default;

        /// <summary>
        /// 敌人生成点
        /// </summary>
        [SerializeField]
        NaviTileContent spawnPointPrefab = default;

        /// <summary>
        /// 防御塔点
        /// </summary>
        [SerializeField]
        Tower[] towerPrefabs = default;

        #endregion

        #region 生产方块点重载方法集

        public NaviTileContent GetContent(NaviTileContentType type)
        {
            switch (type)
            {
                case NaviTileContentType.Destination: return GetContent(destinationPrefab);
                case NaviTileContentType.Empty: return GetContent(emptyPrefab);
                case NaviTileContentType.Wall: return GetContent(wallPrefab);
                case NaviTileContentType.SpawnPoint: return GetContent(spawnPointPrefab);
            }
            Debug.Assert(false, "Unsupported non-tower type: " + type);
            return null;
        }

        public Tower GetContent(TowerType type)
        {
            Debug.Assert((int)type < towerPrefabs.Length, "Unsupported tower type!");
            Tower prefab = towerPrefabs[(int)type];
            Debug.Assert(type == prefab.TowerType, "Tower prefab at wrong index!");
            return GetContent(prefab);
        }

        T GetContent<T> (T prefab) where T : NaviTileContent
        {
            T instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }
        #endregion        /// <summary>
        
        /// 回收需要被撤下的方块点
        /// </summary>
        public void Reclaim(NaviTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }
    }


}
