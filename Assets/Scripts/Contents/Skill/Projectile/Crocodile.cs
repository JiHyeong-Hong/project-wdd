using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

         animator = GetComponent<Animator>();
         if (animator == null)
         {
             return false;
         }

         return true; 
}

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        // 랜덤 방향 설정을 위해, direction 대신 Random.insideUnitCircle.normalized 사용
        base.SetSpawnInfo(owner, skill, Random.insideUnitCircle.normalized);
        transform.rotation = Quaternion.Euler(0f, 0f, Util.VectorToAngle(Random.insideUnitCircle.normalized));
   
        // 필요한 경우 여기에 추가 설정 코드 작성
    }

    protected override void Move()
    {
        // Skill 또는 SkillData가 null이면 이동을 중지
        if (Skill == null || Skill.SkillData == null)
        {
            Debug.LogWarning("Skill or SkillData is null in Crocodile.Move");
            return; // 초기화되지 않았으므로 이동 처리를 중단
        }
        base.Move();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                Debug.Log("OnTriggerEnter2D called with " + other.gameObject.name);
                Debug.Log("CollisionDetected Trigger Activated");
                animator.SetTrigger("CollisionDetected"); // 충돌 애니메이션 실행
                monster.OnDamaged(Owner, Skill); // 몬스터에게 데미지 적용

                // 충돌 후 Crocodile 오브젝트 제거 로직
                StartCoroutine(DestroyAfterAnimation());
            }
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // 애니메이션이 재생되는 시간을 기다림, 애니메이션 길이에 맞게 조절 필요
        Managers.Object.Despawn(this); // 안전하게 오브젝트 제거
    }
}