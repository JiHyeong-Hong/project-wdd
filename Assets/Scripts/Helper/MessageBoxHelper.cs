
using UnityEngine;
using static MessageBox;

public class MessageBoxHelper
{

    public static MessageBox.buttonDelegate OnMessageBoxButton;
    public static string messageData = "";
    public static bool IsNative = false;
    public static bool IsOnAction = false;
    private static MessageBox messageBox = null;
    //private static ScrollMessageBox scrollMessageBox = null;
    //private static ContentsOpenMessageBox contentsOpenMessageBox = null;

    public static void ShowMessageBox_OneButton(string title, string text, string btnText, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.OneButton, popupType, btnText, "", "", null, null, null, callback, data, false);
    }

    public static void ShowMessageBox_TwoButton(string title, string text, string btnText1, string btnText2, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.TwoButton, popupType, btnText1, btnText2, "", null, null, null, callback, data, false);
    }

    public static void ShowMessageBox_ThreeButton(string title, string text, string btnText1, string btnText2, string btnText3, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.ThreeButton, popupType, btnText1, btnText2, btnText3, null, null, null, callback, data, false);
    }
    public static void ShowMessageBox_OneButton_WithImage(string title, string text, string btnText, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.OneButton, popupType, btnText, "", "", sp1, null, null, callback, data, false);
    }

    public static void ShowMessageBox_TwoButton_WithImage(string title, string text, string btnText1, string btnText2, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.TwoButton, popupType, btnText1, btnText2, "", sp1, sp2, sp3, callback, data, false);
    }

    public static void ShowMessageBox_ThreeButton_WithImage(string title, string text, string btnText1, string btnText2, string btnText3, MessageBox.PopupType popupType, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.ThreeButton, popupType, btnText1, btnText2, btnText3, sp1, sp2, sp3, callback, data, false);
    }

    public static void ShowMessageBox(string title, string text, MessageBox.Mode mode, MessageBox.PopupType popupType, string btnText1, string btnText2 = "", string btnText3 = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null, MessageBox.buttonDelegate callback = null, string data = "", bool IsWITHDRAW = false)
    {
        switch (mode)
        {
            case MessageBox.Mode.OneButton:
            case MessageBox.Mode.TwoButton:
            case MessageBox.Mode.ThreeButton:
                ShowMessageBoxNext(title, text, mode, popupType, btnText1, sp1, btnText2, sp2, btnText3, sp3, callback, data, IsWITHDRAW);
                break;
        }
    }


    public static void ShowMessageBoxNext(string title, string text, MessageBox.Mode mode, MessageBox.PopupType popupType,
                                            string btnText1, Sprite btnSp1 = null, 
                                            string btnText2 = "", Sprite btnSp2 = null, 
                                            string btnText3 = "", Sprite btnSp3 = null
                                         , MessageBox.buttonDelegate callback = null, string data = "", bool IsWITHDRAW = false)
    {
        if (null == messageBox)
            messageBox = Managers.UI.ShowMessageBox<MessageBox>();

        if (messageBox == null)
        {
            Debug.LogError("messageBox is null");
            return;
        }

        messageBox.Show(title, text, mode, popupType, btnText1, null, btnText2, null, btnText3, null, callback, data, IsWITHDRAW);
    
    }

    public static void HomeButton()
    {
        MessageBoxHelper.ShowMessageBox_TwoButton("게임종료", "신뢰?", "전투포기", "계속하기", MessageBox.PopupType.Back, (button, data) =>
        {
            if (button == 0)
            {
                Managers.Game.IsGamePaused = false;
                Debug.Log("전투포기");
                //Managers.Scene.LoadScene(Define.Scene.Game); // 로비 씬으로 이동
            }
            else if (button == 1)
            {
                Managers.Game.IsGamePaused = false;
                Debug.Log("계속하기");
            }
        }, "");
    }

}
