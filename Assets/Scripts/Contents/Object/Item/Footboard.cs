using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발판 생성 매니저

public class Footboard : MonoBehaviour
{
    public GameObject jumpfab;
    public GameObject boostfab;
    public Transform playerTransform; // 플레이어 위치
    private float spawnInterval = 5f; 

        private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        StartCoroutine(SpawnObjectsWithDelay(spawnInterval));
    }

    private IEnumerator SpawnObjectsWithDelay(float interval)
    {
        // 시작 후 첫 1분 대기
        yield return new WaitForSeconds(interval);

        while (true)
        {
            TrySpawnObject(jumpfab);
            TrySpawnObject(boostfab);

            yield return new WaitForSeconds(interval);
        }
    }

    private void TrySpawnObject(GameObject objectToSpawn)
    {
        if (Random.Range(0, 100) < 70) // 70% 확률로 생성
        {
            Vector3 currentPlayerPosition = playerTransform.position;
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(4, 11); // 4칸에서 10칸 사이
            Vector3 spawnPosition = currentPlayerPosition + new Vector3(randomDirection.x, randomDirection.y, 0); // 2D 게임인 경우 Z 축 대신 Y 축 사용

            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
