using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PhaseInfo
{
    public int Phase;
    public float Duration;
    public List<Spawn> Spawns;

    public PhaseInfo(int phase, float duration)
    {
        Phase = phase;
        Duration = duration;
        Spawns = new List<Spawn>();
    }
}

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    private int currentStageID;
    private Stage currentStage;

    private List<PhaseInfo> phases = new List<PhaseInfo>();
    private int currentPhaseIndex = 0;
    private float phaseTimer = 0;
    private int maxPhase = 0;
    public EStageState state;
    private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();

    private List<Coroutine> coroutines = new List<Coroutine>();

    public void LoadStage(int stageID)
    {
        if (!DataManager.Instance.StageDic.ContainsKey(stageID))
        {
            Debug.LogError($"Stage ID {stageID} not found.");
            return;
        }

        currentStageID = stageID;
        currentStage = DataManager.Instance.StageDic[stageID];
        phases = GetPhasesForStage(stageID);

        Debug.Log($"Loading Stage: {currentStage.Name}, Level: {currentStage.Lv}");

        StartStage();
    }

    private List<PhaseInfo> GetPhasesForStage(int stageID)
    {
        List<PhaseInfo> phaseList = new List<PhaseInfo>();

        StageLevel stageLevel = GetStageLevel(stageID);
        if (stageLevel != null)
        {
            for (int i = 1; i <= stageLevel.MaxPhase; i++)
            {
                phaseList.Add(new PhaseInfo(i, stageLevel.Cycle));
            }
        }

        maxPhase = stageLevel.MaxPhase;


        // {
        // phaseList.Add(new PhaseInfo(1, stageLevel.Phase1Time));
        // phaseList.Add(new PhaseInfo(2, stageLevel.Phase2Time));
        // phaseList.Add(new PhaseInfo(3, stageLevel.Phase3Time));
        // phaseList.Add(new PhaseInfo(4, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(5, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(6, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(7, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(8, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(9, stageLevel.Phase4Time));
        // phaseList.Add(new PhaseInfo(10, stageLevel.Phase4Time));
        // }

        List<Spawn> spawns = GetSpawnsForStage(stageID);
        foreach (var spawn in spawns)
        {
            phaseList[spawn.Phase - 1].Spawns.Add(spawn);
        }

        return phaseList;
    }

    private StageLevel GetStageLevel(int stageID)
    {
        foreach (var kvp in DataManager.Instance.StageLvDic)
        {
            if (kvp.Value.StageID == stageID)
                return kvp.Value;
        }

        return null;
    }

    private List<Spawn> GetSpawnsForStage(int stageID)
    {
        List<Spawn> spawns = new List<Spawn>();
        foreach (var kvp in DataManager.Instance.SpawnDic)
        {
            if (kvp.Value.StageID == stageID)
                spawns.Add(kvp.Value);
        }

        return spawns;
    }

    public void StartStage()
    {
        if (currentStage == null)
        {
            Debug.LogError("No stage loaded.");
            return;
        }

        Debug.Log($"Starting Stage: {currentStage.Name}, Level: {currentStage.Lv}");
        StartCoroutine(StageRoutine()); // DEBUG::
        Managers.Game.isStartGame = true;
        state = EStageState.Nomal;        
    }


    private IEnumerator StageRoutine()
    {
        PhaseInfo phase ;
        
        // currentPhaseIndex = maxPhase-1; // 마지막 페이즈는 보스 스테이지 테스트 코드
        currentPhaseIndex = 0;
        int phaseCount = phases.Count-2; // 마지막 페이즈는 보스 스테이지
        while(currentPhaseIndex < phaseCount) 
        {
            phaseTimer = 0;
            phase = phases[currentPhaseIndex];
            Debug.Log($"<color=red>Starting Phase {phase.Phase} for {phase.Duration} seconds \n phasesCount:{phases.Count} </color>");
            SpawnMonsters("SpawnMonsterCoroutine",phase.Spawns);
            yield return new WaitForSeconds(phase.Duration);
            currentPhaseIndex++;
        }

        phaseTimer = 0;
        phase = phases[maxPhase-1];
        SpawnMonsters("StartBossStage",phase.Spawns);
        yield return new WaitForSeconds(phase.Duration);

        
        // {
        //     // PhaseInfo phase = phases[currentPhaseIndex];
        //     phaseTimer = 0;

        //     if (phase.Phase < maxPhase)
        //     {
        //         Debug.Log($"<color=red>Starting Phase {phase.Phase} for {phase.Duration} seconds \n phasesCount:{phases.Count} </color>");
        //         foreach (var spawn in phase.Spawns)
        //         {
        //             Coroutine coroutine = StartCoroutine(SpawnMonsterCoroutine(spawn));
        //             if (coroutine != null)
        //             {
        //                 //activeCoroutines.Add(coroutine);
        //                 coroutines.Add(coroutine);
        //             }
        //             else
        //             {
        //                 // Debug.LogError($"Failed to start coroutine for spawn ID {spawn.SpawnID}");
        //             }
        //         }

        //         while (phaseTimer < phase.Duration)
        //         {
        //             phaseTimer += Time.deltaTime;
        //             yield return null;
        //         }
        //     }

        // }

        // while (currentPhaseIndex < phases.Count)
        // {
        //     // Debug.Log(currentPhaseIndex);
        //     PhaseInfo phase = phases[currentPhaseIndex];
        //     phaseTimer = 0;

        //     if (phase.Phase < maxPhase)
        //     {
        //         Debug.Log($"<color=red>Starting Phase {phase.Phase} for {phase.Duration} seconds \n phasesCount:{phases.Count} </color>");
        //         foreach (var spawn in phase.Spawns)
        //         {
        //             Coroutine coroutine = StartCoroutine(SpawnMonsterCoroutine(spawn));
        //             if (coroutine != null)
        //             {
        //                 //activeCoroutines.Add(coroutine);
        //                 coroutines.Add(coroutine);
        //             }
        //             else
        //             {
        //                 // Debug.LogError($"Failed to start coroutine for spawn ID {spawn.SpawnID}");
        //             }
        //         }

        //         while (phaseTimer < phase.Duration)
        //         {
        //             phaseTimer += Time.deltaTime;
        //             yield return null;
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log($"<color=red>Last Phase {phase.Phase} is Boss Stage </color>");
        //         Coroutine coroutine = StartCoroutine(StartBossStage(phase.Spawns[0]));
        //         if (coroutine != null)
        //         {
        //             //activeCoroutines.Add(coroutine);
        //             coroutines.Add(coroutine);
        //             yield return null;
        //         }

        //     }
            
        //     PhaseCoroutineRelease();
        // }

        PhaseCoroutineRelease();
        EndStage();
    }

    private void SpawnMonsters(string coroutineName,List<Spawn> spawns)
    {
        PhaseCoroutineRelease();
        foreach (var spawn in spawns)
        {
            Coroutine coroutine = StartCoroutine(coroutineName,spawn);
            if (coroutine != null)
            {
                coroutines.Add(coroutine);
            }
        }
    }

    private void PhaseCoroutineRelease()
    {
        // currentPhaseIndex++;
        // phaseTimer = 0;
        // Debug.Log($"Transitioning to Phase {currentPhaseIndex + 1}");

        // if (currentPhaseIndex < phases.Count)
        // {
            foreach (var coroutine in coroutines)
            {
                StopCoroutine(coroutine);
            }
            coroutines.Clear();
        // }
    }

    private IEnumerator SpawnMonsterCoroutine(Spawn spawn)
    {
        while (true)
        {
            // Debug.Log($"Spawning {spawn.Count} of MonsterID {spawn.MonsterID} at Phase {currentPhaseIndex + 1} spawn.CycleTime : {spawn.CycleTime}");
            for (int i = 0; i < spawn.Count; i++)
            {
                // 몬스터를 지정된 수만큼 소환
                Managers.Spawner.SpawnNew<Monster>(spawn.MonsterID);
            }

            if (spawn.CycleTime == 0)
            {
                break;
            }

            yield return YieldInstructionCache.WaitForSeconds(spawn.CycleTime);
        }
    }

    public IEnumerator StartBossStage(Spawn spawn)
    {
        Structure Barricate = null;
        while (true)
        {
            if (state == EStageState.Nomal)
            {
                state = EStageState.Warning;
                // Managers.UI.ShowPopupUI<UI_Warning>();
                SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Warning);
                UIManagerNew.Instance.ShowPopup<WarningPopup>(false);
            }
            else if (state == EStageState.Warning)
            {
                foreach (var monster in Managers.Object.Monsters)
                {
                    monster.StartFadeOut();
                }

                state = EStageState.Barricade;
                //TODO Eung 바리게이트 오브젝트만들어서 생성하면 될듯 - 바리게이트 Spawn으로 바꾸면 될듯
                Barricate = Managers.Object.Spawn<Structure>(Managers.Object.Hero.transform.position, 1);
            }
            else if (state == EStageState.Barricade)
            {
                if(Barricate == null)  // 박현규
                {
                    Barricate = Managers.Object.Spawn<Structure>(Managers.Object.Hero.transform.position, 1);
                }
                SoundManager.Instance.Play(Define.ESoundMainType.Bgm, Define.ESoundType.Boss);
                //TODO Eung StageLv 테이블을 만들어서 스테이지별 등장 보스몬스터 넘버를 받아와서 대입하면 될듯 
                Debug.Log("보스 생성 " + (spawn != null) + "/" + (Managers.Object != null) + "/" + (Barricate != null));
                Managers.Object.Spawn<Boss>(Barricate.transform.position + Vector3.up*3, spawn.MonsterID);
                state = EStageState.Boss;
                break;
            }

            yield return YieldInstructionCache.WaitForSeconds(2f);
        }
    }

    public void EndStage()
    {
        // Debug.Log("Stage Ended.");
        // Managers.Game.isStartGame = false;
        // // 보스가 등장할 때 카메라 줌 아웃
        // // 보스가 사라질 때 카메라 줌 인
        // // 스테이지 클리어

        // if (currentPhaseIndex == maxPhase)
        // {
        //     StartCoroutine(Managers.Game.BossCount());
        // }
    }

    public void ResetDatas()
    {
        StopAllCoroutines();
        //StopCoroutine(StageRoutine());
        currentStageID = -1;
        currentStage = null;
        phases = new List<PhaseInfo>();
        currentPhaseIndex = 0;
        phaseTimer = 0;
        maxPhase = 0;
        state = EStageState.None;
        activeCoroutines = new Dictionary<int, Coroutine>();
    }


    //Eung 보스 일때 카메라 줌 아웃 기능 테스트
    // public IEnumerator Test()
    // {
    //     Camera camera = Camera.main;
    //     while (camera.orthographicSize < 15)
    //     {
    //
    //         camera.orthographicSize += 0.01f;
    //         
    //         yield return YieldInstructionCache.WaitForFixedUpdate;
    //     }
    //     
    //     
    // }
}