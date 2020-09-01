using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 可以给外部添加针更新事件的接口
/// 可以提供给外部添加协程的方法
/// </summary>
public class GameMgr : BaseMgr<GameMgr>
{
    private GameComController controller;

    public GameMgr()
    {
        //保证MonoController对象的唯一性
        GameObject obj = new GameObject("GameController");
        controller = obj.AddComponent<GameComController>();
    }

    /// <summary>
    /// 给外部提供的，添加针更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }

    /// <summary>
    /// 给外部提供的，移除针更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }

    /// <summary>
    /// 给外部提供的  添加修补针更新事件的函数
    /// </summary>
    /// <param name="actfun">动画事件</param>
    public void AddFixUpdateListener(UnityAction actfun)
    {
        controller.AddFixUpdateListener(actfun);
    }

    /// <summary>
    /// 给外部提供的  去除修补针更新事件的函数
    /// </summary>
    /// <param name="actfun">非动画事件</param>
    public void RemoveFixUpdateListener(UnityAction actfun)
    {
        controller.RemoveFixUpdateListener(actfun);
    }

    public void AddLateUpdateListener(UnityAction latefun)
    {
        controller.AddLateUpdateListener(latefun);
    }

    public void RemoveLateUpdateListener(UnityAction latefun)
    {
        controller.RemoveLateUpdateListener(latefun);
    }

    /// <summary>
    /// 给外部提供的协程方法
    /// </summary>
    /// <param name="Asynfun"></param>
    public void StartCoroutine(IEnumerator Asynfun)
    {
        controller.StartCoroutine(Asynfun);
    }

}