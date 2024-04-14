using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    [SerializeField]
    private GameObject gravityPoint;
    public bool canGravity = false;

    public SkillBase skill2;

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
        if (!canMove) return;

        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                canMove = false;
                rb.velocity = Vector2.zero;

                animator.SetTrigger("CollisionDetected"); // 충돌 애니메이션 실행

                if (canGravity)
                {
                    StartCoroutine(ApplyGravityWell());
                }
                else
                {
                    #region nomalAttack
                    Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, Skill.SkillData.AttackRange); // 충돌한 몬스터 주변에 있는 몬스터들을 찾음

                    StartCoroutine(Util.DrawCircle(transform.position, Skill.SkillData.AttackRange, 16, Color.red, 5f)); // 공격 범위를 시각적으로 표시

                    foreach (var target in targets)
                    {
                        Monster targetMonster = target.GetComponent<Monster>();
                        if (targetMonster == null)
                            continue;

                        targetMonster.OnDamaged(Owner, Skill); // 몬스터에게 데미지 적용
                    }
                    #endregion
                }
                StartCoroutine(DestroyAfterAnimation(Skill.SkillData.Duration));
            }
        }
    }


    IEnumerator ApplyGravityWell()
    {
        float elapsedTime = 0; // 경과 시간

        while (elapsedTime <= skill2.SkillData.Duration)
        {
            elapsedTime+= Time.deltaTime;
            Debug.Log("???");
            Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, skill2.SkillData.AttackRange); // 충돌한 몬스터 주변에 있는 몬스터들을 찾음

            foreach (var item in targets)
            {
                // 1초마다 중력에 따라 오브젝트를 이동시킵니다.
                float moveSpeed = 3f; // 중력에 따른 이동 속도
                Vector3 directionToGravityPoint = (gravityPoint.transform.position - item.gameObject.transform.position).normalized;
                item.gameObject.transform.position += directionToGravityPoint * moveSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }

    IEnumerator DestroyAfterAnimation(float duration)
    {
        float waitTime = 1 > duration ? 1 : duration;
        yield return new WaitForSeconds(waitTime); // 애니메이션이 재생되는 시간을 기다림.

        Managers.Object.Despawn(this);
    }

    IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Managers.Object.Despawn(this);
    }
}