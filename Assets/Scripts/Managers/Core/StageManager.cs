using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    private int currentStageId;

    private Dictionary<int, Data.Stage> stageDic;

    public Data.Stage CurrentStage { get; private set; }

    protected override void Init()
    {
        base.Init();
        stageDic = DataManager.Instance.StageDic;
        currentStageId = GetFirstUnlockedStageId();
        CurrentStage = GetStage(currentStageId);
    }

    private int GetFirstUnlockedStageId()
    {
        foreach (var stage in stageDic.Values)
        {
            if (!stage.Locked)
                return stage.StageID;
        }
        return -1; // 적절한 값을 반환하거나 예외 처리를 할 수 있습니다.
    }

    public Data.Stage GetStage(int stageId)
    {
        if (stageDic.TryGetValue(stageId, out var stage))
        {
            return stage;
        }
        Debug.LogWarning($"Stage with ID {stageId} not found.");
        return null;
    }

    public void NextStage()
    {
        if (CurrentStage == null)
            return;

        int nextStageId = CurrentStage.StageID + 1;
        if (stageDic.TryGetValue(nextStageId, out var nextStage))
        {
            CurrentStage = nextStage;
            currentStageId = nextStageId;
            LoadStage(CurrentStage);
        }
        else
        {
            Debug.Log("Next stage not found");
        }
    }

    public void RestartStage()
    {
        if (CurrentStage == null)
            return;

        LoadStage(CurrentStage);
    }

    private void LoadStage(Data.Stage stage)
    {
        // 실제로 스테이지를 로드하는 로직을 여기에 추가합니다.
        Debug.Log($"Loaded Stage: {stage.Name}");
    }

    public void LockStage(int stageId)
    {
        if (stageDic.TryGetValue(stageId, out var stage))
        {
            stage.Locked = true;
            Debug.Log($"Stage {stageId} locked.");
        }
    }

    public void UnlockStage(int stageId)
    {
        if (stageDic.TryGetValue(stageId, out var stage))
        {
            stage.Locked = false;
            Debug.Log($"Stage {stageId} unlocked.");
        }
    }
}