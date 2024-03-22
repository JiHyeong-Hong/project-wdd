using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;
using static Unity.Collections.AllocatorManager;

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
		ItemAcquireRange = heroData.ItemAcquireRange;
		ResistDisorder = heroData.ResistDisorder;

		// foreach (int skillID in hereData.SkillIdList)
		// 	AddSkill(skillID);

		Managers.Game.RefreshUI();
	}

	void Update()
	{
		if (IsValid(this) == false)
			return;

		SetRigidbodyVelocity(_moveDir * 10);
		// SetRigidbodyVelocity(_moveDir * MoveSpeed);

		// 테스트 용
		if (Input.GetKeyDown(KeyCode.S))
		{
			for (int i = 0; i < 5; ++i)
				Managers.Object.Spawn<Monster>(new Vector3(-2f + i, -1f, 0f), Define.MONSTER_SECURITY1_ID);

			for (int i = 0; i < 5; ++i)
				Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 0f, 0f), Define.MONSTER_SECURITY2_ID);

			for (int i = 0; i < 5; ++i)
				Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 1f, 0f), Define.MONSTER_SECURITY3_ID);
		}
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
		MaxHp = heroLevelData.MaxHp;
		Hp = heroLevelData.MaxHp;
		MaxExp = heroLevelData.Exp;
		MoveSpeed = (heroLevelData.MoveSpeed / 100.0f) * Define.DEFAULT_SPEED;
		ItemAcquireRange = heroLevelData.ItemAcquireRange;
		ResistDisorder = heroLevelData.ResistDisorder;

		Managers.Game.OnLevelUp?.Invoke();
	}

    #endregion
}