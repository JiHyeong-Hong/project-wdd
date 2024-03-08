using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ�� ���� ����
    public float speed = 4f; // �̵� �ӵ�

    private Vector2 moveDirection;
    private Hero hero; // Hero ��ü ����
    public float crocodileHeightOffset = 5f; // Crocodile�� ��ȯ�� �� �߰������� ������ ���� ��

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");

        hero = FindObjectOfType<Hero>(); // Scene ������ Hero ��ü�� ã�� ������ ����

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
        // Hero�� �ٶ󺸴� ������ �������� Crocodile�� ��ȯ ��ġ�� �̵� ���� ����
        Vector3 heroDirection = (hero.Destination.position - hero.transform.position).normalized;
        transform.position = hero.Destination.position + new Vector3(0, crocodileHeightOffset, 0);

        // �̵� ���� ����
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
                animator.SetTrigger("CollisionDetected"); // �浹 �ִϸ��̼� ����
                monster.OnDamaged(Owner, Skill); // ���Ϳ��� ������ ����

                // �浹 �� Crocodile ������Ʈ ����
                StartCoroutine(DestroyAfterAnimation());
            }
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // �ִϸ��̼��� ����Ǵ� �ð��� ��ٸ�, �ִϸ��̼� ���̿� �°� ���� �ʿ�
        Managers.Object.Despawn(this); // �����ϰ� ������Ʈ ����
    }
}