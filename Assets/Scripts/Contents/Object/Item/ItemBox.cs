using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Monster
{
    private float usedTransparency = 0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = Define.ECreatureType.Box;

        return true;
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     BaseObject target = other.GetComponent<BaseObject>();
    //     if (target.IsValid() == false)
    //         return;
    //
    //     Hero hero = target as Hero;
    //     if (hero == null)
    //         return;
    //
    //     /// �����۹ڽ� ����
    //
    //     if (Renderer != null)
    //     {
    //         Color color = Renderer.material.color;
    //         color.a = usedTransparency;
    //         Renderer.material.color = color;
    //     }
    //
    //     Managers.Object.Despawn(this);
    // }
}