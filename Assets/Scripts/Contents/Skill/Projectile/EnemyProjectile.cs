using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    //TODO Eung 투사체 실험용 코드 삭제해야함 - 프리팹도 삭제해야함
    private Hero target;
    private Vector2 dir;
    private CircleCollider2D col;
    private int type;
    public override bool Init()
    {
        if (!base.Init())
            return false;
		
        if (col == null)
            col = GetComponent<CircleCollider2D>();
        
        //TODO 투사체 테이블의 이미지를 넣는방식으로 변경
        Renderer = GetComponent<SpriteRenderer>();
        isInfinityDuration = true;
		
        return true;
    }

    public void SetImage()
    {
        Renderer.sprite = Managers.Resource.Load<Sprite>(ProjectileData.ImageDataurl);
    }
    public void SetTarget(Hero target)
    {
        this.target = target;
        // col.enabled = target == null;
        dir = (target.transform.position - transform.position).normalized;
    }
    
    protected override void Move()
    {
        //TODO 투사체 idx or type 넘버로 투사체 구분하여 이동 구현
        switch (type)
        {
            case 1:
                transform.Translate(Vector2.up * (ProjectileData.MoveSpeed * Time.deltaTime));
                break;
            
            default:
                transform.Translate(Vector2.up * (ProjectileData.MoveSpeed * Time.deltaTime));
                break;
        }
        if(!Util.CheckTargetInScreen(transform.position))
            Managers.Object.Despawn(this);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //if (((1 << (int)Define.ELayer.Hero) & (1 << col.gameObject.layer)) != 0)
        //{
        //    col.GetComponent<Hero>().OnDamaged(this,Skill);
        //    Managers.Object.Despawn(this);
        //}

        if (((1 << (int)Define.ELayer.Hero) & (1 << col.gameObject.layer)) != 0)
        {
            Hero hero = col.GetComponent<Hero>();
            if (hero != null)
            {
                if (hero.IsInvincible) 
                {
                    // Hero가 무적 상태일 때의 처리
                    hero.OnHitByProjectile(); // 보호 횟수 감소 
                    Managers.Object.Despawn(this); // 투사체 제거
                    return; // 추가 피해 처리 방지
                }
                // 무적 상태가 아니면 일반적인 피해 처리
                hero.OnDamaged(this, Skill); // 공격자 정보와 스킬 정보를 넘겨줌
                Managers.Object.Despawn(this); // 투사체 제거
            }
        }
    }
}
