using System.Collections;
using System.Collections.Generic;
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

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        CreatureState = ECreatureState.Attack;

        Renderer.sortingOrder = SortingLayers.MONSTER;
        _hero = Managers.Object.Hero;
        
        Data.MonsterData monsterData = CreatureData as Data.MonsterData;
        //몬스터 클래스에서 몬스터와 보스 타입 재분류
        switch (monsterData.type)
        {
            case 1:
                CreatureType = ECreatureType.Monster;
                break;
            case 2:
                CreatureType = ECreatureType.Boss;
                break;
        }
        
        DropItemID = monsterData.DropItemID;
        DropPersent = monsterData.DropPersent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
            return;

        // TODO
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
    private Hero _hero;
    private float distance = 0f;
    
    protected override void UpdateAttack()
    {
        distance = Vector2.Distance(_hero.transform.position, this.transform.position);
        
        if (_hero.IsValid())
        {
            Vector2 dest = (_hero.transform.position - transform.position).normalized;

            switch (CreatureData.Atktype)
            {
                case 1:
                    SetRigidbodyVelocity(dest * MoveSpeed);
                    Debug.Log("근접 공격!!");
                    break;
                case 2:
                    if(distance >= 5)
                        // Debug.Log("근접 공격!!");
                        SetRigidbodyVelocity(dest * MoveSpeed);
                    else
                        //TODO 원거리 공격 코루틴 작성 필요 - 원거리 공격중 Creature.UpdateAITick 시간 변경후 루프 시간 설정할 예정 
                        SetRigidbodyVelocity(dest * 0);
                    break;
            }
        }
        else
            SetRigidbodyVelocity(Vector2.zero);
    }

    protected override void UpdateHit()
    {
        CreatureState = ECreatureState.Attack;
    }
    #endregion
}
