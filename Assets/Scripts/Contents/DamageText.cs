using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
    Color alpha;
    public float alphaSpeed= 0.5f;
    private float displayDuration = 0.7f; // �ؽ�Ʈ ǥ�� �ð�
    
    private float timer = 0f;   // �ؽ�Ʈ ǥ�� �ð� ������ ��������ϹǷ� �� �ð� ��� Ÿ�̸�

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        alpha = textMesh.color;
    }

    void Update()
    {
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        textMesh.color = alpha;

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
            return;
        }
        textMesh.text = damage.ToString(); // ������ ���� �ؽ�Ʈ�� ����
        timer = 0f; // Ÿ�̸Ӹ� �ʱ�ȭ
    }
}
