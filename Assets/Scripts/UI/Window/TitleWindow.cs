using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleWindow : MonoBehaviour
{
    public Button startButton;


    public void StartLobby()
    {
        SceneManagerNew.Instance.LoadScene(Define.EScene.GameScene);
    }
}
