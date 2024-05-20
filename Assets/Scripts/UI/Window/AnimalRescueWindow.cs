using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRescueWindow : UIWindow
{
    public GameObject AnimalThumbnail;

    public void SetAnimalWindow<T>(List<T> list)
    {
        foreach (var item in list)
        {
            GameObject obj = Instantiate(AnimalThumbnail, transform);
        }
    }

}
