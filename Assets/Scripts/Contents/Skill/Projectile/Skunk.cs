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

    public int quadrant { get; set; }

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
                poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // �� ���� spawn
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


    // ����ũ�� ������ 4�и��� ��ġ�� �������� ���Ѵ�.
    Vector3 ChooseJumpPosition()
    {
        // �÷��̾�κ����� �Ÿ�
        float distance = 3.0f;

        // ���õ� ��и鿡 ���� �ʱ� ��ġ ����
        Vector3 startPosition = Vector3.zero;
        switch (quadrant)
        {
            case 1: // 1��и�
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(45)) * distance;
                break;
            case 2: // 2��и�
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(225)) * distance;
                break;
            case 3: // 3��и�
                startPosition = Owner.transform.position + Util.ConvertVector2ToVector3(Util.AngleToVector(315)) * distance;
                break;
            case 4: // 4��и�
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

    // ����ũ ��ü�� ���������� ����.

    private void OnTriggerStay2D(Collider2D other)
    {
        //if (LayerMask.NameToLayer("Monster") == other.gameObject.layer)
        //{
        //    Monster monster = other.gameObject.GetComponent<Monster>();
        //    monster.OnDamaged(Owner, Skill);
        //}
    }
}
