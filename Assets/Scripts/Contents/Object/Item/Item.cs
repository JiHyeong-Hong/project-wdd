using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : BaseObject
    /// 경험치 구슬 스크립트
{
    public Data.ItemData ItemData { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Item;

        return true;
    }

   //public void SetInfo(int dataTemplateID)
   //{
   //    ItemData = Managers.Data.ItemDic[dataTemplateID];
   //    Renderer.sortingOrder = SortingLayers.ITEM;
   //
   //    Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
   //    Renderer.sprite = sprite;
   //}
    public void SetInfo(int dataTemplateID)
    {
        if (!Managers.Data.ItemDic.ContainsKey(dataTemplateID))
        {
            Debug.LogError("Invalid dataTemplateID: " + dataTemplateID);
            return;
        }

        ItemData = Managers.Data.ItemDic[dataTemplateID];
        Renderer.sortingOrder = SortingLayers.ITEM;

        Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
        if (sprite == null)
        {
            Debug.LogError("Sprite not found at path: " + ItemData.IconPath);
            return;
        }
        Renderer.sprite = sprite;

        Debug.Log("Item data set for ID: " + dataTemplateID);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
    
        if (target.IsValid() == false)
        {
            return;
        }
    
        Hero hero = target as Hero;
        if (hero == null)
        {
            return;
        }

        if (ItemData == null)
        {
            Debug.Log("ItemData is null.");
            return;
        }

        hero.Exp += ItemData.Value;
    
        Managers.Object.Despawn(this);
    }
}
