using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game
{
    [CreateAssetMenu]
    public class EnemyFactory : GameObjectFactory
    {

        [SerializeField]
        Enemy prefab = default;

        /// <summary>
        /// 敌人生产的初始速度区间
        /// </summary>
        [SerializeField, FRSlider(0.2f, 5f)]
        FloatRange speed = new FloatRange(1f);

        /// <summary>
        /// 敌人生产的尺寸区间
        /// </summary>
        [SerializeField, FRSlider(0.5f, 2f)]
        FloatRange scale = new FloatRange(1f);

        /// <summary>
        /// 敌人生产的路线偏移区间
        /// </summary>
        [SerializeField, FRSlider(-0.4f, 0.4f)]
        FloatRange pathOffset = new FloatRange(0f);

        public Enemy GetEnemy()
        {
            Enemy instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            instance.Initialize(
                scale.RandomValueInRange,
                pathOffset.RandomValueInRange,
                speed.RandomValueInRange
                );
            return instance;
        }

        public void Reclaim(Enemy enemy)
        {
            Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(enemy.gameObject);
        }
    }
}


