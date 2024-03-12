using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DolPhine : BaseObject
{
	private class InCircleMonster
	{
		public int id;
		public float elapsedTime;
		public Monster monster;

		public Vector2 GetPos => monster.transform.position;
		public float GetRadius => monster.ColliderRadius;

		public InCircleMonster(int id, Monster monster)
		{
			this.id = id;
			this.monster = monster;
			elapsedTime = 0f;
		}
	}

	[SerializeField] private Transform sprite;
	private readonly float SPEED = 2;

	private float duration;
	private float elapsedTime;
	private float radius;

	private HashSet<int> hitMonsterKeys;
	private List<InCircleMonster> removeList;
	private HashSet<InCircleMonster> hitMonsters;

	public Creature Owner { get; private set; }
	public SkillBase Skill { get; private set; }

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		hitMonsters = new();
		hitMonsterKeys = new();
		removeList = new();
		Renderer = GetComponentInChildren<SpriteRenderer>();

        return true;
	}
	
	/// <summary>
	/// 여기에 hit를 넣지 않고 따로 계산해서 hit 하는 이유는
	/// 적의 움직임이 없으면 이 함수가 적용되지 않기 때문
	/// ex) 스턴걸린 몬스터는 장판딜이 들어가지 않는 문제 발생 
	/// </summary>
	/// <param name="col"></param>
	void OnTriggerStay2D(Collider2D col)
	{
		if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
		{
			var id = col.GetInstanceID();
			bool check = hitMonsterKeys.Add(id);

			if (!check)
				return;

			hitMonsters.Add(new InCircleMonster(id, col.GetComponent<Monster>()));
		}
	}

	private void Update()
	{
		RotateSprite();
		UpdateHitMonsters();
		UpdateDuration();
	}

	public void SetSpawnInfo(Creature owner, SkillBase skill)
	{
		Owner = owner;
		Skill = skill;

		//duration = skill.SkillData.Duration;
		duration = 5;
        Debug.Log($"[DolPhine] SetSpawnInfo called. SkillData Duration: {skill.SkillData.Duration}, duration set to: {duration}");
        //

        //public void SetSpawnInfo(Creature owner, SkillBase skill, int level)
        // 레벨에 따른 duration 설정
        //switch (level)
        //{
        //    case 1:
        //        duration = 5; // 1레벨일 때 5초
        //        break;
        //    case 2:
        //        duration = 10; // 2레벨일 때 10초
        //        break;
        //    case 3:
        //        duration = 15; // 3레벨일 때 15초
        //        break;
        //    default:
        //        duration = 5; // 기본값
        //        break;
        //}
	    //인스턴스가 생성되자마자 파괴되는 버그가 있어서 레벨에 따라 그냥 하드코딩 해버리는 것도...

        hitMonsters.Clear();
		hitMonsterKeys.Clear();
		removeList.Clear();

		radius = skill.SkillData.AttackRange;
		transform.localScale = Vector3.one * radius;
	}

	private void UpdateHitMonsters()
	{
		foreach (var val in hitMonsters)
		{
			val.elapsedTime -= Time.deltaTime;

			if (val.monster == null)
			{
				removeList.Add(val);
				continue;
			}
			
			//적의 collider랑 장판이 겹쳤는지 체크
			bool isIncircle = Util.CheckCircleCollision(transform.position, val.GetPos, radius, val.GetRadius);

			//틱 됐을 때
			if (val.elapsedTime <= 0f)
			{
				//만약 원 밖이면 삭제
				if (!isIncircle)
				{
					removeList.Add(val);
					continue;
				}

				//인서클이면 쿨 초기화 해주고 hit
				val.elapsedTime = 1f;
				if (val.monster != null)
				{
					val.monster.OnDamaged(Owner, Skill);
					// 스턴 추가
				}
			}
		}

		// Remove
		for (int i = removeList.Count - 1; i >= 0; i--)
		{
			hitMonsterKeys.Remove(removeList[i].id);
			hitMonsters.Remove(removeList[i]);
		}
		removeList.Clear();
	}
	
	private void RotateSprite()
	{
		sprite.transform.RotateAround(transform.position, Vector3.forward, SPEED * 100 * Time.deltaTime);
		sprite.rotation = Quaternion.identity;
	}

	private void UpdateDuration()
	{
		elapsedTime += Time.deltaTime;

        Debug.Log($"[DolPhine] elapsedTime: {elapsedTime}, duration: {duration}");

        if (elapsedTime > duration)
		{
			Managers.Object.Despawn(this);
		}
	}
}