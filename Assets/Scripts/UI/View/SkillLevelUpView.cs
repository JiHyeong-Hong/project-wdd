using Data;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class SkillLevelUpView : MonoBehaviour, IView
{
    private SkillLevelUpPresenter presenter;

    [SerializeField]
    private GameObject NewSkill;
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
        levelUpButton.enabled = false;
        this.presenter = presenter;
    }

    public void UpdateUI(object data)
    {
        if (data is SkillBase)
        {
            SkillBase skillBase = (SkillBase)data;
            levelUpButton.enabled = true; // UpdateUI가 호출되는 버튼만 활성화
            string name = skillBase.SkillData.Name; // 영어원문 그대로
            int level = skillBase.SkillData.Level;
            string description = skillBase.SkillData.Kor_Text;

            // string name = Managers.Localization.GetLocalizedText(((SkillBase)data).SkillData.Name); // 백업
            // string description = Managers.Localization.GetLocalizedText(((SkillBase)data).SkillData.Description);
            
            skillLevel.text = $"Level {skillBase.SkillData.Level}";

            // 새로운 스킬에 'New!' 표시
            if(skillBase.SkillData.Level == 1) { NewSkill.gameObject.SetActive(true); }
            else { NewSkill.gameObject.SetActive(false); }

            skillImage.sprite = Managers.Resource.GetSkillSprite(skillBase.SkillData.Name);
            skillDescription.text = description;
            skillName.text = name;
        }
    }

    public void LevelUpSkillClick()
    {
        presenter.HandleLevelUpSkill();

        UIManagerNew.Instance.HideCurrentWindow();        
        // UIManagerNew.Instance.HideWindow<WindowBase>(Define.UIWindowType.SkillLevelUpWindow);     // @홍지형    
        //Managers.UI.windowDic[Define.UIWindowType.SkillLevelUpWindow].Hide();
    }
   
}