using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG_game {

        public class NaviBoard : MonoBehaviour
    {
        #region 场景资源相关变量

        /// <summary>
        /// 建造原点
        /// </summary>
        [SerializeField]
        Transform buildOrigin = default;

        /// <summary>
        /// 搭建地图需要的预制体
        /// </summary>
        [SerializeField]
        NaviTile tilePrefab = default;

        /// <summary>
        /// 需要登记在案的导航点工厂
        /// </summary>
        NaviTileContentFactory NaviTileFactory;

        #endregion

        #region 寻路计算相关变量

        /// <summary>
        /// 场上登记的所有导航点的集合
        /// </summary>
        NaviTile[] tiles;

        /// <summary>
        /// 生成导航点的集合
        /// </summary>
        List<NaviTile> spawnPoints = new List<NaviTile>();

        /// <summary>
        /// 记录场上存在的生成点数量
        /// </summary>
        public int SpawnPointCount => spawnPoints.Count;

        /// <summary>
        /// 点集合的排布尺寸
        /// </summary>
        Vector2Int size_Board;

        /// <summary>
        /// 显示寻路路线
        /// </summary>
        bool showPaths;

        /// <summary>
        /// 储存和更新宽度优先搜索算法的最外围导航点
        /// </summary>
        Queue<NaviTile> searchFrontier = new Queue<NaviTile>();

        #endregion

        /// <summary>
        /// 初始化导航板主体
        /// </summary>
        /// <param name="size">生成尺寸</param>
        /// <param name="tileFactory">登记在案的导航点工厂</param>
        public void Initialize(Vector2Int size, NaviTileContentFactory tileFactory)
        {
            this.size_Board = size;
            this.NaviTileFactory = tileFactory;
            buildOrigin.localScale = new Vector3(size.x, size.y, 1f);

            //寻路点规则排布所需的偏离间隔值
            Vector2 offset = new Vector2(   
                (size.x - 1) * 0.5f, (size.y - 1) * 0.5f   );

            tiles = new NaviTile[size.x * size.y];

            for (int i = 0, y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++, i++)
                {
                    NaviTile tile = tiles[i] = Instantiate(tilePrefab);
                    //给添加到集合中的导航点定位
                    tile.transform.SetParent(transform, false);
                    tile.transform.localPosition = new Vector3(
                        x - offset.x, 0.01f, y - offset.y
                    );

                    //根据导航点的坐标位置，连接东西南北四周的导航点信息
                    if (x > 0)
                    {
                        NaviTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                    }
                    if (y > 0)
                    {
                        NaviTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                    }
                    //在x轴处于偶数位时，当前寻路点允许改变寻路方向优先序列(方向遍历优化)
                    tile.IsAlternative = (x & 1) == 0;
                    //当y轴处于偶数位，则取消优先改变权
                    if ((y & 1) == 0)
                    {
                        tile.IsAlternative = !tile.IsAlternative;
                    }
                    //寻路点默认设置为空目标点
                    tile.Content = tileFactory.GetContent(NaviTileContentType.Empty);
                }
            }

            //FindPaths();
            ToggleDestination(tiles[tiles.Length / 2]);
            ToggleSpawnPoint(tiles[0]);
        }

        /// <summary>
        /// 获取射线确定到的导航点点
        /// </summary>
        public NaviTile GetNaviTile(Ray ray)
        {
            //计算离射线坐标最近的导航点的坐标，根据坐标检索相应的导航点
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

        /// <summary>
        /// 获取指定序号的敌人生成点
        /// </summary>
        public NaviTile GetSpawnPoint(int index)
        {
            return spawnPoints[index];
        }

        /// <summary>
        /// 真正的寻路算法流程(BFS)
        /// </summary>
        bool FindPaths()
        {
            //
            foreach (NaviTile tile in tiles)
            {
                
                if (tile.Content.ContentType == NaviTileContentType.Destination)
                {
                    tile.BecomeDestination();
                    searchFrontier.Enqueue(tile);
                }
                else
                {
                    tile.InitPath();
                }
            }
            //从目的地出发开始遍历
            //tiles[tiles.Length / 2].BecomeDestination();
            //searchFrontier.Enqueue(tiles[tiles.Length / 2]);
            if (searchFrontier.Count == 0)
            {
                return false;
            }

            ///目标点的数量即是搜索边界点
            while (searchFrontier.Count > 0)
            {
                //从目标点逆向生成路径
                NaviTile tile = searchFrontier.Dequeue();
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

            //若场景中有一个导航点不参与路径的生成，则寻路结果失败
            foreach (NaviTile tile in tiles)
            {
                if (!tile.HasPath)
                {
                    return false;
                }
            }

            //激活显示所有的导航点
            if (showPaths) {
                foreach (NaviTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }

            return true;
        }

        #region 地图上物体的控制操作
        /// <summary>
        /// 激活或者关闭所有的导航点的箭头显示
        /// </summary>
        public bool ShowPaths
        {
            get => showPaths;
            set
            {
                showPaths = value;
                if (showPaths)
                {
                    foreach (NaviTile tile in tiles)
                    {
                        tile.ShowPath();
                    }
                }
                else
                {
                    foreach (NaviTile tile in tiles)
                    {
                        tile.HidePath();
                    }
                }
            }
        }

        /// <summary>
        /// 转换导航点类型为目标点
        /// </summary>
        public void ToggleDestination(NaviTile tile)
        {
            //产生和撤去目标点都需要重新计算路径，判断是否允许转化
            //反向操作则变回导航点
            if (tile.Content.ContentType == NaviTileContentType.Destination)
            {
                tile.Content = 
                    NaviTileFactory.GetContent(NaviTileContentType.Empty);
                //保持地图上出现至少一个目标点
                if (!FindPaths())
                {
                    tile.Content =
                        NaviTileFactory.GetContent(NaviTileContentType.Destination);
                    
                    FindPaths();
                }
            }
            else if (tile.Content.ContentType == NaviTileContentType.Empty)
            {
                tile.Content = 
                    NaviTileFactory.GetContent(NaviTileContentType.Destination);
                FindPaths();
            }
        }

        /// <summary>
        /// 转换导航点类型为墙
        /// </summary>
        public void ToggleWall(NaviTile tile)
        {
            //与目标点转换同理
            if (tile.Content.ContentType == NaviTileContentType.Wall)
            {
                tile.Content = NaviTileFactory.GetContent(NaviTileContentType.Empty);
                FindPaths();
            }
            else if (tile.Content.ContentType == NaviTileContentType.Empty)
            {
                tile.Content = NaviTileFactory.GetContent(NaviTileContentType.Wall);
                if (!FindPaths())
                {
                    //检测地图中存在的路线是否被切断，若是则不能转化成障碍物
                    tile.Content = NaviTileFactory.GetContent(NaviTileContentType.Empty);
                    FindPaths();
                }
            }
        }

        /// <summary>
        /// 转换类型为敌人生成点
        /// </summary>
	    public void ToggleSpawnPoint (NaviTile tile)
        {
		    if (tile.Content.ContentType == NaviTileContentType.SpawnPoint) {
                if (spawnPoints.Count > 1)
                {
                    spawnPoints.Remove(tile);
                    tile.Content = NaviTileFactory.GetContent(NaviTileContentType.Empty);
                }
		    }
		    else if (tile.Content.ContentType == NaviTileContentType.Empty) {

			    tile.Content = NaviTileFactory.GetContent(NaviTileContentType.SpawnPoint);
                spawnPoints.Add(tile);
            }
	    }
        #endregion
    }
}


