using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasMaskSkill : SkillBase
{
    public override void Clear()
    {
        Owner.PassiveSkills.Remove(SkillData);
        UpdateBuff(SkillData, true);
    }

    public override void DoSkill()
    {
        Owner.PassiveSkills.Add(SkillData);
        UpdateBuff(SkillData);
    }


    private void UpdateBuff(SkillData skillData, bool removeBuff = false)
    {
        int operatorValue = removeBuff ? -1 : 1;

        //MaxHp += skillData.MaxHp * operatorValue;
        //Hp += skillData.MaxHp * operatorValue;
        //Atk += skillData.Atk * operatorValue;
        //MoveSpeed += skillData.MoveSpeed * operatorValue;
        Debug.Log($"isRemove = {removeBuff} UpdateBuff {skillData.StatValue} ");

    }
}
