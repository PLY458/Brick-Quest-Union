using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{

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
    float maxGroundAngle = 25f;

    ///限制移动范围
    //[SerializeField]
    //Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);
    //
    /////限制反弹比例
    //[SerializeField, Range(0f, 1f)]
    //float bounciness = 0.5f;

    int groundContactCount;

    /// <summary>
    /// 小球确认跳跃信号
    /// </summary>
    bool desired_Jump;

    /// <summary>
    /// 可通过接触面的最小夹角最小点积，代表小球的最大爬坡能力
    /// </summary>
    float minGroundDotProduct;
    
    /// <summary>
    /// 确认小球是否存在接触面
    /// </summary>
    bool OnGround => groundContactCount > 0;

    /// <summary>
    /// 记录小球在空中的已弹跳次数
    /// </summary>
    int jumpPhase;

    /// <summary>
    /// 当前小球速度和期望小球达到的速度
    /// </summary>
    Vector3 velocity, desired_Velocity;

    Vector2 input_Player;

    Vector3 contactNormal;

    Rigidbody body_Sphere;

    

    void Awake()
    {
        body_Sphere = GetComponent<Rigidbody>();
        OnValidate();
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void Update()
    {
        //玩家输入数据进行限制划分
        input_Player.x = Input.GetAxis("Horizontal");
        input_Player.y = Input.GetAxis("Vertical");
        //讲Vector2的摸控制在1以内
        input_Player = Vector2.ClampMagnitude(input_Player, 1f);

        //期望速度，指的是小球加速到最后时的最终速度
        desired_Velocity =
            new Vector3(input_Player.x, 0f, input_Player.y) * max_Speed;

        //期望跳跃指令，OR符号不断检查指令是否一直为true
        desired_Jump |= Input.GetButtonDown("Jump");

        //GetComponent<Renderer>().material.SetColor(
        //    "_BaseColor", Color.white * (groundContactCount * 0.25f)

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

    /// <summary>
    /// 小球状态实时更新
    /// </summary>
    void UpdateState()
    {
        velocity = body_Sphere.velocity;
        if (OnGround)
        {
            jumpPhase = 0;
            
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
    /// 小球跳跃方法
    /// </summary>
    void Jump()
    {
        if (OnGround || jumpPhase < max_AirJumps)
        {
            jumpPhase += 1;
            // 确立跳跃起始速度的大小(动能守恒)
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * height_Jump);
            // 用于检测垂直方向上真实的速度
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                // 确保跳跃起始速度不超过最大高度
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
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

    void ClearState() {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }



    #region 碰撞检测相关
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
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
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
