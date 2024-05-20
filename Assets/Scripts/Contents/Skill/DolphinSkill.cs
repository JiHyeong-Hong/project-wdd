using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinSkill : SkillBase
{
	private readonly float[] angles = { 10f, 190f };
	private readonly float DISTANCE = 5f;
	public override void DoSkill()
	{
		for (int i = 0; i < 2; i++)
		{
			var dir =  Util.AngleToVector(angles[i]);

			var spawnPos = (Vector2)Owner.transform.position + (dir * DISTANCE);
			
			var dolphin = Managers.Object.Spawn<Dolphin>(spawnPos, 1);
			dolphin.SetSpawnInfo(Owner,this);
		}
	}
	public override void Clear()
	{

	}
}