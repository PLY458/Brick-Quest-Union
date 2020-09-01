using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractSlot : UIBasePanel, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //静态固定名方便加载
    public static string slotName = "InteractSlot";
    public static Vector3 OffestVec = new Vector3(0.0f, 135.0f, 0.0f);

    private RectTransform local_RectTrans;
    public CanvasGroup local_CanGroup;
    private Canvas parentCanvas;
    //记录拖拽开始时的临时坐标
    private Vector2 currentPostion;
    //所依附的游戏物体
    public GameObject parentObject;
    //世界空间到UI的转变判断值
    public bool switchTrigger;

    public void InitInfo()
    {
        //载入图片

        //GetControl<Image>("imgSlot").sprite = ResourcesMgr.GetInstance().Load<Sprite>("Icon/Ball");

        local_RectTrans = GetComponent<RectTransform>();
        local_CanGroup = GetComponent<CanvasGroup>();

        //鼠标拖动的特殊处理,需要得到画布
        if (parentCanvas == null)
        {
            Transform testCanvas = transform.parent;
            while (testCanvas != null)
            {
                parentCanvas = testCanvas.GetComponent<Canvas>();
                if (parentCanvas != null)
                {
                    break;
                }
                testCanvas = testCanvas.parent;
            }
        }

    }

    private void Start()
    {
        InitInfo();
    }

    #region Slot拖拽事件
    //开始拖拽时透明度调暗，射线判定取消
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        local_CanGroup.alpha = .6f;
        local_CanGroup.blocksRaycasts = false;
        currentPostion = local_RectTrans.anchoredPosition;
    }

    //结束拖拽恢复各项属性
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        local_CanGroup.alpha = 1f;
        local_CanGroup.blocksRaycasts = true;
        ReSetTheSlot();
    }

    //鼠标拖动过程中同时转变位移
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        local_RectTrans.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }
    #endregion

    public void FollowTheSlot()
    {
        //获得定位后的物体位置
        Vector3 realtargetPositon = Camera.main.WorldToScreenPoint(parentObject.transform.position);
        //更新新的位置
        transform.parent.transform.position = realtargetPositon;
        transform.GetComponent<RectTransform>().anchoredPosition = OffestVec;
    }

    public void ReSetTheSlot()
    {
        //添加事件 如果该格子现在位于区域中
        if (local_RectTrans.parent.name == "slotList")
        {
            //检测格子的Y轴坐标
            Debug.Log(local_RectTrans.anchoredPosition - currentPostion);
            if ((local_RectTrans.anchoredPosition - currentPostion).y > 0)
            {
                //讲格子移除区域中
                EventCenter.GetInstance().EventTrigger("SubtractSlot");
                //放出射线销毁自己产生新物体
                EventCenter.GetInstance().EventTrigger("CreateTarget");
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                //格子返回原位
                local_RectTrans.anchoredPosition = currentPostion;
            }
        }
        else if (local_RectTrans.parent.name == "System")
        {
            //判断是否需要转变
            if (switchTrigger)
            {
                //添加格子在区域中
                EventCenter.GetInstance().EventTrigger("AddSlot");
                //先消除依附的物体
                GameObject.DestroyImmediate(parentObject,true);
                //再消除自身
                GameObject.Destroy(this.gameObject);
            }
            //返回原位
            local_RectTrans.anchoredPosition = currentPostion;
        }
    }

}
