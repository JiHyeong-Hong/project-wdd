using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
	public Creature Owner { get; private set; }
	public SkillBase Skill { get; private set; }
	public Data.ProjectileData ProjectileData { get; private set; }

	protected bool canMove = true;
	protected bool isInfinityDuration;	//임시
	
	private float duration;
	private float elapsedTime;
	
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Projectile;

		return true;
	}

	public void SetInfo(int dataTemplateID)
	{
		ProjectileData = Managers.Data.ProjectileDic[dataTemplateID];
		Renderer.sortingOrder = SortingLayers.PROJECTILE;
	}

	public virtual void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
	{
		Owner = owner;
		Skill = skill;
		
		//duration = skill.SkillData.Duration;
		//TODO Eung 스킬이 아닌 경우 회전값 0으로 일단 둠
		duration = skill != null ? skill.SkillData.Duration : 0;
		float angle = Util.VectorToAngle(direction);
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
	}
	
	
	protected virtual void Move()
	{
		if (canMove)
		{
			if(Skill != null)
				transform.Translate(Vector2.up * (Skill.SkillData.AttackSpeed * Time.deltaTime));
			else
				//스킬이 없는 경우 속도 고정으로 이동
				transform.Translate(Vector2.up * (1 * Time.deltaTime));
		}
	}

	void Update()
	{
		UpdateDuration();
		Move();
	}

	private void UpdateDuration()
	{
		if (isInfinityDuration)
			return;
		
		elapsedTime += Time.deltaTime;
		if (elapsedTime > duration)
		{
			Managers.Object.Despawn(this);
		}
	}
}