using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalmonSkill : SkillBase
{
    public override void Clear()
    {
        UpdatePassive(true);
    }

    public override void DoSkill()
    {
        UpdatePassive();
    }


    private void UpdatePassive(bool removePassive = false)
    {
        int operatorValue = removePassive ? -1 : 1;
        SetPassive(operatorValue);
    }
}