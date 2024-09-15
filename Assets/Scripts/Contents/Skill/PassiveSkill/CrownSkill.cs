using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownSkill : SkillBase
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
        SetPassive();
    }
}
