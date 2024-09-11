using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : PopupBase
{
    [SerializeField]
    private Button EffectButton;
    [SerializeField]
    private TMP_Text EffectText;
    [SerializeField]
    private Button BGMButton;
    [SerializeField]
    private TMP_Text BGMText;
    [SerializeField]
    private Button ExitButton;
    
    private void Awake()
    {
        
        //TODO 저장 상태에 따라 Text변경
        /*
         *  저장 데이터를 가져옴
         *  Mute = false인 경우
         *  EffectText, BGMText.text = "On"
         *  Mute = ture인 경우
         *  EffectText, BGMText.text = "Off"
         */
    }


    public void ChangeSoundState(Define.ESound type)
    {
        switch (type)
        {
            case Define.ESound.Bgm:
                SoundManager.Instance.BGMMute();
                break;
            case Define.ESound.Effect:
                SoundManager.Instance.EffectMute();
                break;
        }
    }

    public void OnClickEffectSound() => ChangeSoundState(Define.ESound.Effect);
    public void OnClickBGMSound() => ChangeSoundState(Define.ESound.Bgm);
    
    
    public void Close()
    {
        Hide();
    }

}
