using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : UI_Base
{
   
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, true); // SetCanvas 사용용도?
        return true;
    }
    
    //public virtual void ClosePopupUI()
    //{
    //    Managers.UI.ClosePopupUI(this);
    //} 
}
