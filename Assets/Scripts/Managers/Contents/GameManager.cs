using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class GameManager
{
    public bool isStartGame = false;

    private EGameState _gameState = EGameState.Nomal;

    public EGameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }

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

    public int currentStageID;

    private List<Data.Stage> stageList;
    public List<Data.Stage> StageList
    {
        get 
        {
            if (stageList == null)
            {
                stageList = new List<Data.Stage>();
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
        Managers.Game.isStartGame = false;

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
                        SceneManagerNew.Instance.LoadScene(Define.EScene.Lobby);
                    }
                    else if(button == 1)
                    {
                        GameObject.Destroy(GameObject.Find("MessageBox"));
                        IsGamePaused = false;
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
    public void RefreshUI() // UI구조 변경으로 삭제해야할듯? @홍지형
    {
        OnUIRefreshed?.Invoke();
    }

    public Action OnLevelUp;
    
    public IEnumerator BossCount()
    {
        while (true)
        {
            if (CurrentTime >= 1f && GameState == EGameState.Nomal)
            {
                GameState = EGameState.Warning;
                // Managers.UI.ShowPopupUI<UI_Warning>();
                UIManagerNew.Instance.ShowPopup<WarningPopup>();
                
            }
            else if (CurrentTime >= 2f && GameState == EGameState.Warning)
            {
                GameState = EGameState.Barricade;
                //TODO Eung 바리게이트 오브젝트만들어서 생성하면 될듯 - 바리게이트 Spawn으로 바꾸면 될듯
                Managers.Object.Spawn<Structure>(Managers.Object.Hero.transform.position, 0);
            }
            else if(CurrentTime >= 3f && GameState == EGameState.Barricade)
            {
                //TODO Eung StageLv 테이블을 만들어서 스테이지별 등장 보스몬스터 넘버를 받아와서 대입하면 될듯 
                Managers.Object.Spawn<Boss>(Managers.Object.Hero.transform.position * 2, 241);
                GameState = EGameState.Boss;
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}
