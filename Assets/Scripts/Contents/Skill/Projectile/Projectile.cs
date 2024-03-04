using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
	public Creature Owner { get; private set; }
	public SkillBase Skill { get; private set; }
	public Data.ProjectileData ProjectileData { get; private set; }

	protected bool canMove = true;

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

		float angle = Util.VectorToAngle(direction);
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
	}

	private float tick = 0f;
	void Update()
	{
		if (canMove)
			transform.Translate(Vector2.up * 5 * Time.deltaTime);

		tick += Time.deltaTime;
		if (tick > 5f)
		{
			Managers.Object.Despawn(this);
		}
	}
}