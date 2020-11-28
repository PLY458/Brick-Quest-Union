using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG_game {

    /// <summary>
    /// 游戏地图中的寻路点瓦片
    /// </summary>
    public class GameTile : MonoBehaviour
    {
        [SerializeField]
        Transform arrow = default;

        GameTile north, east, south, west, nextOnPath;

        /// <summary>
        /// 瓦片指向的运动方向
        /// </summary>
        public Direction PathDirection { get; private set; }

        //两个瓦片之间路径的转换点
        public Vector3 ExitPoint { get; private set; }

        static Quaternion
                northRotation = Quaternion.Euler(0f, 180f, 0f),
                eastRotation = Quaternion.Euler(0f, 0f, 270f),
                southRotation = Quaternion.Euler(0f, 0f, 0f),
                westRotation = Quaternion.Euler(0f, 0f, 90f);

        //从该点出发到达终点所需要经过的寻路点数量
        int distance;

        #region 寻路模式处理

        //简易属性：获取该寻路点路径的下一个寻路点
        public GameTile NextTileOnPath => nextOnPath;
        //简易属性：判断是否被已有路线占据
        public bool HasPath => distance != int.MaxValue;
        //判断是需要被反转寻路方向优先级
        public bool IsAlternative { get; set; }

        /// <summary>
        /// 清除该寻路点在最短路径中存在的位置
        /// </summary>
        public void ClearPath()
        {
            distance = int.MaxValue;
            nextOnPath = null;
        }

        /// <summary>
        /// 与邻近寻路点开始连接成路线
        /// </summary>
        GameTile GrowPathTo(GameTile neighbor, Direction direction)
        {
            Debug.Assert(HasPath, "No path!");
            if (neighbor == null || neighbor.HasPath)
            {
                return null;
            }
            neighbor.distance = distance + 1;
            neighbor.nextOnPath = this;
            //在路径上相邻寻路点的直线上寻找中心值
            neighbor.ExitPoint =
                    neighbor.transform.localPosition + direction.GetHalfVector();

            neighbor.PathDirection = direction;
            return
                neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
        }

        public GameTile GrowPathNorth() => GrowPathTo(north, Direction.North);

        public GameTile GrowPathEast() => GrowPathTo(east, Direction.East);

        public GameTile GrowPathSouth() => GrowPathTo(south,Direction.South);

        public GameTile GrowPathWest() => GrowPathTo(west,Direction.West);

        /// <summary>
        /// 路线生成完毕，调整箭头指向路线，终点箭头需要隐藏
        /// </summary>
        public void ShowPath()
        {
            if (distance == 0)
            {
                HidePath();
                return;
            }
            arrow.gameObject.SetActive(true);
            arrow.localRotation =
                nextOnPath == north ? northRotation :
                nextOnPath == east ? eastRotation :
                nextOnPath == south ? southRotation :
                westRotation;
        }

        public void HidePath()
        {
            arrow.gameObject.SetActive(false);
        }
        /// <summary>
        /// 该寻路点转换为目的地
        /// </summary>
        public void BecomeDestination()
        {
            distance = 0;
            nextOnPath = null;
            ExitPoint = transform.localPosition;
        }

        #endregion

        #region 路径点关联处理
        //作为通用静态方法使用，类似Vector中的Distance
        /// <summary>
        /// 为相邻路径点创造横向关联
        /// </summary>
        public static void MakeEastWestNeighbors(GameTile east, GameTile west)
        {
            //一旦两方已经有一方与其他路径点建立横向关系，则选择报错
            //Assertb不会影响到最后的Release，所以可以在开发中任意使用
            Debug.Assert(
                    west.east == null && east.west == null, "Redefined neighbors!"
            );
            west.east = east;
            east.west = west;
        }

        /// <summary>
        ///  为相邻路径点创造纵向关联
        /// </summary>
        public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
        {
            Debug.Assert(
                south.north == null && north.south == null, "Redefined neighbors!"
            );
            south.north = north;
            north.south = south;
        }

        #endregion

        GameTileContent content;

        /// <summary>
        /// 当前寻路点的标记属性
        /// </summary>
        public GameTileContent Content
        {
            get => content;
            set
            {
                Debug.Assert(value != null, "Null assigned to content!");
                if (content != null)
                {
                    content.Recycle();
                }
                content = value;
                content.transform.localPosition = transform.localPosition;
            }
        }


    }

}


