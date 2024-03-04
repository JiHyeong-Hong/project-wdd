using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_LevelUp : UI_Popup
{
    enum GameObjects
    {
        First,
        Second,
        Third
    }

    enum Texts
    {
        FirstName,
        SecondName,
        ThirdName,
        FirstLevel,
        SecondLevel,
        ThirdLevel
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        
        
        GetObject((int)GameObjects.First).BindEvent(OnClickFirst);
        GetObject((int)GameObjects.Second).BindEvent(OnClickSecond);
        GetObject((int)GameObjects.Third).BindEvent(OnClickThird);

        return true;
    }

    public void SetInfo(List<SkillBase> skills)
    {
        Managers.Game.IsGamePaused = true;
        
        GetText((int)Texts.FirstName).text = skills[0].SkillData.Name;
        GetText((int)Texts.SecondName).text = skills[1].SkillData.Name;
        GetText((int)Texts.ThirdName).text = skills[2].SkillData.Name;

        GetText((int)Texts.FirstLevel).text = (skills[0].SkillData.Level == 0) ? "New!" : "Lv. " + (skills[0].SkillData.Level + 1);
        GetText((int)Texts.SecondLevel).text = (skills[1].SkillData.Level == 0) ? "New!" : "Lv. " + (skills[1].SkillData.Level + 1);
        GetText((int)Texts.ThirdLevel).text = (skills[2].SkillData.Level == 0) ? "New!" : "Lv. " + (skills[2].SkillData.Level + 1);
    }

    public void OnClickFirst(PointerEventData evt)
    {
        Managers.Skill.IncreaseSkillLevel(0);
        Managers.Game.IsGamePaused = false;
        Managers.UI.ClosePopupUI(this);
    }

    public void OnClickSecond(PointerEventData evt)
    {
        Managers.Skill.IncreaseSkillLevel(1);
        Managers.Game.IsGamePaused = false;
        Managers.UI.ClosePopupUI(this);
    }

    public void OnClickThird(PointerEventData evt)
    {
        Managers.Skill.IncreaseSkillLevel(2);
        Managers.Game.IsGamePaused = false;
        Managers.UI.ClosePopupUI(this);
    }
}
