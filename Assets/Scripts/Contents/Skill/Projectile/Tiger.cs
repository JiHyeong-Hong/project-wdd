using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        
        Sequence sequence = DOTween.Sequence()
            .Append(Renderer.DOFade(1f, 0f))
            .Append(transform.DOMoveX(10f * minus, 1f).SetRelative().SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                Animator.SetInteger("state", 1);
            })
            .Append(transform.DOLocalJump(new Vector3(5f * minus, 0, 0), 1, 1, 0.5f).SetRelative())
            .InsertCallback(1.2f, () =>
            {
                Animator.SetInteger("state", 2);
            })
            .AppendCallback(() =>
            {
                Animator.SetInteger("state", 3);
                _collider.enabled = true;
            })
            .AppendInterval(0.5f)
            .InsertCallback(2.0f, () =>
            {
                _collider.enabled = false;
                Animator.SetInteger("state", 4);
            })
            .Append(Renderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
            .Join(transform.DOMoveX(0.5f * minus, 0.5f).SetRelative().SetEase(Ease.Linear))
            .InsertCallback(2.5f, () =>
            {
                Managers.Object.Despawn(this);
            });
        sequence.Restart();

    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
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
