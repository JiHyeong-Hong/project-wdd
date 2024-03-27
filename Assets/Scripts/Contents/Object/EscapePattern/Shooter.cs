using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 길리슈터 @홍지형
public class Shooter : Creature 
{
    private Transform _target;
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;
    private StunBulletSkill _skill;

    private bool _isFired = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 스프라이트
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 레이저 시각화
        _lineRenderer = GetComponent<LineRenderer>(); 
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = 0.05f; // 너비 설정
        _lineRenderer.endWidth = 0.05f;

        _target = Managers.Object.Hero?.transform; // 타겟 위치 초기화
        StartCoroutine(AimAndFireCycle());
        StartCoroutine(CoUpdateAI());
        return true;
    }
    void Update()
    {
        if (!_isFired && _target != null)
        {            
            Aim();
        }

        // 1발 쏘면 더이상 쏘지 않게 멈춘다.
        if (_isFired == true) 
        {
            if(_skill.StunBullet == null) // 발사 후 마취총알이 사라졌으면
                Destroy(gameObject);      // shooter 제거한다.
        }        
    }

    // 일정시간 조준 후 발사 한다.
    IEnumerator AimAndFireCycle()
    {
        // 매초 발사 대기
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("[길리슈터] 조준 중... " + (i + 1) + "초 경과");
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(Fire());
        StopCoroutine(Fire());

        // 길리슈터 객체를 시각적으로 숨긴다.
        _spriteRenderer.enabled = false;
        _lineRenderer.enabled = false;
        
        GetComponentInChildren<TargetCrosshair>().enabled = false;     // 조준점의 이동을 멈춘다.

        // 동작 정지
        StopCoroutine(AimAndFireCycle());    
    }

    // 마취총 발사
    IEnumerator Fire()
    {       
        // 조준 레이저 시각화
        //_lineRenderer.startColor = Color.red;
        //_lineRenderer.endColor = Color.red;

        // 투사체 발사
        Debug.Log("[길리슈터] 발사!");

        // 마취총알 클래스 생성
        _skill = new StunBulletSkill();
        _skill.SetInfo(Managers.Data.SkillDic[9999]); // 임시 스킬 번호. @홍지형 240324
        _skill.SetOwner(this);
        _skill.DoSkill();
        //


        yield return new WaitForSeconds(1f);  // 총알 객체가 생성되기를 기다린다.
      
        _isFired = true; // 발사 상태로 변경
    }

    // 저격수가 플레이어를 조준한다.
    void Aim()
    {
        // 저격수 회전
        Vector2 dir = _target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (dir != Vector2.zero)
            Direction = dir;    // 위치정보 저장

        // 조준 레이저 시각화
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _target.position);
    }
}
