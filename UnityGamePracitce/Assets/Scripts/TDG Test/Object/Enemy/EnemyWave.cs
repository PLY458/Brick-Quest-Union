using UnityEngine;

namespace TDG_game {


    /// <summary>
    /// 敌人每一批次攻击定义
    /// </summary>
    [CreateAssetMenu]
    public class EnemyWave : ScriptableObject
    {

        [SerializeField]
        EnemySpawnSequence[] spawnSequences = {
            new EnemySpawnSequence()
        };

        public State Begin() => new State(this);

        [System.Serializable]
        public struct State
        {

            EnemyWave wave;

            int index;

            EnemySpawnSequence.State sequence;

            public State(EnemyWave wave)
            {
                this.wave = wave;
                index = 0;
                Debug.Assert(wave.spawnSequences.Length > 0, "Empty wave!");
                sequence = wave.spawnSequences[0].Begin();
            }

            public float Progress(float deltaTime)
            {
                //得到攻击波次返回的剩余冷却时间
                deltaTime = sequence.Progress(deltaTime);
                while (deltaTime >= 0f)
                {
                    //确保本批攻击未完成的情况下，准备下一个攻击波次
                    if (++index >= wave.spawnSequences.Length)
                    {
                        return deltaTime;
                    }
                    sequence = wave.spawnSequences[index].Begin();
                    deltaTime = sequence.Progress(deltaTime);
                }
                return -1f;
            }

        }


    }
}


