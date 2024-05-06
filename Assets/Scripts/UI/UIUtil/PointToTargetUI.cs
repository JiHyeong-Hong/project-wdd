using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PointToTargetUI : MonoBehaviour
{
    public Transform target; // 가리키게 될 대상 오브젝트
    public Image arrowImage; // 화살표 UI 이미지
    public Canvas canvas; // 캔버스 UI 오브젝트
    public Transform rotBase;

    public RectTransform thumbnailRect;

    private Vector3 targetDirection;
    private Quaternion arrowRotation;
    private float updateInterval = 0.1f;
    private WaitForSeconds waitTime;
    private RectTransform arrowRectTransform;

    private void Start()
    {
        waitTime = new WaitForSeconds(updateInterval);
        arrowRectTransform = arrowImage.GetComponent<RectTransform>();
        StartCoroutine(UpdateArrowDirection());
    }

    private IEnumerator UpdateArrowDirection()
    {
        while (true)
        {
            targetDirection = Camera.main.WorldToScreenPoint(target.position) - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            arrowRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rotBase.rotation = arrowRotation;

            thumbnailRect.position = Util.LimitScreenConverter(target.position, thumbnailRect.sizeDelta * 0.7f);

            yield return waitTime;
        }

    }
}