using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트에 대한 참조
    public float speed = 4f; // 이동 속도

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트를 가져옴

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
    }

   
    void FixedUpdate()
    {
        Debug.Log("FixedUpdate call Move()");
        Move();
    }

    protected override void Move()
    {
        //// Skill 또는 SkillData가 null이면 이동을 중지
        //if (Skill == null || Skill.SkillData == null)
        //{
        //    return; // 초기화되지 않았으므로 이동 처리를 중단
        //}

        // 오브젝트의 앞 방향으로 지속적으로 이동
        Vector2 moveDirection = transform.up * speed;
        Debug.Log($"Setting velocity to {moveDirection}");
        rb.velocity = moveDirection;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                animator.SetTrigger("CollisionDetected"); // 충돌 애니메이션 실행
                monster.OnDamaged(Owner, Skill); // 몬스터에게 데미지 적용

                // 충돌 후 Crocodile 오브젝트 제거
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