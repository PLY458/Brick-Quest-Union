using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB包管理器
/// TODO:同步加载，异步加载，单包卸载，所有包卸载
/// </summary>
public class ABmgr : BaseMgr<ABmgr>
{
    //主加载包
    private AssetBundle mainAB = null;
    //依赖包获取用的配置文件
    private AssetBundleManifest manifest = null;
    //已加载包的记录
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    //AB包存放路径
    private string ABPathUrl
    {
        get
        {
            //随平台进行修改
            return Application.streamingAssetsPath + "/";
        }
    }

    //AB包载入平台主包名
    private string MainABName
    {
        get
        {
            //判断不同平台后缀
#if UNITY_AND
            return "Android";
#elif UNITY_IOS
            return "IOS";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 预加载AB包
    /// </summary>
    /// <param name="abName"></param>
    public void LoadAB(string abName)
    {
        //加载主包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(ABPathUrl + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        //获取依赖包的相关信息
        AssetBundle ab = null;
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //判断依赖包是否被加载过
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(ABPathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }

        //加载存储依赖资源的包
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(ABPathUrl + abName);
            abDic.Add(abName, ab);
        }
    }


    #region AB包同步加载
    /// <summary>
    /// 同步加载无类型判断
    /// </summary>
    public object LoadRes(string abName,string resName)
    {
        LoadAB(abName);
        //在字典中直接提取资源，并看情况实例化
        Object abobj = abDic[abName].LoadAsset(resName);
        if (abobj is GameObject)
            return Object.Instantiate(abobj);
        else
            return abobj;
    }

    /// <summary>
    /// 同步加载有类型判断
    /// </summary>
    /// <param name="type">资源类型名</param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB(abName);
        //加入资源类型判断
        Object abobj = abDic[abName].LoadAsset(resName,type);
        if (abobj is GameObject)
            return Object.Instantiate(abobj);
        else
            return abobj;
    }

    /// <summary>
    /// 同步加载有泛型判断
    /// </summary>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        LoadAB(abName);

        T obj = abDic[abName].LoadAsset<T>(resName);
        if (obj is GameObject)
            return Object.Instantiate(obj) as T;
        else
            return obj;
    }
    #endregion

    #region AB包异步加载(资源实例化时异步)

    /// <summary>
    /// 异步加载AB包
    /// </summary>
    public void LoadResAsync(string abName, string resName,UnityAction<Object> callBack)
    {
        GameMgr.GetInstance().StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        LoadAB(abName);
        //获取AssetBundle回调
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;
        //异步加载结束后，给外部回调传递AssetBundle回调，让外部来处理
        if (abr.asset is GameObject)
            callBack(Object.Instantiate(abr.asset));
        else
            callBack(abr.asset);
    }

    /// <summary>
    /// 异步加载AB包+Type判断
    /// </summary>
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        GameMgr.GetInstance().StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        LoadAB(abName);
        //获取AssetBundle回调
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName,type);
        yield return abr;
        //异步加载结束后，给外部回调传递AssetBundle回调，让外部来处理
        if (abr.asset is GameObject)
            callBack(Object.Instantiate(abr.asset));
        else
            callBack(abr.asset);
    }

    /// <summary>
    /// 异步加载AB包+泛型判断
    /// </summary>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        GameMgr.GetInstance().StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        LoadAB(abName);
        //获取AssetBundle回调
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        //异步加载结束后，给外部回调传递AssetBundle回调，让外部来处理
        if (abr.asset is GameObject)
            callBack(Object.Instantiate(abr.asset) as T);
        else
            callBack(abr.asset as T);
    }
    #endregion

    #region AB包卸载
    /// <summary>
    /// 单包卸载
    /// </summary>
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }


    /// <summary>
    /// 所有包卸载
    /// </summary>
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
    #endregion
}
