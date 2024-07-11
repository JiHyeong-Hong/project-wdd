using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    public Dictionary<int, Data.MonsterData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.HeroData> HeroDic { get; private set; } = new Dictionary<int, Data.HeroData>();
    public Dictionary<int, Data.HeroLevelData> HeroLevelDic { get; private set; } = new Dictionary<int, Data.HeroLevelData>();
    public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.ProjectileData> ProjectileDic { get; private set; } = new Dictionary<int, Data.ProjectileData>();
    public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.DropItemData> DropItemDic { get; private set; } = new Dictionary<int, Data.DropItemData>();
    public Dictionary<int, Data.HpConditionData> HpConditionDic { get; private set; } = new Dictionary<int, Data.HpConditionData>();
    public Dictionary<int, Data.PatternPerData> PatternPerDic { get; private set; } = new Dictionary<int, Data.PatternPerData>();
    public Dictionary<int ,Data.BreakthroughData> BreakthroughDic { get; private set; } = new Dictionary<int, Data.BreakthroughData>();
    //Test
    public Dictionary<int, Data.Stage> StageDic { get; private set; } = new Dictionary<int, Data.Stage>();

    public Dictionary<int, Data.StageLevel> StageLvDic { get; private set; } = new Dictionary<int, Data.StageLevel>();
    public Dictionary<int, Data.Spawn> SpawnDic { get; private set; } = new Dictionary<int, Data.Spawn>();

    protected override void Init()
    {
        base.Init();
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        HeroLevelDic = LoadJson<Data.HeroLevelDataLoader, int, Data.HeroLevelData>("HeroLevelData").MakeDict();
        SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        ProjectileDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
        ItemDic = LoadJson<Data.ItemDataLoader, int, Data.ItemData>("ItemData").MakeDict();
        DropItemDic = LoadJson<Data.DropItemDataLoader, int, Data.DropItemData>("DropItemData").MakeDict();
        HpConditionDic = LoadJson<Data.HpConditionDataLoader, int, Data.HpConditionData>("HpConditionData").MakeDict();
        PatternPerDic = LoadJson<Data.PatternPerDataLoader, int, Data.PatternPerData>("PatternPerData").MakeDict();
        BreakthroughDic = LoadJson<Data.BreakthroughDataLoader, int, Data.BreakthroughData>("BreakthroughData").MakeDict();
        //StageDataDic = LoadJson<Data.StageDataLoader, int, Data.StageData>("StageData").MakeDict();

        StageDic = LoadJson<Data.StageLoader, int, Data.Stage>("StageData").MakeDict();
        StageLvDic = LoadJson<Data.StageLevelLoader, int, Data.StageLevel>("StageLvData").MakeDict();
        SpawnDic = LoadJson<Data.SpawnLoader, int, Data.Spawn>("SpawnData").MakeDict();

    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>($"Data/JsonData/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
