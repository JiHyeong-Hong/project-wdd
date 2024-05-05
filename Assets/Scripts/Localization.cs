using TMPro;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public TMP_Text tmpText;

    public TMP_Text TEXT 
    { 
        get
        {
            if(tmpText == null)
            {
                tmpText = GetComponent<TMP_Text>();
            }

            return tmpText;
        }
    }

    public string key;


    private void OnEnable()
    {
        SetLocalizeText(key);
    }

    private void SetLocalizeText(string key)
    {
        TEXT.text = Test.Instance.localizationManager.GetLocalizedText(key);
    }

}
