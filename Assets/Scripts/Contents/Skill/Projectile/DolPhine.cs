using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DolPhine : Projectile
{
	[SerializeField] private Transform spriteTransform;
	private Vector3 point;
	
	private readonly float SPEED = 2;
	
	public override bool Init()
	{
		if (base.Init() == false)
			return false;
		
		Renderer = GetComponentInChildren<SpriteRenderer>(); 
		return true;
	}


	public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
	{
		base.SetSpawnInfo(owner, skill, direction);
		Collider.radius = skill.SkillData.AttackRange;
		point = (Vector2)transform.position + (Vector2.left * skill.SkillData.AttackRange);
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
		{
			Monster monster = other.gameObject.GetComponent<Monster>();
			monster.OnDamaged(Owner, Skill);
			
			//넉백 추가
		}
	}

	protected override void Move()
	{
		Transform tr;
		(tr = transform).RotateAround(point,Vector3.forward, SPEED * 100 * Time.deltaTime);
		
		var rotation = tr.rotation;
		spriteTransform.rotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, rotation.z));
	}
}