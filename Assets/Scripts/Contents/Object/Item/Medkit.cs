using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : BaseObject
{
    public float healthRestorePercent = 50.0f;  // 몇퍼 회복시킬지 아직 미정. 임의값 50
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

        float healthToRestore = hero.MaxHp * (healthRestorePercent / 100.0f);
        hero.Hp = Mathf.Min(hero.Hp + healthToRestore, hero.MaxHp);

        if (Renderer != null)
        {
            Color color = Renderer.material.color;
            color.a = usedTransparency;
            Renderer.material.color = color;
        }

        Managers.Object.Despawn(this); 
    }
}
