using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �Ŵ���

public class Footboard : MonoBehaviour
{
    public GameObject jumpfab;
    public GameObject boostfab;
    public Transform playerTransform; // �÷��̾� ��ġ
    private float spawnInterval = 5f; 

        private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        StartCoroutine(SpawnObjectsWithDelay(spawnInterval));
    }

    private IEnumerator SpawnObjectsWithDelay(float interval)
    {
        // ���� �� ù 1�� ���
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
        if (Random.Range(0, 100) < 70) // 70% Ȯ���� ����
        {
            Vector3 currentPlayerPosition = playerTransform.position;
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(4, 11); // 4ĭ���� 10ĭ ����
            Vector3 spawnPosition = currentPlayerPosition + new Vector3(randomDirection.x, randomDirection.y, 0); // 2D ������ ��� Z �� ��� Y �� ���

            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
