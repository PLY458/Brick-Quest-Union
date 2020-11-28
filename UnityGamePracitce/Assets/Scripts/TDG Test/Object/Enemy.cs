using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game {

        public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// 需要被控制的敌人单位模型
        /// </summary>
        [SerializeField]
        Transform model = default;

        //用于计算瓦片间行进路线的参数
        GameTile tileFrom, tileTo;
        Vector3 positionFrom, positionTo;
        float progress, progressFactor;

        //用于计算敌人单位的旋转表现参数
        Direction direction;
        DirectionTurnTo directionChange;
        float directionAngleFrom, directionAngleTo;

        EnemyFactory originFactory;

            public EnemyFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined origin factory!");
                originFactory = value;
            }
        }

        /// <summary>
        /// 定位在出生点出生
        /// </summary>
        public void SpawnOn(GameTile tile)
        {
            //transform.localPosition = tile.transform.localPosition;
            Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
            tileFrom = tile;
            tileTo = tile.NextTileOnPath;

            progress = 0f;
            PrepareIntro();
        }

        /// <summary>
        /// 出生时期的单位信息配置
        /// </summary>
        void PrepareIntro()
        {
            positionFrom = tileFrom.transform.localPosition;
            positionTo = tileFrom.ExitPoint;

            direction = tileFrom.PathDirection;
            directionChange = DirectionTurnTo.None;
            directionAngleFrom = directionAngleTo = direction.GetAngle();
            transform.localRotation = direction.GetRotation();
        }

        /// <summary>
        /// 敌人单位的行动逻辑
        /// </summary>
        public bool EnemyUpdate()
        {
            //做移动操作
            progress += Time.deltaTime * progressFactor;
            while (progress >= 1f)
            {
                tileFrom = tileTo;
                tileTo = tileTo.NextTileOnPath;
                if (tileTo == null)
                {
                    OriginFactory.Reclaim(this);
                    return false;
                }
                progress -= 1f;
                PrepareNextState();
            }

            if (directionChange == DirectionTurnTo.None)
            {
                transform.localPosition =
                    Vector3.LerpUnclamped(positionFrom, positionTo, progress);
            }
            else //做转向操作
            {
                float angle = Mathf.LerpUnclamped(
                    directionAngleFrom, directionAngleTo, progress
                );
                transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
            return true;
        }

        /// <summary>
        /// 更新目的地时的单位信息配置
        /// </summary>
        void PrepareNextState()
        {
            positionFrom = positionTo;
            positionTo = tileFrom.ExitPoint;//？终点计算有误？

            directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);

            direction = tileFrom.PathDirection;
            directionAngleFrom = directionAngleTo;

            switch (directionChange)
            {
                case DirectionTurnTo.None: PrepareForward(); break;
                case DirectionTurnTo.TurnRight: PrepareTurnRight(); break;
                case DirectionTurnTo.TurnLeft: PrepareTurnLeft(); break;
                default: PrepareTurnAround(); break;
            }
        }


        void PrepareForward()
        {
            transform.localRotation = direction.GetRotation();
            directionAngleTo = direction.GetAngle();

            model.localPosition = Vector3.zero;
        }

        void PrepareTurnRight()
        {
            directionAngleTo = directionAngleFrom + 90f;

            model.localPosition = new Vector3(-0.5f, 0f);

            transform.localPosition = positionFrom + direction.GetHalfVector();
        }

        void PrepareTurnLeft()
        {
            directionAngleTo = directionAngleFrom - 90f;

            model.localPosition = new Vector3(0.5f, 0f);

            transform.localPosition = positionFrom + direction.GetHalfVector();
        }

        void PrepareTurnAround()
        {
            directionAngleTo = directionAngleFrom + 180f;

            model.localPosition = Vector3.zero;

            transform.localPosition = positionFrom;
        }
    }
}


