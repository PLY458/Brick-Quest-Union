using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game { 

    /// <summary>
    /// 炮台瞄准检测的目标点
    /// 附加在：敌人单位
    /// </summary>
    public class TargetPoint : MonoBehaviour
    {
        public Enemy Enemy { get; private set; }

        public Vector3 Position => transform.position;

        /// <summary>
        /// 敌人层级遮罩
        /// </summary>
        const int enemyLayerMask = 1 << 9;

        /// <summary>
        /// 射线检测到的敌人缓冲集
        /// </summary>
        static Collider[] buffer = new Collider[100];

        /// <summary>
        /// 缓冲集中的敌人数目
        /// </summary>
        public static int BufferedCount { get; private set; }

        /// <summary>
        /// 随机抽取的一个敌人作为输出目标
        /// </summary>
        public static TargetPoint RandomBuffered =>
                    GetBuffered(Random.Range(0, BufferedCount));

        void Awake()
        {
            Enemy = transform.root.GetComponent<Enemy>();
            Debug.Assert(
                    Enemy != null, 
                    "Target point without Enemy root!", this);
            Debug.Assert(
                    GetComponent<SphereCollider>() != null,
                    "Target point without sphere collider!", this);
            Debug.Assert(
                gameObject.layer == 9, 
                 "Target point on wrong layer!", this);
        }

        /// <summary>
        /// 填充敌人缓冲，即检测指定范围的敌人集合
        /// </summary>
        public static bool FillBuffer(Vector3 position, float range)
        {
            Vector3 top = position;
            top.y += 3f;
            BufferedCount = Physics.OverlapCapsuleNonAlloc(
                position, top, range, buffer, enemyLayerMask
            );
            return BufferedCount > 0;
        }

        /// <summary>
        /// 根据序列号提取缓冲集的敌人
        /// </summary>
        public static TargetPoint GetBuffered(int index)
        {
            var target = buffer[index].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
            return target;
        }
    }


}