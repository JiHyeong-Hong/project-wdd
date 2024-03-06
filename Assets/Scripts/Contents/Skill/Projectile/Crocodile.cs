using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Projectile
{
    private Animator animator;

    void Start()
    {
        animator.ResetTrigger("CollisionDetected");
    }

    public override bool Init()
    {
         if (!base.Init())
             return false;

         animator = GetComponent<Animator>();
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
   
        // �ʿ��� ��� ���⿡ �߰� ���� �ڵ� �ۼ�
    }

    protected override void Move()
    {
        // Skill �Ǵ� SkillData�� null�̸� �̵��� ����
        if (Skill == null || Skill.SkillData == null)
        {
            Debug.LogWarning("Skill or SkillData is null in Crocodile.Move");
            return; // �ʱ�ȭ���� �ʾ����Ƿ� �̵� ó���� �ߴ�
        }
        base.Move();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                Debug.Log("OnTriggerEnter2D called with " + other.gameObject.name);
                Debug.Log("CollisionDetected Trigger Activated");
                animator.SetTrigger("CollisionDetected"); // �浹 �ִϸ��̼� ����
                monster.OnDamaged(Owner, Skill); // ���Ϳ��� ������ ����

                // �浹 �� Crocodile ������Ʈ ���� ����
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