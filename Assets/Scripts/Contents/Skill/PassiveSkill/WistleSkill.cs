using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WistleSkill : SkillBase
{
    public override void Clear()
    {
        Owner.PassiveSkills.Remove(SkillData);
        UpdatePassive(SkillData, true);
    }

    public override void DoSkill()
    {
        Owner.PassiveSkills.Add(SkillData);
        UpdatePassive(SkillData);
    }


    private void UpdatePassive(SkillData skillData, bool removePassive = false)
    {
        int operatorValue = removePassive ? -1 : 1;
        SetPassive((Define.PassiveSkillStatusType)skillData.StatType, operatorValue);
        Debug.Log($"isRemove = {removePassive} \nUpdateBuffType= {(Define.PassiveSkillStatusType)skillData.StatType} \nvalue= {skillData.StatValue} ");
    }
}
