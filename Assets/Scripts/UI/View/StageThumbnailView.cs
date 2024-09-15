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
    private Image stage1Image;
     [SerializeField]
    private Image stage2Image;
     [SerializeField]
    private Image lockImage;
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

    // ���� ������
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
        SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Button);
        SceneManagerNew.Instance.LoadScene(Define.EScene.GameScene);
        Managers.Game.IsGamePaused = false;
    }

    private void OnClickRightStageButton()
    {
        if(stageIndex == 0)
            leftButton.gameObject.SetActive(true);
            stageName.text = "???";
            stage1Image.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            stage2Image.gameObject.SetActive(true);
            rightButton.interactable = false;
            stageStartButton.interactable = false;

        ChangedStage(++stageIndex);
    }

    private void OnClickLeftStageButton()
    {
         if(stageIndex == 1)
            leftButton.gameObject.SetActive(false);
            stageName.text = "동물원 제 1구역";
            lockImage.gameObject.SetActive(false);
            stage2Image.gameObject.SetActive(false);
            stage1Image.gameObject.SetActive(true);
            rightButton.interactable = true;
            stageStartButton.interactable = true;

        ChangedStage(--stageIndex);
    }

    private void ChangedStage(int stageIndex)
    {
        // stageIndex �� 0���� ������ 0���� �ʱ�ȭ �Ǵ� stageIndex�� StageList�� Count���� ũ�� Count - 1�� �ʱ�ȭ
        if (stageIndex < 0) stageIndex = 0;
        if (stageIndex >= Managers.Game.StageList.Count) stageIndex = Managers.Game.StageList.Count - 1;

        // stageIndex�� �ش��ϴ� StageData�� ������
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
        //stageName.text = currentStageData.Name;
        stageDescription.text = currentStageData.Info;


        // �̹��� �ε�
        //Managers.Resource.LoadResource<Sprite>(currentStageData.IconURL);
        //Sprite sprite = Managers.Resource.GetResource<Sprite>(currentStageData.IconURL);
        //stageImage.sprite = sprite;

        // ��� ���ο� ���� ��ư Ȱ��ȭ
        stageStartButton.interactable = !currentStageData.Locked;
        stageLockImage.gameObject.SetActive(currentStageData.Locked);

    }

    public void UpdateUI(object data)
    {
        throw new NotImplementedException();
    }
}