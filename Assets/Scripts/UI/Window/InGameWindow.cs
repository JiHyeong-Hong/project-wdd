using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameWindow : UIWindow
{
    [SerializeField]
    private InGameView view;
    private InGamePresenter presenter;
    ProfileData profileData;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        presenter = new InGamePresenter(view);
        profileData = new ProfileData(Managers.Object.Hero.name, Managers.Object.Hero.Level, Managers.Object.Hero.Exp, Managers.Object.Hero.Gold, Managers.Object.Hero.Hp, Managers.Game.CurrentTime);
    }

    public void UpdateGameData(ProfileData profileData)
    {
        presenter.SetGameData(profileData);
    }

    private void Update()
    {
        if (!Managers.Game.IsGamePaused)
        {
            UpdateTimer(); // TODO:
        }

        profileData.Name = Managers.Object.Hero.name;
        profileData.Level = Managers.Object.Hero.Level;
        profileData.Exp = Managers.Object.Hero.Exp;
        profileData.Gold = Managers.Object.Hero.Gold;
        profileData.Time = Managers.Game.CurrentTime;
        profileData.Hp = Managers.Object.Hero.Hp;
        UpdateGameData(profileData);

        if (Managers.Game.GameState == Define.EGameState.Boss)
        {
            profileData.BossHp = Managers.Object.Bosses.Hp;
        }
    }

    private void UpdateTimer()
    {
        Managers.Game.CurrentTime += Time.deltaTime;
    }
}