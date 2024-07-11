using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : Item
{
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

        ItemType = Define.EItemType.BulletproofVest;
        currentProtectionHits = maxProtectionHits;
        protectionEndTime = Time.time + protectionDuration;
        isProtecting = true;

        return true;
    }

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
