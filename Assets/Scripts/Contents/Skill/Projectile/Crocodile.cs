using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ�� ���� ����
    public float speed = 4f; // �̵� �ӵ�

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ�� ������

        if (animator == null)
         {
             return false;
         }

         return true; 
}

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        // ���� ���� ������ ����, direction ��� Random.insideUnitCircle.normalized ���
        base.SetSpawnInfo(owner, skill, Random.insideUnitCircle.normalized);
        transform.rotation = Quaternion.Euler(0f, 0f, Util.VectorToAngle(Random.insideUnitCircle.normalized));
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
        Vector2 moveDirection = transform.up * speed;
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