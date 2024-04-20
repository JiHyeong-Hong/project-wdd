using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bear : Projectile
{
	public GameObject[] Skills;
	public CircleCollider2D myCollider2D;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		canMove = false;
		Animator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
		Renderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        myCollider2D = GetComponent<CircleCollider2D>();
        ///
        isInfinityDuration = true;
		///


        Despawn().Forget();
		return true;
	}

	public float GetAnimLength()
	{
		if(!IsAnimationPlaying()) return 0;

		return Animator.GetCurrentAnimatorStateInfo(0).length;
	}

	private bool IsAnimationPlaying()
	{
		if (Animator == null) return false;
		if (Animator.isActiveAndEnabled == false) return false;

		AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
		float animTime = stateInfo.normalizedTime;
		if (animTime > 0 && animTime < 1.0f) return true;


		return false;
	}

    void OnTriggerEnter2D(Collider2D other)
	{
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
		{
            Monster monster = other.gameObject.GetComponent<Monster>();
			monster.OnDamaged(Owner, Skill);
        }
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
