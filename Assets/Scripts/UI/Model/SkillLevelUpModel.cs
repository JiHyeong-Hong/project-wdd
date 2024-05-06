using Data;
using System.Collections.Generic;
public class SkillLevelUpModel
{
    public SkillData skillData;

    internal void CheckSkillData()
    {
        foreach (var skill in Managers.Skill.usingSkillDic[skillData.skillType])
        {
            if (skill.SkillData.Name == skillData.Name)
            {
                skill.LevelUp(Managers.Skill.allSkillDic[skillData.Name][skill.SkillData.Level].SkillData);
                return;
            }
        }
        AddSkillData(skillData);
    }

    private void AddSkillData(SkillData skillData)
    {
        Managers.Skill.usingSkillDic[skillData.skillType].Add(Managers.Skill.allSkillDic[skillData.Name][skillData.Level - 1]);
        Managers.Skill.usingSkillDic[skillData.skillType][Managers.Skill.usingSkillDic[skillData.skillType].Count - 1].SetInfo(skillData);
        Managers.Skill.usingSkillDic[skillData.skillType][Managers.Skill.usingSkillDic[skillData.skillType].Count - 1].SetOwner(Managers.Object.Hero);

        //BreakthroughHelper.Instance.GetBreakthroughBaseSkill(skillData.skillType, skillData.Index);

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