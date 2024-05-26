using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SkillLevelUpView : MonoBehaviour, IView
{
    private SkillLevelUpPresenter presenter;

    [SerializeField]
    private Image skillImage;
    [SerializeField]
    private TMP_Text skillName;
    [SerializeField]
    private TMP_Text skillLevel;
    [SerializeField]
    private TMP_Text skillDescription;
    
    public Button levelUpButton;

    public void SetPresenter(SkillLevelUpPresenter presenter)
    {
        this.presenter = presenter;
    }

    public void UpdateUI(object data)
    {
        if (data is SkillBase)
        {
            string name = Managers.Localization.GetLocalizedText(((SkillBase)data).SkillData.Name);
            //string description = Managers.Localization.GetLocalizedText(((SkillBase)data).SkillData.Description);
            skillName.text = name;
            skillLevel.text = $"Level {((SkillBase)data).SkillData.Level}";
            //skillDescription.text = description;
        }
    }

    public void LevelUpSkillClick()
    {
        presenter.HandleLevelUpSkill();

        Managers.UI.windowDic[Define.UIWindowType.SkillLevelUp].Hide();
    }

}