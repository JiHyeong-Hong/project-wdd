using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AchievementType
{
    KillEnemies,
    CollectItems,
    CompleteLevels,
    // Add more achievement types as needed
}

[System.Serializable]
public class Achievement
{
    public string ID;
    public string Name;
    public string Description;
    public AchievementType Type;
    public int GoalValue;
    public int CurrentValue;
    public bool IsCompleted;

    public Achievement(string id, string name, string description, AchievementType type, int goalValue)
    {
        ID = id;
        Name = name;
        Description = description;
        Type = type;
        GoalValue = goalValue;
        CurrentValue = 0;
        IsCompleted = false;
    }

    public void Increment(int amount = 1)
    {
        if (IsCompleted) return;

        CurrentValue += amount;
        if (CurrentValue >= GoalValue)
        {
            CurrentValue = GoalValue;
            IsCompleted = true;
            OnAchievementCompleted();
        }
    }

    public void OnAchievementCompleted()
    {
        UnityEngine.Debug.Log($"Achievement Completed: {Name}");
    }
}

public class AchievementsHelper
{
    // ΩÃ±€≈Ê¿∏∑Œ ¿€º∫
    private static AchievementsHelper instance;
    public static AchievementsHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AchievementsHelper();
            }
            return instance;
        }
    }
    private Dictionary<string, Achievement> achievements;


    public void AddAchievement(Achievement achievement)
    {
        if (!achievements.ContainsKey(achievement.ID))
        {
            achievements.Add(achievement.ID, achievement);
        }
        else
        {
            Debug.LogWarning($"Achievement with ID {achievement.ID} already exists.");
        }
    }

    public Achievement GetAchievement(string id)
    {
        if (achievements.ContainsKey(id))
        {
            return achievements[id];
        }
        else
        {
            Debug.LogWarning($"Achievement with ID {id} not found.");
            return null;
        }
    }

    public void IncrementAchievement(string id, int amount = 1)
    {
        Achievement achievement = GetAchievement(id);
        if (achievement != null)
        {
            achievement.Increment(amount);
        }
    }

    public List<Achievement> GetAllAchievements()
    {
        return new List<Achievement>(achievements.Values);
    }
}
