using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;


//
// 어설픈 상속으로 로직이 꼬이면서 개발의 어려움이 가중됨.
// InitBase -> BaseObject -> Creature -> Monster -> Boss로 이어지는 끔찍한 구조를 검토한 결과
// Interface로 충분히 해결 가능한 수준의 시스템을 상속으로 어렵게 만듬.
// 개념 이해하지 못한 상태에서 뭔가 있어 보이고 그럴듯해 보이는 상속을 사용한 것으로 추정.
// 인스턴스 관리를 목적으로 InitBase를 만들었다면 모를까
// 아무런 기능을 하지 않는 InitBase의 존재는 비단 메모리의 낭비를 넘어서 생각 없이 또는
// 짧은 생각으로 즉흥적 개발로 비침. 
// (InitBase만 문제가 있다는 것이 아님, 메인 프로그래머의 수준을 들어내는 상징적인 부분임.)
//
// C#, 유니티에 대한 이해가 부족한 상태에서 코드와 애셋이 얼키설키 엮인 프로젝트를 보면 고민의 흔적이 
// 보이지 않음. (고민 했다 하더라도 결과를 보면 무의미한 고민.)
// 무분별한 상속에 어설픈 개념이 더해져 아무것도 건드리지 못하는 끔찍한 상황이 되어버림.
// 문제를 수정하기 위해 중간 계층 어딘가를 수정하면, 사이드이펙트로 무너질 수밖에 없는 빈약한 구조.
// 프로그래머가 자신의 실력을 인정하지 못하면 팀을 위기에 빠트림.
//
// 진지한 태도로 공부하며 개발해야 하는데 새로운 기술을 시도하거나, 지금보다 더 나은 상황을 만들기 위한
// 그 어떠한 노력도 보이지 않음.
//
// "하다 안 되면 갈아 엎지 뭐" 같은 생각은 팀과 본인에게 아무런 도움도 되지 않음.
// 자신이 읽은 책 한 권, 가르친 강사의 스타일 만 부여잡고 전혀 발전하지 않는 프로그래머는 팀과 회사에 
// 손해를 끼침.
//
// 코딩한답시고 혼자 퍼즐 만들어서 노는 동안 팀원들은 고통 받고 프로젝트는 망해가는 것.
// 책임감을 가져야 하고, 자신의 부족함을 발견했다면 발품을 팔아서라도 채워 넣어야 함. 그런데 이 코드에는 
// 그런 노력이 안 보임.
//
// 루믹스 큐브 기본 공식도 모르고 한 면이라도 맞춰보겠다고 이리저리 굴리는 것은 노력이 아님.
// 언젠간 모든 면이 맞아떨어질 수 있겠지만, 그 순간이 언제 올지 아무도 모르고 온다는 보장도 없음.
// 
// 적당한 조직에 적당히 빌붙어 월급이나 받는 것이 목적이라면, 이 코멘트는 무시해도 됨.
// 능력? 실력? 경력? 보다 중요한 건 책임감이고 마무리하는 능력임.
//
// PS 1.객체지향 프로그래밍을 하고 싶다면, 제대로 공부해야 함. 
// 시중에 나와 있는 "강아지는 멍멍", "오리는 꽥꽥" , "비행기랑 자동차는 탈 것" 이런식으로 설명하는 책 말고,
// "Operating System Concepts"같은 책으로 객체지향에 대해 생각하면서 공부하는 것이 좋음.
// 
// PS 2. 인코딩을 euc-kr로 사용하는 개발자를 발견 했다면 즉시 사살.
//



// 
// Boss 클래스 재작업 방향
//
// - 패턴 테이블 확률 배열 초기화 방식 변경
// - 로직과 애니메이션 분리
// - 애니메이션 파일 분리 (4.Boss_phk.controller)
// - 트랜지션은 트리거 방식으로 변경
// - CreatureType의 타입을 상속의 부족한 이해로 Init에 넣어버리는 바람에 제대로 설정되지 않음.
//    SetInfo()에서 다시 초기화

public class Boss : Monster
{

    enum EPattern
    {
        None = -1,
        Normal1 = 0,
        Normal2 = 1,
        Normal3 = 2
    }
    public int Phase { get; set; } //현재 보스 페이즈

    public Data.HpConditionData Phase_Percent { get; set; } //패턴 구간 배열 - [체력]
    public List<Data.PatternPerData> Pattern_Percent_List { get; set; } //패턴 확률 배열 - [페이즈][확률]
    public int patternidx;



    private List<EPattern[]> Pattern_table ;
    private List<Func<IEnumerator>> Pattern_Coroutine = new List<Func<IEnumerator>>();
    private List<float> checkHpList = new List<float>();
    private bool InitializeCheck = false;

