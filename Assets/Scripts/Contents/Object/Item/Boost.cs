using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null)
            {
                // 속도를 4배로 증가시키고, 3초 후 원래 속도로 돌아오게 함
                StartCoroutine(hero.SpeedBoost(3f, 4f));
            }
        }
    }
}
