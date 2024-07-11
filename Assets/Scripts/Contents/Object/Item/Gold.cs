using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : BaseObject
{
    public int value;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Gold;

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

        // 골드 누적 로직
        hero.AddGold(value);
        
        hero.AddExp(10); // 테스트용, 삭제가능.  @홍지형

        Managers.Object.Despawn(this);
    }
}
