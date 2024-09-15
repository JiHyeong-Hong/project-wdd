using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : PopupBase
{
    [SerializeField]
    private TMP_Text Title;
    [SerializeField]
    private TMP_Text subTitle;
    [SerializeField]
    private Image mainImage;
    [SerializeField]
    private TMP_Text recordInfo;
    [SerializeField]
    private Image ItemIco;
    [SerializeField]
    private TMP_Text ItemCnt;
    [SerializeField]
    private Button homeButton;

    private void Awake()
    {
        switch (Managers.Stage.state)
        {
            case Define.EStageState.Clear:
                Title.text = "구출 성공";
                subTitle.text = "";
                break;
            case Define.EStageState.Fail:
                Title.text = "구출 실패";
                subTitle.text = "많은 동물들이 구조대를 기다립니다.";
                break;
        }
        ItemCnt.text = Managers.Object.Hero.Gold.ToString();
    }

    public void OnClickHomeButton()
    {
        SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Button);
        Managers.Data.UserDic.Select(x => x.Value).FirstOrDefault().GoldAmt += Managers.Object.Hero.Gold;
        SaveManager.Instance.SaveClearData(Managers.Data.UserDic.Select(x => x.Value).FirstOrDefault().GoldAmt);
        SceneManagerNew.Instance.LoadScene(Define.EScene.Lobby);
        Managers.Instance.ResetManagers();
    }

}

/*
 일단 기본 코드는 일시정지 팝업이랑 기능이 비슷해보여 일시정지 팝업 코드를 가져와 사용하였습니다.
 
 팝업이 열리는 상황
 1. 게임 오버
 2. 게임 클리어
 
 팝업 살황별 기능
 1. 게임 오버
 <상황>
 플레이어의 체력이 0이 되어 부활 팝업 발생 - 전투포기 클릭 - ResultPopup 생성
 
 플레이어의 체력이 0이 되어 부활 팝업 
 Title - 게임 패배 / 게임오버 텍스트 출력
 subTitle - 다시 도와주세요.
 mainImage - 뭘 쓸지 모르겟지만 일단 아무거나 넣으셔도 될듯
 recordInfo - 동물 탈출 숫자 값 가져와서 출력
 homeButton - Lobby.Scene으로 이동
 ItemInfo - 일단 현재 획득한 골드 값 가져와서 출력하되 아이콘도 같이 불러와서 해야할거 같습니다.
            로비나 스킬 UI처럼 유동적으로 작동을 하는것이 좋아보이지만 현재에는 고정적으로 골드 하나만 출력하게 해야할거 같습니다.
            
 2. 게임 클리어
 <상황>
 보스몬스터의 체력이 0이 되어 EndBox 생성 - Hero > EndBox 상호 작용 후 - ResultPopup 생성
 
 Title - "게임 클리어"  텍스트 출력
 subTitle - "동물 구출에 성공하였습니다." 텍스트 출력
 mainImage - 1번과 동일
 recordInfo - 1번과 동일
 homeButton - 1번과 동일
 ItemInfo - 1번과 동일
 
 
 */
