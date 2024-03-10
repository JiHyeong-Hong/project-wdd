using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 얼룩말 스킬 클래스. @홍지형
public class ZebraSkill : SkillBase
{
    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        SpawnZebras(direction);
    }

    private void SpawnZebras(Vector2 direction)
    {
        Vector2 centerPosition = Managers.Object.Hero.CenterPosition;
        float boxWidth = 5f; // 가상의 네모칸 너비
        float boxHeight = 9f; // 가상의 네모칸 높이
        Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);            

        Zebra zebra = Managers.Object.Spawn<Zebra>(topRight, SkillData.Projectile);
        zebra.SetSpawnInfo(Owner, this, direction);
    }

    public override void Clear()
    {

    }
}
