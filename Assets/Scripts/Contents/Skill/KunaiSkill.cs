using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiSkill : SkillBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);
    }

    public override void DoSkill()
    {
        Vector2 direction = Vector2.zero;
        
        Monster target = Managers.Object.FindClosestMonster(transform.position, 5);
        if (target == null)
        {
            float randomAngle = Random.Range(0f, 360f);
            float radianAngle = Mathf.Deg2Rad * randomAngle;
            direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
        }
        else
        {
            direction = target.transform.position - transform.position;
        }

        AttackKunai(direction, 0);

        for (int i = 2; i <= SkillData.AtkCount; ++i)
        {
            float angle = (i / 2) * SkillData.AtkAngle;
            if (i % 2 == 1)
                angle *= -1;
            AttackKunai(direction, angle);
        }
    }

    private void AttackKunai(Vector2 direction, float angle)
    {
        Kunai proj = Managers.Object.Spawn<Kunai>(Owner.transform.position, SkillData.ProjectileId);
        proj.SetSpawnInfo(Owner, this, Util.RotateVectorByAngle(direction, angle));
    }

    public override void Clear()
    {

    }
}