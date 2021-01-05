using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace TDG_game {

    public class MortarTower : Tower
    {
        [SerializeField, Range(0.5f, 3f)]
        float shellBlastRadius = 1f;

        [SerializeField, Range(1f, 100f)]
        float shellDamage = 10f;

        [SerializeField, Range(0.5f, 2f)]
        float shotsPerSecond = 1f;

        [SerializeField]
        Transform mortar = default;

        public override TowerType TowerType => TowerType.Mortar;

        float launchSpeed;

        float launchProgress;

        void Awake()
        {
            OnValidate();
        }

        void OnValidate()
        {
            //控制炮弹发射速度在可以显示的范围
            float x = targetingRange + 0.25001f;
            float y = -mortar.position.y;
            launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
        }

        public override void ContentUpdate()
        {
            launchProgress += shotsPerSecond * Time.deltaTime;
            while (launchProgress >= 1f)
            {
                if (AcquireTarget(out TargetPoint target))
                {
                    Launch(target);
                    launchProgress -= 1f;
                }
                else
                {
                    launchProgress = 0.999f;
                }
            }
        }

        public void Launch(TargetPoint target)
        {
            Vector3 launchPoint = mortar.position;
            Vector3 targetPoint = target.Position;
            targetPoint.y = 0f;

            #region 水平瞄准距离计算
            Vector2 dir;
            dir.x = targetPoint.x - launchPoint.x;
            dir.y = targetPoint.z - launchPoint.z;
            float x = dir.magnitude;
            float y = -launchPoint.y;
            dir /= x;
            #endregion

            #region 抛射和旋转角度计算
            float g = 9.81f;
            float s = launchSpeed;
            float s2 = s * s;

            float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
            Debug.Assert(r >= 0f, "Launch velocity insufficient for range!");
            float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
            float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
            float sinTheta = cosTheta * tanTheta;

            mortar.localRotation =
                    Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

            GameMgr.SpawnShell().Initialize(
                launchPoint, targetPoint,
                new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y),
                shellBlastRadius, shellDamage
            );

            #endregion


        }
    }

}

