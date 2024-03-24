using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WistleSkill : SkillBase
{
    public override void Clear()
    {
        Owner.PassiveSkills.Remove(SkillData);
        UpdatePassive(true);
    }

    public override void DoSkill()
    {
        Owner.PassiveSkills.Add(SkillData);
        UpdatePassive();
    }


    private void UpdatePassive(bool removePassive = false)
    {
        int operatorValue = removePassive ? -1 : 1;
        SetPassive(operatorValue);
    }
}
