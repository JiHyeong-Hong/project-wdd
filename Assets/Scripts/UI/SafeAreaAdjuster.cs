using UnityEngine;
using UnityEngine.UI;

public class SafeAreaAdjuster : MonoBehaviour
{
    private RectTransform panelRectTransform;
    private Rect lastSafeArea = Rect.zero;

    // void Start()
    // {
    //     panelRectTransform = GetComponent<RectTransform>();
    //     ApplySafeArea();
    // }
    //
    // void ApplySafeArea()
    // {
    //     Debug.Log("테스트!");
    //     Rect safeArea = Screen.safeArea;
    //
    //     // Safe Area가 변경된 경우에만 업데이트
    //     if (safeArea != lastSafeArea)
    //     {
    //         lastSafeArea = safeArea;
    //
    //         // Safe Area를 기준으로 UI 패널 크기 및 위치 조정
    //         Vector2 anchorMin = safeArea.position;
    //         Vector2 anchorMax = safeArea.position + safeArea.size;
    //         anchorMin.x /= Screen.width;
    //         anchorMin.y /= Screen.height;
    //         anchorMax.x /= Screen.width;
    //         anchorMax.y /= Screen.height;
    //
    //         panelRectTransform.anchorMin = anchorMin;
    //         panelRectTransform.anchorMax = anchorMax;
    //     }
    // }

    // void Start()
    // {
    //     ApplySafeArea();
    // }
    //
    // void ApplySafeArea()
    // {
    //     Rect safeArea = Screen.safeArea;
    //     Debug.Log($"Rect Size : {safeArea.size}");
    //     Vector2 anchorMin = safeArea.position;
    //     Vector2 anchorMax = safeArea.position + safeArea.size;
    //     anchorMin.x /= Screen.width;
    //     anchorMin.y /= Screen.height;
    //     anchorMax.x /= Screen.width;
    //     anchorMax.y /= Screen.height;
    //
    //     RectTransform rectTransform = GetComponent<RectTransform>();
    //     rectTransform.anchorMin = anchorMin;
    //     rectTransform.anchorMax = anchorMax;
    // }

    // public Canvas canvas;
    //
    // void Start()
    // {
    //     AdjustCanvasToSafeArea();
    // }
    //
    // void AdjustCanvasToSafeArea()
    // {
    //     if (canvas == null)
    //     {
    //         Debug.LogError("Canvas reference is missing!");
    //         return;
    //     }
    //
    //     RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
    //     Rect safeArea = Screen.safeArea;
    //
    //     // Screen resolution
    //     float screenWidth = Screen.width;
    //     float screenHeight = Screen.height;
    //
    //     // Safe Area calculation
    //     Vector2 anchorMin = safeArea.position;
    //     Vector2 anchorMax = safeArea.position + safeArea.size;
    //     anchorMin.x /= screenWidth;
    //     anchorMin.y /= screenHeight;
    //     anchorMax.x /= screenWidth;
    //     anchorMax.y /= screenHeight;
    //
    //     // Apply Safe Area to Canvas RectTransform
    //     canvasRectTransform.anchorMin = anchorMin;
    //     canvasRectTransform.anchorMax = anchorMax;
    // }

    // public RectTransform targetRectTransform;
    //
    // void Start()
    // {
    //     AdjustToSafeArea();
    // }
    //
    // void AdjustToSafeArea()
    // {
    //     if (targetRectTransform == null)
    //     {
    //         Debug.LogError("RectTransform reference is missing!");
    //         return;
    //     }
    //
    //     Rect safeArea = Screen.safeArea;
    //
    //     // Screen resolution
    //     float screenWidth = Screen.width;
    //     float screenHeight = Screen.height;
    //
    //     // Safe Area calculation
    //     Vector2 anchorMin = safeArea.position;
    //     Vector2 anchorMax = safeArea.position + safeArea.size;
    //     anchorMin.x /= screenWidth;
    //     anchorMin.y /= screenHeight;
    //     anchorMax.x /= screenWidth;
    //     anchorMax.y /= screenHeight;
    //
    //     // Apply Safe Area to the RectTransform
    //     targetRectTransform.anchorMin = anchorMin;
    //     targetRectTransform.anchorMax = anchorMax;
    // }

