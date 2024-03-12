using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Sequence = DG.Tweening.Sequence;

// ����ũ Ŭ����. @ȫ���� // TODO: ����ũ ��Ʈ�� �����ؾ���.
public class Skunk : Projectile
{
    SkunkSkill _skill;
    public Vector3 jumpPos; // �߻�ü�� ������ ��ǥ ��ġ
    private SkunkPoison poison; // �� ���� �ν��Ͻ�
   
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // ����ũ ��ų Ŭ������ ã�´�.
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
                poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // �� ���� spawn                
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

    // ����ũ�� ������ 4�и��� ��ġ�� �������� ���Ѵ�.
    Vector3 ChooseJumpPosition()
    {
        // ���������� ���õ� ��и��� ��Ͽ��� ����
        if (_skill.lastQuadrant != 0)
        {
            _skill.availableQuadrants.Remove(_skill.lastQuadrant);
        }

        // ���� ��и� �߿��� �������� �ϳ� ����
        int quadrantIndex = Random.Range(0, _skill.availableQuadrants.Count);
        int selectedQuadrant = _skill.availableQuadrants[quadrantIndex];

        // �÷��̾�κ����� �Ÿ�
        float distance = 3.0f;

        // ���õ� ��и鿡 ���� �ʱ� ��ġ ����
        Vector3 startPosition = Vector3.zero;
        switch (selectedQuadrant)
        {
            case 1: // 1��и�
                startPosition = Owner.transform.position + new Vector3(distance, distance, 0);
                break;
            case 2: // 2��и�
                startPosition = Owner.transform.position + new Vector3(-distance, distance, 0);
                break;
            case 3: // 3��и�
                startPosition = Owner.transform.position + new Vector3(-distance, -distance, 0);
                break;
            case 4: // 4��и�
                startPosition = Owner.transform.position + new Vector3(distance, -distance, 0);
                break;
        }

        // ��� ������ ��и� ����� �缳���ϰ�, ���������� ���õ� ��и� ������Ʈ
        _skill.availableQuadrants = new List<int> { 1, 2, 3, 4 };
        _skill.lastQuadrant = selectedQuadrant;

        return startPosition;
    }


    private void Update()
    {
        // transform.rotation = Quaternion.identity;
    }

    // ����ũ ��ü�� ���������� ����.
    void OnTriggerEnter2D(Collider2D other)
    {
        //if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        //{
        //    Monster monster = other.gameObject.GetComponent<Monster>();
        //    monster.OnDamaged(Owner, Skill);
        //}
    }


}
