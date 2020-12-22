using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDG_game
{
        public class GameMgr : MonoBehaviour
    {
        /// <summary>
        /// 塔防地图的尺寸
        /// </summary>
        [SerializeField]
        Vector2Int boardSize = new Vector2Int(11, 11);

        [SerializeField]
        GameScenario scenario = default;

        TowerType selectedTowerType;

        /// <summary>
        /// 游戏场景状态
        /// </summary>
        GameScenario.State activeScenario;

        /// <summary>
        /// 定位导航点转换位置的摄像机射线
        /// </summary>
        Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        #region 游戏场景中调用的游戏物体
        [SerializeField]
        NaviBoard board = default;

        [SerializeField]
        NaviTileContentFactory tileContentFactory = default;

        [SerializeField]
        WarFactory warFactory = default;

        /// <summary>
        /// 初始玩家生命
        /// </summary>
        [SerializeField, Range(0, 100)]
        int startingPlayerHealth = 10;

        [SerializeField, Range(1f, 10f)]
        float playSpeed = 1f;

        int playerHealth;

        const float pausedTimeScale = 0f;

        GameBehaviorCollection enemies = new GameBehaviorCollection();
        GameBehaviorCollection warEntities = new GameBehaviorCollection();


        /// <summary>
        /// 游戏管理器单例模式
        /// </summary>
        static GameMgr instance;
        #endregion

        #region 游戏生命周期

        private void OnEnable()
        {
            instance = this;
        }

        void Awake()
        {
            playerHealth = startingPlayerHealth;
            board.Initialize(boardSize,tileContentFactory);
            board.ShowPaths = !board.ShowPaths;//测试调换
            activeScenario = scenario.Begin();
        }

        /// <summary>
        /// 限定地图尺寸的大小 
        /// - 知识点待整备
        /// </summary>
        void OnValidate()
        {
            if (boardSize.x < 2)
            {
                boardSize.x = 2;
            }
            if (boardSize.y < 2)
            {
                boardSize.y = 2;
            }
        }

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleAlternativeTouch();
            }

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    board.ShowPaths = !board.ShowPaths;
            //}

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedTowerType = TowerType.Laser;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedTowerType = TowerType.Mortar;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale =
                    Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
            }
            else if (Time.timeScale > pausedTimeScale)
            {
                Time.timeScale = playSpeed;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                BeginNewGame();
            }

            if (playerHealth <= 0)
            {
                Debug.Log("Defeat!");
                BeginNewGame();
            }

            if (!activeScenario.Progress() && enemies.IsEmpty)
            {
                Debug.Log("Victory!");
                BeginNewGame();
                activeScenario.Progress();
            }

            activeScenario.Progress();

            enemies.CollectionUpdate();
            //强制更新场景中的物理组件信息，这期间会对物理检测进行屏蔽
            Physics.SyncTransforms();
            board.BoardUpdate();
            warEntities.CollectionUpdate();
        }

        #endregion

        #region 游戏操作方法
        /// <summary>
        /// 鼠标左键操作方法
        /// </summary>
        void HandleAlternativeTouch()
        {
            NaviTile tile = board.GetNaviTile(TouchRay);
            if (tile != null) {
                if (Input.GetKey(KeyCode.A))
                {
                    board.ToggleDestination(tile);
                }
                else
                {
                    board.ToggleSpawnPoint(tile);
                }
            }
        }

        /// <summary>
        /// 鼠标右键操作方法
        /// </summary>
        void HandleTouch()
        {
            NaviTile tile = board.GetNaviTile(TouchRay);
            if (tile != null)
            {
                //tile.Content =
                //tileContentFactory.GetContent(GameTileContentType.Destination);
                if (Input.GetKey(KeyCode.A))
                {
                    board.ToggleTower(tile, selectedTowerType);
                }
                else
                {
                    board.ToggleWall(tile);
                }
            }
        }

        #endregion

        /// <summary>
        /// 炮弹生成唤醒
        /// </summary>
        public static Shell SpawnShell()
        {
            Shell shell = instance.warFactory.Shell;
            instance.warEntities.Add(shell);
            return shell;
        }

        /// <summary>
        /// 敌人生成唤醒
        /// </summary>
        public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
        {
            //随机抽取场上的生成点
            NaviTile spawnPoint =
                 instance.board.GetSpawnPoint(
                     Random.Range(0, instance.board.SpawnPointCount));
            //生成敌人在抽到的生成点上
            Enemy enemy = factory.GetEnemy(type);
            enemy.SetSpownPoint(spawnPoint);

            instance.enemies.Add(enemy);
        }

        /// <summary>
        /// 爆炸特效生成唤醒
        /// </summary>
        /// <returns></returns>
        public static Explosion SpawnExplosion()
        {
            Explosion explosion = instance.warFactory.Explosion;
            instance.warEntities.Add(explosion);
            return explosion;
        }


        public static void EnemyReachedDestination()
        {
            instance.playerHealth -= 1;
        }

        /// <summary>
        /// 
        /// </summary>
        void BeginNewGame()
        {
            playerHealth = startingPlayerHealth;
            enemies.Clear();
            board.Clear();
            activeScenario = scenario.Begin();
        }
    }


}
