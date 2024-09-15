using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasMaskSkill : SkillBase
{
    public override void Clear()
    {
        UpdatePassive();
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
