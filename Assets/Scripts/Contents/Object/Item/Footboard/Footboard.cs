using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발판 생성 매니저

public class Footboard : MonoBehaviour
{
    public GameObject jumpfab;
    public GameObject boostfab;
    public Transform playerTransform; // 플레이어 위치

    private UI_GameScene uiGameScene;
    private float lastSpawnMinute = -1f;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        uiGameScene = FindObjectOfType<UI_GameScene>();

        jumpfab = Resources.Load<GameObject>("Prefabs/JumpFootboard");
        boostfab = Resources.Load<GameObject>("Prefabs/BoostFootboard");
    }

    private void Update()
    {
        if (uiGameScene == null)
            return;

        float currentTime = uiGameScene.GetCurrentTimer();
        float currentMinute = Mathf.Floor(currentTime / 60);
        float currentSecond = currentTime % 60;

        // 테스트용으로 게임 시작하고 바로 발판 생성해봄
        //if (currentSecond <= 15 && lastSpawnMinute != currentMinute)
        if (currentMinute >= 1 && currentSecond <= 15 && lastSpawnMinute != currentMinute)
        {
            if (Random.Range(0, 100) < 70) 
            {
                TrySpawnObject(jumpfab);
            }
            if (Random.Range(0, 100) < 70)
            {
                TrySpawnObject(boostfab);
            }
            lastSpawnMinute = currentMinute;
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
