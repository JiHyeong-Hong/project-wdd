using Cysharp.Threading.Tasks;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Define;
using static UnityEditor.Progress;


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

    public Dictionary<int, int> nomalSkillToBTSkill = new Dictionary<int, int>();
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

        if (compositeSkill != 0 && 
            compositeSkillTable[compositeSkill].Active.Item2 == true && compositeSkillTable[compositeSkill].Passive.Item2 == true)
        {
            Managers.Skill.BreakthroughAdd(compositeSkill);
        }

    }

    public bool CheckBreakthrough(int index)
    {
        if (!nomalSkillToBTSkill.TryGetValue(index, out int breakthroughIndex))
            return false;

        CompositeData data = compositeSkillTable[breakthroughIndex];
        if (data.Active.Item2 == true && data.Passive.Item2 == true)
        {
            SkillBase breakthroughSkill = null;
            foreach (var item in Managers.Skill.usingSkillDic[SkillType.Breakthrough])
            {
                if (item.SkillData.Index == breakthroughIndex)
                {
                    breakthroughSkill = item;
                    break;
                }
            }

            if (nomalSkillCastCount.ContainsKey(index) && nomalSkillCastCount[index] >= breakthroughSkill.SkillData.SkillTurn && IsActivated(breakthroughSkill.SkillData.CastPer))
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

    public string FindPassiveName(string activeName)
    {
        Managers.Skill.allSkillDic.TryGetValue(activeName, out List<SkillBase> findSkillList);

        Data.BreakthroughData btData = Managers.Data.BreakthroughDic.Select(x => x.Value).FirstOrDefault(x => x.G_Skill_ID1 == findSkillList.Last().SkillData.Index);

        //Managers.Data.BreakthroughDic.TryGetValue(findSkillList.Last().SkillData.Index, out Data.BreakthroughData breakthroughData);

        SkillData passiveSkill = Managers.Data.SkillDic.Select(x => x.Value).FirstOrDefault(x => x.Index == btData.G_Skill_ID2);

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

}
