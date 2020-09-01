using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayserUIsystem : BaseMgr<RayserUIsystem>
{
    private Camera _cameraUI;
    private InteractSlot tipUI ;
    //游戏物体与UI坐标交互的层级
    private LayerMask _interactionMask;
    private LayerMask _movementMask;
    //捕获到的临时游戏物体
    private GameObject _wallobject;

    /// <summary>
    /// 初始化系统并且负责注册有关事件
    /// </summary>
    public void InitRayserSystem(Camera camera)
    {
        _interactionMask = LayerMask.GetMask("Interactable");
        _movementMask = LayerMask.GetMask("Ground");
        _cameraUI = camera;

        EventCenter.GetInstance().AddEventListener("CatchTarget",CatchTarget);
        EventCenter.GetInstance().AddEventListener("MoveTarget",MoveTarget);
        EventCenter.GetInstance().AddEventListener("CreateTarget", CreateTarget);
    }

    /// <summary>
    /// 射线获取游戏物体
    /// </summary>
    private void CatchTarget()
    {
        // 射出一条射线
        Ray ray = _cameraUI.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 如果射线与物体交互
        if (Physics.Raycast(ray, out hit, _interactionMask) )
        {
            //隐藏获得的目标物体
            Debug.Log("捕捉物体" + hit.collider.name + " " + hit.point);
            //获取物体信息
            _wallobject = hit.transform.GetComponent<GameObject>();
            //隐藏物体
            _wallobject.GetComponent<InteractObject>().HideAndShowSelf();
        }
    }


    /// <summary>
    /// 射线检测不断移动物体
    /// </summary>
    private void MoveTarget()
    {
        // 射出一条射线
        Ray ray = _cameraUI.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // 检测射线在Ground标签上的位置
        if (Physics.Raycast(ray, out hit, _movementMask))
        {
        
            Debug.Log("重新设置物体：" + hit.point);
            //改变物体方位
            _wallobject.transform.position = hit.point;
            //更新UI的位置
            _wallobject.GetComponent<InteractObject>().TipUI.GetComponent<InteractSlot>().FollowTheSlot();
            //显示物体
            _wallobject.GetComponent<InteractObject>().HideAndShowSelf();
        }
    }

    /// <summary>
    /// 射线产生新物体
    /// </summary>
    private void CreateTarget()
    {
        // 射出一条射线
        Ray ray = _cameraUI.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 检测射线在Ground标签上的位置
        if (Physics.Raycast(ray, out hit, _movementMask))
        {

            Debug.Log("创建新物体：" + hit.point);
            //得到新的物体
            _wallobject = ResourcesMgr.GetInstance().Load<GameObject>("Prefabs/WallObject");
            //调整物体方位
            _wallobject.transform.position = hit.point;
            //初始化物体
            _wallobject.GetComponent<InteractObject>().InitObject();
        }
    }


}
