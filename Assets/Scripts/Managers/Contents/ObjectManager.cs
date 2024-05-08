using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public Hero Hero { get; private set; }
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    public HashSet<Boss> Bosses { get; } = new HashSet<Boss>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
    public HashSet<Item> Items { get; } = new HashSet<Item>();
    public HashSet<Structure> Structures { get; } = new HashSet<Structure>();
    public HashSet<Spawner> Spawners { get; } = new HashSet<Spawner>();
    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    //TODO Eung 몬스터총합후 보스삭제
    public Transform BossRoot { get { return GetRootTransform("@Boss"); } }
    
    //TODO Eung 몬스터와 보스를 객체를 나눈어서 보는 경우 보스 루트를 생성 해야함 - 의논 필요
    // public Transform BossRoot { get { return GetRootTransform("@Boss"); } }
    
    public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform ItemRoot { get { return GetRootTransform("@Item"); } }
    public Transform StructureRoot { get { return GetRootTransform("@Structure"); } }
    public Transform SpawnerRoot { get { return GetRootTransform("@Spawners"); } }
    #endregion

    public T Spawn<T>(Vector3 position, int templateID, Transform parent = null) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if (obj.ObjectType == EObjectType.Creature)
        {
            Creature creature = go.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Hero:
                    obj.transform.parent = (parent == null) ? HeroRoot : parent;
                    Hero hero = creature as Hero;
                    Hero = hero;
                    hero.SetInfo(templateID);
                    hero.transform.position = Vector3.zero;
                    break;
                case ECreatureType.Monster:
                    //TODO Eung Pool사용시 Root 설정 변경
                    obj.transform.parent = (obj.transform.parent == null) ? MonsterRoot : obj.transform.parent;
                    Monster monster = creature as Monster;
                    Monsters.Add(monster);
                    monster.SetInfo(templateID);
                    break;
                case ECreatureType.Boss:
                    obj.transform.parent = (parent == null) ? BossRoot : parent;
                    //TODO Eung 보스 소환 코드 수정 필요
                    Boss boss = creature as Boss;
                    //TODO Eung 몬스터 통합하면 boss 수정
                    Monsters.Add(boss);
                    boss.SetInfo(templateID);
                    break;
                
                //TODO Eung ECreatureType.Boss의 경우 코드 작성 
            }
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            obj.transform.parent = (parent == null) ? ProjectileRoot : parent;

            Projectile projectile = go.GetComponent<Projectile>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Item)
        {
            obj.transform.parent = (parent == null) ? ItemRoot : parent;

            Item item = go.GetComponent<Item>();
            Items.Add(item);

            item.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Structure)
        {
            obj.transform.parent = (parent == null) ? StructureRoot : parent;
            
            Structure structure = go.GetComponent<Structure>();
            Structures.Add(structure);
        }
        else if (obj.ObjectType == EObjectType.Spawner)
        {
            obj.transform.parent = (parent == null) ? SpawnerRoot : parent;
            
            Spawner spawner = go.GetComponent<Spawner>();
            Spawners.Add(spawner);
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        EObjectType objectType = obj.ObjectType;

        if (obj.ObjectType == EObjectType.Creature)
        {
            Creature creature = obj.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Hero:
                    Hero hero = creature as Hero;
                    Hero = null;
                    break;
                case ECreatureType.Monster:
                    Monster monster = creature as Monster;
                    Monsters.Remove(monster);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            Projectile projectile = obj as Projectile;
            if (projectile.GetComponent<Poolable>() != null)
                Managers.Pool.Push(projectile.GetComponent<Poolable>());
            else
                Projectiles.Remove(projectile);
        }
        else if (obj.ObjectType == EObjectType.Item)
        {
            Item item = obj as Item;
            Items.Remove(item);
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    public List<Monster> FindMonsterByRange(Vector2 position, float range)
    {
        List<Monster> list = new List<Monster>();

        foreach (Monster monster in Monsters)
        {
            if (monster.Hp <= 0)
            {
                continue;
            }

            float distance = Vector2.Distance(position, monster.transform.position);
            if (distance > range)
            {
                continue;
            }

            list.Add(monster);
        }

        return list;
    }

    public Monster FindClosestMonster(Vector2 position, float range)
    {
        Monster closestMonster = null;
        float closestDistance = Mathf.Infinity;

        foreach (Monster monster in Monsters)
        {
            if (monster.Hp <= 0)
            {
                continue;
            }

            float distance = Vector2.Distance(position, monster.transform.position);
            if (distance > range)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMonster = monster;
            }
        }

        return closestMonster;
    }
}
