using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : Item
{
    private float usedTransparency = 0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.Magnet;

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

        if (Renderer != null)
        {
            Color color = Renderer.material.color;
            color.a = usedTransparency;
            Renderer.material.color = color;
        }

        // ���� �ִ� ��� ����ġ ���� ã��
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