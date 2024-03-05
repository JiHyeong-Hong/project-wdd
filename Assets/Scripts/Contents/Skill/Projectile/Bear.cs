using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Bear : Projectile
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;
		canMove = false;
		Animator = transform.GetChild(0).GetComponent<Animator>();
		Renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		
		Despawn();
		return true;
	}

	public float GetAnimLength()
	{
		return Animator.GetCurrentAnimatorStateInfo(0).length;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		BaseObject target = other.GetComponent<BaseObject>();
		if (target.IsValid() == false)
			return;

		Creature creature = target as Creature;
		
		if (creature.CreatureType != Define.ECreatureType.Monster)
			return;
		
		creature.OnDamaged(Owner, Skill);
	}
	private async void Despawn()
	{
		await Task.Delay(TimeSpan.FromSeconds(GetAnimLength() + 0.15f));
		Managers.Object.Despawn(this);
	}
}
