using Data;
using System.Collections.Generic;
using System.Diagnostics;
public class SkillLevelUpModel
{
    public SkillData skillData;

    internal void CheckSkillData()
    {        
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