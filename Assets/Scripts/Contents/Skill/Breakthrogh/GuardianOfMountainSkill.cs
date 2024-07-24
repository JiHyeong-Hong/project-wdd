using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianOfMountainSkill : TigerSkill
{
    public override void DoSkill()
    {
        base.DoSkill();
    }

    public override void AttackTiger(Vector2 direction)
    {
        float offsetX = (direction.x >= 0) ? -15f : 15f;
        float offsetY = (direction.y >= 0) ? 2.5f : -2.5f;  

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            offsetX *= -2;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            offsetY *= -2;

        Tiger tiger = Managers.Object.Spawn<Tiger>(Owner.transform.position + new Vector3(offsetX, offsetY, 0f), SkillData.ProjectileNum);
        tiger.isBTSkill = true;
        tiger.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        tiger.SetSpawnInfo(Owner, this, direction);
    }
}