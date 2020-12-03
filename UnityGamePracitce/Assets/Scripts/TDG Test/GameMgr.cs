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

        /// <summary>
        /// 敌人的生成速度
        /// </summary>
        [SerializeField, Range(0.1f, 10f)]
        float spawnSpeed = 1f;

        /// <summary>
        /// 敌人生产间隔计时器
        /// </summary>
        float spawnProgress;

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
        EnemyFactory enemyFactory = default;

        EnemyCollection enemies = new EnemyCollection();
        #endregion

        #region 游戏生命周期
        void Awake()
        {
            board.Initialize(boardSize,tileContentFactory);
            board.ShowPaths = !board.ShowPaths;//测试调换
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

            spawnProgress += spawnSpeed * Time.deltaTime;
            while (spawnProgress >= 1f)
            {
                spawnProgress -= 1f;
                SpawnEnemy();
            }

            enemies.CollectionUpdate();
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
                if (Input.GetKey(KeyCode.LeftShift))
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
                board.ToggleWall(tile);
            }
        }

        #endregion

        /// <summary>
        /// 敌人生成唤醒
        /// </summary>
        void SpawnEnemy()
        {
            //随机抽取场上的生成点
            NaviTile spawnPoint =
                board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
            //生成敌人在抽到的生成点上
            Enemy enemy = enemyFactory.GetEnemy();
            enemy.SetSpownPoint(spawnPoint);

            enemies.Add(enemy);
        }
    }


}
