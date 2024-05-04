using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;
using static UnityEditor.Progress;
using Data;

public class SkillLevelUpWindow : UIWindow
{
    [SerializeField]
    private Transform thumbnailGrid;
    [SerializeField]
    private GameObject thumbnailPrefab;
    [SerializeField]

    private List<IView> views = new List<IView>();
    private List<SkillLevelUpPresenter> presenters = new List<SkillLevelUpPresenter>();

    public bool isTestScene;

    private void Awake()
    {
        thumbnailGrid = Util.FindChild<Transform>(gameObject, "ThumbnailGrid", true);

        for(int i = 0; i < 3; ++i)
        {
            GameObject obj = Instantiate(thumbnailPrefab, thumbnailGrid);
            SkillLevelUpView view = obj.GetComponent<SkillLevelUpView>();
            views.Add(view);

            SkillLevelUpPresenter presenter = new SkillLevelUpPresenter(view, Managers.Skill);
            view.SetPresenter(presenter);
            presenters.Add(presenter);
        }
    }


    private void OnEnable()
    {
        Managers.Game.IsGamePaused = true;
        Managers.UI.Joystick.gameObject.SetActive(false);
        SetLevelUpUI();
        ViewsOnOff(true);
    }

    private void OnDisable()
    {
        Managers.Game.IsGamePaused = false;
        Managers.UI.Joystick.gameObject.SetActive(true);
        ViewsOnOff(false);
    }

    private void SetLevelUpUI()
    {
        List<string> list = Util.SelectUniqueElements(Managers.Skill.canPickSkillList, 3);

        Assert.IsTrue(list.Count != 0, "Skill List is Empty");


        for(int i = 0; i < views.Count; ++i)
        {
            bool isSkillFound = false;
            SkillData skillData = Managers.Skill.allSkillDic[list[i]][0].SkillData;
            List<SkillBase> skillsWithSameType = Managers.Skill.usingSkillDic[skillData.skillType];

            foreach (var item in skillsWithSameType)
            {
                if (item.SkillData.Name == list[i])
                {
                    UpdateViewAndPresenter(views[i], presenters[i], item.SkillData);
                    isSkillFound = true;
                    break;
                }
            }

            if (!isSkillFound)
            {
                UpdateViewAndPresenter(views[i], presenters[i], skillData);
            }
        }
    }
    void UpdateViewAndPresenter(IView view, SkillLevelUpPresenter presenter, SkillData skillData)
    {
        view.UpdateUI(Managers.Skill.allSkillDic[skillData.Name][skillData.Level]);
        presenter.SetSkillData(skillData);
    }

    private void ViewsOnOff(bool enable)
    {
        foreach (var item in views)
        {
            item.gameObject.SetActive(enable);
        }
    }

}
