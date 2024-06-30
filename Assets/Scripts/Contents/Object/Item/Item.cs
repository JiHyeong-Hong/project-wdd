using System;
using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Item : BaseObject
{
    public ItemData ItemData { get; private set; }

    public EItemType ItemType { get; protected set; }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Item;

        return true;
    }

    public void SetInfo(int dataTemplateID)
    {
        ItemData = Managers.Data.ItemDic[dataTemplateID];
        ItemType = (EItemType)ItemData.Type;
        Renderer.sortingOrder = SortingLayers.ITEM;

        Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
        Renderer.sprite = sprite;
        
        this.name = ItemType.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;
    
        Hero hero = target as Hero;
        if (hero == null)
            return;
        switch (ItemType)
        {
            case EItemType.Exp:
                hero.Exp += ItemData.Value;
                Debug.Log($"����ġ {ItemData.Value}��ŭ ȹ��. �� ����ġ : {hero.Exp}");
                break;
            case EItemType.Gold:
                hero.AddGold(ItemData.Value);
                break;
            case EItemType.Magnet:
                if (Renderer != null)
                {
                    Color color = Renderer.material.color;
                    color.a = 0f;
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
                break;
            case EItemType.Trumpet:
                break;
            case EItemType.Medkit:
                float healthToRestore = hero.MaxHp * (50f / 100.0f);
                hero.Hp = Mathf.Min(hero.Hp + healthToRestore, hero.MaxHp);

                if (Renderer != null)
                {
                    Color color = Renderer.material.color;
                    color.a = 0f;
                    Renderer.material.color = color;
                }
                break;
            case EItemType.BulletproofVest:
                break;
                
        }

        Managers.Object.Despawn(this);
    }

    
}
