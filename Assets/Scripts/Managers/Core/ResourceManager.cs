using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
{
    private Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();

    public void CheckAllLoadedResources()
    {
        foreach (var item in loadedResources)
        {
            Debug.Log(item);
        }
    }

    public GameObject Load(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {path}");
        }
        return prefab;
    }

    public void LoadResource<T>(string path) where T : Object
    {
        T resource = Resources.Load<T>(path);
        if (resource != null)
        {
            loadedResources[path] = resource;
            Debug.Log($"Resource loaded: {path}");
        }
        else
        {
            Debug.LogError($"Failed to load resource: {path}");
        }
    }

    public T GetResource<T>(string path) where T : Object
    {
        if (loadedResources.ContainsKey(path))
        {
            return loadedResources[path] as T;
        }
        else
        {
            Debug.LogError($"Resource not found: {path}");
            return null;
        }
    }

    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

    internal Sprite GetSkillSprite(string skillName)
    {
        Sprite sprite = Load<Sprite>($"Art/Skills/{skillName}");
        if(sprite == null) Debug.LogWarning($"Failed to load sprite : {skillName}");
        return sprite;
    }
}
