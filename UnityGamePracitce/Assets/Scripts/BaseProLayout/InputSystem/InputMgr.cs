using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 1.输入控制管理
/// 2.接入事件中心模块和公共模块
/// </summary>
public class InputMgr: BaseMgr<InputMgr>
{
    /// <summary>
    /// 开启检测开关
    /// </summary>
    private bool isStart = false;

    /// <summary>
    /// 在构造函数中添加Updata监听
    /// </summary>
    public InputMgr()
    {
        //MonoMgr.GetInstance().AddFixUpdateListener(MyUpdate);
    }

    /// <summary>
    /// 是否开启输入检测
    /// </summary>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    /// <summary>
    /// 检测按键抬起按下状态，并分发事件
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key,string methodname = "")
    {
        
        if (Input.GetKeyDown(key))
        {
            EventCenter.GetInstance().EventTrigger("移动飞船", key);
        }
        if (Input.GetKeyUp(key))
        {
            EventCenter.GetInstance().EventTrigger("某健按下", key);
        }
    }


    /// <summary>
    /// 输入更新
    /// </summary>
    public void MyUpdate()
    {
        if (!isStart)
        {
            return;
        }
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.D);
    }
}
