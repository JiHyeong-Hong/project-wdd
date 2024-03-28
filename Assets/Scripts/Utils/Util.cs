﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
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

	public static Vector3 ConvertVector2ToVector3(Vector2 vector)
	{
		return new Vector3(vector.x, vector.y, 0f);
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

	/// 원형 범위 안에 적이 들어왔는지 체크
	public static bool CheckMonsterInCircle(this Transform circleTr, Vector2 monsterPos, float radius)
	{
		var circlePos = (Vector2)circleTr.position;
		return Vector2.SqrMagnitude(circlePos - monsterPos) <= Mathf.Pow(radius, 2);
	}

	/// <summary>
	/// 두 원이 겹쳤는지 체크
	/// </summary>
	/// <param name="circle1"></param>
	/// <param name="circle2"></param>
	/// <param name="radius1"></param>
	/// <param name="radius2"></param>
	/// <returns></returns>
	public static bool CheckCircleCollision(Vector2 circle1, Vector2 circle2, float radius1, float radius2)
	{
		return Vector3.SqrMagnitude(circle1 - circle2) <= Mathf.Pow(radius1 + radius2, 2);
	}
	
	/// <summary>
	/// 타겟이 화면 안에 있는지 (포착범위 내)
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public static bool CheckTargetInScreen(Vector2 position)
	{
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
		return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0;
	}


    public static List<int> SelectRandomElements(List<int> elements, int numberOfSelections)
    {
        List<int> selectedElements = new List<int>();

        for (int i = 0; i < numberOfSelections; i++)
        {
            // 요소 리스트에서 무작위로 인덱스 선택
            int randomIndex = Random.Range(0, elements.Count);

            // 선택된 요소 추가
            selectedElements.Add(elements[randomIndex]);

            // 이미 선택한 요소는 다시 선택하지 않도록 제거
            elements.RemoveAt(randomIndex);
        }

        return selectedElements;
    }
}