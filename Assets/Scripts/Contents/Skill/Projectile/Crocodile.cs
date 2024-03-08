using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트에 대한 참조
    public float speed = 4f; // 이동 속도

    private Vector2 moveDirection;
    private Hero hero; // Hero 객체 참조
    public float crocodileHeightOffset = 5f; // Crocodile이 소환될 때 추가적으로 더해줄 높이 값

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");

        hero = FindObjectOfType<Hero>(); // Scene 내에서 Hero 객체를 찾아 참조를 저장

        InitializeDirectionAndPosition();
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

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate call Move()");
        Move();
    }

    private void InitializeDirectionAndPosition()
    {
        // Hero의 바라보는 방향을 기준으로 Crocodile의 소환 위치와 이동 방향 설정
        Vector3 heroDirection = (hero.Destination.position - hero.transform.position).normalized;
        transform.position = hero.Destination.position + new Vector3(0, crocodileHeightOffset, 0);

        // 이동 방향 설정
        moveDirection = new Vector2(heroDirection.x, heroDirection.y).normalized * speed;
    }

    protected override void Move()
    {
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