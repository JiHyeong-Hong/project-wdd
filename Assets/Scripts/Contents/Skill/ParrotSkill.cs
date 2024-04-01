using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotSkill : SkillBase
{
    private bool isTimeActive = false;
    private float durationTick = 0.0f;
    
    public override void UpdateCoolTime(float deltaTime)
    {
        if (SkillData.Level < 1)
            return;
        
        RotateSatellites();
    
        if (isTimeActive)
        {
            durationTick += deltaTime;
            if (durationTick >= SkillData.Duration + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.Duration))
            {
                ClearSatellites();
                isTimeActive = false;
                durationTick = 0.0f;
            }
        }
    
        base.UpdateCoolTime(deltaTime);
    }

    private float orbitRadius = 1f;
    private float rotationSpeed = 200f;
    private List<Bird> birds = new List<Bird>();

    public override void DoSkill()
    {
        ClearSatellites();

        for (int i = 0; i < SkillData.CastCount; ++i)
        {
            float angle = i * 360f / SkillData.CastCount;
            Vector2 spawnPosition = GetCirclePosition(angle, orbitRadius);

            Bird bird = Managers.Object.Spawn<Bird>(spawnPosition, SkillData.ProjectileNum, Owner.transform);
            bird.SetSpawnInfo(Owner, this, Vector2.up);

            birds.Add(bird);
        }

        isTimeActive = true;
    }

    private Vector2 GetCirclePosition(float angle, float radius)
    {
        Transform hero = Managers.Object.Hero.transform;

        float radian = Mathf.Deg2Rad * angle;
        float x = hero.position.x + Mathf.Cos(radian) * radius;
        float y = hero.position.y + Mathf.Sin(radian) * radius;
        return new Vector2(x, y);
    }

    private void RotateSatellites()
    {
        foreach (Bird bird in birds)
        {
            bird.transform.RotateAround(Owner.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private void ClearSatellites()
    {
        foreach (Bird bird in birds)
        {
            bird.Clear(() =>
            {
                Managers.Object.Despawn(bird);
                birds.Remove(bird);
            });
        }
    }

    public override void Clear()
    {
        ClearSatellites();
        isTimeActive = false;
        durationTick = 0.0f;
    }
}
