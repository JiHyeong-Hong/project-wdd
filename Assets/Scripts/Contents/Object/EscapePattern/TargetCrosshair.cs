using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// 길리슈터가 목표물을 조준하면 나오는 조준점 클래스. @홍지형
public class TargetCrosshair : MonoBehaviour
{
    private Transform _target; // 추적할 목표물의 Transform
    public float size;      // 조준점의 크기
    public float circleSize = 0.1f; // 중앙 원의 크기
    private LineRenderer lineRenderer_square;
    private LineRenderer lineRenderer_horizontal;
    private LineRenderer lineRenderer_vertical;
    public GameObject squarePrefab; // 정사각형을 그릴 프리팹
    public GameObject horizontalPrefab; // 십자선을 그릴 프리팹
    public GameObject verticalPrefab; // 십자선을 그릴 프리팹

    void Start()
    {
        size = Managers.Object.Hero.transform.localScale.x * 0.6f;


        // LineRenderer 컴포넌트 추가 및 설정
        GameObject squareObj = Instantiate(squarePrefab, transform);
        lineRenderer_square = squareObj.GetComponent<LineRenderer>();
        lineRenderer_square.positionCount = 5; 
        lineRenderer_square.startWidth = 0.05f; // 선의 두께
        lineRenderer_square.endWidth = 0.05f;

        GameObject horizontalObj = Instantiate(horizontalPrefab, transform);
        lineRenderer_horizontal = horizontalObj.GetComponent<LineRenderer>();
        //lineRenderer_horizontal.positionCount = 4;
        lineRenderer_horizontal.startWidth = 0.05f; // 선의 두께
        lineRenderer_horizontal.endWidth = 0.05f;

        GameObject verticalObj = Instantiate(verticalPrefab, transform);
        lineRenderer_vertical = verticalObj.GetComponent<LineRenderer>();
        //lineRenderer_vertical.positionCount = 4;
        lineRenderer_vertical.startWidth = 0.05f; // 선의 두께
        lineRenderer_vertical.endWidth = 0.05f;
    }

    void Update()
    {
        _target = Managers.Object.Hero.transform;
        if (_target == null) return;
        UpdateCrosshair(_target.position, size); 
    }

    void UpdateCrosshair(Vector3 center, float size)
    {
        // 정사각형 모양을 그립니다.
        Vector3[] squarePositions = new Vector3[5];
        squarePositions[0] = center + (Vector3.up * size) - (Vector3.right * size); // 왼쪽 위
        squarePositions[1] = center + (Vector3.up * size) + (Vector3.right * size); // 오른쪽 위
        squarePositions[2] = center - (Vector3.up * size) + (Vector3.right * size); // 오른쪽 아래
        squarePositions[3] = center - (Vector3.up * size) - (Vector3.right * size); // 왼쪽 아래
        squarePositions[4] = squarePositions[0]; // 처음 점으로 복귀하여 정사각형 완성
        lineRenderer_square.SetPositions(squarePositions);

        // 수평선을 그립니다.
        Vector3[] horizontalPositions = new Vector3[2];
        horizontalPositions[0] = center - (Vector3.right * size); // 가운데에서 왼쪽으로
        horizontalPositions[1] = center + (Vector3.right * size); // 가운데에서 오른쪽으로
        lineRenderer_horizontal.SetPositions(horizontalPositions);

        // 수직선을 그립니다.
        Vector3[] verticalPositions = new Vector3[2];
        verticalPositions[0] = center - (Vector3.up * size);    // 가운데에서 아래로
        verticalPositions[1] = center + (Vector3.up * size);    // 가운데에서 위로
        lineRenderer_vertical.SetPositions(verticalPositions);

    }   
}
