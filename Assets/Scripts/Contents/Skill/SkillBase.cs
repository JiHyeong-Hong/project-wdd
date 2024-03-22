using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public abstract class SkillBase
{
    public Creature Owner { get; protected set; }
    public SkillData SkillData { get; private set; }
    
    public void SetInfo(SkillData data)
    {
        SkillData = data;
        _cooldownTick = SkillData.CoolTime;
    }

    public virtual void SetOwner(Creature owner)
    {
        Owner = owner;
    }

    public virtual void LevelUp(SkillData data)
    {
        SkillData = data;
        Clear();
        _cooldownTick = SkillData.CoolTime;
    }

    public  void SetPassive(Define.PassiveSkillStatusType stateType, int operatorValue)
    {
        Hero hero = Owner as Hero;
        float stateValue = SkillData.StatValue * operatorValue;
        switch (stateType)
        {
            case Define.PassiveSkillStatusType.CoolTimeDown:
                // 모든 스킬의 쿨타임을 감소 시켜준다.
                foreach (var skill in Managers.Skill.usingSkillDic[Define.SkillType.Active])
                {
                    skill.SkillData.CoolTime -= stateValue;
                }
                break;
            case Define.PassiveSkillStatusType.Farming:
                hero.ItemAcquireRange += stateValue;
                break;
            case Define.PassiveSkillStatusType.Exp:
                hero.Exp += stateValue;
                break;
            case Define.PassiveSkillStatusType.Gold:
                //Managers.Game.Gold += (int)stateValue;
                break;
            case Define.PassiveSkillStatusType.Duration:
                foreach (var skill in Managers.Skill.usingSkillDic[Define.SkillType.Active])
                {
                    skill.SkillData.Duration -= stateValue;
                }
                break;
            case Define.PassiveSkillStatusType.Recovery:
                hero.Hp += stateValue;
                break;
            case Define.PassiveSkillStatusType.Hp:
                hero.MaxHp += stateValue;
                break;
            case Define.PassiveSkillStatusType.MoveSpeed:
                hero.MoveSpeed += stateValue;
                break;
            case Define.PassiveSkillStatusType.Attack:
                hero.Atk += (int)stateValue;
                break;
            case Define.PassiveSkillStatusType.AttackSpeed:
                //hero.AttackSpeed += stateValue;
                break;
            case Define.PassiveSkillStatusType.AttackRange:
                //hero.AttackRange += stateValue;
                break;
            case Define.PassiveSkillStatusType.DamageCare:
                //hero.DamageCare += stateValue;
                break;
            case Define.PassiveSkillStatusType.CastPer:
                //hero.CastPer += stateValue;
                break;
            default:
                break;
        }
    }

    protected float _cooldownTick = 0f;
    public virtual void UpdateCoolTime(float deltaTime)
    {
        if (SkillData.Level < 1)
            return;
        
        _cooldownTick += deltaTime;
        
        if (_cooldownTick <= SkillData.CoolTime)
            return;
        
        _cooldownTick = 0.0f;
        DoSkill();
    }

    public abstract void DoSkill();
    public abstract void Clear();
}
