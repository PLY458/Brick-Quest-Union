using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace TDG_game {

    public enum TowerType
    {
        Laser, Mortar
    }


    public abstract class Tower : NaviTileContent
    {
        /// <summary>
        /// 防御塔目标搜索半径
        /// </summary>
        [SerializeField, Range(1.5f, 10.5f)]
        protected float targetingRange = 1.5f;

        /// <summary>
        /// 
        /// </summary>
        public abstract TowerType TowerType { get; }

        /// <summary>
        /// 检测和指定瞄准目标敌人
        /// </summary>
        protected bool AcquireTarget( out TargetPoint target)
        {
            if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
            {
                target = TargetPoint.RandomBuffered;
                return true;
            }
            target = null;
            return false;
        }

        /// <summary>
        /// 一次只追踪一个目标
        /// </summary>
        protected bool TrackTarget( ref TargetPoint target)
        {
            if (target == null)
            {
                return false;
            }
            Vector3 a = transform.localPosition;
            Vector3 b = target.Position;
            //勾股定理求解xz平面距离问题
            float x = a.x - b.x;
            float z = a.z - b.z;
            float r = targetingRange + 0.125f * target.Enemy.Scale;
            if (x * x + z * z > r * r)
            {
                target = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 辅助线绘制
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 position = transform.localPosition;
            position.y += 0.01f;
            Gizmos.DrawWireSphere(position, targetingRange);
        }



    }

}

