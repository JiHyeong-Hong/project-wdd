using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : BaseObject
{
    public Data.ItemData ItemData { get; private set; }

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
        Renderer.sortingOrder = SortingLayers.ITEM;

        Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
        Renderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        hero.Exp += ItemData.Value;

        Managers.Object.Despawn(this);
    }
}
