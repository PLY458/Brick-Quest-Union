using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


/// <summary>
/// 负责生产自己的UItips
/// 完成UItip对自己的跟踪
/// 完成隐藏自己的功能
/// </summary>
public class InteractObject : MonoBehaviour
{
    public GameObject TipUI;
    


    public void InitObject()
    {
        BuildSlot();
    }

    private void Start()
    {

    }

    /// <summary>
    /// 在对应位置上生成UI(还会加入数据读取balbal)
    /// </summary>
    /// <param name="itemHit"></param>
    private void BuildSlot()
    {
        //获得设定目标方位
        Vector3 realtargetPositon = Camera.main.WorldToScreenPoint(transform.position);
        ///格子资源加载
        /// 1.绑定到画布对象上
        /// 2.设置位置层级
        /// 3.设置相对位置和大小
        TipUI =  ResourcesMgr.GetInstance().Load<GameObject>("UI/"+InteractSlot.slotName);

        TipUI.transform.SetParent(UIManagercs.GetInstance().GetUIlayer(E_UI_Layer.System));

        //TipUI.transform.localPosition = Vector3.zero;
        TipUI.transform.localScale = Vector3.one*0.7f;

        (TipUI.transform as RectTransform).offsetMax = Vector2.zero;
        (TipUI.transform as RectTransform).offsetMin = Vector2.zero;
        //设置各项数据
        TipUI.transform.parent.transform.position = realtargetPositon;
        TipUI.GetComponent<RectTransform>().anchoredPosition = InteractSlot.OffestVec;
        TipUI.GetComponent<InteractSlot>().parentObject = this.gameObject;
    }

    /// <summary>
    /// 开始拖动时隐藏自己
    /// </summary>
    public void HideAndShowSelf()
    {
        if (transform.GetComponentInChildren<MeshRenderer>().enabled)
        {
            transform.GetComponentInChildren<MeshRenderer>().enabled = false;
            TipUI.GetComponent<InteractSlot>().local_CanGroup.alpha = 0.0f;
        }
        else
        {
            transform.GetComponentInChildren<MeshRenderer>().enabled = true;
            TipUI.GetComponent<InteractSlot>().local_CanGroup.alpha = 1.0f;
        }
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseUp()
    {
        
    }
}
