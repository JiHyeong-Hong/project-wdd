using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HungryBearSkill : SkillBase
{
    private readonly float DISTANCE_OFFSET = 3f;
    private readonly float ANGLE_OFFSET = 70f;

    public override void DoSkill()
    {
      
        Debug.Log("HungryBearSkill");

        var hero = Managers.Object.Hero;
        var dir = Owner.Direction;
        bool isFlip = dir.x < 0;

        var angle = hero.Pivot.eulerAngles.z;
        angle += isFlip ? -ANGLE_OFFSET : ANGLE_OFFSET;

        Bear bear = Managers.Object.Spawn<Bear>((Vector2)Owner.transform.position + Owner.Direction * DISTANCE_OFFSET, SkillData.ProjectileNum);
        bear.SetSpawnInfo(Owner, this, Vector3.zero);
        bear.transform.localScale = isFlip ? new Vector3(-1.5f, 1.5f, 1.5f) : Vector3.one * 1.5f;
        bear.transform.eulerAngles = new Vector3(0, 0, angle);

    }

    public override void Clear()
    {
    }

}
