using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    //TODO Eung 투사체 실험용 코드 삭제해야함 - 프리팹도 삭제해야함
    private Hero target;
    private Vector2 dir;
    private CircleCollider2D col;
    private float moveSpeed = 5f;
    public override bool Init()
    {
        if (!base.Init())
            return false;
		
        if (col == null)
            col = GetComponent<CircleCollider2D>();
		
        Renderer = GetComponentInChildren<SpriteRenderer>();
        isInfinityDuration = true;
		
        return true;
    }
    
    public void SetTarget(Hero target)
    {
        this.target = target;
        col.enabled = target == null;
        dir = (target.transform.position - transform.position).normalized;
    }
    
    protected override void Move()
    {
        if (target != null)
        {
            transform.Translate(dir * moveSpeed * Time.deltaTime);

            if (Vector2.SqrMagnitude(dir) <= Mathf.Pow(0.5f,2))
            {
                target.OnDamaged(Owner,Skill);
                Managers.Object.Despawn(this);
            }
        }
        else
        {
            transform.Translate(Vector2.up * (moveSpeed* Time.deltaTime));
        }
        if(!Util.CheckTargetInScreen(transform.position))
            Managers.Object.Despawn(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
        {
            col.GetComponent<Hero>().OnDamaged(Owner,Skill);
            Managers.Object.Despawn(this);
        }
    }
}
