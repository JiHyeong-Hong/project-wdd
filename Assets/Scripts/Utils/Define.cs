using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
	public enum EScene
	{
		Unknown,
		TitleScene,
		GameScene,
	}

	public enum EUIEvent
	{
		Click,
		PointerDown,
		PointerUp,
		Drag,
	}

	public enum EJoystickState
	{
		PointerDown,
		PointerUp,
		Drag,
	}

	public enum ESound
	{
		Bgm,
		Effect,
		Max,
	}

	public enum EObjectType
	{
		None,
		Creature,
		Projectile,
		Env,
		Item,
	}

	public enum ECreatureType
	{
		None,
		Hero,
		Monster,
		Npc,
	}

	public enum ECreatureState
	{
		None,
		Idle = 1,
		Move = 2,
		Attack = 3,
		Hit = 4,
		Dead = 5,
	}

	public enum ELayer
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Dummy1 = 3,
		Water = 4,
		UI = 5,
		Hero = 6,
		Monster = 7,
		Env = 8,
		Obstacle = 9,
		Projectile = 10,
	}

	public enum SkillType
	{
		Active = 1,
		Passive = 2,
		Breakthrogh = 3
	}

	public enum PassiveSkillStatusType
	{
		CoolTimeDown = 1,
		Farming = 2,
		Exp = 3,
		Gold = 4,
		Duration = 5,
		Recovery = 6,
		Hp = 7,
		MoveSpeed = 8,
		Attack = 9,
		AttackSpeed = 10,
		AttackRange = 11,
		DamageCare = 12,
		CastPer = 13,
    }


    public const float DEFAULT_SPEED = 3;
	public const int MAX_SKILL_LEVEL = 5;
	public const int MAX_LEVEL = 60;

	public const int HERO_ZOOKEEPER_ID = 100;

	public const int MONSTER_SECURITY1_ID = 411;
	public const int MONSTER_SECURITY2_ID = 412;
	public const int MONSTER_SECURITY3_ID = 413;
}

public static class SortingLayers
{
	public const int ITEM = 50;
	public const int ENV = 100;
	public const int MONSTER = 100;
	public const int HERO = 100;
	public const int PROJECTILE = 200;
}