using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public float speed = 4f; // �̵� �ӵ�

    private Vector2 moveDirection;
    private Hero hero;
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
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
         {
             return false;
         }

         return true; 
}

 //  void Update()
 //  {
 //      CheckDirection();
 //      // ��������Ʈ ȸ�� ���Ѿ�.. ���η� ���󰡰� ���η� ���󰡰�
 //  }
 //
    void FixedUpdate()
    {
        Move();
    }

   // void CheckDirection()
   // {
   //     if (spriteRenderer == null) return;
   //
   //     float angle = hero.transform.eulerAngles.z;
   //
   //     Debug.Log($"Hero Angle: {angle}");
   //
   //     spriteRenderer.flipX = false;
   //     transform.rotation = Quaternion.Euler(0, 0, 0); 
   //
   //     // Hero�� ���� �ٶ� ��
   //     if (angle > 45 && angle <= 135)
   //     {
   //         transform.rotation = Quaternion.Euler(0, 0, 90);
   //     }
   //
   // //    // Hero�� �������� �ٶ� ��
   // //    else if (angle > 135 && angle <= 225)
   // //    {
   // //        transform.rotation = Quaternion.Euler(0, 0, 0);
   // //        spriteRenderer.flipX = false;
   //  //
   //  //}
   //
   //     // Hero�� ������ �ٶ� ��
   //     else if ((angle > 225 && angle <= 315) || (angle > -135 && angle <= -45))
   //     {
   //         spriteRenderer.flipX = true;
   //     }
   //     // Hero�� �Ʒ��� �ٶ� ��
   //     else if (angle > 315 || angle <= 45)
   //     {
   //         transform.rotation = Quaternion.Euler(0, 0, -90);
   //     }
   // }

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
            rb.velocity = Vector2.zero; // �̵� ����
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                canMove = false; // �̵� ����
                rb.velocity = Vector2.zero; // Rigidbody2D�� �ӵ��� ��� 0���� �����Ͽ� ����

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