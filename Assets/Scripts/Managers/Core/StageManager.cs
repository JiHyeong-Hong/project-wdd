using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            phaseList.Add(new PhaseInfo(1, stageLevel.Phase1Time));
            phaseList.Add(new PhaseInfo(2, stageLevel.Phase2Time));
            phaseList.Add(new PhaseInfo(3, stageLevel.Phase3Time));
            phaseList.Add(new PhaseInfo(4, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(5, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(6, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(7, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(8, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(9, stageLevel.Phase4Time));
            phaseList.Add(new PhaseInfo(10, stageLevel.Phase4Time));
        }

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
        //StartCoroutine(StageRoutine());
        Managers.Game.isStartGame = true;
    }


    private IEnumerator StageRoutine()
    {
        while (currentPhaseIndex < phases.Count)
        {
            PhaseInfo phase = phases[currentPhaseIndex];
            phaseTimer = 0;

            Debug.Log($"<color=red>Starting Phase {phase.Phase} for {phase.Duration} seconds \n phasesCount:{phases.Count} </color>");

            foreach (var spawn in phase.Spawns)
            {
                Coroutine coroutine = StartCoroutine(SpawnMonsterCoroutine(spawn));
                if (coroutine != null)
                {
                    //activeCoroutines.Add(coroutine);
                    coroutines.Add(coroutine);

                }
                else
                {
                    Debug.LogError($"Failed to start coroutine for spawn ID {spawn.SpawnID}");
                }
            }

            while (phaseTimer < phase.Duration)
            {
                phaseTimer += Time.deltaTime;
                yield return null;
            }

            TransitionToNextPhase();
        }

        EndStage();
    }

    private void TransitionToNextPhase()
    {
        currentPhaseIndex++;
        phaseTimer = 0;
        Debug.Log($"Transitioning to Phase {currentPhaseIndex + 1}");

        foreach (var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
        coroutines.Clear();
    }

    private IEnumerator SpawnMonsterCoroutine(Spawn spawn)
    {
        while (true)
        {
            Debug.Log($"Spawning {spawn.Count} of MonsterID {spawn.MonsterID} at Phase {currentPhaseIndex + 1} spawn.CycleTime : {spawn.CycleTime}");
            for (int i = 0; i < spawn.Count; i++)
            {
                // 실제 몬스터 생성 로직을 여기에 추가
                Managers.Spawner.SpawnNew<Monster>(spawn.MonsterID);
            }

            if (spawn.CycleTime == 0)
            {
                break;
            }

            yield return YieldInstructionCache.WaitForSeconds(spawn.CycleTime);
        }
    }

    private void EndStage()
    {
        Debug.Log("Stage Ended.");
        Managers.Game.isStartGame = false;
        // 스테이지 종료 시 필요한 로직을 여기에 추가
        // 스테이지 종료 시 종료 팝업
        // 클리어 판별
    }
}