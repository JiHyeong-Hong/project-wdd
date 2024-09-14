using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static Define;

public class Crocodile : Projectile
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer swampRenderer;

    public float speed = 7f;
    private Vector2 moveDirection;

    [SerializeField]
    private GameObject gravityPoint;
    public bool isBreakthrough = false;

    public SkillBase skill2;
    private Monster target; // 주변의 몬스터 타겟

    void Start()
    {
        SetTarget();    

        StartCoroutine(DespawnAfterTime(4f));

        SizeControl(0.7f);
        swampRenderer = Util.FindChild<Transform>(transform.gameObject, "Swamp").GetComponent<SpriteRenderer>();
    }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        isInfinityDuration = true;

        SoundManager.Instance.Play(Define.ESoundMainType.Skill1, Define.ESoundType.Crocodile);
        return true;
    }

    private void SetTarget()
    {
        Boss boss = FindObjectOfType<Boss>(); 

        if (boss != null)
        {
            target = boss;
        }
        else
        {
            Collider2D[] nearbyMonsters = Util.SearchCollidersInRadius(transform.position, 20f); // 반경 10 내의 몬스터 검색
            foreach (var collider in nearbyMonsters)
            {
                Monster monster = collider.GetComponent<Monster>();
                if (monster != null)
                {
                    target = monster; // 첫 번째로 발견된 몬스터를 타겟으로 설정
                    break;
                }
            }
        }
    }

    protected override void Move()
    {
        if (target != null && canMove)
        {
            moveDirection = (target.transform.position - transform.position).normalized * speed;
            rb.velocity = moveDirection;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public Sprite testSp;
    private async UniTask DetectedMonster()
    {
        #region õõ�� �������� �ִϸ��̼�

        spriteRenderer.DOFade(0, 1);
        #endregion

        await UniTask.Delay(TimeSpan.FromSeconds(1));

        transform.rotation = Quaternion.Euler(0, 0, 0); // �浹 �� ȸ���� �ʱ�ȭ
        if (spriteRenderer.flipY)
        {
            spriteRenderer.flipY = false;
            spriteRenderer.flipX = true;
        }

        spriteRenderer.DOFade(1, 0);

        if (isBreakthrough)
        {
            SizeControl(2);
            swampRenderer.sprite = Resources.Load<Sprite>("Art/Effects/SwampBT");// �浹 ����Ʈ ����
            spriteRenderer.sprite = Util.Load("Art/Skills/CrocodileBT", "CrocodileBT_0");

            StartCoroutine(ApplyGravityWell());
        }
        else
        {
            #region nomalAttack
            SizeControl(1);
            
            spriteRenderer.sprite = Util.Load("Art/Skills/Crocodile", "Crocodile_4");
            StartCoroutine(ChangeSpriteAfterDelay(0.7f));
            swampRenderer.sprite = Resources.Load<Sprite>("Art/Effects/Swamp");// �浹 ����Ʈ ����
            SoundManager.Instance.Play(Define.ESoundMainType.Skill2, Define.ESoundType.Crocodile);
            

            Collider2D[] targets = Util.SearchCollidersInRadius(transform.position, Skill.SkillData.AttackRange * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.AttackRange))); // �浹�� ���� �ֺ��� �ִ� ���͵��� ã��

            StartCoroutine(Util.DrawCircle(transform.position, Skill.SkillData.AttackRange * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.AttackRange)), 16, Color.red, 5f)); // ���� ������ �ð������� ǥ��

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

    private void SizeControl(float size)
    {
        transform.localScale = new Vector3(size, size, 1);
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

                DetectedMonster().Forget();
            }
        }
    }


    IEnumerator ApplyGravityWell()
    {
        Collider2D[] targets = null;
        float elapsedTime = 0; // ��� �ð�

        swampRenderer.transform.DOScale(new Vector3(0.5f, 0.5f, 1), skill2.SkillData.AttackRange * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.AttackRange)));
        swampRenderer.transform.DOMove(gravityPoint.transform.position, skill2.SkillData.AttackRange * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.AttackRange)));

        while (elapsedTime <= 2)
        {
            elapsedTime+= Time.deltaTime;
            targets = Util.SearchCollidersInRadius(transform.position, skill2.SkillData.AttackRange * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.AttackRange))); // �浹�� ���� �ֺ��� �ִ� ���͵��� ã��

            foreach (var item in targets)
            {
                // 1�ʸ��� �߷¿� ���� ������Ʈ�� �̵���ŵ�ϴ�.
                float moveSpeed = 3f; // �߷¿� ���� �̵� �ӵ�
                Vector3 directionToGravityPoint = (gravityPoint.transform.position - item.gameObject.transform.position).normalized;
                item.gameObject.transform.position += directionToGravityPoint * moveSpeed * Time.deltaTime;
            }

            yield return null;
        }

        spriteRenderer.sprite = Util.Load("Art/Skills/CrocodileBT", "CrocodileBT_1");

        if (targets != null)
        {
            foreach (var item in targets)
            {
                item.GetComponent<Monster>().OnDamaged(Owner, skill2);
            }
        }
    }

     IEnumerator ChangeSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.sprite = Util.Load("Art/Skills/Crocodile", "Crocodile_5");
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

    private void OnDestroy()
    {
        swampRenderer.transform.localScale = new Vector3(1, 1, 1);
    }

}