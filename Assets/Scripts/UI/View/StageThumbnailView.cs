using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageThumbnailView : MonoBehaviour, IView
{
    [SerializeField]
    private TMP_Text stageName;
    [SerializeField]
    private TMP_Text stageDescription;
    [SerializeField]
    private Image stageImage;
    [SerializeField]
    private Button stageStartButton;
    [SerializeField]
    private Button stageLockImage;
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;


    private Data.Stage currentStageData;
    private int stageIndex = 0;

    // 더미 데이터
    private Data.Stage stageData = new Data.Stage()
    {
        StageID = 1,
        Name = "Stage 1",
        Lv = 1,
        Info = "Stage 1 Info",
        Locked = true,
        //IconURL = "https://via.placeholder.com/150"
    };

    private void Start()
    {
        stageStartButton.onClick.AddListener(OnClickStageButton);
        leftButton.onClick.AddListener(OnClickLeftStageButton);
        rightButton.onClick.AddListener(OnClickRightStageButton);

        Init();
    }

    private void Init()
    {
        Managers.Game.currentStageID = Managers.Data.StageDic.First().Value.StageID;
    }


    private void OnClickStageButton()
    {
        SceneManagerNew.Instance.LoadScene(Define.EScene.GameScene);

    }

    private void OnClickRightStageButton()
    {
        ChangedStage(++stageIndex);
    }

    private void OnClickLeftStageButton()
    {
        ChangedStage(--stageIndex);
    }

    private void ChangedStage(int stageIndex)
    {
        // stageIndex 가 0보다 작으면 0으로 초기화 또는 stageIndex가 StageList의 Count보다 크면 Count - 1로 초기화
        if (stageIndex < 0) stageIndex = 0;
        if (stageIndex >= Managers.Game.StageList.Count) stageIndex = Managers.Game.StageList.Count - 1;

        // stageIndex에 해당하는 StageData를 가져옴
        Data.Stage stageData = Managers.Game.StageList[stageIndex];
        if (currentStageData != stageData)
        {
            currentStageData = stageData;
            UpdateUI();
        }
        
        Managers.Game.currentStageID = currentStageData.StageID;
    }

    private void UpdateUI()
    {
        stageName.text = currentStageData.Name;
        stageDescription.text = currentStageData.Info;


        // 이미지 로드
        //Managers.Resource.LoadResource<Sprite>(currentStageData.IconURL);
        //Sprite sprite = Managers.Resource.GetResource<Sprite>(currentStageData.IconURL);
        //stageImage.sprite = sprite;

        // 잠금 여부에 따라 버튼 활성화
        stageStartButton.interactable = !currentStageData.Locked;
        stageLockImage.gameObject.SetActive(currentStageData.Locked);

    }

    public void UpdateUI(object data)
    {
        throw new NotImplementedException();
    }
}