using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

public class Hero : Creature
{
	private Vector2 _moveDir = Vector2.zero;

    #region Stat

	public int Level { get; set; }
	public int MaxExp { get; set; }
	public float ItemAcquireRange { get; set; }
	public float ResistDisorder { get; set; }

	private float _exp = 0;
	public float Exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
			if (_exp >= MaxExp)
				LevelUp();
			Managers.Game.RefreshUI();
		}
	}

    private bool isInvincible = false;	//jh

    #endregion

    public Transform Pivot { get; private set; }
	public Transform Destination { get; private set; }


	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		CreatureType = ECreatureType.Hero;

		Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
		Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
		Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
		Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

		Pivot = Util.FindChild<Transform>(gameObject, "Pivot", true);
		Destination = Util.FindChild<Transform>(gameObject, "Destination", true);

		return true;
	}

	public override void SetInfo(int templateID)
	{
		base.SetInfo(templateID);

		CreatureState = ECreatureState.Idle;
		Renderer.sortingOrder = SortingLayers.HERO;

		HeroData heroData = CreatureData as HeroData;

		Level = heroData.Level;
		MaxExp = heroData.MaxExp;
		Exp = 0;
		MaxHp = heroData.MaxHp + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Hp);
		Hp = heroData.MaxHp + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Hp);
		ItemAcquireRange = heroData.ItemAcquireRange + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Farming);
		ResistDisorder = heroData.ResistDisorder + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.DamageCare);

		// foreach (int skillID in hereData.SkillIdList)
		// 	AddSkill(skillID);

		Managers.Game.RefreshUI();
	}

	void Update()
	{
		if (IsValid(this) == false)
			return;

		SetRigidbodyVelocity(_moveDir * MoveSpeed);

		// 테스트 용
		if (Input.GetKeyDown(KeyCode.S))
		{
			//몬스터 출현 갯수 수정
			// for (int i = 0; i < 1; ++i)
			// 	Managers.Object.Spawn<Monster>(new Vector3(-2f + i, -1f, 0f), Define.MONSTER_SECURITY1_ID);

			// for (int i = 0; i < 1; ++i)
			// 	Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 0f, 0f), Define.MONSTER_SECURITY2_ID);

			// for (int i = 0; i < 1; ++i)
			// 	Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 1f, 0f), Define.MONSTER_SECURITY3_ID);
		}
		
		// TODO Eung 스포너 생성 테스트 코드
		// if (Input.GetKeyDown(KeyCode.Alpha6))
		// {
		// 	Debug.Log("Test");
		// 	int ran = Random.Range(0, Managers.Spawner.spawner_List.Count);
		// 	
		// 	Managers.Spawner.spawner_List[ran].Spawn(415);
		// }
	}
	private void HandleOnMoveDirChanged(Vector2 dir)
	{
		_moveDir = dir;

		if (dir != Vector2.zero)
		{
			Direction = dir;

			float angle = Mathf.Atan2(-dir.x, +dir.y) * 180 / Mathf.PI;
			Pivot.eulerAngles = new Vector3(0, 0, angle);
		}
	}

	private void HandleOnJoystickStateChanged(EJoystickState joystickState)
	{
		switch (joystickState)
		{
			case Define.EJoystickState.PointerDown:
				break;
			case Define.EJoystickState.Drag:
				CreatureState = Define.ECreatureState.Move;
				break;
			case Define.EJoystickState.PointerUp:
				CreatureState = Define.ECreatureState.Idle;
				break;
			default:
				break;
		}
	}

    #region Battle

	public override void OnDamaged(BaseObject attacker, SkillBase skill)
	{
        if (isInvincible)
            return; // 무적 상태일 때는 아무런 처리를 하지 않음

        base.OnDamaged(attacker, skill);	

		Managers.Game.RefreshUI();
	}

	public override void OnDead(BaseObject attacker, SkillBase skill)
	{
		base.OnDead(attacker, skill);

		SetRigidbodyVelocity(Vector2.zero);
		Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
		Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;

	}

	private void LevelUp()
	{
		if (Level >= Define.MAX_LEVEL)
			return;

		Level += 1;
		Data.HeroLevelData heroLevelData = Managers.Data.HeroLevelDic[DataID + Level];
		
		Exp = 0;
		MaxHp = heroLevelData.MaxHp + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Hp);
		Hp = heroLevelData.MaxHp + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Hp);
		MaxExp = heroLevelData.Exp;
		MoveSpeed = ((heroLevelData.MoveSpeed + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.MoveSpeed)) / 100.0f) * Define.DEFAULT_SPEED;
		ItemAcquireRange = heroLevelData.ItemAcquireRange + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.Farming);
		ResistDisorder = heroLevelData.ResistDisorder + PassiveHelper.Instance.GetPassiveValue(PassiveSkillStatusType.DamageCare);

		Managers.Game.OnLevelUp?.Invoke();
	}

    /// jh 부스터 발판 밟았을 시 속도 변화
    public IEnumerator SpeedBoost(float duration, float multiplier)
    {
        float originalSpeed = MoveSpeed; 
        MoveSpeed *= multiplier; // 속도 증가

        isInvincible = true; // 무적 상태 설정

        // duration 시간만큼 대기
        yield return new WaitForSeconds(duration);

        // 서서히 속도를 원래대로 돌려놓음
        float elapsed = 0;
        while (elapsed < duration)
        {
            MoveSpeed = Mathf.Lerp(MoveSpeed, originalSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        MoveSpeed = originalSpeed; // 부스트 끝나면 다시 원래 속도로 설정
    }

    #endregion
}