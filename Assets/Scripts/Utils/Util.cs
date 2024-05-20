using System.Collections;
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

	public static T FindParent<T>(GameObject go, string name = null) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        Transform transform = go.transform;
        while (transform.parent != null)
        {
            transform = transform.parent;
            if (string.IsNullOrEmpty(name) || transform.name == name)
            {
                T component = transform.GetComponent<T>();
                if (component != null)
                    return component;
            }
        }

        return null;
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
        List<int> elementsPool = new List<int>(elements);
        for (int i = 0; i < numberOfSelections; i++)
        {
            // 요소 리스트에서 무작위로 인덱스 선택
            int randomIndex = Random.Range(0, elementsPool.Count);

            // 선택된 요소 추가
            selectedElements.Add(elementsPool[randomIndex]);

            // 이미 선택한 요소는 다시 선택하지 않도록 제거
            elementsPool.RemoveAt(randomIndex);
        }

        return selectedElements;
    }
    public static Collider2D[] SearchCollidersInRadius(Vector3 position , float range)
    {
        // 지정된 중심점 주변에 있는 모든 콜라이더를 반환
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range, 1 << (int)Define.ELayer.Monster);
        return colliders;
    }

    public static IEnumerator DrawCircle(Vector3 position, float radius, int segments, Color color, float duration)
    {
		float currentTime = 0f;

		while (currentTime <= duration)
		{
			currentTime += Time.deltaTime;
			
			// Single segment of the circle covers (360 / number of segments) degrees
			float angleStep = (360.0f / segments);

			// Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
			// which are required by Unity's Mathf class trigonometry methods

			angleStep *= Mathf.Deg2Rad;

			// lineStart and lineEnd variables are declared outside of the following for loop
			Vector3 lineStart = Vector3.zero;
			Vector3 lineEnd = Vector3.zero;

			for (int i = 0; i < segments; i++)
			{
				// Line start is defined as starting angle of the current segment (i)
				lineStart.x = Mathf.Cos(angleStep * i);
				lineStart.y = Mathf.Sin(angleStep * i);

				// Line end is defined by the angle of the next segment (i+1)
				lineEnd.x = Mathf.Cos(angleStep * (i + 1));
				lineEnd.y = Mathf.Sin(angleStep * (i + 1));

				// Results are multiplied so they match the desired radius
				lineStart *= radius;
				lineEnd *= radius;

				// Results are offset by the desired position/origin 
				lineStart += position;
				lineEnd += position;

				// Points are connected using DrawLine method and using the passed color
				Debug.DrawLine(lineStart, lineEnd, color);
			}
			
			yield return null;
		}
    }

	public static float TimeForOneRotation(float speed)
	{
        float degreesPerSecond = speed * Mathf.Rad2Deg;
        float timeForOneRotation = 360f / degreesPerSecond;

        return timeForOneRotation;
	}

    public static Sprite Load(string imagePath, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>(imagePath);

        foreach (var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }

    public static List<T> SelectUniqueElements<T>(List<T> lst, int n)
    {
        List<T> uniqueElements = new List<T>();
        List<T> remainingElements = new List<T>(lst);

        System.Random random = new System.Random();

        while (uniqueElements.Count < n && remainingElements.Count > 0)
        {
            int randomIndex = random.Next(remainingElements.Count);
            T element = remainingElements[randomIndex];
            uniqueElements.Add(element);
            remainingElements.RemoveAt(randomIndex);
        }

        return uniqueElements;
    }

	public static Vector2 LimitScreenConverter(Vector2 target, Vector2 uiSize, GameObject invisibleObject)
	{
        // 해상도를 구한다.
        float width = Screen.width;
        float height = Screen.height;
        
		// 화면밖으로 나가지 않도록 제한
        Vector2 screenPos = Camera.main.WorldToScreenPoint(target);

		if (Mathf.Abs(screenPos.x) >= width - uiSize.x || Mathf.Abs(screenPos.y) >= height - uiSize.y)
		{
			if(invisibleObject.activeSelf == false ) invisibleObject.SetActive(true);
			
			screenPos.x = Mathf.Clamp(screenPos.x, uiSize.x, width - uiSize.x);
			screenPos.y = Mathf.Clamp(screenPos.y, uiSize.y, height - uiSize.y);
		}
		else
		{
            if (invisibleObject.activeSelf == true) invisibleObject.SetActive(false);
        }

        return screenPos;
    }

    public static IEnumerator FadeOut(SpriteRenderer spriteRenderer, float duration, System.Action onComplete = null)
	{
        Color originalColor = spriteRenderer.color;
        float startAlpha = originalColor.a;
        float time = 0;

        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(startAlpha, 0, t);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            time += Time.deltaTime;
			//Debug.Log(time);
            yield return null;
        }
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        onComplete?.Invoke();
    }

}