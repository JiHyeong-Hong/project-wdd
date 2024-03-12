using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public float speed = 4f; 

    private Vector2 moveDirection;
    private Hero hero;
    public float crocodileHeightOffset = 5f; // Crocodile이 소환될 때 추가적으로 더해줄 높이 값

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");

        hero = FindObjectOfType<Hero>();

        InitializeDirectionAndPosition();

        // 충돌하지 않은 인스턴스는 자동으로 Despawn되도록 함
        StartCoroutine(DespawnAfterTime(5f));
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
         {
             return false;
         }

        isInfinityDuration = true;

        return true; 
    }

    void FixedUpdate()
    {
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
        if (canMove)
        {
            rb.velocity = moveDirection; 
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                canMove = false; 
                rb.velocity = Vector2.zero; 

                animator.SetTrigger("CollisionDetected"); // 충돌 애니메이션 실행
                monster.OnDamaged(Owner, Skill); // 몬스터에게 데미지 적용

                StartCoroutine(DestroyAfterAnimation());
            }
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // 애니메이션이 재생되는 시간을 기다림 일단 1초로...
       
        Managers.Object.Despawn(this);
    }

    IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Managers.Object.Despawn(this);
    }
}