using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : Object
{
    public delegate void ValueChangedHandler(Stat stat, float currentValue, float prevValue);

    [SerializeField]
    private bool isPercentType;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float minValue;
    [SerializeField]
    private float defaultValue;

    private Dictionary<object, Dictionary<object, float>> bonusValueByKey = new();

    public int ID { get; set; }
    public bool IsPercentType => isPercentType;
    public float MaxValue => maxValue;
    public float MinValue => minValue;
    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            float prevValue = Value;
            defaultValue = Mathf.Clamp(value, minValue, maxValue);
            TryInvokeValueChangedEvent(Value, prevValue);
        }
    }

    public float BonusValue { get; private set; }
    public float Value => Mathf.Clamp(defaultValue + BonusValue, minValue, maxValue);
    public bool IsMax => Mathf.Approximately(Value, maxValue);
    public bool IsMin => Mathf.Approximately(Value, minValue);

    public event ValueChangedHandler onValueChanged;
    public event ValueChangedHandler onValueMax;
    public event ValueChangedHandler onValueMin;

    private void TryInvokeValueChangedEvent(float currentValue, float prevValue)
    {
        if(!Mathf.Approximately(currentValue, prevValue))
        {
            onValueChanged?.Invoke(this, currentValue, prevValue);
            if (Mathf.Approximately(currentValue, maxValue))
            {
                onValueMax?.Invoke(this, maxValue, prevValue);
            }
            else if (Mathf.Approximately(currentValue, minValue))
            {
                onValueMin?.Invoke(this, minValue, prevValue);
            }
        }
    }

    public void SetBonusValue(object key, object subKey, float value)
    {
        if(!bonusValueByKey.ContainsKey(key))
        {
            bonusValueByKey[key] = new Dictionary<object, float>();
        }
        else
        {
            BonusValue -= bonusValueByKey[key][subKey];
        }

        float prevValue = Value;
        bonusValueByKey[key][subKey] = value;
        BonusValue += value;

        TryInvokeValueChangedEvent(Value, prevValue);
    }

    public void SetBonusValue(object key, float value) => SetBonusValue(key, string.Empty, value);

    public float GetBonusValue(object key)
    {
        if(bonusValueByKey.ContainsKey(key))
        {
            return bonusValueByKey[key].Sum(x => x.Value);
        }
        return 0;
    }

    public float GetBonusValue(object key, object subKey)
    {
        if(bonusValueByKey.ContainsKey(key) && bonusValueByKey[key].ContainsKey(subKey))
        {
            return bonusValueByKey[key][subKey];
        }
        return 0;
    }

    public bool RemoveBonusValue(object key)
    {
        if(bonusValueByKey.ContainsKey(key))
        {
            float prevValue = Value;
            BonusValue -= bonusValueByKey[key].Sum(x => x.Value);
            bonusValueByKey.Remove(key);
            TryInvokeValueChangedEvent(Value, prevValue);
            return true;
        }
        return false;
    }
    public bool RemoveBonusValue(object key, object subKey)
    {
        if(bonusValueByKey.ContainsKey(key) && bonusValueByKey[key].ContainsKey(subKey))
        {
            float prevValue = Value;
            BonusValue -= bonusValueByKey[key][subKey];
            bonusValueByKey[key].Remove(subKey);
            TryInvokeValueChangedEvent(Value, prevValue);
            return true;
        }
        return false;
    }

    public bool ContainsBonusValue(object key) => bonusValueByKey.ContainsKey(key);
    public bool ContainsBonusValue(object key, object subKey) => bonusValueByKey.ContainsKey(key) && bonusValueByKey[key].ContainsKey(subKey);
}
