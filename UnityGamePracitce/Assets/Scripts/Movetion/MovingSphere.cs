using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    ///限制最大移动速度
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    ///限制最大移动加速度
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    ///限制移动范围
    [SerializeField]
    Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    ///限制反弹比例
    [SerializeField, Range(0f, 1f)]
    float bounciness = 0.5f;

    Vector3 velocity;

    Vector2 playerInput;

    void Update()
    {
        //玩家输入数据进行限制划分
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        //讲Vector2的摸控制在1以内
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        //期望速度，指的是小球加速到最后时的最终速度
        Vector3 desiredVelocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        //最大可支持的速度瞬间变化值
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        //讲当前速度渐变到期望速度上
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        //小球的真正位移
        Vector3 displacement = velocity * Time.deltaTime;
        
        //得到位移之后的全新位置
        Vector3 newPosition = transform.localPosition + displacement;
        // if (!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        // {
        //     newPosition.x =
        //         Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //     newPosition.z =
        //         Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        // }

        //将新的位置与限定的运动范围进行全面比较
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            //模拟碰壁反弹效果
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
    }


}
