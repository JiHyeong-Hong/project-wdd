using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    public event System.Action OnSceneLoaded;
    private AsyncOperation asyncOperation;

    public override void Init()
    {
        // 씬 관련 초기화 작업을 수행합니다.
        //Debug.Log("SceneManager initialized.");
    }

    public void SaveData()
    {
        //TODO 현재 저장된 데이터 저장
    }
    
    public void SaveClearData(int value)
    {
        //TODO 현재 저장된 데이터 저장
        PlayerPrefs.SetInt("GoldAmt", value);
    }
    
    public void SaveSettingData()
    {
        //TODO 현재 저장된 데이터 저장
        PlayerPrefs.SetInt("BGMState", SoundManager.Instance.BGMState ? 1 : 0);
        PlayerPrefs.SetInt("EffectState", SoundManager.Instance.EffectState ? 1 : 0);
    }
    
    public void LoadData()
    {
        // Managers.Data.UserDic[0].GoldAmt = PlayerPrefs.GetInt("GoldAmt");
        SoundManager.Instance.BGMState = Convert.ToBoolean(PlayerPrefs.GetInt("BGMState"));
        SoundManager.Instance.EffectState = Convert.ToBoolean(PlayerPrefs.GetInt("EffectState"));
        Managers.Data.UserDic.Select(x => x.Value).FirstOrDefault().GoldAmt = PlayerPrefs.GetInt("GoldAmt");
        SoundManager.Instance.Init();
        
        //TODO 현재 저장된 데이터 불러오기
    }
    
    

    
}
