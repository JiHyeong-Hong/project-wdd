using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private float visibleTime;
    private float tick;

    public void SetInfo(Vector2 position, float visibleTime = 0.5f)
    {
        transform.position = position;
        this.visibleTime = visibleTime;
        tick = 0f;
    }

    void Update()
    {
        tick += Time.deltaTime;
        if (tick > visibleTime)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
