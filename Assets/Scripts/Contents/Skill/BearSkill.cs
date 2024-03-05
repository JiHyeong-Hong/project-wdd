using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BearSkill : SkillBase
{
	private readonly float DISTANCE_OFFSET = 3f;
	private readonly float ANGLE_OFFSET = 70f;

	public override void DoSkill()
	{
		var hero = Managers.Object.Hero;
		var dir = Owner.Direction;
		bool isFlip = dir.x < 0;

		var angle = hero.Pivot.eulerAngles.z;
		angle += isFlip ? -ANGLE_OFFSET : ANGLE_OFFSET;

		Bear bear = Managers.Object.Spawn<Bear>((Vector2)Owner.transform.position + Owner.Direction * DISTANCE_OFFSET, SkillData.Projectile);
		bear.SetSpawnInfo(Owner, this, Vector3.zero);
		bear.transform.localScale = isFlip ? new Vector3(-1, 1, 1) : Vector3.one;
		bear.transform.eulerAngles = new Vector3(0, 0, angle);

		if (SkillData.Level >= Define.MAX_SKILL_LEVEL)
		{
			SpawnSecond(isFlip, bear.transform.eulerAngles.z, bear.GetAnimLength());
		}
	}

	private async void SpawnSecond(bool isFlip, float angle, float animLength)
	{
		await Task.Delay(TimeSpan.FromSeconds(animLength));
		isFlip = !isFlip;

		Bear bear2 = Managers.Object.Spawn<Bear>((Vector2)Owner.transform.position + Owner.Direction * (-1 * DISTANCE_OFFSET),
			SkillData.Projectile);

		bear2.transform.localScale = isFlip ? new Vector3(-1, 1, 1) : Vector3.one;
		bear2.SetSpawnInfo(Owner, this, Vector3.zero);
		bear2.transform.eulerAngles = new Vector3(0, 0, isFlip ? angle + (ANGLE_OFFSET * 0.5f) : angle - (ANGLE_OFFSET * 0.5f));
	}

	public override void Clear()
	{

	}
}