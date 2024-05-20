using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

// 회피패턴(탄막) Manager 클래스. @홍지형
public class EscapePatternManager : SingletonMonoBehaviour<EscapePatternManager>
{
    private Data.MonsterData monsterData; // ??
    public GameObject warningPrefab; // 경고 표시 프리팹
    public GameObject netPrefab; // 그물망 프리팹
    public int netCount = 5; // 떨어지는 그물망의 개수
    public float spawnInterval = 0.4f; // 그물망이 떨어지는 시간 간격
    private float spawnAreaSize = 3f; // 스폰 영역 크기
    private List<Vector2> usedPositions = new List<Vector2>(); // 이미 사용된 위치 목록

    // 회피패턴을 생성한다.
    public void SpawnEscapePattern()
    {
        // TODO: 상황과 조건에 따라서 회피패턴이 스폰되도록 조정할것. 240415
        //SpawnGhillieShooter();
        //SpawnNet();

        SpawnTourist();
    }

    public void SpawnTourist()
    {
        // TODO: 출현 경고 UI 필요.

        Vector2 camPos = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)); // 인게임 카메라의 오른쪽 상단 위치        

        // 여러 관광객을 동시에 spawn한다.
        for (int i = 0; i < 20; i++) // TODO: 관광객 수 하드코딩됨.
        {
            // 관광객 spawn 범위: 타원의 가로축과 세로축 길이 설정
            float horizontalScale = 0.8f;
            float verticalScale = 1.5f;
            Vector2 ovalRandom = new Vector3(Random.insideUnitCircle.x * horizontalScale, Random.insideUnitCircle.y * verticalScale); // 랜덤 타원 범위            
            Vector3 spawnPosition = ovalRandom * spawnRadius + camPos;

            Managers.Object.Spawn<Tourist>(spawnPosition, 261); // TODO: 관광객 번호 하드코딩

            if (!isDirectionSet)
            {
                // 첫 몬스터가 생성되면 플레이어를 향하는 방향을 계산한다.
                initialDirection = (Managers.Object.Hero.transform.position - spawnPosition).normalized; 
                // initialDirection = (spawnPosition - Managers.Object.Hero.transform.position).normalized; // 반대방향
                isDirectionSet = true;
            }
        }
        // 생성된 관광객들에게 이동 방향을 설정한다.
        Tourist[] Tourists = FindObjectsOfType<Tourist>();
        foreach (Tourist tourist in Tourists)
        {            
            tourist.SetDirection(initialDirection);
        }
    }

    public void SpawnGhillieShooter()
    {
        // 길리슈터 생성 테스트용. @홍지형
        Managers.Object.Spawn<GhillieShooter>(new Vector3(-5f, 5f, 0f), 251); // TODO: 길리슈터 번호 하드코딩
    }

    public void SpawnNet()
    {
        // 프리팹 로드
        warningPrefab = Resources.Load<GameObject>("Prefabs/warning");
        netPrefab = Resources.Load<GameObject>("Prefabs/Net");

        SpawnNetPattern().Forget();
    }

    #region 그물망
    // 무작위로 그물망 패턴 생성.
    private async UniTaskVoid SpawnNetPattern()
    {
        Vector2 _targetPos = (Vector2)Managers.Object.Hero?.transform.position; // 그물망 생성 시점의 타겟 위치 가져오기

        for (int i = 0; i < netCount; i++)
        {
            Vector2 spawnPosition = GetRandomPosition(_targetPos);
            // StartCoroutine(SpawnWarning(spawnPosition));
            SpawnWarning(spawnPosition).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval));
        }
    }

    // 무작위 위치 구하기
    Vector2 GetRandomPosition(Vector2 _targetPos)
    {
        Vector2 randomPosition = Vector2.zero;
        do
        {
            float x = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);
            float y = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);
            randomPosition = new Vector2(x, y) + _targetPos;
        }
        while (usedPositions.Contains(randomPosition));
        usedPositions.Add(randomPosition);
        return randomPosition;
    }

    // 스폰 전 경고표시
    private async UniTaskVoid SpawnWarning(Vector2 position)    
    {
        // 백업
        GameObject warning = Instantiate(warningPrefab, position, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1f)); // 경고 표시 지속 시간
        Destroy(warning);

        // TODO: 프리팹생성하지 않고, 이미지만 바꿀것.
        //GameObject warning = Instantiate(warningPrefab, position, Quaternion.identity);
        //await UniTask.Delay(TimeSpan.FromSeconds(1f)); // 경고 표시 지속 시간
        //Destroy(warning);

        Managers.Object.Spawn<Net>(position, 271); // 그물망 스폰. Net 번호 하드코딩.
    }
    #endregion








}
