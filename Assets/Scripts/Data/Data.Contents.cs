using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    #region CreatureData

	[Serializable]
	public class CreatureData
	{
		public int Index;
		public string DescriptionTextID;
		public int MaxHp;
		public int Atk;
		public float MoveSpeed;
		public float ResistDisorder;
		public string AnimatorDataID;
	}

    #endregion

    #region MonsterData

	[Serializable]
	public class MonsterData
    {
        /* 기존 테이블 데이터
		//TODO 몬스터의 드랍 테이블에서 사용햇던 데이터 - 드랍 아이템 관련 이야기를 해봐야함
		// public int DropItemID;			*****
		// public int DropPersent;			*****
		// public float AtkRange;
		// //TODO Eung 몬스터 타입 - 테이블에 따라 변경필요
		// public int type;
		// //TODO Eung 공격 타입 - 테이블에 따라 변경필요
		// public int Atktype;
		*/

		public int MonsterID;
		public string Name;
		public int Type;
		public int AttackType;
		public int HP;
		public int MoveSpeed;
		public int ContactDmg;
		public int CoolTime;
		public int ProjectileID;
		public int ProjectileNum;
		public int CastAngle;
		public int MinStance;
		public int MaxStane;
		public int KnockbackPower;
		public int ConditionType;
		public int SkillLvDown;
		public int ConditionHitType;
		public int KnockbackPowerHitType;


    }


    //TODO Eung 몬스터를 그냥 하나의 객체로 만들때 사용할수도? 아니먄 그냥 기존 구조로 공통 변수 묶어서 CreatureData + MonsterData로 가는 경우 필요없어짐
    [Serializable]
	public class MonsterData2
	{
		public int Type;
		public int AttackType;
		public int CoolTime;
		public int ProjectileID;
		public int ProjectileNum;
		public int CastAngle;
		public int MinStance;
		public int MaxStane;
		public int KnockbackPower;
		public int ConditionType;
		public int SkillLvDown;
		public int ConditionHitType;
		public int KnockbackPowerHitType;
	}

	[Serializable]
	public class MonsterDataLoader : ILoader<int, MonsterData>
	{
		public List<MonsterData> monsters = new List<MonsterData>();
		public Dictionary<int, MonsterData> MakeDict()
		{
			Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
			foreach (MonsterData monster in monsters)
				dict.Add(monster.MonsterID, monster);
			return dict;
		}
	}
	
	//TODO Eung MonsterDataLoader2 테스트 코드 필요없어지는 경우 삭제할것
	// public class MonsterDataLoader2 : ILoader<int, MonsterData2>
	// {
	// 	public List<MonsterData2> monsters = new List<MonsterData2>();
	// 	public Dictionary<int, MonsterData2> MakeDict()
	// 	{
	// 		Dictionary<int, MonsterData2> dict = new Dictionary<int, MonsterData2>();
	// 		foreach (MonsterData2 monster in monsters)
	// 			dict.Add(monster.Index, monster);
	// 		return dict;
	// 	}
	// }
    #endregion

    #region HeroData

	[Serializable]
	public class HeroData : CreatureData
	{
		public int Level;
		public int MaxExp;
		public float ItemAcquireRange;
		public List<int> SkillIdList = new List<int>();
	}

	[Serializable]
	public class HeroDataLoader : ILoader<int, HeroData>
	{
		public List<HeroData> heroes = new List<HeroData>();
		public Dictionary<int, HeroData> MakeDict()
		{
			Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
			foreach (HeroData hero in heroes)
				dict.Add(hero.Index, hero);
			return dict;
		}
	}

    #endregion

    #region HeroLevelData

	[Serializable]
	public class HeroLevelData
	{
		public int DataId;
		public int Level;
		public int Exp;
		public float MoveSpeed;
		public int MaxHp;
		public float ItemAcquireRange;
		public int ResistDisorder;
	}

	[Serializable]
	public class HeroLevelDataLoader : ILoader<int, HeroLevelData>
	{
		public List<HeroLevelData> levels = new List<HeroLevelData>();

		public Dictionary<int, HeroLevelData> MakeDict()
		{
			Dictionary<int, HeroLevelData> dict = new Dictionary<int, HeroLevelData>();
			foreach (HeroLevelData level in levels)
				dict.Add(level.DataId, level);
			return dict;
		}
	}

	#endregion

	#region SkillData

	[Serializable]
	public class SkillData
	{
		// Common
		public int Index;
		public string Name;
		public int AttackPattern;
		public Define.SkillType skillType;
		public int Level;

        //ActtiveSkill_Stat
        public int ProjectileNum;
		public float CastAngle;
		public float Damage;
		public float AttackSpeed;
		public float AttackRange;
		public float KnockbackPower;
		public float StunTime;
        public float ConditionRange;
        public int CastCount;
		public float Duration;
		public int SkillTurn; // 일반스킬 몇번 나가야지 발동 가는한지
		public float CoolTime;
		public float CastPer;

        //PassiveSkill_Stat
        public int StatType;
		public float StatValue;
		public int SkillGetType;
		public int SkillMaxLv;
	}

	[Serializable]
	public class SkillDataLoader : ILoader<int, SkillData>
	{
		public List<SkillData> skills = new List<SkillData>();

		public Dictionary<int, SkillData> MakeDict()
		{
			Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
			foreach (SkillData skill in skills)
				dict.Add(skill.Index, skill);
			return dict;
		}
	}

    #endregion

    #region ProjectileData

	[Serializable]
	public class ProjectileData
	{
		public int Index;
		public int ProjectileId;
		public string Name;
		public int MoveSpeed;
		public int ContactDmg;
		public int ConditionType;
		public int SkillLvDown;
		public int SkillID;
		public string ImageDataurl;
	}

	[Serializable]
	public class ProjectileDataLoader : ILoader<int, ProjectileData>
	{
		public List<ProjectileData> projectiles = new List<ProjectileData>();

		public Dictionary<int, ProjectileData> MakeDict()
		{
			Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
			foreach (ProjectileData projectile in projectiles)
				dict.Add(projectile.ProjectileId, projectile);
			return dict;
		}
	}

    #endregion

    #region ItemData

	[Serializable]
	public class ItemData
	{
		public int ItemId;
		public string Name;
		public int Type;
		public int Value;
		public string IconPath;
	}

	[Serializable]
	public class ItemDataLoader : ILoader<int, ItemData>
	{
		public List<ItemData> items = new List<ItemData>();

		public Dictionary<int, ItemData> MakeDict()
		{
			Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
			foreach (ItemData item in items)
				dict.Add(item.ItemId, item);
			return dict;
		}
	}

    #endregion
    
    #region DropData
    [Serializable]
    public class DropItemData
    {
	    public int DropItemID;
	    public int MonsterID;
	    public int ItemID;
	    public int DropPer;
    }

    [Serializable]
    public class DropItemDataLoader : ILoader<int, DropItemData>
    {
	    public List<DropItemData> drops = new List<DropItemData>();

	    public Dictionary<int, DropItemData> MakeDict()
	    {
		    Dictionary<int, DropItemData> dict = new Dictionary<int, DropItemData>();
		    foreach (DropItemData dropitem in drops)
			    dict.Add(dropitem.DropItemID, dropitem);
		    return dict;
	    }
    }
    
    #endregion
    
    #region Boss_Patturn

    [Serializable]
    public class HpConditionData
    {
	    public int Index;
	    public int MonsterID;
	    public int Phase1Hp;
	    public int Phase2Hp;
	    public int Phase3Hp;
	    public int Phase4Hp;
    }

    [Serializable]
    public class HpConditionDataLoader : ILoader<int, HpConditionData>
    {
	    public List<HpConditionData> datas = new List<HpConditionData>();

	    public Dictionary<int, HpConditionData> MakeDict()
	    {
		    Dictionary<int, HpConditionData> dict = new Dictionary<int, HpConditionData>();
		    foreach (HpConditionData item in datas)
			    dict.Add(item.Index, item);
		    return dict;
	    }
    }
    

    #endregion
    
    #region Boss_Patturn_Per

    [Serializable]
    public class PatternPerData
    {
	    public int Index;
	    public int MonsterID;
	    public int PhaseNum;
	    public int Pattern1;
	    public int Pattern2;
	    public int Pattern3;
    }

    [Serializable]
    public class PatternPerDataLoader : ILoader<int, PatternPerData>
    {
	    public List<PatternPerData> datas = new List<PatternPerData>();

	    public Dictionary<int, PatternPerData> MakeDict()
	    {
		    Dictionary<int, PatternPerData> dict = new Dictionary<int, PatternPerData>();
		    foreach (PatternPerData item in datas)
			    dict.Add(item.Index, item);
		    return dict;
	    }
    }

    #endregion

    #region BreakthroughData
    [Serializable]
    public class BreakthroughData
	{
		public int Index;
		public int SkillID;
		public string Name;
		public int Type;
		public int Lv;
		public int C_Skill_ID1;
		public int C_Skill_ID2;
		public int G_Skill_ID1;
		public int Skill_Lv;
		public int G_Skill_ID2;
		public int Skill_Lv2;
    }
    [Serializable]
    public class BreakthroughDataLoader : ILoader<int, BreakthroughData>
    {
        public List<BreakthroughData> breakthroughData = new List<BreakthroughData>();

        public Dictionary<int, BreakthroughData> MakeDict()
        {
            Dictionary<int, BreakthroughData> dict = new Dictionary<int, BreakthroughData>();
            foreach (BreakthroughData item in breakthroughData)
                dict.Add(item.C_Skill_ID1, item);
            return dict;
        }
    }

    #endregion

    #region Stage


    //public string IconURL;
    //----------------------------------------
    [Serializable]
    public class Stage
    {
		public int StageID;
		public string Name;
        public int Lv;
        public string Info;
        public bool Locked;
	}

	[Serializable]
    public class StageLoader : ILoader<int, Stage>
    {
		public List<Stage> stages = new List<Stage>();
        public Dictionary<int, Stage> MakeDict()
        {
            Dictionary<int, Stage> dict = new Dictionary<int, Stage>();
            foreach (Stage stage in stages)
                dict.Add(stage.StageID, stage);
            return dict;
        }
    }

    [Serializable]
    public class StageLevel
    {
		public int StageLvID;
		public int StageID;
		public float Phase1Time;
		public float Phase2Time;
		public float Phase3Time;
		public float Phase4Time;
    }

    [Serializable]
    public class StageLevelLoader : ILoader<int, StageLevel>
	{
		public List<StageLevel> stageLevels = new List<StageLevel>();
        public Dictionary<int, StageLevel> MakeDict()
        {
            Dictionary<int, StageLevel> dict = new Dictionary<int, StageLevel>();
            foreach (StageLevel stageLevel in stageLevels)
                dict.Add(stageLevel.StageLvID, stageLevel);
            return dict;
        }
    }

	[Serializable]
    public class Spawn
    {
		public int SpawnID;
		public int StageID;
		public int MonsterID;
		public int Phase;
		public float CycleTime;
		public int Count;
		public float RiseRate;
    }

    [Serializable]
    public class SpawnLoader : ILoader<int, Spawn>
    {
        public List<Spawn> spawns = new List<Spawn>();

        public Dictionary<int, Spawn> MakeDict()
        {
            Dictionary<int, Spawn> dict = new Dictionary<int, Spawn>();
            foreach (Spawn spawn in spawns)
                dict.Add(spawn.SpawnID, spawn);
            return dict;
        }
    }



    #endregion
}