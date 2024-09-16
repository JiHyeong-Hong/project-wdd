using System.Timers;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class ParrotSkill : SkillBase
{
    private GameObject birdPool;
    private float orbitRadius = 1f;
    private List<Bird> birds = new List<Bird>();
    private Sequence sequence; // 기존 시퀀스를 저장할 변수 추가
    private float elapsedTime;
    private float duration;

    public override void DoSkill()
    {
        //duration = SkillData.Duration * (1 + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.Duration));
        SoundManager.Instance.Play(Define.ESoundMainType.Skill1, Define.ESoundType.Parrot);
        ClearSatellites();

        if (BreakthroughHelper.Instance.CheckBreakthrough(SkillData.SkillID))
        {
            Clear();
            return;
        }

        if (!GameObject.Find("BirdPool"))
        {
            InitBirdPool();
        }
        
        if (birdPool == null)
        {
            birdPool = GameObject.Find("BirdPool");
        }        
        

        for (int i = 0; i < SkillData.ProjectileNum; ++i)
        {
            float angle = i * 360f / SkillData.ProjectileNum;
            Vector2 spawnPosition = GetCirclePosition(angle, orbitRadius);
            Bird bird = Managers.Object.Spawn<Bird>(spawnPosition, 1, birdPool.transform);
            bird.SetSpawnInfo(Owner, this, Vector2.up);
            bird.Animator.SetTrigger("Normal");
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
        // 기존 시퀀스가 존재하면 중지하고 삭제
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();

        }
        sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => birdPool.transform.localRotation.eulerAngles,
                  x => birdPool.transform.localRotation = Quaternion.Euler(x),
                  new Vector3(0, 0, 360 * SkillData.CastCount), 7) // 7: 공전속도, 낮을수록 빠름
             .SetEase(Ease.Linear))
            //.AppendInterval(1)      // 1초 동안 대기 후 다음 애니메이션 실행
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
        // 기존 시퀀스가 존재하면 중지하고 삭제
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();

        }
        ClearSatellites();
        // InitBirdPool();
    }

    private void InitBirdPool()
    {        
        birdPool = null;
        birdPool = new GameObject("BirdPool");
        birdPool.transform.parent = Owner.transform;
        birdPool.transform.localPosition = Vector3.zero; // Owner의 위치로 이동
    }

    // duration 사용시, 현재 미사용
    private void UpdateDuration()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > duration)
        {
            Clear();
        }
    }
}
