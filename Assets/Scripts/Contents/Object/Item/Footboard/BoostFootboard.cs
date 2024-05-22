using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostFootboard : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(1f, 1f, 1f, 0f); 
                }

                StartCoroutine(WaitForSpeedBoostToEnd(hero));
            }
        }
    }

    IEnumerator WaitForSpeedBoostToEnd(Hero hero)
    {
        // n칸 이동까지 4배 스피드
        yield return StartCoroutine(hero.SpeedBoost(3f, 4f));

        Destroy(gameObject);
    }
}
