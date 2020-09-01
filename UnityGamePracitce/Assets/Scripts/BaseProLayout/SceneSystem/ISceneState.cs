using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


public class ISceneState
{
    // 状态名称和默认名
    private string m_StateName = "TemplateState";
    // 状态名称保护变量
    public string StateName
    {
        get { return m_StateName; }
        set { m_StateName = value; }
    }

    // 控制者
    protected SceneStateController m_Controller = null;

    // 建構者
    public ISceneState(SceneStateController Controller)
    {
        m_Controller = Controller;
    }

    /// <summary>
    /// 场景转换成功后会利用这个方法通知类对象。其中可以实现在该场景执行时需要加载的资源及游戏参数的设置
    /// SceneState Controller在此时才传入
    /// </summary>
	public virtual void StateBegin()
    { }

    /// <summary>
    /// 场景将要被释放时会利用这个方法通知类对象。其中可以释放游戏不再使用的资源，或者重新设置游戏场景状态。
    /// </summary>
	public virtual void StateEnd()
    { }

    /// <summary>
    /// “游戏定时更新”时会利用这个方法通知类对象。该方法可以让Unity3D
    ///的“定时更新功能”被调用，并通过这个方法让其他游戏系统也定期更新。这个方法可以
    ///让游戏系统类不必继承Unity3D的MonoBehaviour类，也可以拥有定时更新功能
    /// </summary>
	public virtual void StateUpdate()
    { }

    //控制台信息
    public override string ToString()
    {
        return string.Format("[I_SceneState: StateName={0}]", StateName);
    }

}
