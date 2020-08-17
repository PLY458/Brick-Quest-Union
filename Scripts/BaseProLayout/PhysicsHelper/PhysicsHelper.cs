using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


public class PhysicsHelper : BaseManeger<PhysicsHelper>
{
    /// <summary>
    /// 应用受到的力达到对应的速度
    /// </summary>
    /// <param name="rigidbody">受力刚体</param>
    /// <param name="velocity">限定的速度</param>
    /// <param name="force">受到的力大小</param>
    /// <param name="mode">力的应用模式</param>
    public void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        if (force == 0 || velocity.magnitude == 0)
            return;

        velocity = velocity + velocity.normalized * 0.2f * rigidbody.drag;

        //force = 1 => need 1 s to reach velocity (if mass is 1) => force can be max 1 / Time.fixedDeltaTime
        force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude https://www.youtube.com/watch?v=h0NJK4mEIJU
        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }

    /// <summary>
    /// 应用受到的力达到对应的角速度
    /// </summary>
    /// <param name="rigidbody">目标刚体</param>
    /// <param name="rotation">只提供轴的四元数？</param>
    /// <param name="rps">目标角速度</param>
    /// <param name="force">切线上受到的作用力</param>
    public void ApplyTorqueToReachRPS(Rigidbody rigidbody, Quaternion rotation, float rps, float force = 1)
    {
        var radPerSecond = rps * 2 * Mathf.PI + rigidbody.angularDrag * 20;

        float angleInDegrees;
        Vector3 rotationAxis;
        rotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

        if (force == 0 || rotationAxis == Vector3.zero)
            return;

        //设置刚体最大旋转速度（默认是7，在默认和目标速度取最大值）
        rigidbody.maxAngularVelocity = Mathf.Min(rigidbody.maxAngularVelocity, radPerSecond);

        force = Mathf.Clamp(force, -rigidbody.mass * 2 * Mathf.PI / Time.fixedDeltaTime, rigidbody.mass * 2 * Mathf.PI / Time.fixedDeltaTime);

        var currentSpeed = Vector3.Project(rigidbody.angularVelocity, rotationAxis).magnitude;

        rigidbody.AddTorque(rotationAxis * (radPerSecond - currentSpeed) * force);
    }

    //四元数转换角速度
    public  Vector3 QuaternionToAngularVelocity(Quaternion rotation)
    {
        float angleInDegrees;
        Vector3 rotationAxis;
        rotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

        return rotationAxis * angleInDegrees * Mathf.Deg2Rad;
    }


    public  Quaternion AngularVelocityToQuaternion(Vector3 angularVelocity)
    {
        var rotationAxis = (angularVelocity * Mathf.Rad2Deg).normalized;
        float angleInDegrees = (angularVelocity * Mathf.Rad2Deg).magnitude;

        return Quaternion.AngleAxis(angleInDegrees, rotationAxis);
    }

    /// <summary>
    /// 获取点之间的法线向量
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public  Vector3 GetNormal(Vector3[] points)
    {
        //https://www.ilikebigbits.com/2015_03_04_plane_from_points.html
        if (points.Length < 3)
            return Vector3.up;

        var center = GetCenter(points);

        float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            var r = points[i] - center;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        var det_x = yy * zz - yz * yz;
        var det_y = xx * zz - xz * xz;
        var det_z = xx * yy - xy * xy;

        if (det_x > det_y && det_x > det_z)
            return new Vector3(det_x, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
        if (det_y > det_z)
            return new Vector3(xz * yz - xy * zz, det_y, xy * xz - yz * xx).normalized;
        else
            return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, det_z).normalized;

    }

    /// <summary>
    /// 获取物体质心
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public Vector3 GetCenter(Vector3[] points)
    {
        var center = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
            center += points[i] / points.Length;
        return center;
    }
}
