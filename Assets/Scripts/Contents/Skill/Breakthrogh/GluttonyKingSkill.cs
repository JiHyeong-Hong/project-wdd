using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyKingSkill : SkillBase
{
    public override void DoSkill()
    {
        Attack();
    }

    private void Attack()
    {
        Crocodile crocodile = Managers.Object.Spawn<Crocodile>(Owner.transform.position, SkillData.ProjectileNum);
        crocodile.SetSpawnInfo(Owner, this, Vector2.zero);
        crocodile.isBreakthrough = true;
        crocodile.skill2 = BreakthroughHelper.Instance.FindBreakthroughSkill(SkillData.Index);
    }

    public override void Clear()
    {
    }
}
