using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class LionSkill : SkillBase
{
	private LionRange targetChecker;

	public override void SetOwner(Creature owner)
	{
		base.SetOwner(owner);
		CreateRange();
	}

	private void CreateRange()
	{
		targetChecker = Managers.Resource.Instantiate("Area/InvisibleCircle", Owner.transform).GetOrAddComponent<LionRange>();
		targetChecker.transform.localPosition = Vector3.zero;
		targetChecker.SetData(SkillData.AttackRange + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.AttackRange),
			SkillData.ConditionRange + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.AttackRange));
	}

	public override void LevelUp(SkillData data)
	{
		base.LevelUp(data);
		targetChecker.SetData(SkillData.AttackRange + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.AttackRange),
			SkillData.ConditionRange + PassiveHelper.Instance.GetPassiveValue(Define.PassiveSkillStatusType.AttackRange));
	}

	public override void DoSkill()
	{
		targetChecker.CheckTarget();
		GetTargets().Forget();
	}

	private async UniTaskVoid GetTargets()
	{
		var lists = await targetChecker.GetTargets();

		var hitList = lists.hitList;
		var stunList = lists.stunList;
		
		foreach (var target in hitList)
		{
			target.OnDamaged(Owner, this);
		}
		
		foreach (var target in stunList)
		{
			Debug.Log($"{target.name} 스턴");
		}
	}

	public override void Clear()
	{

	}
}