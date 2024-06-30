using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Item
{
    public int value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        // ��� ���� ����
        hero.AddGold(value);

        Managers.Object.Despawn(this);
    }
}
