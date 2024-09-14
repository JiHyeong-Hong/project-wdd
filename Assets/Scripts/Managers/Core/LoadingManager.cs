using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingBar;
    public TMP_Text loadingText;

    public GameObject startButton;

    private List<string> resourcesToLoad = new List<string>
    {        
        "Art/Creature",
        "Art/Effects",
        "Art/EscapePatterns",
        "Art/Item",
        "Art/Jail",
        "Art/Materials",
        "Art/Projectile",
        "Art/Sign",
        "Art/Skills",
        "Prefabs",
        "Animations",
        //"UI/Popups"
        // �ʿ��� ���ҽ��� ���⿡ �߰�
    };

    IEnumerator Start()
    {
        // if (Managers.Instance.isTest)
        // {
        //     OnLoadingComplete();
        // }
        // else
        // {
            StartCoroutine(LoadResources<Object>());
        // }
       

        yield return null;
        SaveManager.Instance.LoadData();

    }

    private IEnumerator LoadResources<T>() where T : Object
    {
        loadingScreen.SetActive(true);

        int totalResources = 0;
        int loadedResources = 0;

        //foreach (string folderPath in resourcesToLoad)
        //{
        //    T[] resources = Resources.LoadAll<T>(folderPath);
        //    totalResources += resources.Length;
        //}
        //Debug.Log($"totalResources : {totalResources}");
        
        DataManager.Instance.Init();
        // SoundManager.Instance.Init();

        foreach (string folderPath in resourcesToLoad)
        {
            T[] resources = Resources.LoadAll<T>(folderPath);
            totalResources += resources.Length;

            foreach (T resource in resources)
            {
                string path = GetResourcePath(resource);
                ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);

                while (!resourceRequest.isDone)
                {
                    UpdateLoadingBar(resource.GetType().Name, (float)loadedResources / totalResources + resourceRequest.progress / totalResources);
                    yield return YieldInstructionCache.WaitForEndOfFrame;
                }

                ResourceManager.Instance.LoadResource<T>(path);
                loadedResources++;
                // Debug.Log(loadedResources);
                UpdateLoadingBar(resource.GetType().Name, (float)loadedResources / totalResources);
            }
        }

        yield return YieldInstructionCache.WaitForSeconds(1f);
        OnLoadingComplete();
    }

    private void UpdateLoadingBar(string name, float progress)
    {
        loadingBar.fillAmount = progress;
        // loadingText.text = $"{name}\n Data Loading...  {Mathf.RoundToInt(progress * 100)}%";
        loadingText.text = $"Resource Data Loading...  {Mathf.RoundToInt(progress * 100)}%";
    }

    private void OnLoadingComplete()
    {
        loadingScreen.SetActive(false);
        // �ε� �Ϸ� �� GameManager �ʱ�ȭ ȣ��
        //GameManager.Instance.InitializeManagers();
        startButton.SetActive(true);
        Debug.Log("Loading Complete");
    }

    private string GetResourcePath<T>(T resource) where T : Object
    {
        string assetPath = AssetDatabase.GetAssetPath(resource);
        string resourcePath = assetPath.Substring(assetPath.IndexOf("Resources/") + 10);
        // ���� �̸��� . �� �͵� ���� or Ȯ���� ã�Ƽ� �ϳ��� �߰�
        resourcePath = resourcePath.Substring(0, resourcePath.IndexOf("."));
        //resourcePath = resourcePath.Replace(".prefab", "").Replace(".asset", ""); // �ʿ��� ��� �ٸ� Ȯ���ڵ� �߰�
        return resourcePath;
    }

    // [ContextMenu("CheckAll")]
    // public void TestCheckAll()
    // {
    //     ResourceManager.Instance.CheckAllLoadedResources();
    // }
}
