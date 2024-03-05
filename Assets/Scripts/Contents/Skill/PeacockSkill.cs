using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeacockSkill : SkillBase
{
	private PeacockRange targetChecker;


	/// <summary>
	/// 공작 범위 체크용 오브젝트 생성
	/// </summary>
	private void CreateRange()
	{
		GameObject obj = new GameObject("PeacockRange", typeof(PeacockRange));
		obj.transform.SetParent(Owner.transform);
		obj.transform.localPosition = Vector3.zero;

		targetChecker = obj.GetComponent<PeacockRange>();
		targetChecker.Init();
		targetChecker.SetData( /*SkillData.AttackRange*/3, SkillData.Projectile);
	}

	public override void DoSkill()
	{
		if (targetChecker == null)
		{
			CreateRange();
			return;
		}
		targetChecker.CheckTarget();
		GetTargets();
	}

	private async void GetTargets()
	{
		var targets = await targetChecker.GetTargets();
		for (int i = 0; i < targets.Length; i++)
		{
			
		}
	}

	public override void Clear()
	{

	}
}