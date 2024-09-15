using System;
using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : BaseObject
{
    public ItemData ItemData { get; private set; }

    public EItemType ItemType { get; protected set; }

    public bool isMove;
    
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
    
    public static IEnumerator Move(Item item)
    {
        
        while (true)
        {
            
            item.transform.position = 
                Vector3.MoveTowards(item.transform.position, 
                    Managers.Object.Hero.transform.position, 
                    3f * Time.deltaTime);
            //Debug.Log("이동중!!");

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
    }

    // Manager reset 시 삭제.
    public void ResetDatas()
    {
        Managers.Object.Despawn(this);
    }
}
