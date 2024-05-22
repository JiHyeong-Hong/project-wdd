using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingBar;
    public TMP_Text loadingText;

    public GameObject startButton;

    private List<string> resourcesToLoad = new List<string>
    {
        "Art",
        "Prefabs",
        "Animations",
        //"UI/Popups"
        // 필요한 리소스를 여기에 추가
    };

    IEnumerator Start()
    {
        if (Managers.Instance.isTest)
        {
            OnLoadingComplete();
        }
        else
        {
            StartCoroutine(LoadResources<Object>());
        }

        yield return null;
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
                Debug.Log(loadedResources);
                UpdateLoadingBar(resource.GetType().Name, (float)loadedResources / totalResources);
            }
        }

        yield return YieldInstructionCache.WaitForSeconds(1f);
        OnLoadingComplete();
    }

    private void UpdateLoadingBar(string name, float progress)
    {
        loadingBar.fillAmount = progress;
        loadingText.text = $"{name}\n Data Loading...  {Mathf.RoundToInt(progress * 100)}%";
    }

    private void OnLoadingComplete()
    {
        loadingScreen.SetActive(false);
        // 로딩 완료 후 GameManager 초기화 호출
        //GameManager.Instance.InitializeManagers();
        startButton.SetActive(true);
        Debug.Log("Loading Complete");
    }

    private string GetResourcePath<T>(T resource) where T : Object
    {
        string assetPath = AssetDatabase.GetAssetPath(resource);
        string resourcePath = assetPath.Substring(assetPath.IndexOf("Resources/") + 10);
        // 파일 이름에 . 들어간 것들 수정 or 확장자 찾아서 하나씩 추가
        resourcePath = resourcePath.Substring(0, resourcePath.IndexOf("."));
        //resourcePath = resourcePath.Replace(".prefab", "").Replace(".asset", ""); // 필요한 경우 다른 확장자도 추가
        return resourcePath;
    }

    [ContextMenu("CheckAll")]
    public void TestCheckAll()
    {
        ResourceManager.Instance.CheckAllLoadedResources();
    }
}
