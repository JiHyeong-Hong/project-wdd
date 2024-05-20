using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �Ŵ���

public class Footboard : MonoBehaviour
{
    public GameObject jumpfab;
    public GameObject boostfab;
    public Transform playerTransform; // �÷��̾� ��ġ

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

        // �׽�Ʈ������ ���� �����ϰ� �ٷ� ���� �����غ�
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
        if (Random.Range(0, 100) < 70) // 70% Ȯ���� ����
        {
            Vector3 currentPlayerPosition = playerTransform.position;
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(4, 11); // 4ĭ���� 10ĭ ����
            Vector3 spawnPosition = currentPlayerPosition + new Vector3(randomDirection.x, randomDirection.y, 0); // 2D ������ ��� Z �� ��� Y �� ���

            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
