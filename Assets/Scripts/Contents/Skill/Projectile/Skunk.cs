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

        jumpPos = ChooseJumpPosition();

        Sequence sequence = DOTween.Sequence()
            .Append(Renderer.DOFade(1f, 0f))            
            .AppendCallback(() =>
            {
                Animator.SetInteger("state", 1);
            })
            // .Append(transform.DORotate(new Vector3(0, 0, 45), 0.1f, RotateMode.LocalAxisAdd))               
            // .Append(transform.DOJump(new Vector3(2f, 2f, 0) * 2, 0.5f, 1, 0.5f));            
            .Append(transform.DOJump(jumpPos, 0.5f, 1, 0.5f))
             .InsertCallback(1.2f, () =>
             {
                 Animator.SetInteger("state", 2);
             })
            .AppendCallback(() =>
            {                
                Animator.SetInteger("state", 3);
                Renderer.color = Color.red;
                poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // 독 장판 spawn                
            })
            .AppendInterval(0.5f)
            .InsertCallback(2.0f, () =>
            {
                Animator.SetInteger("state", 4);
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
        // 마지막으로 선택된 사분면을 목록에서 제거
        if (_skill.lastQuadrant != 0)
        {
            _skill.availableQuadrants.Remove(_skill.lastQuadrant);
        }

        // 남은 사분면 중에서 무작위로 하나 선택
        int quadrantIndex = Random.Range(0, _skill.availableQuadrants.Count);
        int selectedQuadrant = _skill.availableQuadrants[quadrantIndex];

        // 플레이어로부터의 거리
        float distance = 3.0f;

        // 선택된 사분면에 따라 초기 위치 결정
        Vector3 startPosition = Vector3.zero;
        switch (selectedQuadrant)
        {
            case 1: // 1사분면
                startPosition = Owner.transform.position + new Vector3(distance, distance, 0);
                break;
            case 2: // 2사분면
                startPosition = Owner.transform.position + new Vector3(-distance, distance, 0);
                break;
            case 3: // 3사분면
                startPosition = Owner.transform.position + new Vector3(-distance, -distance, 0);
                break;
            case 4: // 4사분면
                startPosition = Owner.transform.position + new Vector3(distance, -distance, 0);
                break;
        }

        // 사용 가능한 사분면 목록을 재설정하고, 마지막으로 선택된 사분면 업데이트
        _skill.availableQuadrants = new List<int> { 1, 2, 3, 4 };
        _skill.lastQuadrant = selectedQuadrant;

        return startPosition;
    }


    private void Update()
    {
        // transform.rotation = Quaternion.identity;
    }

    // 스컹크 자체는 공격판정이 없음.
    void OnTriggerEnter2D(Collider2D other)
    {
        //if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        //{
        //    Monster monster = other.gameObject.GetComponent<Monster>();
        //    monster.OnDamaged(Owner, Skill);
        //}
    }


}
