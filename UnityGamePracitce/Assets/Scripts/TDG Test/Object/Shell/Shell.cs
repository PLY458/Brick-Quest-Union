using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game {

        
        public class Shell : WarEntity
    {
        Vector3 launchPoint, targetPoint, launchVelocity;

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
            Vector3 p = launchPoint + launchVelocity * age;
            p.y -= 0.5f * 9.81f * age * age;
            //保证炮弹落地后消失
            if (p.y <= 0f)
            {
                GameMgr.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
                OriginFactory.Reclaim(this);
                return false;
            }
            transform.localPosition = p;

            //炮弹的旋转位移
            Vector3 d = launchVelocity;
            d.y -= 9.81f * age;
            transform.localRotation = Quaternion.LookRotation(d);

            return true;
        }

    }

}

