using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

		///
        isInfinityDuration = true;
		///


        Despawn().Forget();
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
	private async UniTaskVoid Despawn()
	{
        //await UniTask.Delay(TimeSpan.FromSeconds(GetAnimLength() + 0.15f));
        await UniTask.Delay(TimeSpan.FromSeconds(GetAnimLength() + 0.5f));
		
		// 객체가 아직 존재하는지 확인
		if (this == null || gameObject == null)
		{
			return;
		}
		//
      
        Managers.Object.Despawn(this);
	}
}
