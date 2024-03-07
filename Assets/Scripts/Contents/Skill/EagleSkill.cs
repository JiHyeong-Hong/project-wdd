using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleSkill : SkillBase
{
	private List<Monster> targetList = new List<Monster>();
	
	public override void DoSkill()
	{
		targetList.Clear();
		var monsterList = Managers.Object.Monsters;

		foreach (var monster in monsterList)
		{
			if(monster.Hp<= 0)
				continue;
			
			if (Util.CheckTargetInScreen(monster.transform.position))
			{
				targetList.Add(monster);
			}
		}
		foreach (var target in targetList)
		{
			//보스,중간보스 판별할 방법부재로 일단 랜덤
		}
	}
	public override void Clear()
	{
		
	}


}