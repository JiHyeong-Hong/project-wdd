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
        Renderer.sortingOrder = SortingLayers.ITEM;

        Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
        Renderer.sprite = sprite;
        
        this.name = ItemType.ToString();
    }
    
}
