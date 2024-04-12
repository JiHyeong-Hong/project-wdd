using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : Item
{
    public int protectionCount = 5; // 방어 가능한 횟수. 아직 미정
    public float protectionDuration = 10f; // 방탄조끼 지속 시간

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

        // 방탄효과 함수 넣어야
        // 투사체? 총알? 얘네가 어디에 있는지 모르겠다

        yield return new WaitForSeconds(protectionDuration);

        Destroy(gameObject);
    }
}
