using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour, IView
{
    [SerializeField]
    private TMP_Text playerNameText;
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text expText;
    [SerializeField]
    private TMP_Text goldText;
    [SerializeField]
    private TMP_Text animalSaveCountText;    
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]        
    private Image _hpImg;
    [SerializeField]
    private Image _expImg;
    [SerializeField]
    private Image _bosshpbackImg;
    [SerializeField]
    private Image _bosshpImg;


    void Awake()
    {
 
    }

    private void UpdateUI(ProfileData data)
    {
        // playerNameText.text = data.Name;     // @홍지형 테스트
        levelText.text = data.Level.ToString();
        _expImg.fillAmount = (float)Managers.Object.Hero.Exp / Managers.Object.Hero.MaxExp;
        goldText.text = data.Gold.ToString();
        animalSaveCountText.text = data.AnimalSaveCount.ToString();
        UpdateTimerUI(data.Time);
        _hpImg.fillAmount = (float)Managers.Object.Hero.Hp / Managers.Object.Hero.MaxHp;
        
        if (Managers.Game.GameState == Define.EGameState.Boss &&
            Managers.Object.Bosses != null)
        {
            if(!_bosshpbackImg.gameObject.activeSelf)
                _bosshpbackImg.gameObject.SetActive(true);
            if(!_bosshpImg.gameObject.activeSelf)
                _bosshpImg.gameObject.SetActive(true);
            _bosshpImg.fillAmount = Managers.Object.Bosses.Hp / Managers.Object.Bosses.MaxHp;
        }
    }

    public void UpdateUI(object data)
    {
        UpdateUI((ProfileData)data);
    }

    private void UpdateTimerUI(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // TODO:
    }
}
