using UnityEngine;

namespace TDG_game {

    /// <summary>
    /// 不同种敌人的每一波进攻的定义
    /// </summary>
    [System.Serializable]
    public class EnemySpawnSequence
    {

        [SerializeField]
        EnemyFactory factory = default;

        [SerializeField]
        EnemyType type = EnemyType.Medium;

        [SerializeField, Range(1, 100)]
        int amount = 1;

        [SerializeField, Range(0.1f, 10f)]
        float cooldown = 1f;

        public State Begin() => new State(this);

        /// <summary>
        /// 一波进攻需要被记录的状态和进攻的引用
        /// (设置成结构来避免内存分配)
        /// </summary>
        [System.Serializable]
        public struct State
        {
            /// <summary>
            /// 一共发动的进攻次数记录
            /// </summary>
            int count;

            /// <summary>
            /// 进攻间隔的冷却时间记录
            /// </summary>
            float cooldown;

            EnemySpawnSequence sequence;

            public State(EnemySpawnSequence sequence)
            {
                this.sequence = sequence;
                count = 0;
                cooldown = sequence.cooldown;
            }

            /// <summary>
            /// 始终记录所有进攻波的进程
            /// </summary>
            public float Progress(float deltaTime)
            {
                cooldown += deltaTime;
                while (cooldown >= sequence.cooldown)
                {
                    cooldown -= sequence.cooldown;
                    if (count >= sequence.amount)
                    {
                        //进程需在进攻波完成敌人生成后，返回剩余冷却时间
                        return cooldown;
                    }
                    count += 1;
                    GameMgr.SpawnEnemy(sequence.factory, sequence.type);
                }
                return -1f;
            }
        }
    }
}


