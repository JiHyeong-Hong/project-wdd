using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트에 대한 참조
    public float speed = 4f; // 이동 속도

    private Vector2 heroDirection = Vector2.right; // 기본 방향을 오른쪽으로 설정

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); 

        if (animator == null)
         {
             return false;
         }

         return true; 
}

    // Crocodile 소환 위치 및 방향 설정을 위함
    public void SetInitialPositionAndDirection(Transform arrowTransform, Vector2 direction)
    {
        this.transform.position = arrowTransform.position + new Vector3(0, arrowTransform.localScale.y / 2, 0);
        this.heroDirection = direction.normalized; // 방향 저장
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
        Vector2 moveDirection = heroDirection * speed;
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