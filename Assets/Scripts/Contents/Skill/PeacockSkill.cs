using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Data;
public class PeacockSkill : SkillBase
{
    private GameObject peacockEffect;
    
    public override void DoSkill()
    {
       Vector2 direction = Vector2.zero;
        
        Monster target = Managers.Object.FindClosestMonster(Owner.CenterPosition, 20);
        if (target == null)
        {
            direction = Util.GetRandomDir();
        }
        else
        {
            direction = target.transform.position - Owner.CenterPosition;
        }
        
        AttackKunai(direction, 0);
        PeacockEffectFindSetActive(true, 0.1f * SkillData.CastCount);

        for (int i = 2; i <= SkillData.CastCount; ++i)
        {
            float angle = (i / 2) * SkillData.CastAngle;
            if (i % 2 == 1)
                angle *= -1;
            AttackKunai(direction, angle);
        }
    }

   private void AttackKunai(Vector2 direction, float angle)
    {
        // Kunai proj = Managers.Object.Spawn<Kunai>(Owner.transform.position, SkillData.ProjectileNum);
        // proj.SetSpawnInfo(Owner, this, Util.RotateVectorByAngle(direction, angle));

        Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, 1);
        peacock.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        peacock.SetSpawnInfo(Owner, this, Util.RotateVectorByAngle(direction, angle));
    }


    private void PeacockEffectFindSetActive(bool active, float time)
    {
        Hero hero = Managers.Object.Hero;

        // �̹� ���� ȿ���� �����ϴ��� Ȯ��
        peacockEffect = hero.gameObject.transform.Find("PeacockEffect")?.gameObject;

        if (peacockEffect == null)
        {
            peacockEffect = new GameObject("PeacockEffect");
            peacockEffect.transform.SetParent(hero.transform);
            SpriteRenderer sr = peacockEffect.AddComponent<SpriteRenderer>();
            sr.sprite = Managers.Resource.Load<Sprite>("Art/PeacockEffect");
            sr.sortingOrder = 10;

            Color color = sr.color;
            color.a = 0.6f; // 투명도 50
            sr.color = color;
        }
        peacockEffect.transform.localPosition = Vector3.zero;

        BreakthroughHelper.Instance.SetActiveObject(peacockEffect, active, 1.0f);
    }

    public override void Clear()
    {
    }
}
