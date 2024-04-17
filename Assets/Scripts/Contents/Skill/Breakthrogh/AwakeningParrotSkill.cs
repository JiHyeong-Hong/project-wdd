using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AwakeningParrotSkill : SkillBase
{
    private GameObject birdPool;
    private float orbitRadius = 1f;
    private List<Bird> birds = new List<Bird>();

    public override void DoSkill()
    {
        ClearSatellites();

        if (birdPool == null)
        {
            if (!GameObject.Find("BirdPool"))
            {
                birdPool = new GameObject("BirdPool");
                birdPool.transform.parent = Owner.transform;
            }
            else
            {
                birdPool = GameObject.Find("BirdPool");
            }
            
        }

        for (int i = 0; i < SkillData.ProjectileNum; ++i)
        {
            float angle = i * 360f / SkillData.ProjectileNum;
            Vector2 spawnPosition = GetCirclePosition(angle, orbitRadius);
            Bird bird = Managers.Object.Spawn<Bird>(spawnPosition, SkillData.ProjectileNum, birdPool.transform);
            bird.SetSpawnInfo(Owner, this, Vector2.up);
            bird.Animator.SetTrigger("Breakthrough");
            birds.Add(bird);
        }
        RotateSatellites();
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
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => birdPool.transform.localRotation.eulerAngles,
                  x => birdPool.transform.localRotation = Quaternion.Euler(x),
                  new Vector3(0, 0, 360 * SkillData.CastCount),
                  3)
             .SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                ClearSatellites();
            });
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
    }

}
