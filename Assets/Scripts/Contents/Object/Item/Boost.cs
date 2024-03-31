using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : Footboard
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null)
            {
                // n칸 이동까지 4배 스피드
                StartCoroutine(hero.SpeedBoost(2f, 4f));
            }
        }
    }
}
