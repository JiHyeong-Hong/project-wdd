using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ�� ���� ����
    public float speed = 4f; // �̵� �ӵ�

    private Vector2 heroDirection = Vector2.right; // �⺻ ������ ���������� ����

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

    // Crocodile ��ȯ ��ġ �� ���� ������ ����
    public void SetInitialPositionAndDirection(Transform arrowTransform, Vector2 direction)
    {
        this.transform.position = arrowTransform.position + new Vector3(0, arrowTransform.localScale.y / 2, 0);
        this.heroDirection = direction.normalized; // ���� ����
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate call Move()");
        Move();
    }
    
    protected override void Move()
    {
        //// Skill �Ǵ� SkillData�� null�̸� �̵��� ����
        //if (Skill == null || Skill.SkillData == null)
        //{
        //    return; // �ʱ�ȭ���� �ʾ����Ƿ� �̵� ó���� �ߴ�
        //}
    
        // ������Ʈ�� �� �������� ���������� �̵�
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