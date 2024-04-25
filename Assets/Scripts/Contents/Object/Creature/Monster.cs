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
        //ëª¬ìŠ¤???´ëž˜?¤ì—??ëª¬ìŠ¤?°ì? ë³´ìŠ¤ ?€???¬ë¶„ë¥?
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
        //TODO Eung Drop ë°ì´í„° í…Œì´ë¸” ë§Œë“¤ê³ ë‚˜ì„œ ë´ì•¼í• ë“¯?
        DropItemID = monsterData.DropItemID;
        DropPersent = monsterData.DropPersent;
        //TODO Eung Drop ?°ì´???Œì´ë¸?ë§Œë“¤ê³ ë‚˜??ë´ì•¼? ë“¯?
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
        /// jh ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½Æ°ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
        {
            Hero hero = creature as Hero;
            if (hero != null && hero.IsInvincible)
            {
                // ï¿½ï¿½ï¿½Æ°ï¿½ï¿½ï¿½ ï¿½Ö´Ï¸ï¿½ï¿½Ì¼ï¿½ ï¿½ï¿½ï¿½ï¿½
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.AddForce(new Vector2(0, 500)); // ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½
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
        //ê³µê²© ì£¼ê¸°
        cooltime = monsterData.CoolTime/2;
        float time = 0f;

        while (true)
        {
            if (time >= cooltime)
            {
                Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                Debug.Log("?ê±°ë¦?ê³µê²©!!");
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
            //Hero searchê°€ ??ê²½ìš° idle ?íƒœë¡?ë³€ê²?
            CreatureState = ECreatureState.Idle;
        }
    }

    public Coroutine cotest = null;
    //ê³µê²© ?€ê¸°ìƒ??
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
        
        //TODO Eung ê³µê²© ì½”ë£¨???„ìš”
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
                //TODO Eung 5,6 ìƒìˆ˜ëŠ” min / max ê±°ë¦¬ ë°ì´í„°ë¡œ ì¹˜í™˜
                //ì²˜ìŒ íƒìƒ‰í• ë•Œ ê±°ë¦¬ê°€ minì´í•˜ë¡œ ì ‘ê·¼ í•´ì•¼í•¨ min = 5
                if (distance > monsterData.MinStance)

                //TODO Eung 5,6 ?ìˆ˜??min / max ê±°ë¦¬ ?°ì´?°ë¡œ ì¹˜í™˜
                //ì²˜ìŒ ?ìƒ‰? ë•Œ ê±°ë¦¬ê°€ min?´í•˜ë¡??‘ê·¼ ?´ì•¼??min = 5
                if (distance > 5)

                    return false;
                else
                {
                    //min?´í•˜??ê±°ë¦¬???‘ê·¼?œê²½??ê³µê²© ê°€?¥ìƒ?œë¡œ ë³€ê²?
                    Atk_chk = !Atk_chk;
                    //ê³µê²© ?€ê¸°ìƒ?Œì¸ Idle ?íƒœ ë³€ê²?
                    CreatureState = ECreatureState.Idle;
                    return true;
                }
                    
            else
            {

                //ê³µê²© ê°€ëŠ¥í•œ ìƒíƒœì—ì„œ ê±°ë¦¬ê°€ minë³´ë‹¤ ê±°ë¦¬ê°€ ë©€ì–´ì§€ëŠ”ê²½ìš° maxì´ˆê³¼ì˜ ê±°ë¦¬ë¡œ ë²—ì–´ë‚œ ê²½ìš° ë‹¤ì‹œ ìž¬íƒìƒ‰
                if (distance > monsterData.MaxStane)

                //ê³µê²© ê°€?¥í•œ ?íƒœ?ì„œ ê±°ë¦¬ê°€ minë³´ë‹¤ ê±°ë¦¬ê°€ ë©€?´ì??”ê²½??maxì´ˆê³¼??ê±°ë¦¬ë¡?ë²—ì–´??ê²½ìš° ?¤ì‹œ ?¬íƒ??
                if (distance > 6)

                {
                    //max ì´ˆê³¼??ê²½ìš° ê³µê²© ë¶ˆëŠ¥ ?íƒœë¡?ë³€ê²?
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

            Debug.Log((int)time + "ì´?ê³µê²© ?€ê¸°ì¤‘");

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
