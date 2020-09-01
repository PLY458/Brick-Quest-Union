using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

/// <summary>
/// 场景控制器
/// </summary>
public class SceneStateController
{
    private ISceneState m_State;
    private bool m_bRunBegin = false;
    private UnityAction<float> loadingStateRepeat;

    /// <summary>
    /// 初始化类需要向事件中心注册载入回调方法的事件
    /// </summary>
    public SceneStateController()
    {
        //打包好的回调事件
        loadingStateRepeat = new UnityAction<float>(StateUpdate);
        //注册添加到事件中心的监听名册
        EventCenter.GetInstance().AddEventListener<float>("LoadingRepear", loadingStateRepeat);
    }

    // 設定狀態
    public void SetState(ISceneState State, string LoadSceneName)
    {
        //Debug.Log ("SetState:"+State.ToString());
        m_bRunBegin = false;

        // 載入場景
        LoadScene(LoadSceneName);

        // 通知前一個State結束
        if (m_State != null)
            m_State.StateEnd();

        // 設定
        m_State = State;
    }

    // 載入場景
    private void LoadScene(string LoadSceneName)
    {
        if (LoadSceneName == null || LoadSceneName.Length == 0)
            return;
        ScenceMgr.GetInstance().LoadSceneAsyn(LoadSceneName);

    }

    // 更新
    public void StateUpdate(float progress)
    {
        // 是否還在載入
        if (progress < 1.0)
            return;

        // 通知新的State開始
        if (m_State != null && m_bRunBegin == false)
        {
            m_State.StateBegin();
            m_bRunBegin = true;
        }

        if (m_State != null)
            m_State.StateUpdate();
    }
}
