using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : BaseObject
{
    public override bool Init()
    {
        if (!base.Init())
            return false;

        ObjectType = Define.EObjectType.Item;

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

        // 씬에 있는 모든 경험치 구슬을 찾아서 모은다
        Item[] items = FindObjectsOfType<Item>();
        
        foreach (Item item in items)
        {
            if (item != null && item.ItemData != null)
            {
                hero.Exp += item.ItemData.Value; 
                Managers.Object.Despawn(item);  
            }
        }

        Managers.Object.Despawn(this);
    }
}
