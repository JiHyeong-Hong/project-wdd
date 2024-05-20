using Cysharp.Threading.Tasks;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillLevelUpPresenter
{
    private readonly SkillLevelUpModel model;
    private readonly IView view;

    public SkillLevelUpPresenter(IView view, SkillManager skillManager)
    {
        this.view = view;
        model = new SkillLevelUpModel();
    }

    public void SetSkillData(SkillData skillData)
    {
        model.skillData = skillData;
        view.UpdateUI(skillData);
    }

    public void HandleLevelUpSkill()
    {
        model.LevelUpSkill();
    }

    public void ToggleView(bool enable)
    {
        view.gameObject.SetActive(enable);
    }
}