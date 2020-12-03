using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game {

        public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// 敌人单位模型
        /// </summary>
        [SerializeField]
        Transform model = default;

        /// <summary>
        /// 敌人实时监控的某一对路径想通的导航点
        /// </summary>
        NaviTile tileFrom, tileTo;

        /// <summary>
        /// 从导航点关系实时计算的路线坐标
        /// </summary>
        Vector3 positionFrom, positionTo;
        
        /// <summary>
        /// 路线计算周期计时器以及计时因数(保证单位恒速前进)
        /// </summary>
        float progress, progressFactor;

        /// <summary>
        /// 敌人路线偏移量和起始速度
        /// </summary>
        float pathOffset,speed;

        /// <summary>
        /// 单位目前的朝向方向
        /// </summary>
        Direction direction;

        /// <summary>
        /// 单位旋转顺逆方向
        /// </summary>
        DirectionTurnTo directionChange;

        /// <summary>
        /// 方向起始角和终点角
        /// </summary>
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
        /// 敌人单位属性初始化
        /// </summary>
        public void Initialize(float scale,float pathoffset,float speed)
        {
            model.localScale = new Vector3(scale, scale, scale);
            this.pathOffset = pathoffset;
            this.speed = speed;
        }

        /// <summary>
        /// 设置敌人单位的出生点
        /// </summary>
        public void SetSpownPoint(NaviTile tile)
        {
            //transform.localPosition = tile.transform.localPosition;
            Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);

            tileFrom = tile;
            tileTo = tile.NextTileOnPath;

            progress = 0f;
            PrepareIntro();
        }



        /// <summary>
        /// 敌人单位的行动逻辑刷新
        /// </summary>
        public bool EnemyUpdate()
        {
            //计时开始做移动坐标计算
            progress += Time.deltaTime * progressFactor;
            //progress += Time.deltaTime;
            while (progress >= 1f)
            {

                if (tileTo == null)
                {
                    OriginFactory.Reclaim(this);
                    return false;
                }

                //由周期因数来控制敌人相应的恒定
                //转向会影响敌人单位的响应速度
                progress = (progress - 1f) / progressFactor;
                
                PrepareNextState();
                progress *= progressFactor;
                
            }

            //判断单位是否有转向需求
            if (directionChange == DirectionTurnTo.None)
            {
                //调用全局方法做敌人移动的线性变换
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
            tileFrom = tileTo;
            tileTo = tileTo.NextTileOnPath;

            positionFrom = positionTo;

            //确认是否已到达目标点
            if (tileTo == null)
            {
                PrepareOutro();
                return;
            }

            positionTo = tileFrom.ChannelPoint;
            //transform.localRotation = tileFrom.PathDirection.GetRotation();
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

        /// <summary>
        /// 出生时期的单位信息配置
        /// </summary>
        void PrepareIntro()
        {
            positionFrom = tileFrom.transform.localPosition;
            positionTo = tileFrom.ChannelPoint;
            //transform.localRotation = tileFrom.PathDirection.GetRotation();
            direction = tileFrom.PathDirection;
            directionChange = DirectionTurnTo.None;

            directionAngleFrom = directionAngleTo = direction.GetAngle();

            model.localPosition = new Vector3(pathOffset, 0f);
            transform.localRotation = direction.GetRotation();
            //transform.localRotation = tileFrom.PathDirection.GetRotation();

            progressFactor = 2f * speed;
        }

        /// <summary>
        /// 到达目的地后敌人单位的属性设置
        /// </summary>
        void PrepareOutro()
        {
            positionTo = tileFrom.transform.localPosition;
            directionChange = DirectionTurnTo.None;
            directionAngleTo = direction.GetAngle();

            model.localPosition = Vector3.zero;
            transform.localRotation = tileFrom.PathDirection.GetRotation();
            progressFactor = 2f * speed;
        }

        void PrepareForward()
        {
            transform.localRotation = tileFrom.PathDirection.GetRotation();
            directionAngleTo = direction.GetAngle();

            model.localPosition = new Vector3(pathOffset, 0f);

            progressFactor = speed;
        }

        void PrepareTurnRight()
        {
            directionAngleTo = directionAngleFrom + 90f;

            model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
            //单位坐标进行圆滑偏移
            transform.localPosition = positionFrom + direction.GetHalfVector();
            //协调路线偏移量的周期因数
            progressFactor =
                speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
        }

        void PrepareTurnLeft()
        {
            directionAngleTo = directionAngleFrom - 90f;

            model.localPosition = new Vector3(pathOffset+0.5f, 0f);
            //单位坐标进行圆滑偏移
            transform.localPosition = positionFrom + direction.GetHalfVector();
            //协调路线偏移量的周期因数
            progressFactor =
                speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
        }

        void PrepareTurnAround()
        {
            directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);

            model.localPosition = new Vector3(pathOffset, 0f);
            //单位坐标原地不动
            transform.localPosition = positionFrom;
            //协调路线偏移量的周期因数
            progressFactor =
                speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
        }


    }
}


