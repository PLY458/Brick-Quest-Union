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

        public Enemy GetEnemy()
        {
            Enemy instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

        public void Reclaim(Enemy enemy)
        {
            Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(enemy.gameObject);
        }
    }
}


