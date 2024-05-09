using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trumpet : BaseObject
{
    private float usedTransparency = 0f;

    public override bool Init()
    {
        if (!base.Init())
            return false;

        ObjectType = Define.EObjectType.Item;

        return true;
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

        //
        //트럼펫 기능 채우기
        ///

        Managers.Object.Despawn(this);
    }
}
