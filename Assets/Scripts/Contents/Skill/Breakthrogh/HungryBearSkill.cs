using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HungryBearSkill : SkillBase
{
    public override void DoSkill()
    {
        var dir = Owner.Direction;
        bool isFlip = dir.x < 0;

        Bear bear = Managers.Object.Spawn<Bear>((Vector2)Owner.transform.position + Vector2.up, SkillData.ProjectileNum);
        bear.Skills[1].SetActive(true);
        bear.myCollider2D.radius = 4;

        bear.SetSpawnInfo(Owner, this, Vector3.zero);
        bear.transform.localScale = isFlip ? new Vector3(-1.5f, 1.5f, 1.5f) : Vector3.one * 1.5f;
        bear.transform.eulerAngles = new Vector3(0, 0, 0);

    }

    public override void Clear()
    {
    }

}
