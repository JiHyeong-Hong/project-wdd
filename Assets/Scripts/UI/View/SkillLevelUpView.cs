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
            skillName.text = ((SkillBase)data).SkillData.Name;
            skillLevel.text = $"Level {((SkillBase)data).SkillData.Level}";
            skillDescription.text = ((SkillBase)data).SkillData.Description;
        }
    }

    public void LevelUpSkillClick()
    {
        presenter.HandleLevelUpSkill();

        Managers.UI.windowDic[Define.UIWindowType.SkillLevelUp].Close();
    }

}