using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Data;
public class PeacockSkill : SkillBase
{
    public override void DoSkill()
    {
         if (BreakthroughHelper.Instance.CheckBreakthrough(SkillData.SkillID) || Owner == null)
             return;

       Vector2 direction = Vector2.zero;
        
        Monster target = Managers.Object.FindClosestMonster(Owner.CenterPosition, 20);
        if (target == null)
        {
            direction = Util.GetRandomDir();
        }
        else
        {
            direction = target.transform.position - Owner.CenterPosition;
        }
        
        AttackKunai(direction, 0);

        for (int i = 2; i <= SkillData.ProjectileNum; ++i)
        {
            float angle = (i / 2) * SkillData.CastAngle;
            if (i % 2 == 1)
                angle *= -1;
            AttackKunai(direction, angle);
        }
    }

   private void AttackKunai(Vector2 direction, float angle)
    {
        // Kunai proj = Managers.Object.Spawn<Kunai>(Owner.transform.position, SkillData.ProjectileNum);
        // proj.SetSpawnInfo(Owner, this, Util.RotateVectorByAngle(direction, angle));

        Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, 1);
        peacock.SetSpawnInfo(Owner, this, Util.RotateVectorByAngle(direction, angle));
    }

    public override void Clear()
    {
    }
}
