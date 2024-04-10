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
    public float crocodileHeightOffset = 5f; // Crocodile�� ��ȯ�� �� �߰������� ������ ���� ��

    [SerializeField]
    private GameObject gravityPoint;
    public bool canGravity = false;

    public SkillBase skill2;

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");

        hero = FindObjectOfType<Hero>();

        Direction();

        // �浹���� ���� �ν��Ͻ��� �ڵ����� Despawn�ǵ��� ��
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

                animator.SetTrigger("CollisionDetected"); // �浹 �ִϸ��̼� ����

                if (canGravity)
                {
                    StartCoroutine(ApplyGravityWell());
                }
                else
                {
                    #region nomalAttack
                    Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, Skill.SkillData.AttackRange); // �浹�� ���� �ֺ��� �ִ� ���͵��� ã��

                    StartCoroutine(Util.DrawCircle(transform.position, Skill.SkillData.AttackRange, 16, Color.red, 5f)); // ���� ������ �ð������� ǥ��

                    foreach (var target in targets)
                    {
                        Monster targetMonster = target.GetComponent<Monster>();
                        if (targetMonster == null)
                            continue;

                        targetMonster.OnDamaged(Owner, Skill); // ���Ϳ��� ������ ����
                    }
                    #endregion
                }
                StartCoroutine(DestroyAfterAnimation(Skill.SkillData.Duration));
            }
        }
    }


    IEnumerator ApplyGravityWell()
    {
        float elapsedTime = 0; // ��� �ð�

        while (elapsedTime <= skill2.SkillData.Duration)
        {
            elapsedTime+= Time.deltaTime;
            Debug.Log("???");
            Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, skill2.SkillData.AttackRange); // �浹�� ���� �ֺ��� �ִ� ���͵��� ã��

            foreach (var item in targets)
            {
                // 1�ʸ��� �߷¿� ���� ������Ʈ�� �̵���ŵ�ϴ�.
                float moveSpeed = 3f; // �߷¿� ���� �̵� �ӵ�
                Vector3 directionToGravityPoint = (gravityPoint.transform.position - item.gameObject.transform.position).normalized;
                item.gameObject.transform.position += directionToGravityPoint * moveSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }

    IEnumerator DestroyAfterAnimation(float duration)
    {
        float waitTime = 1 > duration ? 1 : duration;
        yield return new WaitForSeconds(waitTime); // �ִϸ��̼��� ����Ǵ� �ð��� ��ٸ�.

        Managers.Object.Despawn(this);
    }

    IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Managers.Object.Despawn(this);
    }
}