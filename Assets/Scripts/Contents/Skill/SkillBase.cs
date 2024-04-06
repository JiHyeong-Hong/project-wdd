using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

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

    public void SetPassive(int operatorValue)
    {
        PassiveHelper.Instance.SetPassive(SkillData, operatorValue);
    }

    protected float _cooldownTick = 0f;
    public virtual void UpdateCoolTime(float deltaTime)
    {
        if (SkillData.Level < 1)
            return;
        
        _cooldownTick += deltaTime;
        
        if (_cooldownTick < SkillData.CoolTime - PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.CoolTimeDown))
            return;
        
        _cooldownTick = 0.0f;
        DoSkill();
    }

    private int checkCount = 0;
    public bool CheckBTSkill()
    {
        checkCount++;
        if(Managers.Skill.Breakthrogh(SkillData.Index, checkCount))
        {
            checkCount = 0;
            return true;
        }
        return false;
    }

    public abstract void DoSkill();
    public abstract void Clear();
}
