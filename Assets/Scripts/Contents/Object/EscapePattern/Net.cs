using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 그물망 클래스. @홍지형
public class Net : Monster
{
    private float prevSpeed; // 플레이어의 원래이동속도
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Monster;
        
        StartCoroutine(LifeTime());
       
        return true;
    }

    // 일정시간 생성되어 있다가 자동 파괴된다.
    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(5f);

        StopCoroutine(LifeTime());

        Managers.Object.Despawn(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType == Define.ECreatureType.Hero)
        {
            Hero hero = creature as Hero;
            if (hero != null)
            {
                if(hero.isInNet == false)
                {
                    hero.SpeedReduce(0.4f); // 감소량 현재 하드코딩. 240720 @ 홍지형                    
                    target.OnDamaged(this, null); // 그물망은 데미지 없음.
                    hero.isInNet = true;
                }


                return;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType == Define.ECreatureType.Hero)
        {
            Hero hero = creature as Hero;
            if (hero != null)
            {
                hero.SpeedReset(); // 원래 이동속도로 복원
                hero.isInNet = false;
                target.OnDamaged(this, null); // 그물망은 데미지 없음.
                return;
            }
        }
    }


}