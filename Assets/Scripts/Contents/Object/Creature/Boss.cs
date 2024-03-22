using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Boss : Monster
{
    public int Phase { get; set; } //현재 보스 페이즈

    public int[] Phase_Percent { get; set; } //패턴 구간 배열 - [체력]

    //[70,50,30]
    public int[,] Pattern_Percent { get; set; } //패턴 확률 배열 - [페이즈][확률] 

    //[0][50,50,0], [1][20,40,40], [2][10, 45, 45], [3][10, 45, 45]
    
    public float cooltime;

    public int prepatternidx;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = Define.ECreatureType.Boss;
        CreatureState = Define.ECreatureState.Pattern;
        _hero = Managers.Object.Hero;
        Phase_Percent = new[] { 70, 50, 30 };
        Pattern_Percent = new int[,] { { 50, 50, 0 }, { 20, 40, 40 }, { 10, 45, 45 }, { 10, 45, 45 } };
        return true;
    }

    public Coroutine PlayCo;

    protected override void UpdatePattern()
    {
        //패턴이 바뀌는 시간 변수
        cooltime = Random.Range(3f, 6f);
        UpdateAITick = cooltime;

        SelectPattern(Phase);
    }

    public void SelectPattern(int Phase)
    {
        //TODO Eung 코드다듬기 필요
        int min = 1;
        int max = 101;
        int per = Random.Range(min, max - Pattern_Percent[Phase, prepatternidx]);
        int size = Pattern_Percent.GetLength(1);
        int setVal = 0;
        int pattern_idx = 0;

        for (int i = 0; i < size; i++)
        {
            if (i != prepatternidx)
            {
                setVal += Pattern_Percent[Phase, i];

                if (per <= setVal)
                {
                    pattern_idx = i;
                    prepatternidx = i;
                    break;
                }
            }
        }

        switch (pattern_idx)
        {
            case 0:
                StartCoroutine(Pattern(pattern_idx));
                break;
            case 1:
                StartCoroutine(Pattern(pattern_idx));
                break;
            case 2:
                StartCoroutine(Pattern(pattern_idx));
                break;

            default:
                StartCoroutine(Pattern(0));
                break;
        }
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }
    

    #region Battle

    #endregion

    #region AI

    protected override void UpdateAttack()
    {
        Debug.Log("BossAttack!!!");
    }
    public Hero _hero;

    IEnumerator Pattern(int idx_Pattern)
    {
        float count = 0f;
        while (true)
        {
            Debug.Log(idx_Pattern + "패턴 진행중");
            if (count >= UpdateAITick)
            {
                Debug.Log("패턴 종료 새로운 패턴 실핼");
                break;
            }

            count += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion
}