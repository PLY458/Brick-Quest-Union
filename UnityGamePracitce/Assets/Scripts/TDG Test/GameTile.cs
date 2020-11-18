using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG_game {

    public class GameTile : MonoBehaviour
    {
        [SerializeField]
        Transform arrow = default;

        GameTile north, east, south, west, nextOnPath;

        static Quaternion
                northRotation = Quaternion.Euler(0f, 0f, 0f),
                eastRotation = Quaternion.Euler(0f, 90f, 0f),
                southRotation = Quaternion.Euler(0f, 180f, 0f),
                westRotation = Quaternion.Euler(0f, 270f, 0f);

        //从该点出发到达终点所需要经过的寻路点数量
        int distance;

        #region 寻路模式处理
        
        //简易属性：判断是否开始查找寻路模式
        public bool HasPath => distance != int.MaxValue;

        public void ClearPath()
        {
            distance = int.MaxValue;
            nextOnPath = null;
        }

        GameTile GrowPathTo(GameTile neighbor)
        {
            Debug.Assert(HasPath, "No path!");
            if (neighbor == null || neighbor.HasPath)
            {
                return null;
            }
            neighbor.distance = distance + 1;
            neighbor.nextOnPath = this;
            return neighbor;
        }

        public GameTile GrowPathNorth() => GrowPathTo(north);

        public GameTile GrowPathEast() => GrowPathTo(east);

        public GameTile GrowPathSouth() => GrowPathTo(south);

        public GameTile GrowPathWest() => GrowPathTo(west);

        public void ShowPath()
        {
            if (distance == 0)
            {
                arrow.gameObject.SetActive(false);
                return;
            }
            arrow.gameObject.SetActive(true);
            arrow.localRotation =
                nextOnPath == north ? northRotation :
                nextOnPath == east ? eastRotation :
                nextOnPath == south ? southRotation :
                westRotation;
        }


        public void BecomeDestination()
        {
            distance = 0;
            nextOnPath = null;
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
    }

}


