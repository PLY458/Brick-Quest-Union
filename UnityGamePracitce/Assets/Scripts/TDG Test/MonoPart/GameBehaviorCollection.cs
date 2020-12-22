using System.Collections.Generic;

namespace TDG_game
{
    /// <summary>
    /// 游戏物体线程集合管理类
    /// TODO：负责集体更新游戏实例的线程执行方法
    /// </summary>
    [System.Serializable]
    public class GameBehaviorCollection
    {
        List<GameBehavior> behaviors = new List<GameBehavior>();

        public bool IsEmpty => behaviors.Count == 0;

        public void Add(GameBehavior behavior)
        {
            behaviors.Add(behavior);
        }

        /// <summary>
        /// 线程集合执行
        /// </summary>
        public void CollectionUpdate() {
            for (int i = 0; i < behaviors.Count; i++)
            {
                //如果集合中有线程执行失败，调度集合末尾的线程替换该线程
                if (!behaviors[i].GameUpdate())
                {
                    int lastIndex = behaviors.Count - 1;
                    behaviors[i] = behaviors[lastIndex];
                    behaviors.RemoveAt(lastIndex);
                    i -= 1;
                }
            }
        }

        /// <summary>
        /// 清除/回收集合中的所有线程
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                behaviors[i].Recycle();
            }
            behaviors.Clear();
        }
    }

}

