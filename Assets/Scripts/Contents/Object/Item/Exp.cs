using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : Item
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        hero.Exp += ItemData.Value;
        Debug.Log($"����ġ {ItemData.Value}��ŭ ȹ��. �� ����ġ : {hero.Exp}");

        Managers.Object.Despawn(this);
    }
}
