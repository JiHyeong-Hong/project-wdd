using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;
using Random = UnityEngine.Random;

public class Monster : Creature
{
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹
    public void SetDamageTextPrefab(GameObject prefab)
    {
        damageTextPrefab = prefab;
        Debug.Log("DamageTextPrefab set: " + damageTextPrefab);
    }

    #region Stat

    public int DropItemID { get; set; }
    public int DropPersent { get; set; }
    #endregion
    public float temp;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;

        return true;
    }

    protected MonsterData monsterData;
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Renderer.sortingOrder = SortingLayers.MONSTER;
        _hero = Managers.Object.Hero;
        
        monsterData = CreatureData as MonsterData;
        //몬스터 클래스에서 몬스터와 보스 타입 재분류
        switch (monsterData.Type)
        {
            case 1:
                CreatureState = ECreatureState.Move;
                break;
            case 2:
                CreatureType = ECreatureType.MiddleBoss;
                CreatureState = ECreatureState.Move;
                break;
            case 3:
                CreatureState = ECreatureState.Move;
                break;
        }
        
        test = StartCoroutine(CoUpdateAI());
        //TODO Eung Drop 데이터 테이블 만들고나서 봐야할듯?
        DropItemID = monsterData.DropItemID;
        DropPersent = monsterData.DropPersent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        //  Creature creature = target as Creature;
        //  if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
        //      return;
        /// jh ���� ���� �÷��̾�� ������ ���ư� �������
        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
        {
            Hero hero = creature as Hero;
            if (hero != null && hero.IsInvincible)
            {
                // ���ư��� �ִϸ��̼� ����
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.AddForce(new Vector2(0, 500)); // ����, ��
                }

                Destroy(gameObject);
                return;
            }
        }

        // TODO Eung
        //target.OnDamaged(this, null);
    }

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        // damageTextPrefab이 null인 경우 초기화 시도
        if (damageTextPrefab == null)
        {
            //Debug.LogError("DamageTextPrefab is not set in OnDamaged");
            // 프리팹이 설정되지 않은 경우 기본값으로 설정하거나 오류 처리
            damageTextPrefab = Resources.Load<GameObject>("Prefabs/DamageText");
            if (damageTextPrefab == null)
            {
                Debug.LogError("Failed to load DamageTextPrefab from Resources");
                return;
            }
        }

        // 데미지 텍스트 표시
        float damage = CalculateDamage(attacker, skill); // 계산된 데미지를 가져옴
        Debug.Log("OnDamaged called, damageTextPrefab: " + damageTextPrefab);

        // damageTextPrefab이 null인 경우 초기화 시도
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageTextPrefab is not set in OnDamaged");
            // 프리팹이 설정되지 않은 경우 기본값으로 설정하거나 오류 처리
            damageTextPrefab = Managers.Resource.Load<GameObject>("Prefabs/DamageText");
            if (damageTextPrefab == null)
            {
                Debug.LogError("Failed to load DamageTextPrefab");
                return;
            }
        }

        ShowDamageText(damage);

    }

    private float CalculateDamage(BaseObject attacker, SkillBase skill)
    {
        if (attacker.IsValid() == false)
            return 0;

        Creature creature = attacker as Creature;
        Projectile projectile = null;

        if (creature == null)
        {
            projectile = attacker as Projectile;
        }

        if (creature == null && projectile == null)
            return 0;

        float finalDamage = 0;

        if (skill == null)
        {
            if (creature != null)
                finalDamage = creature.Atk;
            else
                finalDamage = projectile.ProjectileData.ContactDmg;
        }
        else if (CreatureType == ECreatureType.Hero)
            finalDamage = skill.SkillData.Damage + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Attack);
        else if (CreatureType == ECreatureType.Monster || CreatureType == ECreatureType.MiddleBoss || CreatureType == ECreatureType.Boss)
            finalDamage = skill.SkillData.Damage + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Attack);

        return finalDamage;
    }

    private void ShowDamageText(float damage)
    {
        if (damageTextPrefab == null)
        {
            //Debug.LogError("DamageTextPrefab is not set");
            return;
        }

        // 데미지 텍스트 프리팹을 인스턴스화하여 현재 몬스터 위치에 표시
        GameObject textObject = Instantiate(Managers.Resource.Load<GameObject>("Prefabs/DamageText"), transform.position, Quaternion.identity);
        // 데미지 텍스트를 몬스터의 자식 오브젝트로 설정
        textObject.transform.SetParent(transform);
        // 텍스트 오브젝트의 위치를 머리 위로 이동
        textObject.transform.localPosition = new Vector3(0, 0, 0); // 몬스터의 머리 위


        // 데미지 텍스트 설정 및 표시
        DamageText damageText = textObject.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.ShowDamage(damage);
        }
        else
        {
            Debug.LogError("DamageText component is missing.");
        }
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        int rand = Random.Range(0, 100);
        if (rand <= DropPersent)
        {
            Managers.Object.Spawn<Item>(transform.position, DropItemID);
        }
        

        if(test != null)
            StopCoroutine(test);
        test = null;
        Managers.Resource.Destroy(gameObject);
    }
    #endregion

    #region AI
    public Hero _hero;
    private float distance = 0f;
    public float cooltime = 0f;
    public bool Atk_chk;

    protected virtual IEnumerator Attack()
    {
        //공격 주기
        cooltime = monsterData.CoolTime/2;
        float time = 0f;

        while (true)
        {
            if (time >= cooltime)
            {
                Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                var proj = Managers.Object.Spawn<EnemyProjectile>(transform.position, monsterData.ProjectileID);
                proj.SetImage();
                proj.SetSpawnInfo(this, null, direction);
                proj.SetTarget(_hero);
                
                cotest = null;
                CreatureState = ECreatureState.Idle;
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    
    protected override void UpdateAttack()
    {
        if (cotest == null)
        {
            cotest = StartCoroutine(Attack());
        }
        
        Vector2 dest = (_hero.transform.position - transform.position).normalized;
        SetRigidbodyVelocity(dest * 0);
        SetImageDirecton(dest);
        
    }
    protected override void UpdateMove()
    {
        bool searching = HeroSearching();
        // UpdateAITick = 0.1f;
        if (!searching)
        {
            if (_hero.IsValid())
            {
                Vector2 dest = (_hero.transform.position - transform.position).normalized;

                SetRigidbodyVelocity(dest * MoveSpeed);
            }
            else
                SetRigidbodyVelocity(Vector2.zero);
        }
        else
        {
            //Hero search가 된 경우 idle 상태로 변경
            CreatureState = ECreatureState.Idle;
        }
    }

    public Coroutine cotest = null;
    //공격 대기상태
    protected override void UpdateIdle()
    {
        // UpdateAITick = 100f;

        if (cotest == null)
        {
            cotest = StartCoroutine(CAttackWait());
        }
        
        Vector2 dest = (_hero.transform.position - transform.position).normalized;
        SetRigidbodyVelocity(dest * 0);
        SetImageDirecton(dest);
        
        //TODO Eung 공격 코루틴 필요
    }

    protected override void UpdateHit()
    {
        if(CreatureType != ECreatureType.Boss)
            CreatureState = ECreatureState.Idle;
    }

    public bool HeroSearching()
    {
        if (monsterData.AttackType == 1 || monsterData.AttackType == 3)
            return false;
        
        distance = Vector2.Distance(_hero.transform.position, this.transform.position);
        
        if (_hero.IsValid())
        {
            Vector2 dest = (_hero.transform.position - transform.position).normalized;

            if(!Atk_chk)
                //TODO Eung 5,6 상수는 min / max 거리 데이터로 치환
                //처음 탐색할때 거리가 min이하로 접근 해야함 min = 5
                if (distance > monsterData.MinStance)
                    return false;
                else
                {
                    //min이하의 거리에 접근한경우 공격 가능상태로 변경
                    Atk_chk = !Atk_chk;
                    //공격 대기상ㅌ인 Idle 상태 변경
                    CreatureState = ECreatureState.Idle;
                    return true;
                }
                    
            else
            {
                //공격 가능한 상태에서 거리가 min보다 거리가 멀어지는경우 max초과의 거리로 벗어난 경우 다시 재탐색
                if (distance > monsterData.MaxStane)
                {
                    //max 초과인 경우 공격 불능 상태로 변경
                    Atk_chk = !Atk_chk;
                    return false;
                }
                else
                {
                    CreatureState = ECreatureState.Idle;
                    return true;
                }
            }
        }
        else
        {
            SetRigidbodyVelocity(Vector2.zero);
            return false;
        }
    }

    IEnumerator CAttackWait()
    {
        cooltime = monsterData.CoolTime/2;
        float time = 0f;
        while (true)
        {
            bool searching = HeroSearching();
            if (!searching)
            {
                CreatureState = ECreatureState.Move;
                cotest = null;
                break;
            }
            
            if (time > cooltime)
            {
                CreatureState = ECreatureState.Attack;
                cotest = null;
                cooltime = 0f;
                break;
            }

            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    protected virtual IEnumerator Skill1()
    {
        yield return null;
    }

    #endregion
}
