using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyBankSkill : SkillBase
{
    public override void Clear()
    {
        UpdatePassive();
    }

    public override void DoSkill()
    {
        UpdatePassive();
    }


    private void UpdatePassive()
    {        
        SetPassive();
    }
}
