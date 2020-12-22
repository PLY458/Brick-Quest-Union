using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    #region 小球移动属性最大范围
    /// <summary>
    /// 最大移动速度限制
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    float max_Speed = 10f;

    /// <summary>
    /// 最大移动加速度限制
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    float max_Acceleration = 10f,max_AirAcceleration = 1f;

    /// <summary>
    /// 最大跳跃高度限制
    /// </summary>
    [SerializeField, Range(0f, 10f)]
    float height_Jump = 2f;

    /// <summary>
    /// 最大空跳次数限制
    /// </summary>
    [SerializeField, Range(0, 5)]
    int max_AirJumps = 0;

    /// <summary>
    /// 最大爬坡角度限制
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    /// <summary>
    /// 玩家输入的位移方向空间
    /// </summary>
    [SerializeField]
    Transform playerInputSpace = default;

    #endregion

    #region 小球接触表面判定参数
    /// <summary>
    /// 探测小球接触表面的遮罩层和判断表面为楼梯的遮罩层
    /// </summary>
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1;

    /// <summary>
    /// 表面探测距离
    /// </summary>
    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    /// <summary>
    /// 表面接触点数量和陡坡接触点数量
    /// </summary>
    int groundContactCount, steepContactCount;

    /// <summary>
    /// 确认小球处于平面上
    /// </summary>
    bool OnGround => groundContactCount > 0;
    /// <summary>
    /// 确认小球处于陡峭平面上
    /// </summary>
    bool OnSteep => steepContactCount > 0;

    /// <summary>
    /// 接触平面时和接触陡坡时的法向量
    /// </summary>
    Vector3 contactNormal , steepNormal;

    /// <summary>
    /// 记录两次落地间的间隔次数
    /// </summary>
    int stepsSinceLastGrounded, stepsSinceLastJump;
    #endregion

    #region 小球相关的性能属性
    /// <summary>
    /// 小球确认跳跃信号
    /// </summary>
    bool desired_Jump;

    /// <summary>
    /// 记录小球在空中的已弹跳次数
    /// </summary>
    int jumpPhase;

    /// <summary>
    /// 可通过接触面的最小夹角最小点积，代表小球的最大爬坡能力
    /// </summary>
    float minGroundDotProduct, minStairsDotProduct; 
    
    /// <summary>
    /// 当前小球速度和期望小球达到的速度
    /// </summary>
    Vector3 velocity, desired_Velocity;

    /// <summary>
    /// 输入数据模型
    /// </summary>
    Vector2 input_Player;

    /// <summary>
    /// 小球刚体
    /// </summary>
    Rigidbody body_Sphere;
    #endregion

    #region 生命周期
    void Awake()
    {
        body_Sphere = GetComponent<Rigidbody>();
        OnValidate();
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    void Update()
    {
        //玩家输入数据进行限制划分
        input_Player.x = Input.GetAxis("Horizontal");
        input_Player.y = Input.GetAxis("Vertical");
        //讲Vector2的摸控制在1以内
        input_Player = Vector2.ClampMagnitude(input_Player, 1f);

        if (playerInputSpace)
        {
            //检索
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            desired_Velocity = (forward * input_Player.y + right * input_Player.x) * max_Speed;
        }
        else
        {
            //期望速度，指的是小球加速到最后时的最终速度
            desired_Velocity =
            new Vector3(input_Player.x, 0f, input_Player.y) * max_Speed;
        }
        //期望跳跃指令，OR符号不断检查指令是否一直为true
        desired_Jump |= Input.GetButtonDown("Jump");

    }

    private void FixedUpdate()
    {

        UpdateState();
        AdjustVelocity();

        #region 直接位移小球方位
        //小球的真正位移
        //Vector3 displacement = velocity * Time.deltaTime;
        //
        ////得到位移之后的全新位置
        //Vector3 newPosition = transform.localPosition + displacement;
        //
        //transform.localPosition = newPosition;
        #endregion

        if (desired_Jump)
        {
            desired_Jump = false;
            Jump();
        }

        body_Sphere.velocity = velocity;

        #region 移动范围判定
        // if (!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        // {
        //     newPosition.x =
        //         Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //     newPosition.z =
        //         Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        // }

        //将新的位置与限定的运动范围进行全面比较
        //if (newPosition.x < allowedArea.xMin)
        //{
        //    newPosition.x = allowedArea.xMin;
        //    //模拟碰壁反弹效果
        //    velocity.x = -velocity.x * bounciness;
        //}
        //else if (newPosition.x > allowedArea.xMax)
        //{
        //    newPosition.x = allowedArea.xMax;
        //    velocity.x = -velocity.x * bounciness;
        //}
        //if (newPosition.z < allowedArea.yMin)
        //{
        //    newPosition.z = allowedArea.yMin;
        //    velocity.z = -velocity.z * bounciness;
        //}
        //else if (newPosition.z > allowedArea.yMax)
        //{
        //    newPosition.z = allowedArea.yMax;
        //    velocity.z = -velocity.z * bounciness;
        //}
        #endregion

        ClearState();
    }

    #endregion

    #region 小球状态更新相关
    /// <summary>
    /// 小球状态实时更新
    /// </summary>
    void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;

        velocity = body_Sphere.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;

            if (stepsSinceLastJump > 1)
            {
                jumpPhase = 0;
            }

            if (groundContactCount > 1)
            {
                //多个接触面共同存在时，需要把获得的法向量单位化
                contactNormal.Normalize();
            }
        }
        else
        {
            //默认接触面法向量为Y轴正坐标
            contactNormal = Vector3.up;
        }
    }

    /// <summary>
    /// 清理小球状态
    /// </summary>
    void ClearState() {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }
    
    /// <summary>
    /// 小球速度属性调整方法
    /// </summary>
    void AdjustVelocity()
    {
        //获取小球与当前接触面的坐标系
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        //得到坐标系下的X,Z坐标值
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        //根据是否接地来决定调用地面加速度还是空中加速度
        float acceleration = OnGround ? max_Acceleration : max_AirAcceleration;
        //最大可支持的速度瞬间变化值
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desired_Velocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desired_Velocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    #endregion

    #region 小球操作方法
    /// <summary>
    /// 小球跳跃方法
    /// 1.可实现蹬墙跳
    /// </summary>
    void Jump()
    {
        //可以在不同斜坡产生的跳跃方向
        Vector3 jumpDirection;
        //平地跳
        if (OnGround)
        {
            jumpDirection = contactNormal;
        }
        //贴墙跳
        else if (OnSteep)
        {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        }
        //空气跳
        else if (max_AirJumps > 0 && jumpPhase <= max_AirJumps)
        {
            if (jumpPhase == 0)
            {
                jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        }
        else
        {
            return;
        }

        stepsSinceLastJump = 0;
        jumpPhase += 1;
        // 确立跳跃起始速度的大小(动能守恒)
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * height_Jump);

        jumpDirection = (jumpDirection + Vector3.up).normalized;
        // 用于检测各种方向上真实的速度
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f)
        {
            // 确保跳跃起始速度不超过最大高度
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
    }
    #endregion

    #region 小球与接触面处理方法
    /// <summary>
    /// 获得小球与接触面的平面坐标
    /// </summary>
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    /// <summary>
    /// 获得不同遮罩层下的小球爬坡能力
    /// </summary>
    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }

    /// <summary>
    /// 与表面贴合方法
    /// </summary>
    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }//检测小球下方的地面区
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }//被限制长度的射线来检测是否需要吸附表面
        if (!Physics.Raycast(body_Sphere.position, 
            Vector3.down, out RaycastHit hit, probeDistance, probeMask))
        {
            return false;
        }//检测小球是否处于接地状态
        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
 
        groundContactCount = 1;
        contactNormal = hit.normal;
        
        float dot = Vector3.Dot(velocity, hit.normal);
        //与新接触的表面夹角不大于90，则小球紧贴表面运行
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }         
        return true;
    }

    /// <summary>
    /// 检测被接触的斜面属性
    /// </summary>
    /// <returns></returns>
    bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            //将所有斜坡的法向量集归一化，作为统一斜面来计算
            steepNormal.Normalize();
            if (steepNormal.y >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 碰撞检测周期相关
    void OnCollisionEnter(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }

    /// <summary>
    /// 不断获取碰撞点之间的法线值，并判断其值是否在Y轴正方向上
    /// </summary>
    void EvaluateCollision(Collision collision)
    {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
                steepContactCount += 1;
                steepNormal += normal;
            }
        }
    }

    ///当与除地面之外的物体接触时，被退出可能会是不同的碰撞体上
    ///void OnCollisionExit()
    ///{
    ///    onGround = false;
    ///}
    #endregion
}
