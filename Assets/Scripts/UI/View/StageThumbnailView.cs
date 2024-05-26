using System;
using System.Collections;
using System.Collections.Generic;
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


    private StageData currentStageData;
    private int stageIndex = 0;

    // ���� ������
    private StageData stageData = new StageData()
    {
        StageID = 1,
        Name = "Stage 1",
        Lv = 1,
        info = "Stage 1 Info",
        Locked = true,
        IconURL = "https://via.placeholder.com/150"
    };

    private void Start()
    {
        stageStartButton.onClick.AddListener(OnClickStageButton);
        leftButton.onClick.AddListener(OnClickLeftStageButton);
        rightButton.onClick.AddListener(OnClickRightStageButton);

        Managers.Game.StageList.Add(stageData);
    }

    private void OnClickStageButton()
    {
        
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
        // stageIndex �� 0���� ������ 0���� �ʱ�ȭ �Ǵ� stageIndex�� StageList�� Count���� ũ�� Count - 1�� �ʱ�ȭ
        if (stageIndex < 0) stageIndex = 0;
        if (stageIndex >= Managers.Game.StageList.Count) stageIndex = Managers.Game.StageList.Count - 1;

        // stageIndex�� �ش��ϴ� StageData�� ������
        StageData stageData = Managers.Game.StageList[stageIndex];
        if (currentStageData != stageData)
        {
            currentStageData = stageData;
            UpdateUI();
        }

    }

    private void UpdateUI()
    {
        stageName.text = currentStageData.Name;
        stageDescription.text = currentStageData.info;


        // �̹��� �ε�
        Managers.Resource.LoadResource<Sprite>(currentStageData.IconURL);
        Sprite sprite = Managers.Resource.GetResource<Sprite>(currentStageData.IconURL);
        stageImage.sprite = sprite;

        // ��� ���ο� ���� ��ư Ȱ��ȭ
        stageStartButton.interactable = !currentStageData.Locked;
        stageLockImage.gameObject.SetActive(currentStageData.Locked);

    }

    public void UpdateUI(object data)
    {
        throw new NotImplementedException();
    }
}