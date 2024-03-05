using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static float VectorToAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    public static Vector2 AngleToVector(float angle)
    {
        float radianAngle = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
    }

    public static Vector2 RotateVectorByAngle(Vector2 vector, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Vector2 rotatedVector = rotation * vector;
        return rotatedVector;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
		if (component == null)
            component = go.AddComponent<T>();
        return component;
	}

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        
        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
		}
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Vector2 GetRandomDir()
    {
        float randomAngle = Random.Range(0f, 360f);
        float radianAngle = Mathf.Deg2Rad * randomAngle;
        return new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
    }
}
