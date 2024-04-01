using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��踻 ��ų Ŭ����. @ȫ����
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
        float boxWidth = 5f; // ������ �׸�ĭ �ʺ�
        float boxHeight = 9f; // ������ �׸�ĭ ����
        Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);            

        Zebra zebra = Managers.Object.Spawn<Zebra>(topRight, SkillData.ProjectileNum);
        zebra.SetSpawnInfo(Owner, this, direction);
    }

    public override void Clear()
    {

    }
}
