using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Internal;


/// <summary>
/// 可以给外部添加针更新事件的接口
/// 可以提供给外部添加协程的方法
/// </summary>
public class MonoMgr : BaseManeger<MonoMgr>
{
    private MonoComController controller;

    public MonoMgr()
    {
        //保证MonoController对象的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoComController>();
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

    //封装协程方法
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName,[DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName,value);
    }

    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }

}
