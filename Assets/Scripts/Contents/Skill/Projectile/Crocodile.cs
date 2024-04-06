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

        Direction();

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

    private void Direction()
    {
        transform.right = Managers.Object.Hero.Destination.position - Owner.transform.position;
        if (transform.right.x < 0)
        {
            spriteRenderer.flipY = true;
        }

        moveDirection = transform.right.normalized * speed;
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


                Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, Skill.SkillData.AttackRange); // 충돌한 몬스터 주변에 있는 몬스터들을 찾음
                for (int i = 0; i < targets.Length; i++) { }

                StartCoroutine(Util.DrawCircle(transform.position, Skill.SkillData.AttackRange, 16, Color.red, 5f)); // 공격 범위를 시각적으로 표시

                foreach (var target in targets)
                {
                    Monster targetMonster = target.GetComponent<Monster>();
                    if (targetMonster == null)
                        continue;

                    targetMonster.OnDamaged(Owner, Skill); // 몬스터에게 데미지 적용

                }


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