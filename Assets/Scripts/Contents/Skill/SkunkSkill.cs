using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ũ ��ų Ŭ����. @ȫ���� // TODO: ����ũ �� ��Ʈ�� �����ؾ���.
public class SkunkSkill : SkillBase
{
    public List<int> availableQuadrants = new List<int> { 1, 2, 3, 4 }; // ��� ������ ��и� ���
    public int lastQuadrant = 0; // ���������� ���õ� ��и�

    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        direction = Util.RotateVectorByAngle(direction, 45f);        
        AttackSkunk(direction);

        
        //for (int i = 2; i <= SkillData.CastCount; ++i)
        //{
        //    direction = Util.RotateVectorByAngle(direction, SkillData.CastAngle); // TODO:
            
        //}
    }

    private void AttackSkunk(Vector2 direction)
    {
        float offsetX = (direction.x >= 0) ? -1f : 1f;
        float offsetY = (direction.y >= 0) ? 1f : -1f;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            offsetX *= -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            offsetY *= -1;

        Skunk skunk = Managers.Object.Spawn<Skunk>(Owner.transform.position + new Vector3(offsetX, offsetY, 0f), SkillData.Projectile);
        skunk.SetSpawnInfo(Owner, this, direction);
    }
    public override void Clear()
    {
        
    }
  
}
