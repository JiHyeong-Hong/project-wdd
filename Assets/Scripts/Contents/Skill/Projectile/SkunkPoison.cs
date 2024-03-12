using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스컹크 지속 데미지 장판 스킬 클래스. @홍지형
public class SkunkPoison : Projectile
{
    private CapsuleCollider2D _collider;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);

        _collider = GetComponent<CapsuleCollider2D>();
        _collider.enabled = true;

        Sequence sequence = DOTween.Sequence()
            .Append(Renderer.DOFade(1f, 0.75f))
            .AppendCallback(() =>
            {
                Animator.SetInteger("state", 1);
            })
             .InsertCallback(1.2f, () =>
             {
                 Animator.SetInteger("state", 2);
             })
            .AppendCallback(() =>
            {
                Animator.SetInteger("state", 3);
            })
            .AppendInterval(0.5f)
            .InsertCallback(2.0f, () =>
            {
                Animator.SetInteger("state", 4);
            })
            .Append(Renderer.DOFade(0f, 0.75f).SetEase(Ease.Linear))
            .InsertCallback(2.5f, () =>
            {
                Managers.Object.Despawn(this);
            });

    }


    private void Update()
    {
   

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.OnDamaged(Owner, Skill);
        }
    }
}
