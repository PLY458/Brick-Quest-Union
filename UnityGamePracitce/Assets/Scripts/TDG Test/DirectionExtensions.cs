using System.Collections.Generic;
using UnityEngine;


namespace TDG_game
{
    /// <summary>
    /// 敌人单位面朝方向属性
    /// </summary>
    public enum Direction
    {
        North, East, South, West
    }

    /// <summary>
    /// 敌人单位旋转方向属性
    /// </summary>
    public enum DirectionTurnTo
    {
        None, TurnRight, TurnLeft, TurnAround
    }



    /// <summary>
    /// 游戏物体转向扩展类
    /// 扩展方法的特征：
    /// 1.实现扩展方法的类必须是静态类并且类的名称和实现扩展方法的类无关
    ///2.实现扩展方法的类方法必须是静态方法
    ///3.实现扩展方法的类方法的第一个参数必须是使用this关键字指明要实现扩展方法的类
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// 方向转换需要的欧拉角参数
        /// </summary>
        static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),//向右
        Quaternion.Euler(0f, 180f, 0f),//向后
        Quaternion.Euler(0f, 270f, 0f)//向左
        };

        /// <summary>
        /// 四个方向向量折半计算
        /// </summary>
        static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f//？
        };


        /// <summary>
        /// 敌人单位前进方向旋转到对应角度
        /// </summary>
        public static Quaternion GetRotation(this Direction direction)
        {
            return rotations[(int)direction];
        }

        /// <summary>
        /// 敌人单位旋转方向确认
        /// </summary>
        public static DirectionTurnTo GetDirectionChangeTo(
            this Direction current, Direction next)
        {
            //当前方向与下一个瓦片的方向相同，无需变化
            if (current == next)
            {
                return DirectionTurnTo.None;
            }//顺时针旋转判定
            else if (current + 1 == next || current - 3 == next)
            {
                return DirectionTurnTo.TurnRight;
            }//逆时针旋转判定
            else if (current - 1 == next || current + 3 == next)
            {
                return DirectionTurnTo.TurnLeft;
            }
            //直接调头旋转判断
            return DirectionTurnTo.TurnAround;
        }

        /// <summary>
        /// 得到敌人朝向所代表的角度
        /// </summary>
        public static float GetAngle(this Direction direction)
        {
            return (float)direction * 90f;
        }

        /// <summary>
        /// 得倒敌人转向需要的过渡位移向量
        /// </summary>
        public static Vector3 GetHalfVector(this Direction direction)
        {
            return halfVectors[(int)direction];
        }
    }

}

