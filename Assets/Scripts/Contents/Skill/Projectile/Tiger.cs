using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : Projectile
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
        _collider.enabled = false;

        int minus = (direction.x >= 0) ? 1 : -1;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            minus = -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            minus = -1;

        LookLeft = (minus == 1);

        if (isBTSkill)
            BTTigerDOTween(minus);
        else
            NormalTigerDOTween(minus);

    }

    private void NormalTigerDOTween(int minus)
    {
        Sequence sequence = DOTween.Sequence()
        .AppendCallback(() =>
        {
            Renderer.DOFade(1f, 0f);
            Animator.SetBool("isNormal", true);
            Animator.SetInteger("state", 1);
        })
        .Append(transform.DOMoveX(10f * minus, 1f).SetRelative().SetEase(Ease.Linear))
        .AppendCallback(() =>
        {
            Animator.SetInteger("state", 2);
            transform.DOLocalJump(new Vector3(5f * minus, 0, 0), 1, 1, 1).SetRelative();

        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            Animator.SetInteger("state", 3);
            _collider.enabled = true;
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            _collider.enabled = false;
            Animator.SetInteger("state", 4);
            DoDamage();
        })
        .Append(Renderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
        .Join(transform.DOMoveX(0.5f * minus, 0.5f).SetRelative().SetEase(Ease.Linear))
        .AppendCallback(() =>
        {
            Managers.Object.Despawn(this);
        });
        sequence.Restart();

    }

    private void BTTigerDOTween(int minus)
    {
        transform.position += new Vector3(10f * minus, 0, 0);
        Animator.SetInteger("state", 5);

        Sequence sequence = DOTween.Sequence()
        .AppendCallback(() =>
        {
            Renderer.DOFade(1f, 0f);
            transform.DOLocalJump(new Vector3(5f * minus, 0, 0), 1, 1, 1f).SetRelative();
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            _collider.enabled = true;
            Animator.SetInteger("state", 6);
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            _collider.enabled = false;
            Animator.SetInteger("state", 7);
            DoDamage();
        })
        .Append(Renderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
        .Join(transform.DOMoveX(0.5f * minus, 0.5f).SetRelative().SetEase(Ease.Linear))
        .AppendCallback(() =>
        {
            Managers.Object.Despawn(this);
        });
        sequence.Restart();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
    //    {
    //        Monster monster = other.gameObject.GetComponent<Monster>();
    //        monster.OnDamaged(Owner, Skill);
    //    }
    //}

    void DoDamage()
    {
        
        Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, Skill.SkillData.AttackRange); // 충돌한 몬스터 주변에 있는 몬스터들을 찾음
        foreach (var target in targets)
        {
            Debug.Log("Tiger DoDamage");
            target.GetComponent<Monster>().OnDamaged(Owner, Skill);
        }
    }
}
