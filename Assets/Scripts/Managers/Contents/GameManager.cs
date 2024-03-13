using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (CurrentTime >= 5f && test == BossCountState.Counting)
            {
                Managers.UI.ShowPopupUI<UI_Warning>();
                test = BossCountState.Warning;
            }
            else if (CurrentTime >= 9f && test == BossCountState.Warning)
            {
                test = BossCountState.Barricade;
                Managers.Object.Spawn<Monster>(new Vector3(0f, -1f, 0f), Define.MONSTER_SECURITY1_ID);
                Debug.Log("바리게이트 생성");
            }
            else if(CurrentTime >= 10f && test == BossCountState.Barricade) 
            {
                Managers.Object.Spawn<Monster>(new Vector3(0f, 0f, 0f), Define.MONSTER_SECURITY2_ID);
                Debug.Log("보스 생성");
                break;
            }

            yield return new WaitForFixedUpdate();
            
        }
    }
    #endregion
}
