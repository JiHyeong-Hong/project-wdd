using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GoodsData
{
    public string Name;
    public int Price;
    public int Count;
    public GoodsData(string name, int price, int count)
    {
        Name = name;
        Price = price;
        Count = count;
    }
}

public class ProfileData
{
    public string Name;
    public int Level;
    public float Exp;
    public int Gold;
    public int AnimalSaveCount;
    public ProfileData(string name, int level, int exp, int gold)
    {
        Name = name;
        Level = level;
        Exp = exp;
        Gold = gold;
    }
}


public class InGameModel
{
    public ProfileData ProfileData { get; private set; }
    public event Action<ProfileData> OnProfileDataChanged;

    public void SetProfileData(ProfileData profileData)
    {
        ProfileData = profileData;
        OnProfileDataChanged?.Invoke(profileData);
    }
}