using System.Linq;
using UnityEngine;
using System.Reflection;
using System;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    [SerializeField]
    private Stat hpStat;
    
    public Entity Owner { get; private set; }
    public Stat HPStat { get; private set; }

    public Stat[] stats;

    GUIStyle style = new ();

    private int titleSize = 80;
    private int labelSize = 50;


    float spacing = 2;
    Rect textRect;
    Rect plusButtonRect;
    Rect minusButtonRect;

    bool isShow = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F7))
        {
            isShow = !isShow;
        }
    }

    private void OnGUI()
    {
        if (!Owner.IsPlayer)
            return;
        style.fontSize = titleSize;

        if (!isShow)
            return;


        float screenX = Screen.width;
        float screenY = Screen.height;
        GUI.Box(new Rect(spacing, spacing, screenX/2, screenY/2),string.Empty);
        GUI.Label(new Rect(2 + spacing, spacing, 100, 30), "Player Stat", style);

        textRect = new Rect(4, 20 + spacing + style.fontSize, 200, 30);

        plusButtonRect = new Rect(textRect.x + textRect.width + 150, textRect.y, 80, 80);
        minusButtonRect = plusButtonRect;
        minusButtonRect.x += spacing +20 + plusButtonRect.width;
        
        SetLabelAndButton(textRect, spacing, "Hp", Managers.Object.Hero.Hp);

        SetLabelAndButton(textRect, spacing, "Level", Managers.Object.Hero.Level);

        SetLabelAndButton(textRect, spacing, "MoveSpeed", Managers.Object.Hero.MoveSpeed);


    }

    // 텍스트 크기를 조절합니다.
    private void SetLabelAndButton(Rect textRect, float spacing, string statName, float stat)
    {
        style.fontSize = labelSize;
        GUIStyle buttonTextStyle = new GUISkin().button;

        buttonTextStyle.alignment = TextAnchor.MiddleCenter;
        buttonTextStyle.fontSize = 80;

        GUI.Label(textRect, $"{statName}: {stat}", style);

        if (GUI.Button(plusButtonRect, "+", buttonTextStyle))
        {
            Managers.Object.Hero.OnChangeValue(statName, 1);
        }

        if (GUI.Button(minusButtonRect, "-", buttonTextStyle))
        {
            Managers.Object.Hero.OnChangeValue(statName, -1);
        }

        SpacingTextAndButton();
    }
    private void SetLabelAndButton(Rect textRect, float spacing, string statName, ref int stat)
    {
        style.fontSize = labelSize;
        GUIStyle buttonTextStyle = new GUISkin().button;

        buttonTextStyle.alignment = TextAnchor.MiddleCenter;
        buttonTextStyle.fontSize = 80;

        GUI.Label(textRect, $"{statName}: {stat}", style);

        if (GUI.Button(plusButtonRect, "+", buttonTextStyle))
        {
            stat += 1;
        }

        if (GUI.Button(minusButtonRect, "-", buttonTextStyle))
        {
            stat -= 1;
        }

        SpacingTextAndButton();
    }
    private void SpacingTextAndButton()
    {
        textRect.y += textRect.height + spacing + labelSize;
        plusButtonRect.y = minusButtonRect.y = textRect.y;
    }

    public void Setup(Entity entity)
    {
        Owner = entity;
        HPStat = hpStat ? hpStat : null;
    }

    private void OnDestroy()
    {
        
    }

    public float GetValue(Stat stat)
    {
        return stat.Value;
    }

#if UNITY_EDITOR
    [ContextMenu("LoadStats")]
    private void LoadStats()
    {
        var stats = Resources.LoadAll<Stat>("Stat").OrderBy(x => x.ID);

    }
#endif

}