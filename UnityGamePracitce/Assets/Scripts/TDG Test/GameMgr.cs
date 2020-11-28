using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDG_game
{
        public class GameMgr : MonoBehaviour
    {
        [SerializeField]
        Vector2Int boardSize = new Vector2Int(11, 11);

        [SerializeField]
        GameBoard board = default;

        [SerializeField]
        GameTileContentFactory tileContentFactory = default;

        [SerializeField]
        EnemyFactory enemyFactory = default;

        [SerializeField, Range(0.1f, 10f)]
        float spawnSpeed = 1f;

        //敌人集合体管理器
        EnemyCollection enemies = new EnemyCollection();

        //敌人生成间隔计时器
        float spawnProgress;

        Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        void Awake()
        {
            board.Initialize(boardSize,tileContentFactory);
        }

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

            if (Input.GetKeyDown(KeyCode.V))
            {
                board.ShowPaths = !board.ShowPaths;
            }

            spawnProgress += spawnSpeed * Time.deltaTime;
            while (spawnProgress >= 1f)
            {
                spawnProgress -= 1f;
                SpawnEnemy();
            }

            enemies.CollectionUpdate();
        }

        /// <summary>
        /// 副位操作
        /// </summary>
        void HandleAlternativeTouch()
        {
            GameTile tile = board.GetTile(TouchRay);
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
        /// 主位操作
        /// </summary>
        void HandleTouch()
        {
            GameTile tile = board.GetTile(TouchRay);
            if (tile != null)
            {
                //tile.Content =
                    //tileContentFactory.GetContent(GameTileContentType.Destination);
                board.ToggleWall(tile);
            }
        }

        void SpawnEnemy()
        {
            GameTile spawnPoint =
                board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
            Enemy enemy = enemyFactory.GetEnemy();
            enemy.SpawnOn(spawnPoint);

            enemies.Add(enemy);
        }
    }


}
