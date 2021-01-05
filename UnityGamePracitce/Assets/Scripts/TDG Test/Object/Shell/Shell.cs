using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game {

        
        public class Shell : WarEntity
    {
        Vector3 launchPoint, targetPoint, launchVelocity;

        /// <summary>
        /// 炮弹生命周期，爆炸范围，炮弹伤害
        /// </summary>
        float age, blastRadius, damage;

        public void Initialize(
            Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,
            float blastRadius, float damage
        )
        {
            this.launchPoint = launchPoint;
            this.targetPoint = targetPoint;
            this.launchVelocity = launchVelocity;
            this.blastRadius = blastRadius;
            this.damage = damage;
        }

        public override bool GameUpdate()
        {
            age += Time.deltaTime;

            //炮弹的位置移动
            Vector3 pos = launchPoint + launchVelocity * age;
            pos.y -= 0.5f * 9.81f * age * age;
            //保证炮弹落地后消失
            if (pos.y <= 0f)
            {
                GameMgr.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
                OriginFactory.Reclaim(this);
                return false;
            }
            transform.localPosition = pos;

            //炮弹的旋转位移
            Vector3 d = launchVelocity;
            d.y -= 9.81f * age;
            transform.localRotation = Quaternion.LookRotation(d);

            //小范围生成爆炸特效用于实现尾迹效果
            GameMgr.SpawnExplosion().Initialize(pos, 0.1f);
            return true;
        }

    }

}

