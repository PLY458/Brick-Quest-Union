using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG_game {

    /// <summary>
    /// 游戏地图中的寻路点瓦片
    /// </summary>
    public class NaviTile : MonoBehaviour
    {
        /// <summary>
        /// 依赖的箭头方位
        /// </summary>
        [SerializeField]
        Transform arrow = default;

        /// <summary>
        /// 搜索时记录的各个方向的导航点
        /// </summary>
        NaviTile north, east, south, west, nextOnTile;

        /// <summary>
        /// 获取路径上下一个导航点
        /// </summary>
        public NaviTile NextTileOnPath => nextOnTile;
        
        /// <summary>
        /// 瓦片指向的运动方向
        /// </summary>
        public Direction PathDirection { get; private set; }

        /// <summary>
        /// 导航点之间的互通管道点-取两导航点的边界中点
        /// </summary>
        public Vector3 ChannelPoint { get; private set; }

        /// <summary>
        /// 箭头做正确显示需要的转向欧拉角
        /// </summary>
        static Quaternion
                northRotation = Quaternion.Euler(90f, 0f, 0f),
                eastRotation = Quaternion.Euler(90f, 90f, 0f),
                southRotation = Quaternion.Euler(90f, 180f, 0f),
                westRotation = Quaternion.Euler(90f, 270f, 0f);

        /// <summary>
        ///  记录该导航点到目标点的距离，为0则本身为目标点
        /// </summary>
        int distance;


        NaviTileContent content;

        /// <summary>
        /// 当前导航点的类型属性
        /// </summary>
        public NaviTileContent Content
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
        
        /// <summary>
        /// 判断该导航点是否已在路径当中
        /// </summary>
        public bool HasPath => distance != int.MaxValue;
        
        /// <summary>
        /// 判断是否反转寻路方向优先级
        /// </summary>
        public bool IsAlternative { get; set; }
        
        
        #region 寻路模式处理

        /// <summary>
        /// 为导航点设置初始属性
        /// </summary>
        public void InitPath()
        {
            distance = int.MaxValue;
            nextOnTile = null;
        }

        
        /// <summary>
        /// 与指定方向上的导航点连接形成路径
        /// </summary>
        /// <returns>被连接的导航点</returns>
        NaviTile GrowPathTo(NaviTile neighbor, Direction direction)
        {
            Debug.Assert(HasPath, "No path!");
            if (neighbor == null || neighbor.HasPath)
            {
                return null;
            }
            neighbor.distance = distance + 1;
            neighbor.nextOnTile = this;
            //在路径上相邻寻路点的直线上寻找中心值
            neighbor.ChannelPoint =
                    neighbor.transform.localPosition + direction.GetHalfVector();

            neighbor.PathDirection = direction;
            return
                neighbor.Content.ContentType != NaviTileContentType.Wall ? neighbor : null;
        }

        /// <summary>
        /// 连接北边
        /// </summary>
        public NaviTile GrowPathNorth() => GrowPathTo(north, Direction.South);

        /// <summary>
        /// 连接东边
        /// </summary>
        public NaviTile GrowPathEast() => GrowPathTo(east, Direction.West);

        /// <summary>
        /// 连接南边
        /// </summary>
        public NaviTile GrowPathSouth() => GrowPathTo(south,Direction.North);

        /// <summary>
        /// 连接西边
        /// </summary>
        public NaviTile GrowPathWest() => GrowPathTo(west,Direction.East);

        /// <summary>
        /// 根据导航点的类型，显示相应的箭头或是其他内容
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
                nextOnTile == north ? northRotation :
                nextOnTile == east ? eastRotation :
                nextOnTile == south ? southRotation :
                westRotation;
        }

        /// <summary>
        /// 隐藏导航点的箭头
        /// </summary>
        public void HidePath()
        {
            arrow.gameObject.SetActive(false);
        }

        /// <summary>
        /// 为目标点设置初始属性
        /// </summary>
        public void BecomeDestination()
        {
            distance = 0;
            nextOnTile = null;
            ChannelPoint = transform.localPosition;
        }

        #endregion

        #region 路径点关联处理
        //作为通用静态方法使用，类似Vector中的Distance
        /// <summary>
        /// 为相邻导航点创造东西方向关联
        /// </summary>
        public static void MakeEastWestNeighbors(NaviTile east, NaviTile west)
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
        ///  为相邻导航点创造南北方向关联
        /// </summary>
        public static void MakeNorthSouthNeighbors(NaviTile north, NaviTile south)
        {
            Debug.Assert(
                south.north == null && north.south == null, "Redefined neighbors!"
            );
            south.north = north;
            north.south = south;
        }

        #endregion

    }

}


