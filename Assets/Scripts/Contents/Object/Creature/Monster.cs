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
        
        //TODO Eung Drop 데이터 테이블 만들고나서 봐야할듯?
        // DropItemID = monsterData.DropItemID;
        // DropPersent = monsterData.DropPersent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
            return;

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
            // Debug.Log(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (time >= cooltime)
            {
                Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                Debug.Log("원거리 공격!!");
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
        Debug.Log("보스 맞음");
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
            Debug.Log((int)time + "초 공격 대기중");
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
