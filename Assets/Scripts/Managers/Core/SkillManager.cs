using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using SkillType = Define.SkillType;

public class SkillManager
{
    public Dictionary<string, List<SkillBase>> allSkillDic { get; } = new Dictionary<string, List<SkillBase>>(); //string => 스킬 영문이름, List index => 스킬레벨 레벨1~5 가 index 0~4로 들어가있다.
    public Dictionary<SkillType, List<SkillBase>> usingSkillDic { get; } = new Dictionary<SkillType, List<SkillBase>>();

    public List<SkillBase> sampleSkillList { get; } = new List<SkillBase>();
    public List<string> canPickSkillList { get; } = new List<string>(); //만랩이 아닌 스킬들 이름 저장한 리스트 (레벨업 가능)
    public List<string> canPickBTskillList { get; } = new List<string>(); // 돌파스킬 리스트

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
        BreakthroughHelper.Instance.Init();

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

        // 보유한 모든스킬 공격
        //for (int i = 0; i < usingSkillDic[SkillType.Active].Count; i++)
        //{
        //    usingSkillDic[SkillType.Active][i].UpdateCoolTime(deltaTime);
        //}

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     Hero hero = Managers.Object.Hero;
        //     hero.Exp += 1000;
        // }

        // 액티브 스킬
        foreach (var skill in usingSkillDic[SkillType.Active])
        {
            skill?.UpdateCoolTime(deltaTime);
        }
        
