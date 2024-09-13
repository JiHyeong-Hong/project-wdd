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
    
    public RectTransform targetRectTransform;
    public RectTransform TopAreaRectTransform;
    public RectTransform TopMenuAreaRectTransform;
    public RectTransform CanvasRectTransform;
    
    void Start()
    {
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
    
        // 전체 화면에서 Safe Area의 Top까지의 간격
        float topGap = screenTop - safeAreaTop + screenTopMenu;

        Vector2 topAreaSize = TopAreaRectTransform.sizeDelta;
        topAreaSize.y = topGap;
        TopAreaRectTransform.sizeDelta = topAreaSize;
    
        Debug.Log($"Top Gap between Safe Area and Screen Top: {topGap}");
        Debug.Log($"Top Gap between Safe Area and Screen Top: {TopArea}");
    }
    // void CalculateAndLogTopGap()
    // {
    //     if (targetRectTransform == null)
    //     {
    //         Debug.LogError("RectTransform reference is missing!");
    //         return;
    //     }
    //     
    //     Rect safeArea = Screen.safeArea;
    //     float screenHeight = Screen.height;
    //     
    //     // Calculate the top edge of the Safe Area
    //     float safeAreaTop = screenHeight - safeArea.yMax;
    //     
    //     // Calculate the top edge of the entire screen
    //     float screenTop = screenHeight;
    //     
    //     // Calculate the gap between the Safe Area and the top of the screen
    //     float topGap = screenTop - safeAreaTop;
    //     
    //     Debug.Log($"Top Gap between Safe Area and Screen Top: {topGap}");
        
        // if (targetRectTransform == null)
        // {
        //     Debug.LogError("RectTransform reference is missing!");
        //     return;
        // }
        //
        //
        // Rect safeArea = targetRectTransform.rect;
        // float screenHeight = Screen.height;
        //
        // // Calculate Safe Area's top position relative to screen
        // Debug.Log($"{}");
        // float safeAreaTop = screenHeight - safeArea.yMax;
        //
        // // Calculate the distance from the top of the screen to Safe Area's top edge
        // float topGap = safeAreaTop;
        //
        // Debug.Log($"Top Gap between Safe Area and Screen Top: {topGap}");
    // }
}