using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : InitBase
{
    Camera camera;
    public override bool Init()
    {        
        if (base.Init() == false)
            return false;
        Camera.main.orthographicSize = 8.0f;
        camera = Camera.main;

        UpdateCameraPosition(); // 클래스 초기화 시점에서 호출 @홍지형
        return true;
    }

    private void UpdateCameraPosition()
    {
        Transform player = Managers.Object.Hero.transform;

        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, -10f);
            transform.position = targetPosition;
        }
        Vector2 camPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)); // 인게임 카메라의 오른쪽 상단 위치        
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void OnDrawGizmos()
    {
        if (camera != null)
        {
            // 뷰포트의 각 코너 포인트를 월드 좌표로 변환
            Vector3 topLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
            Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
            Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            Vector3 bottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));

            // Gizmos로 각 포인트에 구를 그립니다
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(topLeft, 0.1f);
            Gizmos.DrawSphere(topRight, 0.1f);
            Gizmos.DrawSphere(bottomLeft, 0.1f);
            Gizmos.DrawSphere(bottomRight, 0.1f);

            // 각 코너를 선으로 연결하여 뷰포트의 경계를 그립니다
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