        // 돌파 스킬 CheckBreakThroughSkill 함수 호출로만 실행할것.
        //foreach (var skill in usingSkillDic[SkillType.Breakthrough])
        //{
        //    skill?.UpdateCoolTime(deltaTime);
        //}
    }

    /// <summary>
    /// 전체 스킬 캐싱
    /// </summary>
    public void RegisterAllSkills()
    {
        foreach (var item in allSkillDic)
        {
            item.Value.Clear();
        }
        foreach (var item in usingSkillDic)
        {
            item.Value.Clear();
        }
        canPickSkillList.Clear();
        canPickBTskillList.Clear();

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
                allSkillDic.Add(className, new List<SkillBase>());
                list = allSkillDic[className];
            }
            
            list.Add(skill);

            if (!beforeID.Equals(skill.SkillData.Name))
            {
                beforeID = skill.SkillData.Name;
                if(skill.SkillData.skillType == SkillType.Breakthrough) 
                {
                    canPickBTskillList.Add(beforeID); // 돌파스킬
                }
                else 
                {
                    canPickSkillList.Add(beforeID); // 액티브+패시브 스킬
                }
            }
        }

        // 플레이어 고유 스킬
        var heroData = Managers.Object.Hero.CreatureData as HeroData;

        foreach (var skillID in heroData.SkillIdList)
        {          
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

            BreakthroughHelper.Instance.GetBreakthroughBaseSkill(ownSkill.SkillData.skillType, ownSkill.SkillData.SkillID);
        }

    }

    /// <summary>
    /// 스킬 3개 랜덤 뽑기
    /// </summary>
    private void CreateRandomSkills()
    {
        sampleSkillList.Clear();
        if (canPickSkillList.Count == 0) { Debug.Log("레벨업 가능한 스킬이 더이상 없습니다.");  return; } // 더 이상 레벨업시킬 스킬이 없을 떄, 예외처리
        List<string> tempList = new List<string>();
        tempList.AddRange(canPickSkillList);

        // 칸이 가득 찼는지 체크 (나중에 추가)
        bool isFullActive = usingSkillDic[SkillType.Active].Count == 6;
        bool isFullPassive = usingSkillDic[SkillType.Passive].Count == 6;

        // 240811 @홍지형
        // 1) usingSkillDic에서 현재 sampleSkillList에 추가하려는 스킬이 있는지 검사한다.
        // 2) 현재 추가하려는 스킬이 usingSkillDic에 있으면, 그 존재하는 스킬의 레벨+1 된 스킬을 sampleSkillList에 넣는다.

        int pick = 0;
        while (pick < 3)
        {
            // 시작
            var randomIndex = Random.Range(0, tempList.Count);
            if (tempList.Count == 0) // 남은 스킬리스트가 2개 이하일 때
            {
                var randomName = canPickSkillList[Random.Range(0, canPickSkillList.Count)];
                sampleSkillList.Add(allSkillDic[randomName][0]);
                RemoveSameSkillList(allSkillDic[randomName][0]);
                pick++;
                continue;
            }
            SkillBase selectedSkillBase = allSkillDic[tempList[randomIndex]][0];

            // 1) usingSkillDic 에서 현재 추가하려는 스킬이 있는지 검사한다.
            SkillData selectedSkillData = selectedSkillBase.SkillData;
            foreach (var skill in Managers.Skill.usingSkillDic[selectedSkillData.skillType])
            {
                // 2) 현재 추가하려는 스킬이 usingSkillDic에 존재하면
                if (skill.SkillData.Name == selectedSkillData.Name)
                {
                    // 3) 존재하는 스킬의 레벨+1 된 스킬을 sampleSkillList에 넣는다.
                    int currentLevel = skill.SkillData.Level - 1; // level값을 인덱스 값으로 변환
                    selectedSkillBase = allSkillDic[tempList[randomIndex]][currentLevel + 1];
                }
            }
            // 끝

            // 스킬 선택 리스트에 새 스킬 추가
            sampleSkillList.Add(selectedSkillBase);
            RemoveSameSkillList(selectedSkillBase);

            tempList.RemoveAt(randomIndex);

            pick++;
        }

        // DEBUG::
        foreach (var skill in sampleSkillList)
        {
            // Debug.Log("sampleSkillList:" + skill.SkillData.Name);
        }

        // UIManagerNew.Instance.ShowWindow<WindowBase>(Define.UIWindowType.SkillLevelUpWindow); // 캐싱
        UIManagerNew.Instance.ShowWindow<SkillLevelUpWindow>(true); // 논캐싱
    }


    // 중복 스킬 제거
    private void RemoveSameSkillList(SkillBase selectedSkillBase)
    {
        bool hasDuplicates = false;
        // 중복 검사
        HashSet<string> set = new HashSet<string>();
        foreach (var item in sampleSkillList)
        {
            if (!set.Add(item.SkillData.Name))
            {
                hasDuplicates =  true; 
            }
        }        
        if (hasDuplicates == true) { sampleSkillList.Remove(selectedSkillBase); }
    }

    // 백업 240811 @홍지형
    ///// <summary>
    ///// 스킬 3개 랜덤 뽑기
    ///// </summary>
    //private void CreateRandomSkills()
    //{
    //    sampleSkillList.Clear();

    //    List<string> tempList = new List<string>();
    //    tempList.AddRange(canPickSkillList);

    //    // 칸이 가득 찼는지 체크 (나중에 추가)
    //    bool isFullActive = usingSkillDic[SkillType.Active].Count == 6;
    //    bool isFullPassive = usingSkillDic[SkillType.Passive].Count == 6;

    //    // TODO:
    //    // 1) usingSkillDic 에서 현재 추가하려는 스킬이 있는지 검사한다.
    //    // 2) 현재 추가하려는 스킬이 usingSkillDic에 있으면, 그 존재하는 스킬의 레벨+1 된 스킬을 sampleSkillList에 넣는다.

    //    int pick = 0;
    //    while (pick < 3)
    //    {
    //        if (tempList.Count == 0)
    //        {
    //            var randomName = canPickSkillList[Random.Range(0, canPickSkillList.Count)];
    //            sampleSkillList.Add(allSkillDic[randomName][0]); // TODO: 240811
    //        }
    //        else
    //        {
    //            var randomIndex = Random.Range(0, tempList.Count);
    //            sampleSkillList.Add(allSkillDic[tempList[randomIndex]][0]); // TODO: 240811
    //            tempList.RemoveAt(randomIndex);
    //        }
    //        pick++;
    //    }

    //    // DEBUG::
    //    foreach (var skill in sampleSkillList)
    //    {
    //        // Debug.Log("sampleSkillList:" + skill.SkillData.Name);
    //    }

    //    //Managers.UI.ShowWindowUI<MonoBehaviour>("SkillWindow");
    //    //Managers.UI.ShowPopupUI<UI_LevelUp>().SetInfo(sampleSkillList);
    //    //UIManagerNew.Instance.ShowWindow<SkillLevelUpWindow>(Define.UIWindowType.SkillLevelUpWindow);

    //    UIManagerNew.Instance.ShowWindow<WindowBase>(Define.UIWindowType.SkillLevelUpWindow); // 캐싱
    //    // UIManagerNew.Instance.ShowWindow<SkillLevelUpWindow>(); // 논캐싱

    //    // Managers.UI.ShowWindowUI<SkillLevelUpWindow>(Define.UIWindowType.SkillLevelUpWindow).Show();
    //    //Managers.UI.ShowWindowUI<SkillLevelUpWindow>("SkillLevelUpWindow");
    //}

    private void AddSkill(SkillData skillData)
    {
        var list = usingSkillDic[skillData.skillType];

        if (list.Count == 6)
        {
            Debug.Log("스킬 추가할 자리 없음");
            return;
        }

        // 나중에 제대로 동작하는지 체크
        list.Add(allSkillDic[skillData.Name][skillData.Level - 1]);

        int index = list.Count - 1;
        list[index].SetInfo(skillData);
        list[index].SetOwner(Managers.Object.Hero);

        BreakthroughHelper.Instance.GetBreakthroughBaseSkill(skillData.skillType, skillData.SkillID);
    }

    public void AddActiveSkill(string skillName)
    {
        foreach (SkillBase item in usingSkillDic[SkillType.Active])
        {
            if (item.SkillData.Name.Equals(skillName))
            {
                // 같은 스킬이 이미 있음 -> 스킬 레벨업
                item.LevelUp(allSkillDic[skillName][item.SkillData.Level].SkillData);
                return;
            }
        }

        AddSkill(allSkillDic[skillName][0].SkillData);
    }

    public void BreakthroughAdd(int breakthroughIndex)
    {
        var list = usingSkillDic[SkillType.Breakthrough];
        SkillData skillData = allSkillDic[Managers.Data.BreakthroughDic[breakthroughIndex].Name][0].SkillData;

        list.Add(allSkillDic[Managers.Data.BreakthroughDic[breakthroughIndex].Name][0]);

        int index = list.Count - 1;
        list[index].SetInfo(skillData);
        list[index].SetOwner(Managers.Object.Hero);
    }


    public void IncreaseSkillLevel(SkillData skillData)
    {
        if (skillData.Level > 1)
        {
            if (Managers.Skill.usingSkillDic[skillData.skillType].Remove(Managers.Skill.allSkillDic[skillData.Name][skillData.Level - 2])) // 이전레벨의 스킬 삭제
            {
                // UnityEngine.Debug.Log($"삭제성공"); // TODO: 240811 usingSkillDic가 remove가 제대로 되고 있지 않는 문제 디버깅 필요.
            }
            else
            {
                // UnityEngine.Debug.Log($"삭제실패..");
            }
        }
        Managers.Skill.usingSkillDic[skillData.skillType].Add(Managers.Skill.allSkillDic[skillData.Name][skillData.Level - 1]); // 현재레벨의 스킬 추가
        Managers.Skill.usingSkillDic[skillData.skillType][Managers.Skill.usingSkillDic[skillData.skillType].Count - 1].SetInfo(skillData);
        Managers.Skill.usingSkillDic[skillData.skillType][Managers.Skill.usingSkillDic[skillData.skillType].Count - 1].SetOwner(Managers.Object.Hero);

        BreakthroughHelper.Instance.GetBreakthroughBaseSkill(skillData.skillType, skillData.SkillID);
    }

    // 현재 미사용, 레거시 코드
    public void IncreaseSkillLevel(int index)
    {
        var skillData = sampleSkillList[index].SkillData;
        string className = skillData.Name;
        var skillType = skillData.skillType;


        bool hasSkill = false;

        foreach (var skill in usingSkillDic[skillType])
        {
            if (className.Equals(skill.SkillData.Name))
            {
                skill.LevelUp(allSkillDic[className][skill.SkillData.Level].SkillData);
                hasSkill = true;

                BreakthroughHelper.Instance.GetBreakthroughBaseSkill(skillData.skillType, skillData.SkillID);

                break;
            }
        }

        // 업그레이드 할 스킬이 없음 => 스킬 추가
        if (!hasSkill)
        {
            AddSkill(skillData);
        }

    }

    private void RegisterPassiveSkill()
    {
        foreach (var item in usingSkillDic[SkillType.Passive])
        {
            PassiveHelper.Instance.SetPassive(item.SkillData);
        }
    }
}