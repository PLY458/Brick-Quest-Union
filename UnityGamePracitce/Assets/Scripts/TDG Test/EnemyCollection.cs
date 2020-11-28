using System.Collections.Generic;

namespace TDG_game
{
    [System.Serializable]
    public class EnemyCollection
    {
        List<Enemy> enemies = new List<Enemy>();

        public void Add(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void CollectionUpdate() {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].EnemyUpdate())
                {
                    int lastIndex = enemies.Count - 1;
                    enemies[i] = enemies[lastIndex];
                    enemies.RemoveAt(lastIndex);
                    i -= 1;
                }
            }
        }
    }

}

