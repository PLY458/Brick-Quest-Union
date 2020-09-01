using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// 互动UI接口
/// </summary>
public abstract class I_TipUI_Interact : UIBasePanel
{
    //初始化的调整数据
    //绑定的UI物体名：
    protected string _slotName;
    //与世界物体的偏移量
    protected Vector3 _OffestVec;

    //需要获得的组件
    protected RectTransform local_RectTrans;
    protected CanvasGroup local_CanGroup;
    protected Canvas parent_Canvas;

    //记录拖拽开始的临时坐标
    protected Vector2 currentPostion;

    //需要控制的各项物体
    protected GameObject m_UICompent = null;
    protected I_Object_Interact m_InteractObject = null;

    public I_TipUI_Interact()
    {
        local_RectTrans = GetComponent<RectTransform>();
        local_CanGroup = GetComponent<CanvasGroup>();
        //得到画布的循环算法
        if (parent_Canvas == null)
        {
            Transform testCanvas = transform.parent;
            while (testCanvas != null)
            {
                parent_Canvas = testCanvas.GetComponent<Canvas>();
                if (parent_Canvas != null)
                {
                    break;
                }
                testCanvas = testCanvas.parent;
            }
        }
        //得到开始时的临时坐标
        currentPostion = local_RectTrans.anchoredPosition;
    }

    /// <summary>
    /// 随物体变换位置
    /// </summary>
    /// <param name="object_current_positon">物体传递的当前位置</param>
    protected void FollowTheObject(Vector3 object_current_positon)
    {
        local_RectTrans.parent.position = object_current_positon;
        local_RectTrans.anchoredPosition = _OffestVec;
    }

    /// <summary>
    /// TipUI信息初始化(图标信息等等)
    /// </summary>
    public abstract void InitInfo();

    /// <summary>
    /// 如果未发生UI层转换，则重置UI的位置
    /// </summary>
    public virtual void ReSetTheUI(bool switchTrigger) { }

    /// <summary>
    /// 设定UI拥有者
    /// </summary>
    /// <param name="Owner"></param>
    public void SetOwner(I_Object_Interact Owner)
    {
        m_InteractObject = Owner;
    }

    #region UI预制体操作
    /// <summary>
    /// 进行绑定UI预制体销毁
    /// </summary>
    public void Release()
    {
        if (m_UICompent != null)
            GameObject.Destroy(m_UICompent);
    }

    /// <summary>
    /// 设定绑定的UI
    /// </summary>
    /// <param name="theUICompent"></param>
    public void SetGameObject(GameObject theUICompent)
    {
        m_UICompent = theUICompent;
    }

    /// <summary>
    /// 得到绑定的UI
    /// </summary>
    /// <returns></returns>
    public GameObject GetUICompent()
    {
        return m_UICompent;
    }
    #endregion
}
