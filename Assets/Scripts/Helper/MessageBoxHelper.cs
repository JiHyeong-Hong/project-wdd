
using UnityEngine;

public class MessageBoxHelper
{

    public static MessageBox.buttonDelegate OnMessageBoxButton;
    public static string messageData = "";
    public static bool IsNative = false;
    public static bool IsOnAction = false;
    private static MessageBox messageBox = null;
    //private static ScrollMessageBox scrollMessageBox = null;
    //private static ContentsOpenMessageBox contentsOpenMessageBox = null;

    public static void ShowMessageBox_OneButton(string title, string text, string btnText, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.OneButton, btnText, "", "", null, null, null, callback, data, false);
    }

    public static void ShowMessageBox_TwoButton(string title, string text, string btnText1, string btnText2, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.TwoButton, btnText1, btnText2, "", null, null, null, callback, data, false);
    }

    public static void ShowMessageBox_ThreeButton(string title, string text, string btnText1, string btnText2, string btnText3, MessageBox.buttonDelegate callback = null, string data = "")
    {
        ShowMessageBox(title, text, MessageBox.Mode.ThreeButton, btnText1, btnText2, btnText3, null, null, null, callback, data, false);
    }
    public static void ShowMessageBox_OneButton_WithImage(string title, string text, string btnText, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.OneButton, btnText, "", "", sp1, null, null, callback, data, false);
    }

    public static void ShowMessageBox_TwoButton_WithImage(string title, string text, string btnText1, string btnText2, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.TwoButton, btnText1, btnText2, "", sp1, sp2, sp3, callback, data, false);
    }

    public static void ShowMessageBox_ThreeButton_WithImage(string title, string text, string btnText1, string btnText2, string btnText3, MessageBox.buttonDelegate callback = null, string data = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null)
    {
        ShowMessageBox(title, text, MessageBox.Mode.ThreeButton, btnText1, btnText2, btnText3, sp1, sp2, sp3, callback, data, false);
    }

    public static void ShowMessageBox(string title, string text, MessageBox.Mode mode, string btnText1, string btnText2 = "", string btnText3 = "", Sprite sp1 = null, Sprite sp2 = null, Sprite sp3 = null, MessageBox.buttonDelegate callback = null, string data = "", bool IsWITHDRAW = false)
    {
        switch (mode)
        {
            case MessageBox.Mode.OneButton:
            case MessageBox.Mode.TwoButton:
            case MessageBox.Mode.ThreeButton:
                ShowMessageBoxNext(title, text, mode, btnText1, sp1, btnText2, sp2, btnText3, sp3, callback, data, IsWITHDRAW);
                break;
        }
    }


    public static void ShowMessageBoxNext(string title, string text, MessageBox.Mode mode,
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

        messageBox.Show(title, text, mode, btnText1, null, btnText2, null, btnText3, null, callback, data, IsWITHDRAW);
    
    }

}
