using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TrumpetZebra : MonoBehaviour
{
    public float speed = 0.5f;
    private Vector3 targetPosition;

    void Start()
    {
        Camera mainCamera = Camera.main;
        float screenWidth = 2 * mainCamera.orthographicSize * mainCamera.aspect;

        if (transform.position.x < mainCamera.transform.position.x) // 왼쪽에서 스폰될 경우
        {
            targetPosition = new Vector3(mainCamera.transform.position.x + screenWidth / 2+5.0f, transform.position.y, 0);
            // 이미지 반전
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else  // 오른쪽에서 스폰될 경우
        {
            targetPosition = new Vector3(mainCamera.transform.position.x - screenWidth / 2 -5.0f, transform.position.y, 0);
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void DestroyAllMonsters()
    {
        Creature[] creatures = FindObjectsOfType<Creature>();
        foreach (Creature creature in creatures)
        {
            if (creature.CreatureType == ECreatureType.Monster)
            {
                Destroy(creature.gameObject);
            }
        }
    }
}
