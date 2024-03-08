using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public float speed = 4f; // 이동 속도

    private Vector2 moveDirection;
    private Hero hero;
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
 //      // 스프라이트 회전 시켜야.. 세로로 날라가고 가로로 날라가고
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
   //     // Hero가 위를 바라볼 때
   //     if (angle > 45 && angle <= 135)
   //     {
   //         transform.rotation = Quaternion.Euler(0, 0, 90);
   //     }
   //
   // //    // Hero가 오른쪽을 바라볼 때
   // //    else if (angle > 135 && angle <= 225)
   // //    {
   // //        transform.rotation = Quaternion.Euler(0, 0, 0);
   // //        spriteRenderer.flipX = false;
   //  //
   //  //}
   //
   //     // Hero가 왼쪽을 바라볼 때
   //     else if ((angle > 225 && angle <= 315) || (angle > -135 && angle <= -45))
   //     {
   //         spriteRenderer.flipX = true;
   //     }
   //     // Hero가 아래를 바라볼 때
   //     else if (angle > 315 || angle <= 45)
   //     {
   //         transform.rotation = Quaternion.Euler(0, 0, -90);
   //     }
   // }

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
            rb.velocity = Vector2.zero; // 이동 멈춤
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                canMove = false; // 이동 멈춤
                rb.velocity = Vector2.zero; // Rigidbody2D의 속도를 즉시 0으로 설정하여 멈춤

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