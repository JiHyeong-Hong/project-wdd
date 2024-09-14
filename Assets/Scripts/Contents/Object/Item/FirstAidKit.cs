using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FirstAidKit : Item
{    
    private float usedTransparency = 0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.FirstAidKit;        
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

        float increaseVal = PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Recovery); // 패시브 스킬 반영 @홍지형 240914        
        float amount = (float)ItemData.Value/100 + ((float)ItemData.Value/100 * increaseVal);  // 총 회복량=기본 회복량+(기본 회복량×패시브 증가율)      
        float healthToRestore = hero.MaxHp * (amount);        

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
