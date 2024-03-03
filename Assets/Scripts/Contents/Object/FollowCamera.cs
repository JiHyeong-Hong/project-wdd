using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : InitBase
{

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Camera.main.orthographicSize = 8.0f;

        return true;
    }

    void LateUpdate()
    {
        Transform player = Managers.Object.Hero.transform;

        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, -10f);
            transform.position = targetPosition;
        }
    }
}
