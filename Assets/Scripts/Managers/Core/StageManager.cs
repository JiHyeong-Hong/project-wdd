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
        StartCoroutine(StageRoutine());
        Managers.Game.isStartGame = true;
        state = EStageState.Nomal;        
    }

    private IEnumerator StageRoutine()
    {
        while (currentPhaseIndex < phases.Count)
        {
            PhaseInfo phase = phases[currentPhaseIndex];
            phaseTimer = 0;

            if (phase.Phase < maxPhase)
            {
                Debug.Log($"<color=red>Starting Phase {phase.Phase} for {phase.Duration} seconds \n phasesCount:{phases.Count} </color>");
                SpawnMonsters("SpawnMonsterCoroutine", phase.Spawns);
                yield return new WaitForSeconds(phase.Duration);
            }
            else
            {
                Debug.Log($"<color=red>Last Phase {phase.Phase} is Boss Stage </color>");
                SpawnMonsters("StartBossStage", phase.Spawns);
                yield return new WaitForSeconds(phase.Duration);
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

    private void SpawnMonsters(string coroutineName, List<Spawn> spawns)
    {
        foreach (var spawn in spawns)
        {
            Coroutine coroutine = StartCoroutine(coroutineName, spawn);
            if (coroutine != null)
            {
                coroutines.Add(coroutine);
            }
        }
    }

    private IEnumerator SpawnMonsterCoroutine(Spawn spawn)
    {
        while (true)
        {
            for (int i = 0; i < spawn.Count; i++)
            {
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
                Barricate = Managers.Object.Spawn<Structure>(Managers.Object.Hero.transform.position, 1);
            }
            else if (state == EStageState.Barricade)
            {
                SoundManager.Instance.Play(Define.ESoundMainType.Bgm, Define.ESoundType.Boss);
                Managers.Object.Spawn<Boss>(Barricate.transform.position + Vector3.up * 3, spawn.MonsterID);
                state = EStageState.Boss;
                break;
            }

            yield return YieldInstructionCache.WaitForSeconds(2f);
        }
    }

    public void EndStage()
    {
        // Logic for ending the stage can be added here
    }

    public void ResetDatas()
    {
        StopAllCoroutines();
        currentStageID = -1;
        currentStage = null;
        phases = new List<PhaseInfo>();
        currentPhaseIndex = 0;
        phaseTimer = 0;
        maxPhase = 0;
        state = EStageState.None;
    }
}