using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("@"+ typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }

                (instance as SingletonMonoBehaviour<T>)?.Init();
            }

            return instance;
        }
    }

    protected virtual void Init() { }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            //gameObject.transform.SetParent(Managers.Instance.transform);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}