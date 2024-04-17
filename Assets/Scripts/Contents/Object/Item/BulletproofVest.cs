using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : BaseObject
    // Item 스크립트가 모든 아이템들 아우르는 구조인줄 알고
    // 아무 생각없이 상속받아서 코드 다 짰는데 
    // Item 스크립트는 경험치구슬 내용이었다
    // 일단 Baseobject 상속으로 바꾸긴 했는지 오류가 날지 안날지
{
    // 일단...
    //public Data.ItemData ItemData { get; private set; }

    public int maxProtectionHits = 5; // 방어 가능한 횟수. 아직 미정
    public float protectionDuration = 10f; // 방탄조끼 지속 시간
    private float usedTransparency = 0f;

    private int currentProtectionHits;
    private float protectionEndTime;
    private bool isProtecting;

    public override bool Init()
    {
        if (!base.Init())
            return false;

        ObjectType = Define.EObjectType.Item;

        currentProtectionHits = maxProtectionHits;
        protectionEndTime = Time.time + protectionDuration;
        isProtecting = true;

        return true;
    }
    
    // 일단...
    //public void SetInfo(int dataTemplateID)
    //{
    //    ItemData = Managers.Data.ItemDic[dataTemplateID];
    //    Renderer.sortingOrder = SortingLayers.ITEM;
    //
    //    Sprite sprite = Managers.Resource.Load<Sprite>(ItemData.IconPath);
    //    Renderer.sprite = sprite;
    //}

    private void Update()
    {
        if (isProtecting && Time.time >= protectionEndTime)
        {
            EndProtection();
        }
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

        else
        {
            Debug.LogError("Renderer not found on BulletproofVest object!");
        }

        hero.AddProtection(this); 

        Invoke(nameof(EndProtection), protectionDuration);
    }

    private void EndProtection()
    {
        isProtecting = false;
        Managers.Object.Despawn(this);
    }
}
