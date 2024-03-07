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

        CreatureState = ECreatureState.Move;

        Renderer.sortingOrder = SortingLayers.MONSTER;

        Data.MonsterData monsterData = CreatureData as Data.MonsterData;
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

    protected override void UpdateMove()
    {
        _hero = Managers.Object.Hero;

        if (_hero.IsValid())
        {
            Vector2 dest = (_hero.transform.position - transform.position).normalized;

          //  SetRigidbodyVelocity(dest * MoveSpeed);
            SetRigidbodyVelocity(dest * 0);
        }
        else
            SetRigidbodyVelocity(Vector2.zero);
    }

    protected override void UpdateHit()
    {
        CreatureState = ECreatureState.Move;
    }
    #endregion
}
