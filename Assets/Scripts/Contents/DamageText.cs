using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float displayDuration = 1f; // �ؽ�Ʈ ǥ�� �ð�
    private float timer = 0f;   // �ؽ�Ʈ ǥ�� �ð� ������ ��������ϹǷ� �� �ð� ��� Ÿ�̸�

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
            timer += Time.deltaTime; // ����� �ð��� �߰�
            if (timer >= displayDuration) // ��� �ð��� ǥ�� �ð��� �ʰ��ϸ�
            {
                Destroy(gameObject); // ���� �ð� �Ŀ� ������Ʈ�� �ı�
            }
    }

    public void ShowDamage(float damage)
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not set.");
            return;
        }
        textMesh.text = damage.ToString(); // ������ ���� �ؽ�Ʈ�� ����
        timer = 0f; // Ÿ�̸Ӹ� �ʱ�ȭ
    }
}
