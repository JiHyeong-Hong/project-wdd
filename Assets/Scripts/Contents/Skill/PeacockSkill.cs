using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Data;
public class PeacockSkill : SkillBase
{
	private List<Monster> monsterList = new List<Monster>();

	public override void DoSkill()
	{
		monsterList.Clear();
		GetTargets();
	}
	public override void LevelUp(SkillData data)
	{
		base.LevelUp(data);
		//targets = new Monster[data.Projectile];
	}

	private void GetTargets()
	{
		var list = Managers.Object.Monsters;

		foreach (var monster in list)
		{
			if (monster.Hp <= 0)
				continue;

			if (Util.CheckTargetInScreen(monster.transform.position))
			{
				monsterList.Add(monster);
			}
		}

		//var targets = await targetChecker.GetTargets();
		for (int i = 0; i < SkillData.Projectile; i++)
		{
			bool isNull = monsterList.Count == 0;

			int idx = Random.Range(0, monsterList.Count);
			Monster target = !isNull ? monsterList[idx] : null;
			Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, 1);

			peacock.SetTarget(target);
			peacock.SetSpawnInfo(Owner, this, isNull ? Util.GetRandomDir() : Vector2.zero);
	
			if (!isNull)
				monsterList.RemoveAt(idx);
		}
	}

	public override void Clear()
	{

	}
}