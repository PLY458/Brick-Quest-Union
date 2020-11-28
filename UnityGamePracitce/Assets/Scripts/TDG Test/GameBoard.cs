using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG_game {

        public class GameBoard : MonoBehaviour
    {
        #region 场景资源相关变量
        [SerializeField]
        Transform ground = default;//地图地板形态信息

        [SerializeField]
        GameTile tilePrefab = default;//寻路点预制体

        //需要被记录在案的场景内的状态工厂
        GameTileContentFactory contentFactory;

        #endregion

        #region 寻路计算相关变量

        //记录在案的地图上的寻路点集合
        GameTile[] tiles;

        //记录在案的地图上的出兵点集合
        List<GameTile> spawnPoints = new List<GameTile>();

        //出兵点总数
        public int SpawnPointCount => spawnPoints.Count;

        //地图中寻路点的排布尺寸
        Vector2Int size_Board;

        bool showPaths;

        //存储行进路线的搜索队列
        Queue<GameTile> searchFrontier = new Queue<GameTile>();

        #endregion

        public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
        {
            this.size_Board = size;
            this.contentFactory = contentFactory;
            ground.localScale = new Vector3(size.x, size.y, 1f);

            //寻路点规则排布所需的偏离值
            Vector2 offset = new Vector2(   (size.x - 1) * 0.5f, (size.y - 1) * 0.5f   );


            tiles = new GameTile[size.x * size.y];

            for (int i = 0, y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++, i++)
                {
                    GameTile tile = tiles[i] = Instantiate(tilePrefab);
                    tile.transform.SetParent(transform, false);
                    tile.transform.localPosition = new Vector3(
                        x - offset.x, 0.01f, y - offset.y
                    );

                    if (x > 0)
                    {
                        GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                    }
                    if (y > 0)
                    {
                        GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                    }
                    //在x轴处于偶数位时，当前寻路点允许改变寻路方向优先序列(方向遍历优化)
                    tile.IsAlternative = (x & 1) == 0;
                    //当y轴处于偶数位，则取消优先改变权
                    if ((y & 1) == 0)
                    {
                        tile.IsAlternative = !tile.IsAlternative;
                    }
                    //寻路点默认设置为空目标点
                    tile.Content = contentFactory.GetContent(GameTileContentType.Empty);
                }
            }

            //FindPaths();
            ToggleDestination(tiles[tiles.Length / 2]);
            ToggleSpawnPoint(tiles[0]);
        }

        /// <summary>
        /// 返回射线确定到的寻路点
        /// </summary>
        public GameTile GetTile(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit) ){
                int x = (int)(hit.point.x + size_Board.x * 0.5f);
                int y = (int)(hit.point.z + size_Board.y * 0.5f);
                if (x >= 0 && x < size_Board.x && y >= 0 && y < size_Board.y)
                {
                    return tiles[x + y * size_Board.x];
                }
            }
            return null;
        }


        public GameTile GetSpawnPoint(int index)
        {
            return spawnPoints[index];
        }

        /// <summary>
        /// 真正的寻路算法流程(BFS)
        /// </summary>
        bool FindPaths()
        {
            foreach (GameTile tile in tiles)
            {
                //检查场景中的寻路点，确定作为终点的寻路点
                if (tile.Content.Type == GameTileContentType.Destination)
                {
                    tile.BecomeDestination();
                    searchFrontier.Enqueue(tile);
                }
                else
                {
                    tile.ClearPath();
                }
            }
            //从目的地出发开始遍历
            //tiles[tiles.Length / 2].BecomeDestination();
            //searchFrontier.Enqueue(tiles[tiles.Length / 2]);
            if (searchFrontier.Count == 0)
            {
                return false;
            }


            while (searchFrontier.Count > 0)
            {
                GameTile tile = searchFrontier.Dequeue();
                if (tile != null)
                {
                    //实现对角线搜索方向优先级变换
                    //存储可能的路线信息并令寻路点自行创建连接关系
                    if (tile.IsAlternative)
                    {
                        searchFrontier.Enqueue(tile.GrowPathNorth());
                        searchFrontier.Enqueue(tile.GrowPathSouth());
                        searchFrontier.Enqueue(tile.GrowPathEast());
                        searchFrontier.Enqueue(tile.GrowPathWest());
                    }
                    else
                    {
                        searchFrontier.Enqueue(tile.GrowPathWest());
                        searchFrontier.Enqueue(tile.GrowPathEast());
                        searchFrontier.Enqueue(tile.GrowPathSouth());
                        searchFrontier.Enqueue(tile.GrowPathNorth());
                    }
                }
            }

            //若场景中的其中一个寻路点没有参与到寻路中，则寻路失败
            foreach (GameTile tile in tiles)
            {
                if (!tile.HasPath)
                {
                    return false;
                }
            }

            if (showPaths) {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }

            return true;
        }

        #region 地图上物体的控制操作
        /// <summary>
        /// 对地图上路径方向的显示进行开关控制
        /// </summary>
        public bool ShowPaths
        {
            get => showPaths;
            set
            {
                showPaths = value;
                if (showPaths)
                {
                    foreach (GameTile tile in tiles)
                    {
                        tile.ShowPath();
                    }
                }
                else
                {
                    foreach (GameTile tile in tiles)
                    {
                        tile.HidePath();
                    }
                }
            }
        }

        /// <summary>
        /// 寻路点更换为目标点
        /// </summary>
        public void ToggleDestination(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.Content = 
                    contentFactory.GetContent(GameTileContentType.Empty);
                //保持地图上出现至少一个目标点
                if (!FindPaths())
                {
                    tile.Content =
                        contentFactory.GetContent(GameTileContentType.Destination);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = 
                    contentFactory.GetContent(GameTileContentType.Destination);
                FindPaths();
            }
        }

        /// <summary>
        /// 寻路点更换为障碍物
        /// </summary>
        public void ToggleWall(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Wall)
            {
                tile.Content = contentFactory.GetContent(GameTileContentType.Empty);
                FindPaths();
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = contentFactory.GetContent(GameTileContentType.Wall);
                if (!FindPaths())
                {
                    //检测地图中存在的路线是否被切断，若是则不能转化成障碍物
                    tile.Content = contentFactory.GetContent(GameTileContentType.Empty);
                    FindPaths();
                }
            }
        }

        /// <summary>
        /// 寻路点更换为敌人生成点
        /// </summary>
	    public void ToggleSpawnPoint (GameTile tile)
        {
		    if (tile.Content.Type == GameTileContentType.SpawnPoint) {
                if (spawnPoints.Count > 1)
                {
                    spawnPoints.Remove(tile);
                    tile.Content = contentFactory.GetContent(GameTileContentType.Empty);
                }
		    }
		    else if (tile.Content.Type == GameTileContentType.Empty) {
			    tile.Content = contentFactory.GetContent(GameTileContentType.SpawnPoint);
                spawnPoints.Add(tile);
            }
	    }
        #endregion
    }
}


