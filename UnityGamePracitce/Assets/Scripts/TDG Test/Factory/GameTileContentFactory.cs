using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDG_game
{
    [CreateAssetMenu]
    public class GameTileContentFactory : GameObjectFactory
    {

        [SerializeField]
        GameTileContent destinationPrefab = default;

        [SerializeField]
        GameTileContent emptyPrefab = default;

        [SerializeField]
        GameTileContent wallPrefab = default;

        [SerializeField]
        GameTileContent spawnPointPrefab = default;

        /// <summary>
        /// 销毁需要被销毁的寻路点状态
        /// </summary>
        public void Reclaim(GameTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }

        /// <summary>
        /// 根据传入的类型生产寻路点状态
        /// </summary>
        public GameTileContent GetContent(GameTileContentType type)
        {
            switch (type)
            {
                case GameTileContentType.Destination: return GetRealContent(destinationPrefab);
                case GameTileContentType.Empty: return GetRealContent(emptyPrefab);
                case GameTileContentType.Wall: return GetRealContent(wallPrefab);
                case GameTileContentType.SpawnPoint: return GetRealContent(spawnPointPrefab);
            }
            Debug.Assert(false, "Unsupported type: " + type);
            return null;
        }

        GameTileContent GetRealContent(GameTileContent prefab)
        {
            GameTileContent instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

    }
}
