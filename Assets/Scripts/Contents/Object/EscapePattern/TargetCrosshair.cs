using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// �渮���Ͱ� ��ǥ���� �����ϸ� ������ ������ Ŭ����. @ȫ����
public class TargetCrosshair : MonoBehaviour
{
    private Transform _target; // ������ ��ǥ���� Transform
    public float size;      // �������� ũ��
    public float circleSize = 0.1f; // �߾� ���� ũ��
    private LineRenderer lineRenderer_square;
    private LineRenderer lineRenderer_horizontal;
    private LineRenderer lineRenderer_vertical;
    public GameObject squarePrefab; // ���簢���� �׸� ������
    public GameObject horizontalPrefab; // ���ڼ��� �׸� ������
    public GameObject verticalPrefab; // ���ڼ��� �׸� ������

    void Start()
    {
        size = Managers.Object.Hero.transform.localScale.x * 0.6f;


        // LineRenderer ������Ʈ �߰� �� ����
        GameObject squareObj = Instantiate(squarePrefab, transform);
        lineRenderer_square = squareObj.GetComponent<LineRenderer>();
        lineRenderer_square.positionCount = 5; 
        lineRenderer_square.startWidth = 0.05f; // ���� �β�
        lineRenderer_square.endWidth = 0.05f;

        GameObject horizontalObj = Instantiate(horizontalPrefab, transform);
        lineRenderer_horizontal = horizontalObj.GetComponent<LineRenderer>();
        //lineRenderer_horizontal.positionCount = 4;
        lineRenderer_horizontal.startWidth = 0.05f; // ���� �β�
        lineRenderer_horizontal.endWidth = 0.05f;

        GameObject verticalObj = Instantiate(verticalPrefab, transform);
        lineRenderer_vertical = verticalObj.GetComponent<LineRenderer>();
        //lineRenderer_vertical.positionCount = 4;
        lineRenderer_vertical.startWidth = 0.05f; // ���� �β�
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
        // ���簢�� ����� �׸��ϴ�.
        Vector3[] squarePositions = new Vector3[5];
        squarePositions[0] = center + (Vector3.up * size) - (Vector3.right * size); // ���� ��
        squarePositions[1] = center + (Vector3.up * size) + (Vector3.right * size); // ������ ��
        squarePositions[2] = center - (Vector3.up * size) + (Vector3.right * size); // ������ �Ʒ�
        squarePositions[3] = center - (Vector3.up * size) - (Vector3.right * size); // ���� �Ʒ�
        squarePositions[4] = squarePositions[0]; // ó�� ������ �����Ͽ� ���簢�� �ϼ�
        lineRenderer_square.SetPositions(squarePositions);

        // ������ �׸��ϴ�.
        Vector3[] horizontalPositions = new Vector3[2];
        horizontalPositions[0] = center - (Vector3.right * size); // ������� ��������
        horizontalPositions[1] = center + (Vector3.right * size); // ������� ����������
        lineRenderer_horizontal.SetPositions(horizontalPositions);

        // �������� �׸��ϴ�.
        Vector3[] verticalPositions = new Vector3[2];
        verticalPositions[0] = center - (Vector3.up * size);    // ������� �Ʒ���
        verticalPositions[1] = center + (Vector3.up * size);    // ������� ����
        lineRenderer_vertical.SetPositions(verticalPositions);

    }   
}
