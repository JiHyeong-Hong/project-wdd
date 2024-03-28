using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGasSkill : SkunkSkill
{
    public new List<int> availableQuadrants = new List<int> { 1, 2, 3, 4, 5, 6 }; // 사용 가능한 사분면 목록

    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        direction = Util.RotateVectorByAngle(direction, 45f);
        AttackSkunk(direction);
    }

    public override void AttackSkunk(Vector2 direction)
    {
        List<int> spawnPointList = Util.SelectRandomElements(availableQuadrants, SkillData.Projectile);

        float offsetX = (direction.x >= 0) ? -1f : 1f;
        float offsetY = (direction.y >= 0) ? 1f : -1f;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            offsetX *= -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            offsetY *= -1;

        for (int i = 0; i < SkillData.Projectile; i++)
        {
            Skunk skunk = Managers.Object.Spawn<Skunk>(Owner.transform.position + new Vector3(offsetX, offsetY, 0f), 4);
            skunk.quadrant = spawnPointList[0];
            skunk.SetSpawnInfo(Owner, this, direction);
            spawnPointList.RemoveAt(0);
        }
    }

}
