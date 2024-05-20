using Data;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using static Define;

// 도망치는 관광객 클래스. @홍지형
public class Tourist : Monster
{    
    private Vector3 dest; // 정해진 목적지 방향 

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Monster;

        _hero = Managers.Object.Hero;
        return true;
    }

    public void SetDirection(Vector3 dir)
    {
        dest = dir;
        
        // 회전, 현재 반대로 되어 있어 사용X
        //float angle = Util.VectorToAngle(dest);
        //transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    // 군집의 움직임으로 재정의
    protected override void UpdateMove()
    {
        transform.Translate(dest * (MoveSpeed * Time.deltaTime));

        // 플레이어와의 거리가 일정 거리보다 멀리 떨어졌을 때 관광객 삭제
        float distance = Vector3.Distance(transform.position, _hero.transform.position);
        if(distance > 15f) 
        {
            Managers.Object.Despawn(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
            return;

        target.OnDamaged(this, null);
    }
}