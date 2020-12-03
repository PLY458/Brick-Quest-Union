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

        /// <summary>
        /// 遍历唤醒集合中的敌人的行动更新
        /// </summary>
        public void CollectionUpdate() {
            for (int i = 0; i < enemies.Count; i++)
            {
                //如果集合中有敌人行为更新失败，则调度末尾的单位到该敌人位置上来
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

