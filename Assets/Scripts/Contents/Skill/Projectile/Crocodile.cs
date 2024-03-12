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
    public float crocodileHeightOffset = 5f; // Crocodile�� ��ȯ�� �� �߰������� ������ ���� ��

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");

        hero = FindObjectOfType<Hero>();

        InitializeDirectionAndPosition();

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

    private void InitializeDirectionAndPosition()
    {
        // Hero�� �ٶ󺸴� ������ �������� Crocodile�� ��ȯ ��ġ�� �̵� ���� ����
        Vector3 heroDirection = (hero.Destination.position - hero.transform.position).normalized;
        transform.position = hero.Destination.position + new Vector3(0, crocodileHeightOffset, 0);

        // �̵� ���� ����
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

                animator.SetTrigger("CollisionDetected"); // �浹 �ִϸ��̼� ����
                monster.OnDamaged(Owner, Skill); // ���Ϳ��� ������ ����

                StartCoroutine(DestroyAfterAnimation());
            }
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // �ִϸ��̼��� ����Ǵ� �ð��� ��ٸ� �ϴ� 1�ʷ�...
       
        Managers.Object.Despawn(this);
    }

    IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Managers.Object.Despawn(this);
    }
}