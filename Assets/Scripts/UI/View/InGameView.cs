using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void UpdateUI(ProfileData data)
    {
        playerNameText.text = data.Name;
        levelText.text = data.Level.ToString();
        expText.text = data.Exp.ToString();
        goldText.text = data.Gold.ToString();
        animalSaveCountText.text = data.AnimalSaveCount.ToString();

    }

    public void UpdateUI(object data)
    {
        UpdateUI((ProfileData)data);
    }
}
