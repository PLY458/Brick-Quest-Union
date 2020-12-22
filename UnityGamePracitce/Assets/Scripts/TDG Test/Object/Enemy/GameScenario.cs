using UnityEngine;

namespace TDG_game {

    /// <summary>
    /// 一整场战斗设定
    /// </summary>
    [CreateAssetMenu]
    public class GameScenario : ScriptableObject
    {

        [SerializeField]
        EnemyWave[] waves = { };

        /// <summary>
        /// 战斗循环次数
        /// </summary>
        [SerializeField, Range(0, 10)]
        int cycles = 1;

        /// <summary>
        /// 战斗循环速度
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        float cycleSpeedUp = 0.5f;

        public State Begin() => new State(this);


        [System.Serializable]
        public struct State
        {

            GameScenario scenario;

            int cycle, index;

            float timeScale;

            EnemyWave.State wave;

            public State(GameScenario scenario)
            {
                this.scenario = scenario;
                cycle = 0;
                index = 0;
                timeScale = 1f;
                Debug.Assert(scenario.waves.Length > 0, "Empty scenario!");
                wave = scenario.waves[0].Begin();
            }

            public bool Progress()
            {
                float deltaTime = wave.Progress(timeScale * Time.deltaTime);
                while (deltaTime >= 0f)
                {
                    if (++index >= scenario.waves.Length)
                    {
                        if (++cycle >= scenario.cycles && scenario.cycles > 0)
                        {
                            return false;
                        }
                        index = 0;
                        //攻击释放间隔会不断缩小
                        timeScale += scenario.cycleSpeedUp;
                    }
                    wave = scenario.waves[index].Begin();
                    deltaTime = wave.Progress(deltaTime);
                }
                return true;
            }
        }

    }


}



