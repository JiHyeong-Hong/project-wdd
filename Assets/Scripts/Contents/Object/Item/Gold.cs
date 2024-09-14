using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Item
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.Gold;

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
            
                              

        SoundManager.Instance.Play(Define.ESoundMainType.Item, Define.ESoundType.Item);
		// 골드 누적 로직        
        //hero.AddExp(10); // 테스트용, 삭제가능.  @홍지형
        hero.AddGold(ItemData.Value);

        Managers.Object.Despawn(this);
    }
}
