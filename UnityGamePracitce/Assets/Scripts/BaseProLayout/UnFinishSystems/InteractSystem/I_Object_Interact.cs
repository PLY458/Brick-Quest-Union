using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 互动游戏物体接口
/// </summary>
public abstract class I_Object_Interact
{ 
    //需要控制的各种组件
    private I_TipUI_Interact m_TipUI = null;

    /// <summary>
    /// 设定绑定的UI
    /// </summary>
    /// <param name="TipUI"></param>
    public void SetTipUI(I_TipUI_Interact TipUI)
    {
        if (m_TipUI != null)
            m_TipUI.Release();
        m_TipUI = TipUI;

        //设定UI拥有者
        m_TipUI.SetOwner(this);

    }

    //提供UI
    public I_TipUI_Interact GetTipUI()
    {
        return m_TipUI;
    }


}
