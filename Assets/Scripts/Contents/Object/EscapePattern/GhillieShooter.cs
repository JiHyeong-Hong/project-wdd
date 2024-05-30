using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 길리슈터 @홍지형
public class GhillieShooter : Monster
{
    private Transform _target;
    private LineRenderer _lineRenderer;
    private bool _isFired = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Monster;

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

    //protected MonsterData monsterData;
    //protected Hero _hero;
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Renderer.sortingOrder = SortingLayers.MONSTER;
        //_hero = Managers.Object.Hero;
        //monsterData = Util.ConvertToMonsterData(CreatureData);
    }

    void Update()
    {
        if (!_isFired && _target != null)
        {            
            Aim();
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
        
        GetComponentInChildren<TargetCrosshair>().enabled = false;     // 조준점의 이동을 멈춘다.

        // 동작 정지
        StopCoroutine(AimAndFireCycle());

        Managers.Object.Despawn(this);
    }

    // 마취총 발사
    IEnumerator Fire()
    {       
        // 투사체 발사
        Debug.Log("[길리슈터] 발사!");
        
        // 마취총알 클래스 생성
        Vector2 direction = (_hero.transform.position - this.transform.position).normalized;
        var proj = Managers.Object.Spawn<GullieBullet>(transform.position, 801); // 하드코딩 테스트용. 추후에 csv에서 불러오는 방법 알아낼것
        proj.SetImage();
        proj.SetSpawnInfo(this, null, direction);
        proj.SetTarget(_hero);

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
