using Data;
using System;
using static Define;
using UnityEngine;
using System.Collections.Generic;


public class PassiveHelper
{
    private static PassiveHelper _instance;
    public static PassiveHelper Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PassiveHelper();
            return _instance;
        }
    }

    Dictionary<PassiveSkillStatusType, float> passiveSkill = new Dictionary<PassiveSkillStatusType, float>();

    public void SetPassive(SkillData skillData, int operatorValue = 1)
    {
        float stateValue = skillData.StatValue * operatorValue;
        UpdatePassiveSkillValue((PassiveSkillStatusType)skillData.StatType, stateValue, passiveSkill);
    }

    private void UpdatePassiveSkillValue(PassiveSkillStatusType statusType, float stateValue, Dictionary<PassiveSkillStatusType, float> passiveSkill)
    {
        passiveSkill[statusType] = passiveSkill.TryGetValue(statusType, out var existingValue) ? existingValue + stateValue : stateValue;

        // 이속증가는 선택 시 바로 적용 240915 @홍지형
        if(statusType == PassiveSkillStatusType.MoveSpeed)
        {
            Hero hero = Managers.Object.Hero;
            HeroData heroData = hero.CreatureData as HeroData;            
            hero.MoveSpeed = hero.MoveSpeed * (1 + Math.Abs(stateValue));
        }
        // 최대체력증가는 선택 시 바로 적용 240915 @홍지형
        if (statusType == PassiveSkillStatusType.Hp)
        {
            Hero hero = Managers.Object.Hero;
            HeroData heroData = hero.CreatureData as HeroData;
            hero.MaxHp = hero.MaxHp * (1 + Math.Abs(stateValue));
        }
    }

    public float GetPassiveValue(PassiveSkillStatusType value) => passiveSkill.TryGetValue(value, out float v) ? v : 0;
}
