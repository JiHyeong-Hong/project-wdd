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
        if (BreakthroughHelper.Instance.CheckBreakthrough(SkillData.Index))
            return;

        monsterList.Clear();
		GetTargets();
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
		
		for (int i = 0; i < SkillData.ProjectileNum; i++)
		{
			bool isNull = monsterList.Count == 0;

			int idx = Random.Range(0, monsterList.Count);
			Monster target = !isNull ? monsterList[idx] : null;
			Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, SkillData.ProjectileNum);

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