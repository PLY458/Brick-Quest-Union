using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏物体工厂模板类
/// 1.控制不同场景下的物体生产
/// </summary>
public abstract class GameObjectFactory : ScriptableObject
{
    /// <summary>
    /// 工厂所处的场景
    /// </summary>
    Scene factoryScence;

    /// <summary>
    /// 根据预制体实例化游戏物体
    /// </summary>
    protected T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour
    {
        if (!factoryScence.isLoaded)
        {
            if (Application.isEditor)
            {
                factoryScence = SceneManager.GetSceneByName(name);
                if (!factoryScence.isLoaded)
                {
                    factoryScence = SceneManager.CreateScene(name);
                }
            }
            else
            {
                factoryScence = SceneManager.CreateScene(name);
            }
        }
        T instance = Instantiate(prefab);
        //保证生产好的游戏物体放置在工厂场景中
        SceneManager.MoveGameObjectToScene(instance.gameObject, factoryScence);
        return instance;
    }

}