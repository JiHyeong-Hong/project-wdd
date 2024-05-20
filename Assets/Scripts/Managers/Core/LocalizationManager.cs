using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class LocalizationManager
{
    private Dictionary<LanguageCode, Dictionary<string, string>> languageCodes;

    private Dictionary<string, string> localizedText = new();

    public LocalizationManager()
    {
        InitLangeuage();
    }

    enum LanguageCode
    {
        English,
        Korean,
        Japanese,
    }

    private LanguageCode currentLanguageCode;


    private void InitLangeuage()
    {
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;

        switch (cultureInfo.TwoLetterISOLanguageName)
        {
            case "en":
                currentLanguageCode = LanguageCode.English;
                break;
            case "ko":
                currentLanguageCode = LanguageCode.Korean;
                break;
            case "ja":
                currentLanguageCode = LanguageCode.Japanese;
                break;
            default:
                currentLanguageCode = LanguageCode.English;
                break;
        }

        LoadLanguageFile(cultureInfo.TwoLetterISOLanguageName);

    }

    private void LoadLanguageFile(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Languages/" + fileName + ".txt");
        //Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] entries = line.Split(new char[] { '=' }, 2);
                if (entries.Length == 2)
                {
                    localizedText.Add(entries[0].Trim(), entries[1]);
                }
            }
        }
        else
        {
            Debug.LogError("Language file not found: " + filePath);
        }
    }


    public string GetLocalizedText(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            Debug.LogWarning("Localized text not found for key: " + key);
            return key;
        }
    }
}