using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Crocodile : Projectile
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer swampRenderer;

    public float speed = 4f;

    private Vector2 moveDirection;
    private Hero hero;
    public float crocodileHeightOffset = 5f; // Crocodile이 소환될 때 추가적으로 더해줄 높이 값

    [SerializeField]
    private GameObject gravityPoint;
    public bool isBreakthrough = false;

    public SkillBase skill2;

    void Start()
    {
        hero = FindObjectOfType<Hero>();

        Direction();

        // 충돌하지 않은 인스턴스는 자동으로 Despawn되도록 함
        StartCoroutine(DespawnAfterTime(5f));

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

    public Sprite testSp;
    private async UniTask DetectedMonster()
    {
        #region 천천히 없어지는 애니메이션

        spriteRenderer.DOFade(0, 1);
        #endregion

        await UniTask.Delay(TimeSpan.FromSeconds(1));

        transform.rotation = Quaternion.Euler(0, 0, 0); // 충돌 시 회전값 초기화
        if (spriteRenderer.flipY)
        {
            spriteRenderer.flipY = false;
            spriteRenderer.flipX = true;
        }

        spriteRenderer.DOFade(1, 0);

        if (isBreakthrough)
        {
            SizeControl(2);
            swampRenderer.sprite = Resources.Load<Sprite>("Art/Effects/SwampBT");// 충돌 이펙트 생성
            spriteRenderer.sprite = Util.Load("Art/Skills/CrocodileBT", "CrocodileBT_0");

            StartCoroutine(ApplyGravityWell());
        }
        else
        {
            #region nomalAttack
            SizeControl(1);
            swampRenderer.sprite = Resources.Load<Sprite>("Art/Effects/Swamp");// 충돌 이펙트 생성
            spriteRenderer.sprite = Util.Load("Art/Skills/Crocodile", "Crocodile_4");

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
        float elapsedTime = 0; // 경과 시간

        swampRenderer.transform.DOScale(new Vector3(0.5f, 0.5f, 1), skill2.SkillData.AttackRange);
        swampRenderer.transform.DOMove(gravityPoint.transform.position, skill2.SkillData.AttackRange);

        while (elapsedTime <= 2)
        {
            elapsedTime+= Time.deltaTime;
            targets = Util.SearchCollidersInRadius(transform.position, skill2.SkillData.AttackRange); // 충돌한 몬스터 주변에 있는 몬스터들을 찾음

            foreach (var item in targets)
            {
                // 1초마다 중력에 따라 오브젝트를 이동시킵니다.
                float moveSpeed = 3f; // 중력에 따른 이동 속도
                Vector3 directionToGravityPoint = (gravityPoint.transform.position - item.gameObject.transform.position).normalized;
                item.gameObject.transform.position += directionToGravityPoint * moveSpeed * Time.deltaTime;
            }

            yield return null;
        }

        if(targets != null)
        {
            foreach (var item in targets)
            {
                item.GetComponent<Monster>().OnDamaged(Owner, skill2);
            }
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

    private void OnDestroy()
    {
        swampRenderer.transform.localScale = new Vector3(1, 1, 1);
    }

}