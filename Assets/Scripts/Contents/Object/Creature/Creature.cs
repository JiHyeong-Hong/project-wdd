using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject
{
    public Vector2 Direction { get; protected set; } = Vector2.up;

    public Data.CreatureData CreatureData { get; private set; }
    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;

    #region Stats
    public int DataID { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Atk { get; set; }
    public float MoveSpeed { get; set; }
    #endregion

    protected bool _freezeStateOneFrame = false;
    protected ECreatureState _creatureState = ECreatureState.None;
    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_freezeStateOneFrame)
                return;

            if (_creatureState != value)
            {
                _creatureState = value;
                PlayAnimation(value);
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Creature;
        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if (CreatureType == ECreatureType.Hero)
            CreatureData = Managers.Data.HeroDic[templateID];
        else
            CreatureData = Managers.Data.MonsterDic[templateID];

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

        AnimatorController animatorController = Managers.Resource.Load<AnimatorController>(CreatureData.AnimatorDataID);
        Animator.runtimeAnimatorController = animatorController;

        DataID = CreatureData.DataId;
        MaxHp = CreatureData.MaxHp;
        Hp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;
        MoveSpeed = (CreatureData.MoveSpeed / 100.0f) * Define.DEFAULT_SPEED;

        CreatureState = ECreatureState.Idle;
    }

    private void LateUpdate()
    {
        _freezeStateOneFrame = false;
    }

    protected override void PlayAnimation(Define.ECreatureState state)
    {
        Animator.SetInteger("state", (int)state);
    }

    protected void SetRigidbodyVelocity(Vector2 velocity)
    {
        RigidBody.velocity = velocity;

        if (velocity.x < 0)
            LookLeft = true;
        else if (velocity.x > 0)
            LookLeft = false;
    }

    #region AI
    public float UpdateAITick { get; protected set; } = 100f;

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Attack:
                    UpdateAttack();
                    break;
                case ECreatureState.Hit:
                    UpdateHit();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
                case ECreatureState.Pattern:
                    UpdatePattern();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }
    //TODO 몬스터와 보스를 하나의 Monster객체로 만들면 사실상필요없는 코드 - CoUpdateAI와 통합가능
    protected IEnumerator CoUpdateBossAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Attack:
                    UpdateAttack();
                    break;
                case ECreatureState.Hit:
                    UpdateHit();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
                case ECreatureState.Pattern:
                    UpdatePattern();
                    break;
            }
            // Debug.Log(CreatureState);
            // Debug.Log(UpdateAITick + "후에 재실행");
            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateHit() { }
    protected virtual void UpdateDead() { }
    protected virtual void UpdatePattern() { }
    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        if (creature == null)
            return;

        int finalDamage = (skill == null) ? creature.Atk : skill.SkillData.Damage;
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        if (Hp <= 0)
        {
            OnDead(attacker, skill);
            CreatureState = ECreatureState.Dead;
        }
        else
        {
            CreatureState = ECreatureState.Hit;
        }

        _freezeStateOneFrame = true;
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);
    }
    #endregion

    #region Misc
    protected bool IsValid(BaseObject bo)
    {
        return bo.IsValid();
    }
    #endregion
}
