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
    private Rigidbody2D rb;    
    private Vector3 dest; // 정해진 목적지 방향 
    private Vector3 targetOffset; // 플레이어 주변의 타겟 오프셋
    private Vector3 targetPosition;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Monster;

        _hero = Managers.Object.Hero;
        rb = GetComponent<Rigidbody2D>();        

        return true;
    }
    public void SetDirection(Vector3 dir)
    {
        dest = dir;
        // 회전
        float angle = Util.VectorToAngle(dest);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));        
    }

    // 군집의 움직임으로 재정의
    protected override void UpdateMove()
    {
        transform.Translate(dest * (MoveSpeed * Time.deltaTime));
        if (!Util.CheckTargetInScreen(transform.position)) // TODO: 240415 현재 인게임 화면 밖에서 발사하지 않게 되어 있음.
            Managers.Object.Despawn(this);

        //if (_hero.IsValid())
        //{
        //    SetRigidbodyVelocity(dest * MoveSpeed);
        //}
        //else
        //    SetRigidbodyVelocity(Vector2.zero);
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