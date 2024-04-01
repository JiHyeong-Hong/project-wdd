using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerSkill : SkillBase
{
    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        AttackTiger(direction);

        for (int i = 2; i <= SkillData.CastCount; ++i)
        {
            direction = Util.RotateVectorByAngle(direction, SkillData.CastAngle);
            AttackTiger(direction);
        }
    }

    private void AttackTiger(Vector2 direction)
    {
        float offsetX = (direction.x >= 0) ? -13f : 13f;
        float offsetY = (direction.y >= 0) ? 2f : -2;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            offsetX *= -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            offsetY *= -1;

        Tiger tiger = Managers.Object.Spawn<Tiger>(Owner.transform.position + new Vector3(offsetX, offsetY, 0f), SkillData.ProjectileNum);
        tiger.SetSpawnInfo(Owner, this, direction);
    }

    public override void Clear()
    {

    }
}
