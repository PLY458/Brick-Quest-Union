using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 拼图碎片控制
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class FragsControl : MonoBehaviour
{
    //发射射线的摄像机
    private Camera interact_Camera;
    //定位碎片的世界坐标
    private Vector3 world_Position;
    //光标与碎片中心的偏移量
    private Vector3 grab_Offest;

    /// <summary>
    /// 开始拖动图片的处理
    /// </summary>
    private void Beign_Dragging()
    {
        //TODO 光标指定的位置转换为3d空间的世界坐标
        grab_Offest = transform.position - world_Position;

    }
}
