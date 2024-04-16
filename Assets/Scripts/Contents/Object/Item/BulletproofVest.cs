using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletproofVest : Item
{
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

        hero.AddProtection(this);  // Suppose Hero has a method to handle protection effects

        Invoke(nameof(EndProtection), protectionDuration);
    }

    private void EndProtection()
    {
        isProtecting = false;
        Managers.Object.Despawn(this);
    }
}
