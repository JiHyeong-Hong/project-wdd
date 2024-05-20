using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileSkill : SkillBase
{
    public override void DoSkill()
    {
        if (BreakthroughHelper.Instance.CheckBreakthrough(SkillData.Index))
            return;

        Attack();
    }

    private void Attack()
    {
        Crocodile crocodile = Managers.Object.Spawn<Crocodile>(Owner.transform.position, SkillData.ProjectileNum);
        crocodile.SetSpawnInfo(Owner, this, Vector2.zero);
    }

    public override void Clear()
    {
    }
}
