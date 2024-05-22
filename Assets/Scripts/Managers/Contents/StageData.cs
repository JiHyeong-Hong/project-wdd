using System.Collections.Generic;

public class StageData
{
    public int StageID;
    public string Name;
    public int Lv;
    public string info;
    public bool Locked;
    public string IconURL;
}

public class StageLvData
{
    public int StageLvID;
    public int StageID;
    public List<int> PhaseTime = new List<int>() { 0, };
}

public class InGameData
{
    public int InGameID;
    public int StageID;
    public int PlayTime;
    public int Phase;
    public int GoldAmount;
    public int AnimalCount;
}

public class SpawnData
{
    public int SpawnID;
    public int StageID;
    public int MonsterID;
    public int Phase;
    public int CycleTime;
    public int Count;
    public int RiseRate;
}
