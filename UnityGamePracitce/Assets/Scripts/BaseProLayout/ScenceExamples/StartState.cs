using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class StartState : ISceneState
{
    public StartState(SceneStateController Controller) : base(Controller)
    {
        this.StateName = "StartState";
    }

    // 開始
    public override void StateBegin()
    {
        //// 取得開始按鈕
        //Button tmpBtn = UITool.GetUIComponent<Button>("StartGameBtn");
        //// 按钮设置监听方法
        //if (tmpBtn != null)
        //    tmpBtn.onClick.AddListener(() => OnStartGameBtnClick(tmpBtn));
    }

    // 更新
    public override void StateUpdate()
    {
        // 更换到主菜单场景
        m_Controller.SetState(new GameState(m_Controller), "GameScene");
    }

    /// <summary>
    /// 按键检测事件，负责转到战斗场景
    /// </summary>
    /// <param name="theButton">所属的按钮组件</param>
    //private void OnStartGameBtnClick(Button theButton)
    //{
    //    Debug.Log("OnStartBtnClick:" + theButton.gameObject.name);
    //    m_Controller.SetState(new BattleState(m_Controller), "BattleScene");
    //}

}
