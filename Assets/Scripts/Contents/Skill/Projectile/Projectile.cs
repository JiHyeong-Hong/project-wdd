using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
	public Creature Owner { get; private set; }
	public SkillBase Skill { get; private set; }
	public Data.ProjectileData ProjectileData { get; private set; }

	protected bool canMove = true;
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

		duration = skill.SkillData.Duration;
		float angle = Util.VectorToAngle(direction);
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
	}

	

	protected virtual void Move()
	{
		if (canMove)
			transform.Translate(Vector2.up * (Skill.SkillData.AttackSpeed * Time.deltaTime));
		
	}

	void Update()
	{
		UpdateDuration();
		Move();
	}

	private void UpdateDuration()
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > duration)
		{
			Managers.Object.Despawn(this);
		}
	}
}