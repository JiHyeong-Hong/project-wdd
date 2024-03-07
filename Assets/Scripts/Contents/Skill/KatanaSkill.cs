using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaSkill : ActiveSkill
{
    public override void DoSkill()
    {
        Vector2 direction = Owner.Direction;
        AttackKatana(direction);

        for (int i = 2; i <= SkillData.CastCount; ++i)
        {
            direction = Util.RotateVectorByAngle(direction, SkillData.CastAngle);
            AttackKatana(direction);
        }
    }

    private void AttackKatana(Vector2 direction)
    {
        Area katana = Managers.Resource.Instantiate("Area/Quarter", Owner.transform).GetOrAddComponent<Area>();
        katana.SetInfo(Owner.transform.position, 0.2f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        katana.transform.rotation = Quaternion.AngleAxis(angle - 45f, Vector3.forward);

        CircleCollider2D circleCollider = katana.GetComponent<CircleCollider2D>();
        float detectionRadius = circleCollider.radius * katana.transform.localScale.x;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(katana.transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            Vector2 directionToTarget = (hitCollider.transform.position - katana.transform.position).normalized;
            float angleToTarget = Vector2.Angle(direction, directionToTarget);

            if (angleToTarget < 90 / 2)
            {
                CheckEnemyInRange(hitCollider);
            }
        }
    }

    private void CheckEnemyInRange(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.OnDamaged(Owner, this);
        }
    }

    public override void Clear()
    {

    }
}
