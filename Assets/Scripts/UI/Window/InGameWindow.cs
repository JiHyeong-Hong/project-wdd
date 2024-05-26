using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWindow : UIWindow
{
    [SerializeField]
    private InGameView view;
    private InGamePresenter presenter;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        presenter = new InGamePresenter(view);
    }

    public void UpdateGameData(ProfileData profileData)
    {
        presenter.SetGameData(profileData);
    }
}