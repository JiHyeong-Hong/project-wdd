using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스컹크 스킬 클래스. @홍지형 // TODO: 스컹크 독 아트로 수정해야함.
public class SkunkSkill : SkillBase
{
    public List<int> availableQuadrants = new List<int> { 1, 2, 3, 4 }; // 사용 가능한 사분면 목록

    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        direction = Util.RotateVectorByAngle(direction, 45f);        
        AttackSkunk(direction);
    }

    public virtual void AttackSkunk(Vector2 direction)
    {
        List<int> spawnPointList = Util.SelectRandomElements(availableQuadrants, SkillData.ProjectileNum);

        float offsetX = (direction.x >= 0) ? -1f : 1f;
        float offsetY = (direction.y >= 0) ? 1f : -1f;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            offsetX *= -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            offsetY *= -1;

        for (int i = 0; i < SkillData.ProjectileNum; i++)
        {
            Skunk skunk = Managers.Object.Spawn<Skunk>(Owner.transform.position + new Vector3(offsetX, offsetY, 0f), SkillData.ProjectileNum);
            skunk.quadrant = spawnPointList[0];
            skunk.SetSpawnInfo(Owner, this, direction);
            spawnPointList.RemoveAt(0);
        }

    }
    public override void Clear()
    {
        
    }
  
}