    private Coroutine CoIdle = null;

    private EPattern lastPattern = EPattern.None;

    public override bool Init()
    {
        _hero = Managers.Object.Hero;
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Boss;
        return true;
    }

    // public Coroutine PlayCo;

    private EPattern SelectPattern(int phase)
    {
        if(attackLock || phase >= Pattern_table.Count)
        {
            Debug.Log("SelectPattern fail");
            return EPattern.None;
        }

        EPattern [] table = Pattern_table[phase];
        int rand;
        EPattern pattern;
        do
        {
            rand = Random.Range(0, 100);
            pattern = table[rand];
        }while (pattern == lastPattern);
        lastPattern = pattern;

        Debug.Log("************ SelectPattern ( " + phase  + " / " + pattern.ToString() + " ) " + CreatureState.ToString()  );
        return pattern;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Phase_Percent = Managers.Data.HpConditionDic[templateID];
        checkHpList = new List<float>
        {
            (float)(Phase_Percent.Phase1Hp-1) / 100f,
            (float)Phase_Percent.Phase2Hp / 100f,
            (float)Phase_Percent.Phase3Hp / 100f
        };

        Pattern_Percent_List = new List<PatternPerData>();
        
        foreach (var item in Managers.Data.PatternPerDic)
        {
            if(item.Value.MonsterID == templateID)
            {
                Pattern_Percent_List.Add(item.Value);
            }
        }

        Pattern_table = new List<EPattern[]>(); 
        for (int i = 0; i < Pattern_Percent_List.Count; i++)
        {
            EPattern [] table = new EPattern[100];
            Array.Fill(table, EPattern.Normal1, 0, Pattern_Percent_List[i].Pattern1);
            Array.Fill(table, EPattern.Normal2, Pattern_Percent_List[i].Pattern1, Pattern_Percent_List[i].Pattern2);
            Array.Fill(table, EPattern.Normal3, Pattern_Percent_List[i].Pattern1+Pattern_Percent_List[i].Pattern2, Pattern_Percent_List[i].Pattern3);
            Pattern_table.Add(table);
        }

        Pattern_Coroutine = new List<Func<IEnumerator>>
        {
            Pattern1,
            Pattern2,
            Pattern3
        };

        // 어설픈 상속으로 인해 SetInfo에서 설정한 값이 유지되지 않아 다시 설정
        CreatureType = ECreatureType.Boss;
        CreatureState = ECreatureState.Idle;

        // 페이즈 초기화
        Phase = -1;

        // 초기화 체크
        InitializeCheck = true;
    }



