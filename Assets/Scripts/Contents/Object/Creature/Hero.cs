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

	// jh
    public Vector2 MoveDir
    {
        get { return _moveDir; }
    }
	//

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

    private bool isInvincible = false;
    public bool IsInvincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }

    public bool isSpeedBoosted = false;


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

		SetRigidbodyVelocity(_moveDir * MoveSpeed);

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
		MaxHp = heroLevelData.MaxHp;
		Hp = heroLevelData.MaxHp;
		MaxExp = heroLevelData.Exp;
		MoveSpeed = (heroLevelData.MoveSpeed / 100.0f) * Define.DEFAULT_SPEED;
		ItemAcquireRange = heroLevelData.ItemAcquireRange;
		ResistDisorder = heroLevelData.ResistDisorder;

		Managers.Game.OnLevelUp?.Invoke();
	}

    /// jh 부스터 발판 밟았을 시 속도 변화
    public IEnumerator SpeedBoost(float targetDistance, float multiplier)
    //public IEnumerator SpeedBoost(float duration, float multiplier)
    {
        float originalSpeed = MoveSpeed; 
        MoveSpeed *= multiplier; // 속도 증가

        isInvincible = true; // 무적 상태 설정
        isSpeedBoosted = true;

        Vector3 startPosition = transform.position;
        float movedDistance = 0;

        while (movedDistance < targetDistance) // 이동 거리가 목표 거리에 도달하면
        {
            yield return null; 
            movedDistance = Vector3.Distance(startPosition, transform.position);
        }

        // 서서히 속도를 원래대로 돌려놓음
        float duration = 1f; // 일단 테스트용
        float elapsed = 0;
        while (elapsed < duration)
        {
            MoveSpeed = Mathf.Lerp(MoveSpeed, originalSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        MoveSpeed = originalSpeed; 
        isInvincible = false;
        isSpeedBoosted = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (isSpeedBoosted && ((1 << (int)Define.ELayer.Monster) & (1 << collider.gameObject.layer)) != 0)
        {
            Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // 플레이어로부터 몬스터까지의 방향을 계산
                Vector2 blowDirection = (enemyRb.transform.position - transform.position).normalized;

                // 몬스터를 뒤로 밀어내고 약간 위로 향하게...
                Vector2 force = blowDirection * 10f + Vector2.up * 5f;
                enemyRb.AddForce(force, ForceMode2D.Impulse);

                // 몬스터가 회전하도록
                enemyRb.AddTorque(10f, ForceMode2D.Impulse);

                Destroy(collider.gameObject, 2f); // 2초 후 소멸
            }
        }
    }
    #endregion
}