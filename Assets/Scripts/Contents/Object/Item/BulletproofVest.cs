using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : Item
{
    public int protectionCount = 5; // ��� ������ Ƚ��. ���� ����
    public float protectionDuration = 10f; // ��ź���� ���� �ð�

    private SpriteRenderer spriteRenderer; 

    public override bool Init()
    {
        if (!base.Init())
            return false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null)
            {
                StartCoroutine(ActivateProtection(hero));
            }
        }
    }

    private IEnumerator ActivateProtection(Hero hero)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); 
        }

        // ��źȿ�� �Լ� �־��
        // ����ü? �Ѿ�? ��װ� ��� �ִ��� �𸣰ڴ�

        yield return new WaitForSeconds(protectionDuration);

        Destroy(gameObject);
    }
}
