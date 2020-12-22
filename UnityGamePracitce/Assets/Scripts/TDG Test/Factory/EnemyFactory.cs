using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game
{
    public enum EnemyType
    {
        Small, Medium, Large
    }


    [CreateAssetMenu]
    public class EnemyFactory : GameObjectFactory
    {

        /// <summary>
        /// 敌人属性的配置信息表
        /// </summary>
        [System.Serializable]
        class EnemyConfig
        {

            public Enemy prefab = default;

            [FRSlider(0.5f, 2f)]
            public FloatRange scale = new FloatRange(1f);

            [FRSlider(0.2f, 5f)]
            public FloatRange speed = new FloatRange(1f);

            [FRSlider(-0.4f, 0.4f)]
            public FloatRange pathOffset = new FloatRange(0f);

            [FRSlider(10f, 1000f)]
            public FloatRange health = new FloatRange(100f);
        }

        /// <summary>
        /// 三种敌人配置，小，中，大型敌人
        /// </summary>
        [SerializeField]
        EnemyConfig small = default, medium = default, large = default;

        /// <summary>
        /// 获取敌人类型配置信息
        /// </summary>
        EnemyConfig GetConfig(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Small: return small;
                case EnemyType.Medium: return medium;
                case EnemyType.Large: return large;
            }
            Debug.Assert(false, "Unsupported enemy type!");
            return null;
        }


        public Enemy GetEnemy(EnemyType type = EnemyType.Medium)
        {
            EnemyConfig config = GetConfig(type);
            //考虑调用资源加载系统进行敌人生成位置管理
            Enemy instance = CreateGameObjectInstance(config.prefab);
            instance.OriginFactory = this;
            instance.Initialize(
                config.scale.RandomValueInRange,
                config.speed.RandomValueInRange,
                config.pathOffset.RandomValueInRange,
                config.health.RandomValueInRange
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


