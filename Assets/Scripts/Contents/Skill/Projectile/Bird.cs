using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : Projectile
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        return true;
    }

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);

        transform.rotation = Quaternion.identity;

        Collider.enabled = false;
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(0, 0))
            .Append(transform.DOScale(0.2f, 0.5f).SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                Collider.enabled = true;
            });
        sequence.Restart();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void Clear(TweenCallback callback)
    {
        Collider.enabled = false;

        Sequence sequence = DOTween.Sequence()
        .Append(transform.DOScale(0.2f, 0))
        .Append(transform.DOScale(0, 0.5f).SetEase(Ease.Linear))
        .AppendCallback(callback);
        sequence.Restart();
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
