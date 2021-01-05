  
using UnityEngine;

/// <summary>
/// 环绕摄像头
/// </summary>
[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    /// <summary>
    /// 对焦目标
    /// </summary>
    [SerializeField]
    Transform focus = default;

    /// <summary>
    /// 目标控制距离
    /// </summary>
    [SerializeField, Range(1f, 20f)]
    float distance = 5f;

    /// <summary>
    /// 物体聚焦半径
    /// </summary>
    [SerializeField, Min(0f)]
    float focusRadius = 1f;

    /// <summary>
    /// 物体视线聚焦中心
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;

    /// <summary>
    /// 摄像机旋转速度
    /// </summary>
    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;

    /// <summary>
    /// 最小和最大摄像机俯仰角度
    /// </summary>
    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    /// <summary>
    /// 自动调整摄像机延迟
    /// </summary>
    [SerializeField, Min(0f)]
    float alignDelay = 5f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    /// <summary>
    /// 上次玩家手动调整镜头的次数记录
    /// </summary>
    float lastManualRotationTime;

    /// <summary>
    /// 目前摄像机焦点和前一个摄像机焦点
    /// </summary>
    Vector3 focusPoint, previousFocusPoint;

    /// <summary>
    /// 当前摄像机环绕角度
    /// </summary>
    Vector2 orbitAngles = new Vector2(45f, 45f);

    Camera regularCamera;



    #region 生命周期
    void Awake()
    {

        regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    void LateUpdate()
    {

        UpdateFocusPoint();
        //摄像机环绕旋转运算
        Quaternion lookRotation;
        //分为玩家手动旋转和摄像机自动根据小球路径旋转
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        //结算更新摄像机位移情况
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        //计算摄像机与场景的隔离情况
        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(
            castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
            lookRotation, castDistance))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
    #endregion

    /// <summary>
    /// 摄像机对焦点迭代更新
    /// </summary>
    void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;

        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float temptRadius = 1f;

            if (distance > 0.01f && focusCentering > 0f)
            {
                //unscaleDeltaTime表示不受慢动作特效(TimeLine)的控制
                temptRadius = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }

            if (distance > focusRadius)
            {
                temptRadius = Mathf.Min(temptRadius, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, temptRadius);

        }
        else {
            focusPoint = targetPoint;
        }
        
    }

    /// <summary>
    /// 手动调节摄像机角度
    /// </summary>
    /// <returns></returns>
    bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera")
        );
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledDeltaTime;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 限制摄像机俯仰角
    /// </summary>
    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    /// <summary>
    /// 摄像机主动旋转(根据小球运作路线)
    /// </summary>
    /// <returns></returns>
    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z
        );
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.000001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr); ;
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }

        orbitAngles.y =
            Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y =
                regularCamera.nearClipPlane *
                Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

}
