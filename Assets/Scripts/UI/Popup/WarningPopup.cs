using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopup : PopupBase
{
    [SerializeField]
    private Image rawImage;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TMP_Text desc;

    private CanvasGroup canvasGroup;
    private CanvasGroup MyCanvasGroup
    {
        get
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup;
        }
    }


    public void Init(bool iconActive, string desc, string imageName = null)
    {
        if (iconActive) 
        {
            rawImage.enabled = true;
            iconImage.enabled = true;

            if (imageName != null) iconImage.sprite = Managers.Resource.Load<Sprite>(imageName);
        }
        else
        {
            rawImage.enabled = false;
            iconImage.enabled = false;
        }

        this.desc.text = desc;
        StartCoroutine(Util.FadeOut(MyCanvasGroup, 2, () => this.gameObject.SetActive(false)));
    }
}
