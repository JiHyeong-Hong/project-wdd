using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float displayDuration = 1f; // 텍스트 표시 시간
    private float timer = 0f;   // 텍스트 표시 시간 지나면 사라져야하므로 그 시간 재는 타이머

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is missing.");
        }
    }

    void Update()
    {
            timer += Time.deltaTime; // 경과된 시간을 추가
            if (timer >= displayDuration) // 경과 시간이 표시 시간을 초과하면
            {
                Destroy(gameObject); // 일정 시간 후에 오브젝트를 파괴
            }
    }

    public void ShowDamage(float damage)
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not set.");
            return;
        }
        textMesh.text = damage.ToString(); // 데미지 값을 텍스트로 설정
        timer = 0f; // 타이머를 초기화
    }
}