    public RectTransform CanvasRectTransform;
    public RectTransform targetRectTransform;
    public RectTransform TopAreaRectTransform;
    public RectTransform TopMenuAreaRectTransform;
    public RectTransform MainAreaRectTransform;
    

    void Start()
    {
        MainAreaRectTransform = GameObject.Find("StagePreview").gameObject.GetComponent<RectTransform>();
        SetUIResolution();
        AdjustToSafeArea();
        CalculateAndLogTopGap();
    }

    void AdjustToSafeArea()
    {
        if (targetRectTransform == null)
        {
            Debug.LogError("RectTransform reference is missing!");
            return;
        }

        Rect safeArea = Screen.safeArea;
        Debug.Log($"Rect Size : {safeArea.size}");
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate Safe Area as normalized anchors
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= screenWidth;
        anchorMin.y /= screenHeight;
        anchorMax.x /= screenWidth;
        anchorMax.y /= screenHeight;

        // Apply Safe Area to RectTransform
        targetRectTransform.anchorMin = anchorMin;
        targetRectTransform.anchorMax = anchorMax;

        // Optionally adjust the size and position
        Vector2 offsetMin = new Vector2(safeArea.x, safeArea.y) / new Vector2(screenWidth, screenHeight);
        Vector2 offsetMax = new Vector2(safeArea.xMax, safeArea.yMax) / new Vector2(screenWidth, screenHeight);

        targetRectTransform.offsetMin = offsetMin;
        targetRectTransform.offsetMax = offsetMax;
    }


    void CalculateAndLogTopGap()
    {
        Rect safeArea = targetRectTransform.rect;
        Rect screen = CanvasRectTransform.rect;
        Rect TopArea = TopAreaRectTransform.rect;
        Rect TopMenuArea = TopMenuAreaRectTransform.rect;

        float screenHeight = screen.height;

        // Safe Area의 Top과 전체 화면의 Top 사이의 간격을 계산
        float safeAreaTop = safeArea.height;
        float screenTop = screenHeight;
        float screenTopMenu = TopMenuArea.height;

        Debug.Log($"Top Gap between Safe Area and Screen Top: {screenTop - safeAreaTop}");
        // 전체 화면에서 Safe Area의 Top까지의 간격
        float topGap = screenTop - safeAreaTop + screenTopMenu;

        Vector2 topAreaSize = TopAreaRectTransform.sizeDelta;
        if (topGap <= 100)
        {
            topAreaSize.y = screenTopMenu;
        }
        else
        {
            topAreaSize.y = topGap;
        }

        TopAreaRectTransform.sizeDelta = topAreaSize;

        // Debug.Log($"Top Gap between Safe Area and Screen Top: {topGap}");
        Debug.Log($"Top Gap between Safe Area and Screen Top: {TopArea}");
    }

    public void SetUIResolution()
    {
        // Vector2 TopMenuSize = TopMenuAreaRectTransform.sizeDelta;
        // TopMenuSize.y = 100f;
        // TopMenuAreaRectTransform.sizeDelta = TopMenuSize;
        
        Vector2 TopMenuSize = TopMenuAreaRectTransform.sizeDelta;
    
        // 새 높이 설정 (너비는 기존 값 유지)
        float newHeight = 100f;
        TopMenuAreaRectTransform.sizeDelta = new Vector2(TopMenuSize.x, newHeight);
    }
    
    // void SetUIResolution()
    // {
    //     if (TopMenuAreaRectTransform == null)
    //     {
    //         Debug.LogError("RectTransform reference is missing!");
    //         return;
    //     }
    //
    //     // 현재 사이즈 가져오기
    //     Vector2 currentSize = TopMenuAreaRectTransform.sizeDelta;
    //
    //     // 새 높이 설정 (너비는 기존 값 유지)
    //     float newHeight = 100f;
    //     TopMenuAreaRectTransform.sizeDelta = new Vector2(currentSize.x, newHeight);
    //
    //     // 새 사이즈가 적용된 후의 크기 확인
    //     Debug.Log($"New SizeDelta: {TopMenuAreaRectTransform.sizeDelta}");
    // }
}