using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PeacockSkill : SkillBase
{
	private PeacockRange targetChecker;

	public override void SetOwner(Creature owner)
	{
		base.SetOwner(owner);
		CreateRange();
	}

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
		targetChecker.CheckTarget();
		GetTargets().Forget();
	}

	private async UniTaskVoid GetTargets()
	{
		var targets = await targetChecker.GetTargets();
		for (int i = 0; i < targets.Length; i++)
		{
			Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, 1);

			peacock.SetTarget(targets[i]);
			peacock.SetSpawnInfo(Owner, this, targets[i] == null ? Util.GetRandomDir() : Vector2.zero);
		}
	}

	public override void Clear()
	{

	}
}