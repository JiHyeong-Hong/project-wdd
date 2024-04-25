using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : BaseObject
{
    public Data.ItemData GoldData { get; private set; }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        ObjectType = Define.EObjectType.Item;  

        return true;
    }

    public void SetInfo(int dataTemplateID)
    {
        GoldData = Managers.Data.ItemDic[dataTemplateID];
        Renderer.sortingOrder = SortingLayers.ITEM;

        Sprite sprite = Managers.Resource.Load<Sprite>(GoldData.IconPath);
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

        if (GoldData == null)
            return;

        hero.Gold += GoldData.Value;

        Managers.Object.Despawn(this);
    }
}
