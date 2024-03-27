using System;
using System.Collections;
using System.Collections.Generic;
using Data;
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

    // public float cooltime;

    public int prepatternidx;

    public override bool Init()
    {
        _hero = Managers.Object.Hero;
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Boss;
        Phase_Percent = new[] { 70, 50, 30 };
        Pattern_Percent = new int[,] { { 50, 50, 0 }, { 20, 40, 40 }, { 10, 45, 45 }, { 10, 45, 45 } };
        StartCoroutine(CoPhaseCheck());
        return true;
    }

    public Coroutine PlayCo;

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
                CreatureState = ECreatureState.Move;
                break;
            case 1:
                CreatureState = ECreatureState.Attack;
                break;
            case 2:
                CreatureState = ECreatureState.Skill1;
                break;

            default:
                CreatureState = ECreatureState.Move;
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

    protected override void UpdateMove()
    {
        if (WaitTest == null)
        {
            WaitTest = StartCoroutine(PatternWait());    
        }
        if (_hero.IsValid())
        {
            switch (monsterData.DataId)
            {
                case 415:
                    Vector2 dest = (_hero.transform.position - transform.position).normalized;
                    SetRigidbodyVelocity(dest * MoveSpeed);
                    break;
            }
        }
        else
            SetRigidbodyVelocity(Vector2.zero);
    }

    protected override void UpdateAttack()
    {
        if (cotest == null)
        {
            cotest = StartCoroutine(Attack());
        }
        
        Vector2 dest = (_hero.transform.position - transform.position).normalized;
        SetRigidbodyVelocity(dest * 0);
        SetImageDirecton(dest);
    }
    
    protected override void UpdateSkill1()
    {
        if (cotest == null)
        {
            
        }
    }

    public Hero _hero;
    
    public Coroutine WaitTest = null;
    IEnumerator PatternWait()
    {
        cooltime = Random.Range(3f, 5f);
        float time = 0f;

        while (true)
        {
            if (time >= cooltime)
            {
                SelectPattern(Phase);
                WaitTest = null;
                break;
            }

            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator CoPhaseCheck()
    {
        while (true)
        {
            if (Hp / MaxHp * 100 <= 70 && Phase == 0)
            {
                Phase++;
                CreatureState = ECreatureState.Pattern1;
                Debug.Log("1 페이즈 시작!");
            }
            else if (Hp / MaxHp * 100 <= 50 && Phase == 1)
            {
                Phase++;
                CreatureState = ECreatureState.Pattern2;
                Debug.Log("2 페이즈 시작!");
            }
            else if (Hp / MaxHp * 100 <= 30 && Phase == 2)
            {
                Phase++;
                Debug.Log("3 페이즈 시작!");
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    //테스트 코드
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        { 
            Hp-= 10;
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            CreatureState = ECreatureState.Move;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            cotest = StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log(cotest);
        }
    }

    protected override IEnumerator Attack()
    {
        CreatureState = ECreatureState.Attack;
        SetRigidbodyVelocity(Vector2.zero);
        
        float angle = 90f;
        
        int proj_num = 12;
        float m_angle = (angle/2) * -1;
        float M_angle = (angle/2);
        
        for (int i = 1; i <= proj_num; i++)
        {
            var proj = Managers.Object.Spawn<EnemyProjectile>(transform.position, 1);
            Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
            float ran_angle = Random.Range(m_angle, M_angle + 1);
            proj.SetSpawnInfo(this, null, Util.RotateVectorByAngle(direction, ran_angle));
            proj.SetTarget(_hero);
            
            if(i % 3 == 0)
                yield return new WaitForSeconds(0.5f);
            if (i == proj_num)
            {
                cotest = null;
                SelectPattern(Phase);
                yield break;    
            }
            yield return new WaitForFixedUpdate();
        }
        
    }
    
    #endregion
}