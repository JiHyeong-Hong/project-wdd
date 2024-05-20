using Data;
using System;
using static Define;

public struct PassiveSkill
{
    public float coolDown;
    public float farming;
    public float exp;
    public float gold;
    public float duration;
    public float recovery;
    public float hp;
    public float moveSpeed;
    public float attack;
    public float attackSpeed;
    public float attackRange;
    public float damageCare;
    public float castPer;
    public float castSpeed;
    public float castRange;
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
                passiveSkill.coolDown += stateValue;
                break;
            case PassiveSkillStatusType.Farming:
                passiveSkill.farming += stateValue;
                break;
            case PassiveSkillStatusType.Exp:
                passiveSkill.exp += stateValue;
                break;
            case PassiveSkillStatusType.Gold:
                passiveSkill.gold += stateValue;
                break;
            case PassiveSkillStatusType.Duration:
                passiveSkill.duration += stateValue;
                break;
            case PassiveSkillStatusType.Recovery:
                passiveSkill.recovery += stateValue;
                break;
            case PassiveSkillStatusType.Hp:
                passiveSkill.hp += stateValue;
                break;
            case PassiveSkillStatusType.MoveSpeed:
                passiveSkill.moveSpeed += stateValue;
                break;
            case PassiveSkillStatusType.Attack:
                passiveSkill.attack += stateValue;
                break;
            case PassiveSkillStatusType.AttackSpeed:
                passiveSkill.attackSpeed += stateValue;
                break;
            case PassiveSkillStatusType.AttackRange:
                passiveSkill.attackRange += stateValue;
                break;
            case PassiveSkillStatusType.DamageCare:
                passiveSkill.damageCare += stateValue;
                break;
            case PassiveSkillStatusType.CastPer:
                passiveSkill.castPer += stateValue;
                break;
            default:
                break;
        }
    }

    public float GetPassiveValue(PassiveSkillStatusType value)
    {
        return (float)passiveSkill.GetType().GetField(value.ToString()).GetValue(passiveSkill);
    }

}