    // 패턴 1
    // 플레이어를 쫒아 이동하며, 근접(0.5)하여 데미지를 줌.
    // 플레이어에게 데미지를 입히면, 1초 멈춰있다가 다시 쫒아감.
    // 3초동안 아무런 데미지를 입히지 못하면 패턴 종료
    const float pattern1_delay = 1f;
    const float pattern1_time = 3f;
    IEnumerator Pattern1()
    {
        if(isChangePhase || !_hero.IsValid())
        {
            Debug.Log("Pattern1 break");
            yield break;
        }

        CreatureState = ECreatureState.Move;
        TriggerAnimation(CreatureState);
        float time = 0;
        while(true)
        {
            float distance = Vector2.Distance(_hero.transform.position, transform.position);
            Vector2 dest = (_hero.transform.position - transform.position).normalized;
            SetRigidbodyVelocity(dest * MoveSpeed);

            // 박현규
            // 로직 진행 중 어디에선가 발생할지 모르는 상태 변화로 로직이 틀어지는 것을 방지
            CreatureState = ECreatureState.Move;
            TriggerAnimation(CreatureState);

            if(time >= pattern1_time)
            {
                Debug.Log("Pattern1 done");
                break;
            }
            else if(distance < 0.5f)
            {
                TriggerAnimation(ECreatureState.Idle);
                SetRigidbodyVelocity(Vector2.zero);
                yield return new WaitForSeconds(pattern1_delay);
                time = 0f;
            }
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        CreatureState = ECreatureState.Idle;
        TriggerAnimation(CreatureState);
 
        CoIdle = null;
    }

    IEnumerator Pattern2()
    {
        if(isChangePhase || !_hero.IsValid())
        {
            Debug.Log("Pattern2 break");
            yield break;
        }

        CreatureState = ECreatureState.Move;
        TriggerAnimation(CreatureState);
        while( true)
        {
            float distance = Vector2.Distance(_hero.transform.position, transform.position);
            if(distance > monsterData.MinStance )
            {
                Vector2 dest = (_hero.transform.position - transform.position).normalized;
                SetRigidbodyVelocity(dest * MoveSpeed);

                // 박현규
                // 로직 진행 중 어디에선가 발생할지 모르는 상태 변화를 막기위한 코드
                CreatureState = ECreatureState.Move;
                TriggerAnimation(CreatureState);
            }
            else
            {
                Debug.Log("Pattern2 1");
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        SetRigidbodyVelocity(Vector2.zero);
        CreatureState = ECreatureState.Attack;
        TriggerAnimation(CreatureState);
        yield return StartCoroutine(Attack());
        Debug.Log("Pattern2 done");

        CreatureState = ECreatureState.Idle;
        TriggerAnimation(CreatureState);

        CoIdle = null;
    }
    bool attackLock = false;
    protected override IEnumerator Attack()
    {
        if(attackLock)
        {
            yield break;
        }
        attackLock = true;

        yield return new WaitForSeconds(0.5f);

        float angle = 90f;
        float m_angle = (angle/5) * -1;
        float M_angle = (angle/5);
        
        SoundManager.Instance.Play(Define.ESoundMainType.Boss, Define.ESoundType.BossShot);

        int count = 0;
        float interval = 0.3f;
        float time = 0f;
        while(count <= monsterData.ProjectileNum)
        {
            if(isChangePhase)
            {
                break;
            }

            time += Time.deltaTime;
            if(time >= interval)
            {
                var proj = Managers.Object.Spawn<EnemyProjectile>(transform.position, monsterData.ProjectileID);
                Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                float ran_angle = Random.Range(m_angle, M_angle + 1);
                proj.SetImage();
                proj.SetSpawnInfo(this, null, Util.RotateVectorByAngle(direction, ran_angle));
                proj.SetTarget(_hero);
                SetImageDirecton(direction);

                count++;
                time = 0f;
            }

            CreatureState = ECreatureState.Attack;
            TriggerAnimation(CreatureState);

            yield return null;
        }


        // for (int i = 1; i <= monsterData.ProjectileNum; i++)
        // {
        //     var proj = Managers.Object.Spawn<EnemyProjectile>(transform.position, monsterData.ProjectileID);
        //     Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
        //     float ran_angle = Random.Range(m_angle, M_angle + 1);
        //     proj.SetImage();
        //     proj.SetSpawnInfo(this, null, Util.RotateVectorByAngle(direction, ran_angle));
        //     proj.SetTarget(_hero);
        //     SetImageDirecton(direction);

        //     // 박현규
        //     // 로직 진행 중 어디에선가 발생할지 모르는 상태 변화를 막기위한 코드
        //     CreatureState = ECreatureState.Attack;
        //     TriggerAnimation(CreatureState);

        //     yield return new WaitForSeconds(0.3f);
        // }
        attackLock = false;
    }





    const float pattern3_skill_delay = 1.5f;
    const float pattern3_rush_delay = 1f;
    const float pattern3_rush_speed = 0.2f;
    const float pattern3_collision_radius = 0.5f;
    IEnumerator Pattern3()
    {
        if(isChangePhase || !_hero.IsValid())
        {
            Debug.Log("Pattern3 break");
            yield break;
        }

        CreatureState = ECreatureState.Move;
        TriggerAnimation(CreatureState);
        while( true)
        {
            float distance = Vector2.Distance(_hero.transform.position, transform.position);
            if(distance > monsterData.MinStance / 2f )
            {
                Vector2 dest = (_hero.transform.position - transform.position).normalized;
                SetRigidbodyVelocity(dest * MoveSpeed);

                // 박현규
                // 로직 진행 중 어디에선가 발생할지 모르는 상태 변화를 막기위한 코드
                CreatureState = ECreatureState.Move;
                TriggerAnimation(CreatureState);
            }
            else
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Pattern3 돌진 준비");
        SetRigidbodyVelocity(Vector2.zero);
        

        CreatureState = ECreatureState.Skill1;
        TriggerAnimation(CreatureState);

        yield return new WaitForSeconds(pattern3_skill_delay);
        Debug.Log("Pattern3 돌진");

        TriggerAnimation("Rush");
        yield return new WaitForSeconds(pattern3_rush_delay);
        while( true)
        {
            float distance = Vector2.Distance(_hero.transform.position, transform.position);
            Vector2 dest = (_hero.transform.position - transform.position).normalized;
            if(distance > pattern3_collision_radius)
            {
                // Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
                SetRigidbodyVelocity(dest * (MoveSpeed + pattern3_rush_speed));
            }
            else
            {
                // StartCoroutine(KnockbackUpdate(dest, 2, 1));
                Debug.Log("Pattern3 완료");
                break;
            }
            yield return new WaitForFixedUpdate();
        }        
        SetRigidbodyVelocity(Vector2.zero);

        CreatureState = ECreatureState.Idle;
        TriggerAnimation(CreatureState);

        CoIdle = null;
        
    }


    // 박현규
    // 넉백 로직 Creature.cs에서 그대로 가져옴
    IEnumerator KnockbackUpdate(Vector3 dir, float knockbackDistance, float knockback)
    {
        // 현재까지 넉백 시간
        float currentknockbackTime = 0f;
        // 이전 Frame에 이동한 거리
        float prevknockbackDistance = 0f;

        while (true)
        {
            currentknockbackTime += Time.deltaTime;

            float timePoint = currentknockbackTime / knockback;
            // Easing InOutSine https://easings.net/ko#easeOutCirc
            float easeOutCirc = Mathf.Sqrt(1 - Mathf.Pow(timePoint - 1, 2));
            float currentknockbackDistance = Mathf.Lerp(0f, knockbackDistance, easeOutCirc);
            // 이번 Frame에 움직여야할 거리를 구함
            float deltaValue = currentknockbackDistance - prevknockbackDistance;

            _hero.transform.position += (dir * deltaValue);
            prevknockbackDistance = currentknockbackDistance;

            if (currentknockbackTime >= knockback)
                break;
            else
                yield return null;
        }
    }





    protected override void UpdateIdle()
    {
        
        if(Phase == -1 || Phase >= Pattern_table.Count)
        {
            // 박현규
            // 페이즈 지정이 되어 있지 않으면 아무것도 하지 않음.
            // Update에서 CheckPhase(); 실행
            return;
        }

        Vector2 dest = (_hero.transform.position - transform.position).normalized;
        SetRigidbodyVelocity(dest * 0);
        SetImageDirecton(dest);

        if (CoIdle == null)
        {
            // EPattern pattern = SelectPattern(1);
            // Debug.Log("UpdateIdle : " + (int)pattern);
            CoIdle = StartCoroutine(Pattern_Coroutine[ (int)SelectPattern(Phase) ]());
        }
    }

    void TriggerAnimation(ECreatureState state, Action endCallback = null)
    {
        TriggerAnimation(state.ToString(), endCallback);
    }
    
    void TriggerAnimation(string state, Action endCallback = null)
    {
        Animator.SetTrigger(state);
        if(endCallback != null)
        {
            StartCoroutine(AnimationEndCallback(endCallback));
        }
    }

    IEnumerator AnimationEndCallback(Action endCallback)
    {
        yield return null;
        AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length+0.5f);
        endCallback?.Invoke();
    }

    // 박현규
    // 별도 처리를 위해 빈 메소드로 오버라이딩
    protected override void PlayAnimation(Define.ECreatureState state)
    {
        return;
    }


    protected override void UpdateMove()
    {

    }


    protected override void UpdateAttack()
    {

    }

    protected override void UpdateSkill1()
    {

    }

    protected override void UpdateHit()
    {
        
    }


    protected override void UpdatePattern1()
    {
        
    }

    protected override void UpdatePattern2()
    {

    }

    protected override void UpdateChangePhase()
    {
        
    }

    
    bool isChangePhase = false;
    void CheckPhase()
    {
        float hp = Hp / MaxHp;
        for(int i = 0; i < checkHpList.Count; i++)
        {
            if(hp > checkHpList[i])
            {
                if(Phase != i)
                {
                    if(CoIdle != null)
                    {
                        StopCoroutine(CoIdle);
                        CoIdle = null;
                    }

                    Phase = i;
                    isChangePhase = true;
                    CreatureState = ECreatureState.ChangePhase;
                    Debug.Log("CheckPhase : " + Phase);
                    TriggerAnimation(CreatureState, () => {
                        isChangePhase = false;
                        CreatureState = ECreatureState.Idle;
                        TriggerAnimation(CreatureState);
                    });
                }
                break;
            }
        }

    }


    void Update()
    {
        if(InitializeCheck == false)
        {
            return;
        }

        // 박현규
        // 플레이어가 사라지면 아이들 상태로 변경
        if(!_hero.IsValid())
        {
            CreatureState = ECreatureState.Idle;
            TriggerAnimation(CreatureState);
        }
        else
        {
            CheckPhase();
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.blue;
        GUI.Label(new Rect(10, 10, 100, 100), "Phase : " + (Phase + 1) + " Pattern : " + lastPattern.ToString(), style);
        GUI.Label(new Rect(10, 40, 100, 100), "Boss State : " + CreatureState.ToString(), style);
    }



}