using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 游戏物体运行线程类
/// TODO: 给场景中的游戏实例提供Mono组件
/// </summary>
public abstract class GameBehavior : MonoBehaviour
{
    public virtual bool GameUpdate() => true;

    public abstract void Recycle();
}
