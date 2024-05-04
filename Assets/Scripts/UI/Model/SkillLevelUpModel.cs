using Cysharp.Threading.Tasks;
using Data;
using System.Collections.Generic;
public class SkillLevelUpModel
{
    public SkillData skillData;

    //#region Test

    //private SkillManager skillManager;
    //public SkillManager SkillManager 
    //{ 
    //    get
    //    {
    //        if (skillManager == null)
    //        {
    //            Test();
    //        }
    //        return skillManager;
    //    }
    //}

    //public void Test()
    //{
    //    if (TestManager.Instance != null)
    //    {
    //        skillManager = TestManager.Instance.SkillManager;
    //        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
    //        {
    //            skillManager.usingSkillDic.Add(skillType, new List<SkillBase>());
    //        }
    //    }
    //    else
    //    {
    //        skillManager = Managers.Skill;
    //    }
    //}

    //#endregion


    internal async UniTask CheckSkillData()
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


    private void LevelUpSkill(SkillBase skillData)
    {
        skillData.LevelUp(skillData.SkillData);
    }

    internal List<SkillBase> LoadSkillDatas()
    {
        return Managers.Skill.sampleSkillList;
    }

    internal async UniTask LevelUpSkillAsync()
    {
        await CheckSkillData();
    }
}