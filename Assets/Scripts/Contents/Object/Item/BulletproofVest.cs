using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : BaseObject
    // Item ��ũ��Ʈ�� ��� �����۵� �ƿ츣�� �������� �˰�
    // �ƹ� �������� ��ӹ޾Ƽ� �ڵ� �� ®�µ� 
    // Item ��ũ��Ʈ�� ����ġ���� �����̾���
    // �ϴ� Baseobject ������� �ٲٱ� �ߴ��� ������ ���� �ȳ���
{
    // �ϴ�...
    //public Data.ItemData ItemData { get; private set; }

    public int maxProtectionHits = 5; // ��� ������ Ƚ��. ���� ����
    public float protectionDuration = 10f; // ��ź���� ���� �ð�
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
    
    // �ϴ�...
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
