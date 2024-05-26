using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager
{
    private bool _isGamePaused = false;
    public bool IsGamePaused
    {
        get { return _isGamePaused; }
        set
        {
            _isGamePaused = value;
            Time.timeScale = _isGamePaused ? 0 : 1;
        }
    }

    private float _currentTime = 0;
    public float CurrentTime
    {
        get { return _currentTime; }
        set
        {
            _currentTime = value;
        }
    }

    private int _monsterKillCount = 0;
    public int MonsterKillCount
    {
        get { return _monsterKillCount; }
        set
        {
            _monsterKillCount = value;
        }
    }

    private int _gold = 0;
    public int Gold
    {
        get { return _gold; }
        set
        {
            _gold = value;
        }
    }

    private List<StageData> stageList;
    public List<StageData> StageList
    {
        get 
        {
            if (stageList == null)
            {
                stageList = new List<StageData>();
                //stageList = DataManager.Instance.StageDataDic.ToList();
            }
            return stageList;
        }
    }

    #region Hero
    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private Define.EJoystickState _joystickState;
    public Define.EJoystickState JoystickState
    {
        get { return _joystickState; }
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(_joystickState);
        }
    }

    public void GameOver()
    {
        IsGamePaused = true;

        MessageBoxHelper.ShowMessageBox_TwoButton("GameOver", "Retry?", "Yes", "No", MessageBox.PopupType.Retry, (button, data) =>
        {
            if (button == 0)
            {
                IsGamePaused = false;
                Debug.Log("게임 재시작");
                //Managers.Scene.LoadScene(Define.Scene.Game);
            }
            else if (button == 1)
            {
                IsGamePaused = true;
                Debug.Log("게임종료");

                MessageBoxHelper.ShowMessageBox_TwoButton("게임종료", "신뢰?", "전투포기", "계속하기", MessageBox.PopupType.Back, (button, data) =>
                {
                    // 기능 구현
                    if(button == 0)
                    {
                        SceneManagerNew.Instance.LoadScene(Define.EScene.TitleScene);
                    }
                    else if(button == 1)
                    {
                        SceneManagerNew.Instance.LoadScene(Define.EScene.GameScene);
                    }

                }, "");


                //Managers.Scene.LoadScene(Define.Scene.Lobby);
            }
        }, "StartTimer");
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.EJoystickState> OnJoystickStateChanged;

    public event Action OnUIRefreshed;
    public void RefreshUI()
    {
        OnUIRefreshed?.Invoke();
    }

    public Action OnLevelUp;
    enum BossCountState
    {
        Counting,
        Warning,
        Barricade,
        Appear
    }
    public IEnumerator BossCount()
    {
        BossCountState test = BossCountState.Counting;
        while (true)
        {
            if (CurrentTime >= 1f && test == BossCountState.Counting)
            {
                Managers.UI.ShowPopupUI<UI_Warning>();
                test = BossCountState.Warning;
            }
            else if (CurrentTime >= 2f && test == BossCountState.Warning)
            {
                test = BossCountState.Barricade;
                //TODO Eung 바리게이트 오브젝트만들어서 생성하면 될듯 - 바리게이트 Spawn으로 바꾸면 될듯
                Managers.Object.Spawn<Structure>(Managers.Object.Hero.transform.position, 0);
                Debug.Log("바리게이트 생성");
            }
            else if(CurrentTime >= 3f && test == BossCountState.Barricade) 
            {
                //TODO Eung StageLv 테이블을 만들어서 스테이지별 등장 보스몬스터 넘버를 받아와서 대입하면 될듯 
                Managers.Object.Spawn<Boss>(Managers.Object.Hero.transform.position * 2, 241);
                Debug.Log("보스 생성");
                break;
            }

            yield return new WaitForFixedUpdate();
            
        }
    }
    #endregion
}
