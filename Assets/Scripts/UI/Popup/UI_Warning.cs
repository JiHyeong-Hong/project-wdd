using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Warning : UI_Popup
{
    enum Texts
    {
        Warning,
    }

    enum Images
    {
        BackGround_Frame
    }

    private TextMeshProUGUI _warning;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;       

        BindText(typeof(Texts));
        BindImage(typeof(Images));

        _warning = GetText((int)Texts.Warning);

        StartCoroutine(FadeOut());
        return true;
    }
    float time = 0f;

    private IEnumerator FadeOut()
    {
        
        while (true)
        {
            time += UnityEngine.Time.deltaTime;
            if(time >= 2f)
                Managers.UI.ClosePopupUI(this);
            
            yield return new WaitForFixedUpdate();
            
        }
    }

    public void test()
    {
    }

    
}
