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

    public void LevelUp(SkillData data)
    {
        SkillData = data;
        Clear();
        _cooldownTick = SkillData.CoolTime;
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
