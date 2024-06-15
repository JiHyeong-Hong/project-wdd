using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trumpet : BaseObject
{
    private float usedTransparency = 0f;
    public GameObject zebraPrefab;
    private GameObject zebraInstance;
    public float speed = 0.5f;
    private Vector3 targetPosition;


    public override bool Init()
    {
        if (!base.Init())
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        if (Renderer != null)
        {
            Color color = Renderer.material.color;
            color.a = usedTransparency;
            Renderer.material.color = color;
        }

        SpawnZebra();

        Managers.Object.Despawn(this);
    }

    void SpawnZebra()
    {
        // 카메라의 위치와 화면의 높이, 너비 계산
        Camera mainCamera = Camera.main;
        float screenWidth = 2 * mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = mainCamera.transform.position.x + (Random.value > 0.5f ? -1 : 1) * screenWidth / 2;
        float spawnY = mainCamera.transform.position.y;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        Instantiate(zebraPrefab, spawnPosition, Quaternion.identity);
    }
}
