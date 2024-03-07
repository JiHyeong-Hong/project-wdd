using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using SkillType = Define.SkillType;

public class SkillManager
{
	private Dictionary<string, SkillBase> allSkillDic = new Dictionary<string, SkillBase>(); //string => 스킬 영문이름

	private List<ActiveSkill> usingActiveSkillList = new List<ActiveSkill>();
	private List<PassiveSkill> usingPassiveSkillList = new List<PassiveSkill>();

	private List<SkillData> sampleSkillList = new List<SkillData>();
	private List<string> canPickSkillList = new List<string>(); //만랩이 아닌 스킬들 이름 저장한 리스트 (레벨업 가능)

	private bool isInit;

	public async UniTaskVoid Init()
	{
		await UniTask.WaitUntil(() => Managers.Object.Hero != null);
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

		if (Input.GetKeyDown(KeyCode.A))
		{
			usingActiveSkillList[0].DoSkill();
		}

		foreach (var skill in usingActiveSkillList)
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

		string beforeID = string.Empty;
		// 전체 스킬
		foreach (var skillID in skillKeys)
		{
			if (beforeID.Equals(skillDic[skillID].ClassName))
				continue;

			beforeID = skillDic[skillID].ClassName;
			string skillName = skillDic[skillID].ClassName + "Skill";

			canPickSkillList.Add(beforeID);
			Type t = assembly.GetType(skillName);

			if (t == null)
			{
				canPickSkillList.Remove(beforeID);
				continue;
			}

			object obj = Activator.CreateInstance(t);

			if (obj == null)
			{
				Debug.LogError("스킬 등록 error");
				return;
			}

			var className = skillDic[skillID].ClassName;

			var skill = obj as SkillBase;
			skill.SetData(skillDic[skillID]);
			allSkillDic.Add(className, skill);
		}

		// 플레이어 고유 스킬
		var hero = Managers.Object.Hero;
		var heroData = hero.CreatureData as HeroData;

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
			ownSkill.SetData(Managers.Data.SkillDic[skillID]);
			ownSkill.Init(hero);

			if (ownSkill.SkillData.Level >= Define.MAX_SKILL_LEVEL)
				canPickSkillList.Remove(ownSkill.SkillData.ClassName);

			switch (ownSkill.SkillData.skillType)
			{
				case SkillType.Active:
					usingActiveSkillList.Add(ownSkill as ActiveSkill);
					break;
				case SkillType.Passive:
					usingPassiveSkillList.Add(ownSkill as PassiveSkill);
					break;
			}
		}
	}

	/// <summary>
	/// 스킬 3개 랜덤 뽑기
	/// </summary>
	private void CreateRandomSkills()
	{
		sampleSkillList.Clear();

		List<string> tempList = new List<string>();
		tempList.AddRange(canPickSkillList);

		// 칸이 가득 찼는지 체크 (나중에 추가)
		// bool isFullActive = usingSkillDic[SkillType.Active].Count == 6;
		// bool isFullPassive = usingSkillDic[SkillType.Passive].Count == 6;

		int pick = 0;
		while (pick < 3)
		{
			//이쪽 나중에 작업 필요
			// 현재는 canPickSkillList가 비었을때 고려 x
			if (tempList.Count == 0)
			{
				var randomName = canPickSkillList[Random.Range(0, canPickSkillList.Count)];
				sampleSkillList.Add(allSkillDic[randomName].SkillData);
			}
			else
			{
				var randomIndex = Random.Range(0, tempList.Count);

				var skillData = allSkillDic[tempList[randomIndex]].SkillData;
				var skillType = skillData.skillType;

				SkillData data = skillType switch
				{
					SkillType.Active => usingActiveSkillList.FirstOrDefault(x => x.SkillData.ClassName.Equals(skillData.ClassName))?.SkillData,
					SkillType.Passive => usingPassiveSkillList.FirstOrDefault(x => x.SkillData.ClassName.Equals(skillData.ClassName))?.SkillData,
				};

				// data가 null이면 없는거 (새로운 스킬)
				if (data == null)
					data = allSkillDic[tempList[randomIndex]].SkillData;

				sampleSkillList.Add(data);
				tempList.RemoveAt(randomIndex);
			}

			pick++;
		}

		Managers.UI.ShowPopupUI<UI_LevelUp>().SetInfo(sampleSkillList);
	}

	private void AddSkill(SkillData skillData)
	{
		SkillType skillType = skillData.skillType;

		if (skillType == SkillType.Active)
		{
			if (usingActiveSkillList.Count == 6)
			{
				Debug.Log("스킬 추가할 자리 없음");
				return;
			}

			var skill = allSkillDic[skillData.ClassName] as ActiveSkill;

			skill.Init(Managers.Object.Hero, Managers.Data.SkillDic[skill.SkillData.DataId + skill.SkillData.Level + 1]);
			usingActiveSkillList.Add(skill);
		}
		else if (skillType == SkillType.Passive)
		{
			if (usingPassiveSkillList.Count == 6)
			{
				Debug.Log("스킬 추가할 자리 없음");
				return;
			}
			var skill = allSkillDic[skillData.ClassName] as PassiveSkill;
			skill.Init(Managers.Object.Hero, Managers.Data.SkillDic[skill.SkillData.DataId + skill.SkillData.Level + 1]);
			usingPassiveSkillList.Add(allSkillDic[skillData.ClassName] as PassiveSkill);
		}
	}
	private void LevelUpInUsingSkillList(List<SkillBase> skillList, string className, ref bool isEmpty)
	{
		foreach (var skill in skillList)
		{
			if (className.Equals(skill.SkillData.ClassName))
			{
				skill.LevelUp(Managers.Data.SkillDic[skill.SkillData.DataId + skill.SkillData.Level + 1]);
				CheckMaxLevel(skill.SkillData);
				isEmpty = false;
				break;
			}
		}
	}
	public void IncreaseSkillLevel(int index)
	{
		var skillData = sampleSkillList[index];
		string className = skillData.ClassName;
		var skillType = skillData.skillType;
		bool isEmpty = true;

		if (skillType == SkillType.Active)
		{
			LevelUpInUsingSkillList(usingActiveSkillList.Cast<SkillBase>().ToList(), className, ref isEmpty);
		}
		else if (skillType == SkillType.Passive)
		{
			LevelUpInUsingSkillList(usingPassiveSkillList.Cast<SkillBase>().ToList(), className, ref isEmpty);
		}
		
		// 업그레이드 할 스킬이 없음 => 스킬 추가
		if (isEmpty)
		{
			AddSkill(skillData);
		}
	}

	private void CheckMaxLevel(SkillData data)
	{
		if (data.Level == Define.MAX_SKILL_LEVEL)
			canPickSkillList.Remove(data.ClassName);
	}
}