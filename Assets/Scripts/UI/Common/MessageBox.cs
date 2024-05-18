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


    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private GameObject Timer;



    [SerializeField]
    private Button buttonOne;
    [SerializeField]
    private Button buttonTwo;
    [SerializeField]
    private Button buttonThree;

    string data;
    #region btnText
    private TMP_Text buttonOneText;
    private TMP_Text ButtonOneText 
    { 
        get
        {
            if (buttonOneText == null)
            {
                buttonOneText = buttonOne.transform.Find("Description").GetComponent<TMP_Text>();
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
                buttonTwoText = buttonTwo.transform.Find("Description").GetComponent<TMP_Text>();
            }
            return buttonTwoText;
        }
    }
    private TMP_Text buttonThreeText;
    private TMP_Text ButtonThreeText
    {
        get
        {
            if (buttonThreeText == null)
            {
                buttonThreeText = buttonThree.transform.Find("Description").GetComponent<TMP_Text>();
            }
            return buttonThreeText;
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
    private Image buttonThreeImage;
    public Image ButtonThreeImage
    {
        get
        {
            if (buttonThreeImage == null)
            {
                buttonThreeImage = buttonThree.transform.Find("Image").GetComponent<Image>();
            }
            return buttonThreeImage;
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
        buttonThreeText = null;
        buttonOneImage = null;
        buttonTwoImage = null;
        buttonThreeImage = null;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        Show("hi", "isTest", Mode.TwoButton, "1", testSP, "2", null, "3", null, (button, data) =>
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

    public void Show(string title, string text, Mode mode,
                                string btnText1, Sprite btnSprite1 = null, 
                                string btnText2 = "", Sprite btnSprite2 = null, 
                                string btnText3 = "", Sprite btnSprite3 = null,
                                buttonDelegate callback = null, string data = "", bool isWithdraw = false)
    {
        gameObject.SetActive(true);

        switch (mode)
        {
            case Mode.OneButton:
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(false);
                buttonThree.gameObject.SetActive(false);
                break;
            case Mode.TwoButton:
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(true);
                buttonThree.gameObject.SetActive(false);

                titleText.text = title;
                messageText.text = text;

                ButtonOneText.text = btnText1;
                ButtonTwoText.text = btnText2;



                if(btnSprite1 == null)
                {
                    Debug.Log(ButtonOneImage.name);
                    ButtonOneImage.gameObject.SetActive(false);
                }
                else
                {
                    ButtonOneImage.gameObject.SetActive(true);
                    ButtonOneImage.sprite = btnSprite1;
                }

                if(btnSprite2 == null)
                {
                    ButtonTwoImage.gameObject.SetActive(false);
                }
                else
                {
                    ButtonTwoImage.gameObject.SetActive(true);
                    ButtonTwoImage.sprite = btnSprite2;
                }




                break;
            case Mode.ThreeButton:
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(true);
                buttonThree.gameObject.SetActive(true);
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
            Timer.transform.Find("Time").GetComponent<TMP_Text>().text = limitTime.ToString();
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
