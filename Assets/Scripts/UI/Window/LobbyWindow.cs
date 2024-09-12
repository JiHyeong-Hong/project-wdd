using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyWindow : UIWindow
{
    // [SerializeField]
    // private LobbyView view;
    // private LobbyPresenter presenter;
    [SerializeField]
    private GameObject SettingButton;
    
    [SerializeField]
    private Transform goodsThumbnailListGrid;
    

    [SerializeField]
    private Transform MiddleAnchor;


    [SerializeField]
    private GameObject goodsThumbnail;
    private List<IView> goodsThumbnailList;

    [SerializeField]
    private GameObject stageThumbnail;
    private IView stageThumbnailView;

    // private void Awake()
    // {
    //     Init();
    // }
    // private void Init()
    // {
    //     presenter = new LobbyPresenter(view);
    // }
    
    private void Start()
    {
        goodsThumbnailList = new List<IView>();
        for (int i = 0; i < 3; i++)
        {
            GameObject goodsThumbnailObj = Instantiate(goodsThumbnail, goodsThumbnailListGrid);
            goodsThumbnailObj.SetActive(true);
            goodsThumbnailObj.transform.SetParent(goodsThumbnailListGrid);
            goodsThumbnailList.Add(goodsThumbnailObj.GetComponent<IView>());
        }

        stageThumbnailView = Instantiate(stageThumbnail, MiddleAnchor).GetComponent<IView>();
        SoundManager.Instance.Play(Define.ESoundMainType.Bgm, Define.ESoundType.Lobby);
    }


    public void OnClickSetting()
    {
        UIManagerNew.Instance.ShowPopup<SettingPopup>(false);
    }

    // //현재 사용은 하지않는 업데이트 함수
    // private void Update()
    // {
    //     throw new NotImplementedException();
    // }
}
