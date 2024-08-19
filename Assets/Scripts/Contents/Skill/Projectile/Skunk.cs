using Data;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Sequence = DG.Tweening.Sequence;


enum ESkunkState
{
    Idle,
    Landing,
    Jump,
    Poison,
    End
}

// ����ũ Ŭ����. @ȫ���� // TODO: ����ũ ��Ʈ�� �����ؾ���.
public class Skunk : Projectile
{
    SkunkSkill _skill;
    public Vector3 jumpPos; // �߻�ü�� ������ ��ǥ ��ġ
    private SkunkPoison poison; // �� ���� �ν��Ͻ�

    public bool isBTSkill = false;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] sprites;

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

        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Art/Skills/Skunk");

        return true;
    }

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);

        spriteRenderer.sprite = sprites[(int)ESkunkState.Jump];

        if (isBTSkill)
        {
             Sequence sequence = DOTween.Sequence()
        .Append(transform.DOJump(ChooseJumpPosition(), 0.5f, 1, 0.5f))
        .AppendCallback(() =>
        {
            spriteRenderer.sprite = sprites[(int)ESkunkState.Landing];
            spriteRenderer.DOFade(0, 0.5f);
            poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // �� ���� spawn
            poison.Animator.SetBool("isBreakthrough", true);
            poison.SetInfo(Owner, skill);
        })
        .AppendInterval(skill.SkillData.Duration)
        .AppendCallback(() =>
        {
            Managers.Object.Despawn(this);
        });
        }

        else
        {
            Sequence sequence = DOTween.Sequence()
        .Append(transform.DOJump(ChooseJumpPosition(), 0.5f, 1, 0.5f))
        .AppendCallback(() =>
        {
            spriteRenderer.sprite = sprites[(int)ESkunkState.Landing];
            spriteRenderer.DOFade(0, 0.5f);
            poison = Managers.Resource.Instantiate("SkunkPoison", transform).GetOrAddComponent<SkunkPoison>(); ; // �� ���� spawn
            poison.Animator.SetBool("isNormal", true);
            poison.SetInfo(Owner, skill);
        })
        .AppendInterval(skill.SkillData.Duration)
        .AppendCallback(() =>
        {
            Managers.Object.Despawn(this);
        });
        }   
        
    }

    public void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction, bool isBTSkill)
    {
        if (isBTSkill)
            sprites = Resources.LoadAll<Sprite>("Art/Skills/SkunkBT");
        else
            sprites = Resources.LoadAll<Sprite>("Art/Skills/Skunk");

        SetSpawnInfo(owner, skill, direction);
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

    private void SkunkAnimationSprite()
    {

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
