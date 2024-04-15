using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 길리슈터 - 발사체 클래스. @홍지형, EnemyProjectile 참조
public class GullieBullet : Projectile
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
        if (!Util.CheckTargetInScreen(transform.position)) // TODO: 240415 현재 인게임 화면 밖에서 발사하지 않게 되어 있음.
            Managers.Object.Despawn(this);
    }

    // TODO: 응찬님께 충돌판정 확인하기. 현재 동작 x240406
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << (int)Define.ELayer.Hero) & (1 << col.gameObject.layer)) != 0)
        {
            col.GetComponent<Hero>().OnDamaged(Managers.Object.Hero, Skill);
            Managers.Object.Despawn(this);
        }
    }
}
