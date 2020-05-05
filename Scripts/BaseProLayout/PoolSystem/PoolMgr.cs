using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEditor.iOS;
using UnityEngine.Events;


/*
 *知识点：
 * 1.Dictionary,list
 * 2.C#内存分配
 * 
 */
public class PoolMgr : BaseManeger<PoolMgr>
{
    //缓存池容器（查询表）
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    public GameObject PoolObj;

    //出表
    public void GetPoolobj(string name,UnityAction<GameObject> callback)
    {
        
        if (poolDic.ContainsKey(name) && poolDic[name].PoolList.Count>0)
        {
            callback(poolDic[name].GetObj());
        }
        else if(poolDic.ContainsKey(name) == false)
        {
           ResourcesMgr.GetInstance().LoadAsync<GameObject>(name, (o) =>
           {
               o.name = name;
               callback(o);
           });
        }
       
    }
    //入表
    public void PushPoolobj(string name, GameObject obj)
    {
        if (PoolObj == null)
        {
            PoolObj = new GameObject("Pool");
        }

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name,new PoolData(obj,PoolObj));
        }

    }

    //清表
    public void Clear()
    {
        poolDic.Clear();
        PoolObj = null;
    }
}
