using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PointToTargetUI : MonoBehaviour
{
    public Transform target; // 가리키게 될 대상 오브젝트
    public Image arrowImage; // 화살표 UI 이미지
    public Canvas canvas; // 캔버스 UI 오브젝트
    public Transform rotBase;
    public TMP_Text distanceText;

    public RectTransform thumbnailRect;

    private Vector3 targetDirection;
    private Quaternion arrowRotation;
    private float updateInterval = 0.1f;
    private RectTransform arrowRectTransform;

    private void Start()
    {
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

            thumbnailRect.position = Util.LimitScreenConverter(target.position, thumbnailRect.sizeDelta * 0.7f, arrowImage.gameObject);

            // 스크린의 중앙을 봐야함
            Vector3 dir = thumbnailRect.position - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

            distanceText.rectTransform.position = thumbnailRect.position + dir.normalized * -thumbnailRect.sizeDelta.y;
            
            distanceText.text = $"{(target.position - Managers.Object.Hero.transform.position).magnitude:F1}m";

            yield return YieldInstructionCache.WaitForSecondsRealtime(updateInterval);
        }

    }
}