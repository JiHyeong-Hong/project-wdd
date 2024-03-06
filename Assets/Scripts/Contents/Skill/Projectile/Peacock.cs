using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock : Projectile
{
	private Monster target;
	private Vector2 dir;
	private CircleCollider2D col;
	
	public override bool Init()
	{
		if (col == null)
			col = GetComponent<CircleCollider2D>();

		isInfinityDuration = true;
		return base.Init();
	}

	public void SetTarget(Monster target)
	{
		this.target = target;
		col.enabled = target == null;
	}

	protected override void Move()
	{
		if (target != null)
		{
			transform.position = Vector2.MoveTowards(transform.position,
				target.transform.position, Skill.SkillData.AttackSpeed * Time.deltaTime);

			if (Vector2.SqrMagnitude(transform.position - target.transform.position) <= Mathf.Pow(0.5f,2))
			{
				target.OnDamaged(Owner,Skill);
				Managers.Object.Despawn(this);
			}
		}
		else
		{
			transform.Translate(Vector2.up * (Skill.SkillData.AttackSpeed * Time.deltaTime));
		}
		
		if(!Util.CheckTargetInScreen(transform.position))
			Managers.Object.Despawn(this);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
		{
			col.GetComponent<Monster>().OnDamaged(Owner,Skill);
			Managers.Object.Despawn(this);
		}
	}
}