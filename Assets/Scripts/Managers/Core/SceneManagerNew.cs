using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerNew : SingletonMonoBehaviour<SceneManagerNew>
{
    public event System.Action OnSceneLoaded;
    private AsyncOperation asyncOperation;

    protected override void Init()
    {
        // 씬 관련 초기화 작업을 수행합니다.
        Debug.Log("SceneManager initialized.");
    }

    public void LoadScene(Define.EScene scene)
    {
        StartCoroutine(LoadSceneAsync(SceneManager.GetSceneAt((int)scene).name));
    }

    private void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 씬이 완전히 로드된 후에 활성화합니다.
            if (asyncOperation.progress >= 0.9f)
            {
                Debug.Log("Scene load complete.");
                asyncOperation.allowSceneActivation = true;
                OnSceneLoaded?.Invoke();
            }

            yield return null;
        }
    }

    public bool IsSceneLoaded()
    {
        return asyncOperation != null && asyncOperation.isDone;
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
        Debug.Log($"Next Scene {nextSceneIndex} loaded.");
        Debug.Log($"Next Scene Name {SceneManager.GetSceneAt(nextSceneIndex).name} ");
    }

    public void LoadingBar()
    {

    }

}
