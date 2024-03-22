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
                // �ӵ��� 4��� ������Ű��, 3�� �� ���� �ӵ��� ���ƿ��� ��
                StartCoroutine(hero.SpeedBoost(3f, 4f));
            }
        }
    }
}
