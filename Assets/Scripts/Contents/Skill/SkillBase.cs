using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : InitBase
{
    public Creature Owner { get; protected set; }
    public Data.SkillData SkillData { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(Creature owner, int skillTemplateID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDic[skillTemplateID];
        _cooldownTick = SkillData.CoolTime;
    }

    public void LevelUp()
    {
        SkillData = Managers.Data.SkillDic[SkillData.DataId + 1];

        Clear();
        _cooldownTick = SkillData.CoolTime;
    }

    protected float _cooldownTick = 0f;
    protected virtual void Update()
    {
        if (SkillData.Level == 0)
            return;

        _cooldownTick += Time.deltaTime;
        if (_cooldownTick <= SkillData.CoolTime)
            return;
        _cooldownTick = 0.0f;

        DoSkill();
    }

    public abstract void DoSkill();
    public abstract void Clear();
}
