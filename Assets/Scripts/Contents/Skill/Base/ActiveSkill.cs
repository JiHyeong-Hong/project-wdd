using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public abstract class ActiveSkill : SkillBase
{
    protected float _cooldownTick = 0f;

    public override void Init(Creature owner, SkillData data = null)
    {
        base.Init(owner, data); 
        _cooldownTick = SkillData.CoolTime;
    }

    public override void LevelUp(SkillData data)
    {
        base.LevelUp(data);
        _cooldownTick = SkillData.CoolTime;
    }

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
