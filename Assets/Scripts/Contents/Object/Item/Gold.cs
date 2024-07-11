using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Item
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.Gold;

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

        // ��� ���� ����
        hero.AddGold(ItemData.Value);

        Managers.Object.Despawn(this);
    }
}
