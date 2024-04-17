using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
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
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float Atk { get; set; }
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

        switch (CreatureType)
        {
            case ECreatureType.Hero:
                CreatureData = Managers.Data.HeroDic[templateID];
                break;
            case ECreatureType.Monster:
                CreatureData = Managers.Data.MonsterDic[templateID];
                break;
            case ECreatureType.Boss:
                CreatureData = Managers.Data.MonsterDic[templateID];
                //TODO 
                break;
        }

        gameObject.name = $"{CreatureData.Index}_{CreatureData.DescriptionTextID}";

        var animatorController = Managers.Resource.Load<AnimatorOverrideController>(CreatureData.AnimatorDataID);

        Animator.runtimeAnimatorController = animatorController;

        DataID = CreatureData.Index;
        MaxHp = CreatureData.MaxHp;
        Hp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;
        MoveSpeed = (CreatureData.MoveSpeed) * Define.DEFAULT_SPEED;

        CreatureState = ECreatureState.Idle;
    }

    private void LateUpdate()
    {
        _freezeStateOneFrame = false;
    }

    protected override void PlayAnimation(Define.ECreatureState state)
    {
        // Debug.Log($"현재 타입 : {CreatureType}, 현재 상태 : {state}");
        Animator.SetInteger("state", (int)state);
    }

    protected void SetRigidbodyVelocity(Vector2 velocity)
    {
        RigidBody.velocity = velocity;

        SetImageDirecton(velocity);
    }
    
    //이동하지않고 이미지만 좌우 반전 함수
    protected void SetImageDirecton(Vector2 velocity)
    {
        if (velocity.x < 0)
            LookLeft = true;
        else if (velocity.x > 0)
            LookLeft = false;
    }

    #region AI
    public float UpdateAITick { get; protected set; } = 0.01f;

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
                case ECreatureState.Skill1:
                    UpdateSkill1();
                    break;
                case ECreatureState.Pattern1:
                    UpdatePattern1();
                    break;
                case ECreatureState.Pattern2:
                    UpdatePattern2();
                    break;
                case ECreatureState.ChangePhase:
                    UpdateChangePhase();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }
    //TODO Eung 몬스터와 보스를 하나의 Monster객체로 만들면 사실상필요없는 코드 - CoUpdateAI와 통합가능
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
    protected virtual void UpdateSkill1() { }
    protected virtual void UpdatePattern1() { }
    protected virtual void UpdatePattern2() { }
    protected virtual void UpdateChangePhase() { }
    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        Projectile projectile = null;

        if (creature == null)
        {
            projectile = attacker as Projectile;
        }

        if (creature == null && projectile == null)
            return;
        

        float finalDamage = 0;

        if (skill == null)
        {
            if(creature != null)
                finalDamage = creature.Atk;
            else
                finalDamage = projectile.ProjectileData.ContactDmg;
        }
        else if(CreatureType == ECreatureType.Hero)
            finalDamage = skill.SkillData.Damage + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Attack);
        else if(CreatureType == ECreatureType.Monster || CreatureType == ECreatureType.MiddleBoss || CreatureType == ECreatureType.Boss)
            finalDamage = skill.SkillData.Damage + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Attack);

        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);
        // Debug.LogWarning($"[{gameObject.name}] Hit! HP({Hp}/{MaxHp})"); // 디버깅용. 삭제가능 @홍지형
        if (Hp <= 0)
        {
            OnDead(attacker, skill);
            CreatureState = ECreatureState.Dead;
        }
        else
        {
            if(CreatureType != ECreatureType.Boss)
                CreatureState = ECreatureState.Hit;
        }


        // 넉백
        // if (skill.SkillData.KnockbackPower != 0)
        //     StartCoroutine(knockbackUpdate(transform.position - attacker.transform.position, skill.SkillData.KnockbackPower * 0.01f, 0.5f));

        _freezeStateOneFrame = true;
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);
    }

    private IEnumerator knockbackUpdate(Vector3 dir, float knockbackDistance, float knockback)
    {
        // 현재까지 넉백 시간
        float currentknockbackTime = 0f;
        // 이전 Frame에 이동한 거리
        float prevknockbackDistance = 0f;

        while (true)
        {
            currentknockbackTime += Time.deltaTime;

            float timePoint = currentknockbackTime / knockback;
            // Easing InOutSine https://easings.net/ko#easeOutCirc
            float easeOutCirc = Mathf.Sqrt(1 - Mathf.Pow(timePoint - 1, 2));
            float currentknockbackDistance = Mathf.Lerp(0f, knockbackDistance, easeOutCirc);
            // 이번 Frame에 움직여야할 거리를 구함
            float deltaValue = currentknockbackDistance - prevknockbackDistance;

            transform.position += (dir * deltaValue);
            prevknockbackDistance = currentknockbackDistance;

            if (currentknockbackTime >= knockback)
                break;
            else
                yield return null;
        }
    }

    #endregion

    #region Misc
    protected bool IsValid(BaseObject bo)
    {
        return bo.IsValid();
    }
    #endregion
}
