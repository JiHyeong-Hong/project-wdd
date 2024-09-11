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


    public void OnClickHomeButton()
    {
        SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Button);
        MessageBoxHelper.HomeButton();
        Managers.UI.ClosePopupUI();
    }

    public void OnClickContinueButton()
    {
        Managers.Game.IsGamePaused = false;
        Close();
    }

    public void OnClickMuteButton()
    {
        //Managers.Sound.Mute();
    }

    public void Close()
    {
        Hide();
        
    }

}
