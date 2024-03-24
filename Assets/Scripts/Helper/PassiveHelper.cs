using Data;
using System;
using static Define;

public struct PassiveSkill
{
    public float CoolDown;
    public float Farming;
    public float Exp;
    public float Gold;
    public float Duration;
    public float Recovery;
    public float HP;
    public float MoveSpeed;
    public float Attack;
    public float AttackSpeed;
    public float AttackRange;
    public float DamageCare;
    public float CastPer;
    public float CastSpeed;
    public float CastRange;
}

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

    private PassiveSkill passiveSkill = new PassiveSkill();

    public void SetPassive(SkillData skillData, int operatorValue = 1)
    {
        float stateValue = skillData.StatValue * operatorValue;
        switch ((PassiveSkillStatusType)skillData.StatType)
        {
            case PassiveSkillStatusType.CoolTimeDown:
                passiveSkill.CoolDown += stateValue;
                break;
            case PassiveSkillStatusType.Farming:
                passiveSkill.Farming += stateValue;
                break;
            case PassiveSkillStatusType.Exp:
                passiveSkill.Exp += stateValue;
                break;
            case PassiveSkillStatusType.Gold:
                passiveSkill.Gold += stateValue;
                break;
            case PassiveSkillStatusType.Duration:
                passiveSkill.Duration += stateValue;
                break;
            case PassiveSkillStatusType.Recovery:
                passiveSkill.Recovery += stateValue;
                break;
            case PassiveSkillStatusType.Hp:
                passiveSkill.HP += stateValue;
                break;
            case PassiveSkillStatusType.MoveSpeed:
                passiveSkill.MoveSpeed += stateValue;
                break;
            case PassiveSkillStatusType.Attack:
                passiveSkill.Attack += stateValue;
                break;
            case PassiveSkillStatusType.AttackSpeed:
                passiveSkill.AttackSpeed += stateValue;
                break;
            case PassiveSkillStatusType.AttackRange:
                passiveSkill.AttackRange += stateValue;
                break;
            case PassiveSkillStatusType.DamageCare:
                passiveSkill.DamageCare += stateValue;
                break;
            case PassiveSkillStatusType.CastPer:
                passiveSkill.CastPer += stateValue;
                break;
            default:
                break;
        }
    }

    public float GetPassiveValue(PassiveSkillStatusType value)
    {
        return 0;
        return (float)passiveSkill.GetType().GetField(value.ToString()).GetValue(passiveSkill);
    }

}
