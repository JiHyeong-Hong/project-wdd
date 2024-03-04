using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;
using SkillType = Define.SkillType;

public class SkillManager
{
	private Dictionary<int, SkillBase> allSkillDic = new Dictionary<int, SkillBase>();

	private SkillBase[] usingActiveSkillArr = new SkillBase[6];
	private SkillBase[] usingPassiveSkillArr = new SkillBase[6];

	private List<SkillBase> sampleSkillList = new List<SkillBase>();

	private bool isInit;

	public IEnumerator CoInit()
	{
		yield return new WaitUntil(() => Managers.Object.Hero != null);

		RegisterAllSkills();
		Managers.Game.OnLevelUp += CreateRandomSkills;

		isInit = true;
	}

	/// <summary>
	/// 플레이어가 장착한 스킬들 쿨타임 업데이트
	/// </summary>
	/// <param name="deltaTime"></param>
	public void UpdateSkillCoolTime(float deltaTime)
	{
		if (!isInit)
			return;

		foreach (var skill in usingActiveSkillArr)
		{
			skill?.UpdateCoolTime(deltaTime);
		}
	}

	/// <summary>
	/// 전체 스킬 캐싱
	/// </summary>
	private void RegisterAllSkills()
	{
		var skillDic = Managers.Data.SkillDic;
		var skillKeys = skillDic.Keys.ToList();

		Assembly assembly = Assembly.GetExecutingAssembly();

		// 전체 스킬
		foreach (var skillID in skillKeys)
		{
			string className = skillDic[skillID].ClassName + "Skill";

			Type t = assembly.GetType(className);

			if (t == null)
				continue;

			object obj = Activator.CreateInstance(t);

			if (obj == null)
			{
				Debug.Log("스킬 등록 error");
				return;
			}

			var skill = obj as SkillBase;

			skill.SetInfo(Managers.Data.SkillDic[skillID]);
			allSkillDic.Add(skillID, skill);
		}

		// 플레이어 고유 스킬
		var heroData = Managers.Object.Hero.CreatureData as HeroData;

		int activeIndex = 0;
		int passiveIndex = 0;

		foreach (var skillID in heroData.SkillIdList)
		{
			//임시
			if (skillID / 1000 > 1)
				continue;

			string className = Managers.Data.SkillDic[skillID].ClassName + "Skill";
			Type t = assembly.GetType(className);

			if (t == null)
				continue;

			object obj = Activator.CreateInstance(t);

			if (obj == null)
			{
				Debug.Log("Player 스킬 등록 error");
				return;
			}

			var ownSkill = (obj as SkillBase);
			ownSkill.SetInfo(Managers.Data.SkillDic[skillID]);
			ownSkill.SetOwner(Managers.Object.Hero);

			if (ownSkill.SkillData.skillType == SkillType.Active)
				usingActiveSkillArr[activeIndex++] = ownSkill;
			else if (ownSkill.SkillData.skillType == SkillType.Passive)
				usingPassiveSkillArr[passiveIndex++] = ownSkill;

		}
	}

	private void CreateRandomSkills()
	{
		sampleSkillList.Clear();
		List<int> spawnList = new List<int>();

		var allKeyList = allSkillDic.Keys.ToList();

		for (int i = 0; i < allKeyList.Count; ++i)
		{
			if (allSkillDic[allKeyList[i]].SkillData.Level >= 5)
				continue;
			spawnList.Add(allKeyList[i]);
		}

		for (int i = 0; i < 3; ++i)
		{
			int rand = Random.Range(0, spawnList.Count);

			sampleSkillList.Add(allSkillDic[spawnList[rand]]);
			spawnList.RemoveAt(rand);
		}

		Managers.UI.ShowPopupUI<UI_LevelUp>().SetInfo(sampleSkillList);
	}

	private void AddSkill(SkillData skillData)
	{
		int index = -1;
		var skillType = skillData.skillType;
		var arr = skillType == SkillType.Active ? usingActiveSkillArr : usingPassiveSkillArr;

		for (int i = 0; i < arr.Length; i++)
		{
			if (arr[i] == null)
			{
				index = i;
				break;
			}
		}

		if (index == -1)
		{
			Debug.Log("스킬 추가할 자리가 없음");
			return;
		}

		arr[index] = allSkillDic[skillData.DataId];
		arr[index].SetInfo(skillData);
		arr[index].SetOwner(Managers.Object.Hero);
	}
	
	public void IncreaseSkillLevel(int index)
	{
		var skillData = sampleSkillList[index].SkillData;
		string className = skillData.ClassName;
		var skillType  = skillData.skillType;
		
		bool hasSkill = false;
		
		if (skillType == SkillType.Active)
		{
			for (int i = 0; i < usingActiveSkillArr.Length; i++)
			{
				if (usingActiveSkillArr[i] == null)
					continue;
				
				if (className.Equals(usingActiveSkillArr[i].SkillData.ClassName))
				{
					int skillID = usingActiveSkillArr[i].SkillData.DataId;
					usingActiveSkillArr[i].LevelUp(allSkillDic[skillID + 1].SkillData);
					hasSkill = true;
				}
			}
		}

		// 업그레이드 할 스킬이 없음 => 스킬 추가
		if (!hasSkill)
		{
			AddSkill(skillData);
		}
	}
}