using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game
{
        /// <summary>
        /// 战斗物件生产工厂
        /// </summary>
        [CreateAssetMenu]
        public class WarFactory : GameObjectFactory
        {
                /// <summary>
                /// 爆炸特效
                /// </summary>
                [SerializeField]
                Explosion explosionPrefab = default;

                /// <summary>
                /// 炮弹
                /// </summary>
                [SerializeField]
                Shell shellPrefab = default;
                 
                public Explosion Explosion => GeEntity(explosionPrefab);

                public Shell Shell => GeEntity(shellPrefab);

                T GeEntity<T>(T prefab) where T : WarEntity
                {
                    T instance = CreateGameObjectInstance(prefab);
                    instance.OriginFactory = this;
                    return instance;
                }

                public void Reclaim(WarEntity entity)
                {
                    Debug.Assert(entity.OriginFactory == this, "Wrong factory reclaimed!");
                    Destroy(entity.gameObject);
                }

    }

}


