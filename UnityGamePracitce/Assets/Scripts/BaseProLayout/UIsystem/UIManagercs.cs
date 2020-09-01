using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}


/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIManagercs :BaseMgr<UIManagercs>
{
    public Dictionary<string,UIBasePanel> panelDic = new Dictionary<string, UIBasePanel>();

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIManagercs()
    {
        //创建Canvas 让其过场景的时候 不被移除
        GameObject obj = ResourcesMgr.GetInstance().Load<GameObject>("UI/CanvasSturct");
        Transform canvas = obj.transform;
        GameObject.DontDestroyOnLoad(obj);

        //各层坐标
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");

        //创建EventSystem 让其过场景的时候，不被移除
        obj = ResourcesMgr.GetInstance().Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);

    }


    /// <summary>
    /// 显示面板在Canve结构层级上
    /// </summary>
    /// <typeparam name="T">面板脚步类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示层级</param>
    /// <param name="callBack">当面板加载成功，启动的回调方法</param>
    public void showPanelOnLayer<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where  T:UIBasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            //处理面板创建后的回调
            callBack?.Invoke(panelDic[panelName] as T);
        }

        ///面板资源加载
        /// 1.绑定到画布对象上
        /// 2.设置位置层级
        /// 3.设置相对位置和大小
        ResourcesMgr.GetInstance().LoadAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            Transform ui_layer = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    ui_layer = mid;
                    break;
                case E_UI_Layer.Top:
                    ui_layer = top;
                    break;
                case E_UI_Layer.System:
                    ui_layer = system;
                    break;
            }

            obj.transform.SetParent(ui_layer);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.one;

            //得到预设体身上的面板脚步
            T panel = obj.GetComponent<T>();
            //处理面板创建完成后的逻辑
            callBack?.Invoke(panel);
            //把面板存起来
            panelDic.Add(panelName, panel);
        });
    }

    /// <summary>
    /// 面板销毁
    /// </summary>
    /// <param name="panelName"></param>
    public void DestoryPanelInDic(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 获取UI界面层级坐标
    /// </summary>
    /// <returns></returns>
    public Transform GetUIlayer(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
            default:
                return system;
        }

    }
}
