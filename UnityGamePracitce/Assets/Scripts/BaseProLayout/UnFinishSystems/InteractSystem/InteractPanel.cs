using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractPanel : UIBasePanel, IDropHandler
{
    public Queue<InteractSlot> slotsBufferQueue = new Queue<InteractSlot>();
    //模板Slot
    private InteractSlot templateTips;
    

    protected override void InitPanel()
    {
        //注册格子操作的各项事件
        EventCenter.GetInstance().AddEventListener("SubtractSlot", SubtractInteractSlot);
        EventCenter.GetInstance().AddEventListener("AddSlot", AddInteractSlot);
    }

    //拖入事件
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (!eventData.pointerDrag.GetComponent<InteractSlot>().switchTrigger)
            eventData.pointerDrag.GetComponent<InteractSlot>().switchTrigger = true;
    }

    public void Start()
    {
        InitPanel();
    }

    /// <summary>
    /// 添加互动格
    /// </summary>
    public void AddInteractSlot()
    {
        ///格子资源加载
        /// 1.绑定到画布对象上
        /// 2.设置位置层级
        /// 3.设置相对位置和大小
        ResourcesMgr.GetInstance().LoadAsync<GameObject>("UI/" + InteractSlot.slotName, (obj) =>
        {
            obj.transform.SetParent(GetControl<Image>("slotList").transform);

            //obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到预设体身上的面板脚步,并设置序列号
            InteractSlot slot = obj.GetComponent<InteractSlot>();
            //把面板存起来
            slotsBufferQueue.Enqueue(slot);
        });

    }

    /// <summary>
    /// 删去格子
    /// </summary>
    public void SubtractInteractSlot()
    {
        InteractSlot DieSlot = slotsBufferQueue.Dequeue();

        //GameObject.Destroy(DieSlot.gameObject);
    }


}
