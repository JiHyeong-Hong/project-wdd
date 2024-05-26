using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyStaminaPopup : PopupBase
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private Button admobButton;

    private void Start()
    {
        exitButton.onClick.AddListener(OnClickExit);
        buyButton.onClick.AddListener(OnClickBuyButton);
        admobButton.onClick.AddListener(OnClickAdmobButton);

    }

    private void OnClickExit()
    {
        Hide();
    }
    private void OnClickBuyButton()
    {

    }

    private void OnClickAdmobButton()
    {

    }


}