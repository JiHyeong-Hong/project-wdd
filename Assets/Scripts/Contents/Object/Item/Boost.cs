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
                // nĭ �̵����� 4�� ���ǵ�
                StartCoroutine(hero.SpeedBoost(2f, 4f));
            }
        }
    }
}
