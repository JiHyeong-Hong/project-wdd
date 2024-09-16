using Data;
using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor.Experimental.GraphView;
public class SkillLevelUpModel
{
    public SkillData skillData;

    internal void CheckSkillData()
    {
        // 선택한 스킬이 돌파면
        if (skillData.skillType == Define.SkillType.Breakthrough)
        {
            Managers.Skill.BreakthroughAdd(skillData.SkillID); // 해당 돌파스킬ID로 추가한다.
            return;
        }

        // 돌파가 아니면 렙업
        foreach (var skill in Managers.Skill.usingSkillDic[skillData.skillType])
        {           
            if (skill.SkillData.Name == skillData.Name)
            {                
                skill.LevelUp(Managers.Skill.allSkillDic[skillData.Name][skill.SkillData.Level].SkillData); // +1렙업
                // return;
            }
        }
        Managers.Skill.IncreaseSkillLevel(skillData);
    }

    internal List<SkillBase> LoadSkillDatas()
    {
        return Managers.Skill.sampleSkillList;
    }

    internal void LevelUpSkill()
    {
        CheckSkillData();
    }
}