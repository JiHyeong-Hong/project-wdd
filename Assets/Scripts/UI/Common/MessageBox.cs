using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MessageBox : SingletonMonoBehaviour<MessageBox>
{
    public enum Mode
    {
        OneButton,
        TwoButton,
        ThreeButton
    }

    public enum PopupType
    {
        Back,
        Retry
    }


    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private GameObject Timer;

    [SerializeField]
    private GameObject BackPopup;
    [SerializeField]
    private GameObject RetryPopup;


    [SerializeField]
    private Button buttonOne;
    [SerializeField]
    private Button buttonTwo;

    string data;
    #region btnText
    private TMP_Text buttonOneText;
    private TMP_Text ButtonOneText 
    { 
        get
        {
            if (buttonOneText == null)
            {
                buttonOneText = buttonOne.transform.Find("Description")?.GetComponent<TMP_Text>();
            }
            return buttonOneText;
        }
    }
    private TMP_Text buttonTwoText;
    private TMP_Text ButtonTwoText
    {
        get
        {
            if (buttonTwoText == null)
            {
                buttonTwoText = buttonTwo.transform.Find("Description")?.GetComponent<TMP_Text>();
            }
            return buttonTwoText;
        }
    }
    #endregion
    #region btnImage
    private Image buttonOneImage;
    public Image ButtonOneImage
    {
        get
        {
            if(buttonOneImage == null)
            {
                buttonOneImage = buttonOne.transform.Find("Image").GetComponent<Image>();
            }
            return buttonOneImage;
        }
    }

    private Image buttonTwoImage;
    public Image ButtonTwoImage
    {
        get
        {
            if (buttonTwoImage == null)
            {
                buttonTwoImage = buttonTwo.transform.Find("Image").GetComponent<Image>();
            }
            return buttonTwoImage;
        }
    }

    #endregion

    public Sprite testSP;

    public delegate void buttonDelegate(int buttonIndex, string data);
    buttonDelegate onClickButton;


    [ContextMenu("TestReSet")]
    public void TestReSet()
    {
        buttonOneText = null;
        buttonTwoText = null;
        buttonOneImage = null;
        buttonTwoImage = null;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        Show("hi", "isTest", Mode.TwoButton, PopupType.Retry, "1", testSP, "2", null, "3", null, (button, data) =>
        {
            if (button == 0)
            {
                Debug.Log("0번클릭");
            }

            if (button == 1)
            {
                Debug.Log("1번클릭");
            }
        });

        //MessageBoxHelper.ShowMessageBox_TwoButton("hi","isTest","1","2",(button,data) =>
        //{
        //    if (button == 0)
        //    {
        //        Debug.Log("0번클릭");
        //    }

        //    if (button == 1)
        //    {
        //        Debug.Log("1번클릭");
        //    }
        //});
    }

    public void Show(string title, string text, Mode mode, MessageBox.PopupType popupType,
                                string btnText1, Sprite btnSprite1 = null, 
                                string btnText2 = "", Sprite btnSprite2 = null, 
                                string btnText3 = "", Sprite btnSprite3 = null,
                                buttonDelegate callback = null, string data = "", bool isWithdraw = false)
    {
        gameObject.SetActive(true);

        switch (popupType)
        {
            case PopupType.Back:
                BackPopup.SetActive(true);
                RetryPopup.SetActive(false);

                buttonOne = BackPopup.transform.Find("BrownButtonOne").GetComponent<Button>();
                buttonTwo = BackPopup.transform.Find("BrownButtonTwo").GetComponent<Button>();
                Timer.SetActive(false);

                break;
            case PopupType.Retry:
                BackPopup.SetActive(false);
                RetryPopup.SetActive(true);
                Debug.Log(RetryPopup.transform.Find("BrownButtonOne"));
                buttonOne = RetryPopup.transform.Find("BrownButtonOne").GetComponent<Button>();
                buttonTwo = RetryPopup.transform.Find("BrownButtonTwo").GetComponent<Button>();
                Timer.SetActive(true);

                break;
            default:
                break;
        }


        switch (mode)
        {
            case Mode.OneButton:
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(false);
                break;
            case Mode.TwoButton:
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(true);

                titleText.text = title;
                messageText.text = text;

                if (ButtonOneText != null) ButtonOneText.text = btnText1;
                if (ButtonTwoText != null) ButtonTwoText.text = btnText2;

                //if(btnSprite1 == null)
                //{
                //    Debug.Log(ButtonOneImage.name);
                //    ButtonOneImage.gameObject.SetActive(false);
                //}
                //else
                //{
                //    ButtonOneImage.gameObject.SetActive(true);
                //    ButtonOneImage.sprite = btnSprite1;
                //}

                //if(btnSprite2 == null)
                //{
                //    ButtonTwoImage.gameObject.SetActive(false);
                //}
                //else
                //{
                //    ButtonTwoImage.gameObject.SetActive(true);
                //    ButtonTwoImage.sprite = btnSprite2;
                //}
                break;
            default:
                break;
        }

        //Invoke(data, 0);
        StartCoroutine("Co_Timer");
        onClickButton = callback;
        #region Test
        Invoke(onClickButton.Method.ToString(), 1.0f);

        #endregion

        //SoundManager.Instance.PlayOneShot(SoundManager.Sound.UI, SoundNames.FX.COMMON_OPEN_POPUP_NOTICE, Settings.Game.FxVolume);

    }


    public IEnumerator Co_Timer()
    {
        float limitTime = 10;

        while (limitTime >= 0)
        {
            Debug.Log(limitTime);
            Timer.transform.Find("TimerCount").GetComponent<TMP_Text>().text = limitTime.ToString();
            yield return YieldInstructionCache.WaitForSecondsRealtime(1);

            limitTime--;
        }
    }

    public void OnClickOneButton()
    {
        Hide();
        if (onClickButton != null)
        {
            onClickButton(0, data);
        }
    }

    public void OnClickTwoButton()
    {
        Hide();
        if (onClickButton != null)
        {
            onClickButton(1, data);
        }
    }

    public void OnClickThreeButton() 
    {
        Hide();
        if (onClickButton != null)
        {
            onClickButton(2, data);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
