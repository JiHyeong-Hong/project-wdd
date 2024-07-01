using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.Key;

        return true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        //TODO 키 카운트 작성
        // hero.Exp += ItemData.Value;
        // Debug.Log($"����ġ {ItemData.Value}��ŭ ȹ��. �� ����ġ : {hero.Exp}");

        Managers.Object.Despawn(this);
    }
}
