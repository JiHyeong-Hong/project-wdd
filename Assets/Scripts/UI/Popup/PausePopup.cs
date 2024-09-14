using Cysharp.Threading.Tasks;
using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : PopupBase
{
    [SerializeField]
    private Button homeButton;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button muteButton;
    [SerializeField]
    private Image ActiveAndPassiveImage;
    [SerializeField]
    private Transform activeGrid;
    [SerializeField]
    private Transform passiveGrid;

    public void OnClickHomeButton()
    {
        SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Button);
        Close(false);
        MessageBoxHelper.HomeButton();
    }

    public void OnClickContinueButton()
    {
        Close(true);
        //Managers.Game.IsGamePaused = false;
    }

    public void OnClickMuteButton()
    {
        ChangeSoundState(Define.ESound.Bgm);
        ChangeSoundState(Define.ESound.Effect);
    }

    public void Close(bool isGameStart) // true면 멈춘 게임 재시작, false면 게임 재시작 안함.
    {
        Hide(isGameStart);
        
    }

    protected override void OnShow()
    {
        PreviewImage();
    }

    public void ChangeSoundState(Define.ESound type)
    {
        Color onColor = new Color(85 / 255f, 85 / 255f, 85 / 255f); // 비활성화 색
        Color offColor = new Color(255 / 255f, 255 / 255f, 255 / 255f); // 활성화색
        Image btnImage = muteButton.GetComponent<Image>();        
        switch (type)
        {
            case Define.ESound.Bgm:
                SoundManager.Instance.BGMState = !SoundManager.Instance.BGMState;
                btnImage.color = SoundManager.Instance.BGMState ? onColor : offColor;
                SoundManager.Instance.BGMMute();
                break;
            case Define.ESound.Effect:
                SoundManager.Instance.EffectState = !SoundManager.Instance.EffectState;
                btnImage.color = SoundManager.Instance.BGMState ? onColor : offColor;
                SoundManager.Instance.EffectMute();
                break;
        }

        SaveManager.Instance.SaveSettingData();
    }

    // 현재 스킬 보유 화면 UI
    private void PreviewImage()
    {
        List<SkillBase> activeSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Active];
        List<SkillBase> passiveSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Passive];
        List<SkillBase> btSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Breakthrough];
        foreach (var item in activeSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(activeGrid);
            obj.GetComponent<Image>().sprite = Managers.Resource.GetSkillSprite(item.SkillData.Name);
            WaitPoolPush(obj).Forget();
        }

        foreach (var item in passiveSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(passiveGrid);
            obj.GetComponent<Image>().sprite = Managers.Resource.GetSkillSprite(item.SkillData.Name);
            WaitPoolPush(obj).Forget();
        }

        foreach (var item in btSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(activeGrid);
            obj.GetComponent<Image>().sprite = Managers.Resource.GetSkillSprite(item.SkillData.Name);
            WaitPoolPush(obj).Forget();
        }
    }

    private async UniTask WaitPoolPush(Poolable waitObject)
    {
        await UniTask.WaitUntil(() => this.isActiveAndEnabled == false);
        waitObject.IsUsing = false;
        Managers.Pool.Push(waitObject);

    }

}
