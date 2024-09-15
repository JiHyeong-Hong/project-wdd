using Cysharp.Threading.Tasks;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class CompositeData
{
    public Tuple<int, bool> Active;
    public Tuple<int, bool> Passive;

    public void SetData(SkillType type, int index)
    {
        if(type == SkillType.Active)
        {
            Active = new Tuple<int, bool>(index, true);
        }
        else if(type == SkillType.Passive)
        {
            Passive = new Tuple<int, bool>(index, true);
        }
    }
}

public class BreakthroughHelper
{
    private static BreakthroughHelper _instance;
    public static BreakthroughHelper Instance
    {
        get
        {
            if (_instance == null)
                _instance = new BreakthroughHelper();
            return _instance;
        }
    }

    public Dictionary<int, CompositeData> compositeSkillTable = new Dictionary<int, CompositeData>();

    public Dictionary<int, int> nomalSkillToBTSkill = new Dictionary<int, int>(); // 일반 액티브스킬ID와 매칭되는 돌파스킬ID를 한 쌍으로 저장한다. <액티브스킬ID(만렙), 돌파스킬ID>
    public Dictionary<int, int> passiveSkillToBTSkill = new Dictionary<int, int>();
    public Dictionary<int, int> nomalSkillCastCount = new Dictionary<int, int>();


    private CancellationTokenSource cancellationTokenSource;
    public async void SetActiveObject(GameObject target, bool active, float activeTime)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        target.SetActive(active);
        try
        {
            // activeTime 이후에 gameObject를 비활성화하기 위해 일정 시간 동안 대기합니다.
            await UniTask.Delay((int)(activeTime * 1000), cancellationToken: cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // UniTask가 취소되었습니다. 이는 SetActiveObject 함수가 다시 호출되어 이전 작업이 취소되었음을 나타냅니다.
            return;
        }

        target.SetActive(false);
    }
    public void Init()
    {
        foreach (var item in Managers.Data.BreakthroughDic)
        {
            compositeSkillTable.Add(item.Value.C_Skill_ID1, new CompositeData 
            { 
                Active = new Tuple<int, bool>(item.Value.G_Skill_ID1, false), 
                Passive = new Tuple<int, bool>(item.Value.G_Skill_ID2, false)
            });
            
            nomalSkillToBTSkill.Add(item.Value.G_Skill_ID1, item.Value.C_Skill_ID1);
            //passiveSkillToBTSkill.Add(item.Value.G_Skill_ID2, item.Value.C_Skill_ID1);
        }
    }

    // 돌파스킬 조건을 만족하면 compositeSkillTable에 해당 스킬을 등록한다.
    public void GetBreakthroughBaseSkill(SkillType type, int index)
    {
        int compositeSkill = 0;

        foreach (var item in compositeSkillTable)
        {
            if (type == SkillType.Active && item.Value.Active.Item1 == index)
            {
                compositeSkill = item.Key;
                item.Value.SetData(type, index);
                break;
            }

            if (type == SkillType.Passive && item.Value.Passive.Item1 == index)
            {
                compositeSkill = item.Key;
                item.Value.SetData(type, index);
                break;
            }
        }
       
        // 돌파스킬 조건 만족하면 자동으로 돌파스킬 획득 (미사용)
        //if (compositeSkill != 0 && 
        //    compositeSkillTable[compositeSkill].Active.Item2 == true && compositeSkillTable[compositeSkill].Passive.Item2 == true)
        //{
        //    Managers.Skill.BreakthroughAdd(compositeSkill);
        //}
    }

    // 돌파스킬이 가능하도록 등록하고, 돌파스킬ID를 반환한다.
    public int GetBreakthroughSkillID(SkillType type, int index)
    {
        int compositeSkillID = 0;

        foreach (var item in compositeSkillTable)
        {
            if (type == SkillType.Active && item.Value.Active.Item1 == index)
            {
                compositeSkillID = item.Key;
                item.Value.SetData(type, index);
                break;
            }

            if (type == SkillType.Passive && item.Value.Passive.Item1 == index)
            {
                compositeSkillID = item.Key;
                item.Value.SetData(type, index);
                break;
            }
        }

        // 만약 돌파스킬 발동 조건 성립
        if (compositeSkillID != 0 &&
            compositeSkillTable[compositeSkillID].Active.Item2 == true && compositeSkillTable[compositeSkillID].Passive.Item2 == true)
        {
            return compositeSkillID;
        }
        else
        {
            return -1; // 돌파스킬 발동 조건 성립 X
        }

    }

