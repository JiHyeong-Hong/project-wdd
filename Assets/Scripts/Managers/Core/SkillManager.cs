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
    public Dictionary<string, List<SkillBase>> allSkillDic { get; } = new Dictionary<string, List<SkillBase>>(); //string => 스킬 영문이름, List index => 스킬레벨
    public Dictionary<SkillType, List<SkillBase>> usingSkillDic { get; } = new Dictionary<SkillType, List<SkillBase>>();

    public List<SkillBase> sampleSkillList { get; }  = new List<SkillBase>();
    public List<string> canPickSkillList { get; } = new List<string>(); //만랩이 아닌 스킬들 이름 저장한 리스트 (레벨업 가능)
	
	public Dictionary<int,int> BreakthroghDic { get; } = new Dictionary<int, int>(); //인덱스를 키 벨류로 가지는 딕셔너리 (액티브, 돌파)


    private bool isInit;

	public IEnumerator CoInit()
	{
		yield return new WaitUntil(() => Managers.Object.Hero != null);

		foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
		{
			usingSkillDic.Add(skillType, new List<SkillBase>());
		}

		RegisterPassiveSkill();
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

		//TODO Eung 보유한 모든스킬 공격
		for (int i = 0; i < usingSkillDic[SkillType.Active].Count; i++)
		{
			usingSkillDic[SkillType.Active][i].UpdateCoolTime(deltaTime);
		}
		
		//if (Input.GetKeyDown(KeyCode.A))
		//{
		//	usingSkillDic[SkillType.Passive][0].DoSkill();
		//}

		// foreach (var skill in usingSkillDic[SkillType.Active])
		// {
		// 	skill?.UpdateCoolTime(deltaTime);
		// }
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
			string skillName = skillDic[skillID].Name + "Skill";

			Type t = assembly.GetType(skillName);

			if (t == null)
				continue;

			object obj = Activator.CreateInstance(t);

			if (obj == null)
			{
				Debug.Log("스킬 등록 error");
				return;
			}

			var skill = obj as SkillBase;
			try
			{
                skill.SetInfo(skillDic[skillID]);
            }
            catch (Exception ex)
			{
				Debug.Assert(false, $"{ex.Message} {skillID} 스킬 정보 설정 실패");
				throw;
			}

			var className = skillDic[skillID].Name;
			if (!allSkillDic.TryGetValue(className, out var list))
			{
				allSkillDic.Add(className,new List<SkillBase>());
				list = allSkillDic[className];
			}
			
			list.Add(skill);

			if (!beforeID.Equals(skill.SkillData.Name))
			{
				beforeID = skill.SkillData.Name;
				canPickSkillList.Add(beforeID);
			}
		}

		// 플레이어 고유 스킬
		var heroData = Managers.Object.Hero.CreatureData as HeroData;
		
		foreach (var skillID in heroData.SkillIdList)
		{
            ////임시
            //if (skillID / 1000 > 1)
            //	continue;

            // 현재 SkillData에 얼룩말 1개만 임의로 넣음. (1091)  @홍지형 임시.

            string className = Managers.Data.SkillDic[skillID].Name + "Skill";
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

			if (ownSkill.SkillData.skillType == Define.SkillType.Passive) ownSkill.DoSkill();


            usingSkillDic[ownSkill.SkillData.skillType].Add(ownSkill);
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
		bool isFullActive = usingSkillDic[SkillType.Active].Count == 6;
		bool isFullPassive = usingSkillDic[SkillType.Passive].Count == 6;
		
		int pick = 0;
		while (pick < 3)
		{
			if (tempList.Count == 0)
			{
				var randomName = canPickSkillList[Random.Range(0, canPickSkillList.Count)];
				sampleSkillList.Add(allSkillDic[randomName][0]);
			}
			else
			{
				var randomIndex = Random.Range(0, tempList.Count);
				sampleSkillList.Add(allSkillDic[tempList[randomIndex]][0]);
				tempList.RemoveAt(randomIndex);
			}
			
			pick++;
		}
		
		Managers.UI.ShowPopupUI<UI_LevelUp>().SetInfo(sampleSkillList);
	}

	private void AddSkill(SkillData skillData)
	{
		var list = usingSkillDic[skillData.skillType];

		if (list.Count == 6)
		{
			Debug.Log("스킬 추가할 자리 없음");
			return;
		}
		
		// 나중에 제대로 동작하는지 체크
		list.Add(allSkillDic[skillData.Name][skillData.Level-1]);

		int index = list.Count - 1;
		list[index].SetInfo(skillData);
		list[index].SetOwner(Managers.Object.Hero);
	}

	public void IncreaseSkillLevel(int index)
	{
		var skillData = sampleSkillList[index].SkillData;
		string className = skillData.Name;
		var skillType = skillData.skillType;

		// 데이터 테이블 상의해봐야함.
		int targetID = -1;

		bool hasSkill = false;

        foreach (var skill in usingSkillDic[skillType])
        {
            if (className.Equals(skill.SkillData.Name))
            {
                skill.LevelUp(allSkillDic[className][skill.SkillData.Level].SkillData);
				targetID = skill.SkillData.Index;
                hasSkill = true;
                //만렙이면 뽑을 수 있는 스킬목록에서 삭제
                if (skill.SkillData.Level == Define.MAX_SKILL_LEVEL)
                {
                    canPickSkillList.Remove(skill.SkillData.Name);
                }
                break;
            }
        }



        // 업그레이드 할 스킬이 없음 => 스킬 추가
        if (!hasSkill)
		{
			AddSkill(skillData);
		}

        targetID = (targetID == -1) ? skillData.Index : targetID;
		BreakthroghAdd(skillType, targetID);
    }

	private void RegisterPassiveSkill()
	{
		foreach (var item in usingSkillDic[SkillType.Passive])
		{
			PassiveHelper.Instance.SetPassive(item.SkillData);
        }
    }

	private void BreakthroghAdd(SkillType skillType, int skillIndex)
	{
        int [] activePassiveGetSkill = BreakthroghChecker(skillType, skillIndex);
        int activeSkillIndex = activePassiveGetSkill[0];
        int passiveSkillIndex = activePassiveGetSkill[1];
        int getSkillIndex = activePassiveGetSkill[2];

        if (usingSkillDic[SkillType.Active].Exists(x => x.SkillData.Index == activeSkillIndex) &&
            usingSkillDic[SkillType.Passive].Exists(x => x.SkillData.Index == passiveSkillIndex))
		{
			Debug.Log($"<color=green>돌파스킬 추가{Managers.Data.SkillDic[getSkillIndex].Name}</color>");
			AddSkill(Managers.Data.SkillDic[getSkillIndex]);
			BreakthroghDic.Add(activeSkillIndex, getSkillIndex);

        }
    }

	private int[] BreakthroghChecker(SkillType skillType, int skillIndex)
	{
        int activeSkillIndex = -1;
        int passiveSkillIndex = -1;
        int getSkillIndex = -1;

        foreach ((_, BreakthroghData data) in Managers.Data.BreakthroghDic)
        {
            if ((skillType == SkillType.Active && data.G_Skill_ID1 == skillIndex) ||
            (skillType == SkillType.Passive && data.G_Skill_ID2 == skillIndex))
            {
                activeSkillIndex = data.G_Skill_ID1;
                passiveSkillIndex = data.G_Skill_ID2;
                getSkillIndex = data.C_Skill_ID1;
                break;
            }
        }

		return new int[] { activeSkillIndex, passiveSkillIndex, getSkillIndex };
    }


    public bool Breakthrogh(int activeSkillIndex, int count)
	{
        if (BreakthroghDic.TryGetValue(activeSkillIndex, out int BTIndex))
		{
			foreach (var item in usingSkillDic[SkillType.Breakthrogh])
			{
                if(item.SkillData.Index == BTIndex)
				{
					if (item.SkillData.SkillTurn <= count && IsActivated(item.SkillData.CastPer))
					{
                        //스킬 발동
						item.UpdateCoolTime(0);
                        return true;
                    }
					return false;
                }
            }
		}
		return false;
	}

    public bool IsActivated(float ActivationProbability)
    {
        // 랜덤한 확률을 생성
        float randomValue = Random.Range(0f, 1f); // 0 ~ 1 사이의 랜덤한 값

        // 스킬이 발동되는지 여부를 판단
        return randomValue <= ActivationProbability;
    }


	/////////////
	public void Test(int getSkillIndex)
	{
		AddSkill(Managers.Data.SkillDic[getSkillIndex]);
        BreakthroghDic.Add(1015, getSkillIndex);
    }
}