using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 公共Mono的管理者
/// 1.声明周期函数
/// 2.事件
/// 3.协程
/// </summary>
public class GameComController : MonoBehaviour
{
    //场景状态提前定义
    SceneStateController m_SceneStateController = new SceneStateController();

    private event UnityAction updateEvents;
    private event UnityAction FixupdateEvents;
    private event UnityAction LateupdateEvents;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {

    }

    void Update()
    {
        if (updateEvents != null)
        {
            updateEvents();
        }
    }

    void FixedUpdate()
    {
        if (FixupdateEvents != null)
        {
            FixupdateEvents();
        }
    }

    void LateUpdate()
    {
        if (LateupdateEvents != null)
        {
            LateupdateEvents();
        }
    }
    /// <summary>
    /// 给外部提供的  添加普通针更新事件的函数
    /// </summary>
    /// <param name="fun">非动画事件</param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvents += fun;
    }

    /// <summary>
    /// 给外部提供的  添加修补针更新事件的函数
    /// </summary>
    /// <param name="fun">动画事件</param>
    public void AddFixUpdateListener(UnityAction actfun)
    {
        FixupdateEvents += actfun;
    }

    /// <summary>
    /// 给外部提供的  添加延后针更新事件的函数
    /// </summary>
    /// <param name="fun">动画事件</param>
    public void AddLateUpdateListener(UnityAction latefun)
    {
        LateupdateEvents += latefun;
    }

    /// <summary>
    /// 给外部提供的  去除普通针更新事件的函数
    /// </summary>
    /// <param name="actfun">非动画事件</param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvents -= fun;
    }

    /// <summary>
    /// 给外部提供的  去除修补针更新事件的函数
    /// </summary>
    /// <param name="actfun">动画事件</param>
    public void RemoveFixUpdateListener(UnityAction actfun)
    {
        FixupdateEvents -= actfun;
    }

    /// <summary>
    /// 给外部提供的  去除修补针更新事件的函数
    /// </summary>
    /// <param name="actfun">动画事件</param>
    public void RemoveLateUpdateListener(UnityAction latefun)
    {
        LateupdateEvents -= latefun;
    }

}
