using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePresenter
{
    private readonly InGameModel model;
    private readonly IView view;

    public InGamePresenter(IView view)
    {
        this.view = view;
        model = new InGameModel();
        model.OnProfileDataChanged += OnProfileDataChanged;
    }

    private void OnProfileDataChanged(ProfileData profileData)
    {
        view.UpdateUI(profileData);
    }

    public void SetGameData(ProfileData profileData)
    {
        model.SetProfileData(profileData);
    }
}
