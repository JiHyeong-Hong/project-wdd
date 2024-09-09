using Data;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private Image skillImageCombination;
    [SerializeField]
    private TMP_Text hasSkillText;

    public Button levelUpButton;

    public void SetPresenter(SkillLevelUpPresenter presenter)
    {
        levelUpButton.enabled = false;
        this.presenter = presenter;
    }

    public void UpdateUI(object data)
    {
        if (data is SkillBase) // 스킬 선택 UI thumbNail 갱신
        {
            SkillBase skillBase = (SkillBase)data;
            levelUpButton.enabled = true; // UpdateUI가 호출되는 버튼만 활성화
            name = skillBase.SkillData.Name; // 영어원문 그대로
            int level = skillBase.SkillData.Level;
            string description = skillBase.SkillData.Kor_Text;

            skillLevel.text = $"Level {skillBase.SkillData.Level}";

            // 새로운 스킬에 'New!' 표시
            if (skillBase.SkillData.Level == 1) { NewSkill.gameObject.SetActive(true); }
            else { NewSkill.gameObject.SetActive(false); }

            skillImage.sprite = Managers.Resource.GetSkillSprite(skillBase.SkillData.Name);
            skillDescription.text = description;
            skillName.text = name;
            return;
        }
        else // 조합 보유/미보유 UI 갱신
        {
            // 값 불러오기
            var type = data.GetType();
            var skillNameProperty = type.GetProperty("SkillName");
            var isContainsSkillProperty = type.GetProperty("IsContiansSkill");
            string skillName = "";
            bool isContainsSkill = false;
            if (skillNameProperty != null && isContainsSkillProperty != null)
            {                
                skillName = (string)skillNameProperty.GetValue(data);
                isContainsSkill = (bool)isContainsSkillProperty.GetValue(data);
            }
            //

            if (isContainsSkill == true) // 조합스킬 보유 시
            {
                skillImageCombination.sprite = Managers.Resource.GetSkillSprite(skillName);
                skillImageCombination.color = new Color(255f, 255f, 255f); // 활성화 색
                hasSkillText.text = "보유";
            }
            else // 조합스킬 미보유 시
            {
                skillImageCombination.sprite = Managers.Resource.GetSkillSprite(skillName);
                skillImageCombination.color = new Color(85 / 255f, 85 / 255f, 85 / 255f); // 비활성화 색
                hasSkillText.text = "미보유";
            }
            return;
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