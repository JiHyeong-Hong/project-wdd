using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class Monster : Creature
{
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
        StartCoroutine(CoUpdateAI());

        return true;
    }

    protected MonsterData monsterData;
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Renderer.sortingOrder = SortingLayers.MONSTER;
        _hero = Managers.Object.Hero;
        
        monsterData = CreatureData as MonsterData;
        //몬스???�래?�에??몬스?��? 보스 ?�???�분�?
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
        
        //TODO Eung Drop ?�이???�이�?만들고나??봐야?�듯?
        // DropItemID = monsterData.DropItemID;
        // DropPersent = monsterData.DropPersent;
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
        target.OnDamaged(this, null);
    }

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        int rand = Random.Range(0, 100);
        if (rand <= DropPersent)
        {
            Managers.Object.Spawn<Item>(transform.position, DropItemID);
        }

        Managers.Object.Despawn(this);
    }
    #endregion

    #region AI
    protected Hero _hero;
    private float distance = 0f;
    public float cooltime = 0f;
    public bool Atk_chk;

    protected virtual IEnumerator Attack()
    {
        //공격 주기
        cooltime = 2f;
        float time = 0f;

        while (true)
        {
            if (time >= cooltime)
            {
                Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                Debug.Log("?�거�?공격!!");
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
            //Hero search가 ??경우 idle ?�태�?변�?
            CreatureState = ECreatureState.Idle;
        }
    }

    public Coroutine cotest = null;
    //공격 ?�기상??
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
        
        //TODO Eung 공격 코루???�요
    }

    protected override void UpdateHit()
    {
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
                //TODO Eung 5,6 ?�수??min / max 거리 ?�이?�로 치환
                //처음 ?�색?�때 거리가 min?�하�??�근 ?�야??min = 5
                if (distance > 5)
                    return false;
                else
                {
                    //min?�하??거리???�근?�경??공격 가?�상?�로 변�?
                    Atk_chk = !Atk_chk;
                    //공격 ?�기상?�인 Idle ?�태 변�?
                    CreatureState = ECreatureState.Idle;
                    return true;
                }
                    
            else
            {
                //공격 가?�한 ?�태?�서 거리가 min보다 거리가 멀?��??�경??max초과??거리�?벗어??경우 ?�시 ?�탐??
                if (distance > 6)
                {
                    //max 초과??경우 공격 불능 ?�태�?변�?
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
        cooltime = 2f;
        float time = 0f;
        while (true)
        {
            Debug.Log((int)time + "�?공격 ?�기중");
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
