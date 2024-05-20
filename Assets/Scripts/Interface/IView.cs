using UnityEngine;

public interface IView
{
    GameObject gameObject { get; }

    void UpdateUI(object data);
}