    // 현재 액티브스킬이 돌파스킬이 가능한 상태인지 확인한다.
    public bool CheckBreakthrough(int index)
    {
        if (!nomalSkillToBTSkill.TryGetValue(index, out int breakthroughIndex))
            return false;

        CompositeData data = compositeSkillTable[breakthroughIndex];
        // if (data.Active.Item2 == true && data.Passive.Item2 == true)
        if (true) // DEBUG::
        {
            SkillBase breakthroughSkill = null;
            foreach (var item in Managers.Skill.usingSkillDic[SkillType.Breakthrough])
            {
                if (item.SkillData.SkillID == breakthroughIndex)
                {
                    breakthroughSkill = item;
                    break;
                }
            }
            if (breakthroughSkill == null) { return false; }

            // DEBUG::
            //nomalSkillCastCount[index] = 0;
            //breakthroughSkill.DoSkill();
            //return true;
            
            if (nomalSkillCastCount.ContainsKey(index) && nomalSkillCastCount[index] >= breakthroughSkill.SkillData.SkillTurn && IsActivated(breakthroughSkill.SkillData.CastPer * (1 + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.CastPer))))
            {
                nomalSkillCastCount[index] = 0;
                breakthroughSkill.DoSkill();
                return true;
            }
        }
        
        if (nomalSkillCastCount.ContainsKey(index))
            nomalSkillCastCount[index]++;
        else
            nomalSkillCastCount[index] = 1;

        return false;
    }

    public SkillBase FindBreakthroughSkill(int index)
    {
        foreach (var item in Managers.Data.BreakthroughDic)
        {
            if (item.Value.C_Skill_ID1 == index)
            {
                Managers.Skill.allSkillDic.TryGetValue(item.Value.Name, out List<SkillBase> findSkillList);
                return findSkillList[1];
            }
        }

        return null;
    }
    
    // 현재 스킬의 레벨1 ID값을 가져온다.
    public int GetFirstLvSkillID(int id)
    {
        // 상위 3자리 숫자를 추출
        int topThreeDigits = id / 10;

        // 상위 3자리 숫자에 마지막 자리를 1로 설정하여 레벨1 ID값을 가져온다.
        return topThreeDigits * 10 + 1;
    }
       
    public string FindPassiveName(string activeName)
    {
        Managers.Skill.allSkillDic.TryGetValue(activeName, out List<SkillBase> findSkillList);

        Data.BreakthroughData btData = Managers.Data.BreakthroughDic.Select(x => x.Value).FirstOrDefault(x => x.G_Skill_ID1 == findSkillList.Last().SkillData.SkillID);

        //Managers.Data.BreakthroughDic.TryGetValue(findSkillList.Last().SkillData.Index, out Data.BreakthroughData breakthroughData);

        SkillData passiveSkill = Managers.Data.SkillDic.Select(x => x.Value).FirstOrDefault(x => x.SkillID == btData.G_Skill_ID2);

        // 공백 제거

        passiveSkill.Name = passiveSkill.Name.Replace(" ", "");

        return passiveSkill.Name;
    }

    public bool IsActivated(float ActivationProbability)
    {
        // 랜덤한 확률을 생성
        float randomValue = UnityEngine.Random.Range(0f, 1f); // 0 ~ 1 사이의 랜덤한 값

        // 스킬이 발동되는지 여부를 판단
        return randomValue <= ActivationProbability;
    }

    // 인스턴스를 삭제하고 다시 생성하는 메서드
    public static void ResetInstance()
    {
        _instance = null;
    }
}
