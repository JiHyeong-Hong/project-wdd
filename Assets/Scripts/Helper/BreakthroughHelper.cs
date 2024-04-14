using System;
using System.Collections.Generic;
using System.Linq;
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

    public Dictionary<int,int> nomalSkillToBTSkill = new Dictionary<int, int>();
    public Dictionary<int, int> passiveSkillToBTSkill = new Dictionary<int, int>();
    public Dictionary<int, int> nomalSkillCastCount = new Dictionary<int, int>();

    public void Init()
    {
        foreach (var item in Managers.Data.BreakthroghDic)
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
        if (!nomalSkillToBTSkill.TryGetValue(index, out int breakthroghIndex))
            return false;

        CompositeData data = compositeSkillTable[breakthroghIndex];
        if (data.Active.Item2 == true && data.Passive.Item2 == true)
        {
            SkillBase breakthroghSkill = null;
            foreach (var item in Managers.Skill.usingSkillDic[SkillType.Breakthrough])
            {
                if (item.SkillData.Index == breakthroghIndex)
                {
                    breakthroghSkill = item;
                    break;
                }
            }

            if (nomalSkillCastCount.ContainsKey(index) && nomalSkillCastCount[index] >= breakthroghSkill.SkillData.SkillTurn && IsActivated(breakthroghSkill.SkillData.CastPer))
            {
                nomalSkillCastCount[index] = 0;
                breakthroghSkill.DoSkill();
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
        foreach (var item in Managers.Data.BreakthroghDic)
        {
            if (item.Value.C_Skill_ID1 == index)
            {
                Managers.Skill.allSkillDic.TryGetValue(item.Value.Name, out List<SkillBase> findSkillList);
                return findSkillList[1];
            }
        }

        return null;
    }

    public bool IsActivated(float ActivationProbability)
    {
        // 랜덤한 확률을 생성
        float randomValue = UnityEngine.Random.Range(0f, 1f); // 0 ~ 1 사이의 랜덤한 값

        // 스킬이 발동되는지 여부를 판단
        return randomValue <= ActivationProbability;
    }

}
