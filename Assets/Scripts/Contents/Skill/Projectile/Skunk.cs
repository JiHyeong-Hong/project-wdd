using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Sequence = DG.Tweening.Sequence;

// 스컹크 클래스. @홍지형 // TODO: 스컹크 아트로 수정해야함.
public class Skunk : Projectile
{
    SkunkSkill _skill;
    public Vector3 jumpPos; // 발사체가 점프할 목표 위치
    private SkunkPoison poison; // 독 장판 인스턴스

    public int quadrant { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 스컹크 스킬 클래스를 찾는다.
        List<SkillBase> skillList = Managers.Skill.usingSkillDic[SkillType.Active];
        foreach (SkillBase skill in skillList)
        {
            if (skill is SkunkSkill)
            {
                _skill = (SkunkSkill)skill;
            }
        }

        return true;
    }
    
    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {        
        base.SetSpawnInfo(owner, skill, direction);

        int minus = (direction.x >= 0) ? 1 : -1;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            minus = -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            minus = -1;

        LookLeft = (minus == 1);


        Sequence sequence = DOTween.Sequence()
            .Append(Renderer.DOFade(1f, 0f))
            .AppendCallback(() =>
            {
                Animator.SetBool("IsJump",true);
                //Animator.SetInteger("Jump", 1);
            })
            .Append(transform.DOJump(ChooseJumpPosition(), 0.5f, 1, 0.5f))
             .InsertCallback(1.2f, () =>
             {
                 //Animator.SetInteger("state", 2);
             })
            .AppendCallback(() =>
            {
                //Animator.SetInteger("Jump", 1);

                //Animator.SetInteger("state", 3);
                Renderer.color = Color.red;
                poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // 독 장판 spawn
                poison.SetInfo(Owner, skill);
            })
            .AppendInterval(0.5f)
            .InsertCallback(2.0f, () =>
            {
                //Animator.SetInteger("state", 4);
                Renderer.color = Color.white;
            })
            .Append(Renderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
            .InsertCallback(2.5f, () =>
            {
                Managers.Object.Despawn(this);
            });

    }


    // 스컹크가 점프할 4분면의 위치를 무작위로 정한다.
    Vector3 ChooseJumpPosition()
    {
        // 플레이어로부터의 거리
        float distance = 3.0f;

        // 선택된 사분면에 따라 초기 위치 결정
        Vector3 startPosition = Vector3.zero;
        switch (quadrant)
        {
            case 1: // 1사분면
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(45)) * distance;
                break;
            case 2: // 2사분면
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(225)) * distance;
                break;
            case 3: // 3사분면
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(315)) * distance;
                break;
            case 4: // 4사분면
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(135)) * distance;
                break;
            case 5:
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(0)) * distance;
                break;
            case 6:
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(180)) * distance;
                break;
        }
        
        return startPosition;
    }


    private void Update()
    {
        // transform.rotation = Quaternion.identity;
    }

    // 스컹크 자체는 공격판정이 없음.

    private void OnTriggerStay2D(Collider2D other)
    {
        //if (LayerMask.NameToLayer("Monster") == other.gameObject.layer)
        //{
        //    Monster monster = other.gameObject.GetComponent<Monster>();
        //    monster.OnDamaged(Owner, Skill);
        //}
    }
}
