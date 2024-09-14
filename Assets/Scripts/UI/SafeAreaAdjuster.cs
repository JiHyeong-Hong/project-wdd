using UnityEngine;
using UnityEngine.Serialization;
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
    public RectTransform SafeAreaRectTransform;
    public RectTransform TopAreaRectTransform;
    public RectTransform BottomAreaRectTransform;
    public RectTransform TopMenuAreaRectTransform;
    public RectTransform MainAreaRectTransform;
    public RectTransform TopMenuRectTransform;
    public RectTransform BottomMenuRectTransform;


    void Start()
    {
        // MainAreaRectTransform = GameObject.Find("StagePreview").gameObject.GetComponent<RectTransform>();
        // SetUIResolution();
        // AdjustToSafeArea();
        // CalculateAndLogTopGap();
        // AdjustSafeAreaAndMargins();
        AdjustSafeAreaAndMenus();
    }

    void AdjustToSafeArea()
    {
        if (SafeAreaRectTransform == null)
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
        SafeAreaRectTransform.anchorMin = anchorMin;
        SafeAreaRectTransform.anchorMax = anchorMax;

        // Optionally adjust the size and position
        Vector2 offsetMin = new Vector2(safeArea.x, safeArea.y) / new Vector2(screenWidth, screenHeight);
        Vector2 offsetMax = new Vector2(safeArea.xMax, safeArea.yMax) / new Vector2(screenWidth, screenHeight);

        SafeAreaRectTransform.offsetMin = offsetMin;
        SafeAreaRectTransform.offsetMax = offsetMax;
    }


    void CalculateAndLogTopGap()
    {
        Rect safeArea = SafeAreaRectTransform.rect;
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
        Vector2 TopMenuSize = TopMenuAreaRectTransform.sizeDelta;

        // 새 높이 설정 (너비는 기존 값 유지)
        float topHeight = 100f;
        TopMenuAreaRectTransform.sizeDelta = new Vector2(TopMenuSize.x, topHeight);

        Vector2 BottomMenuSize = BottomMenuRectTransform.sizeDelta;

        // 새 높이 설정 (너비는 기존 값 유지)
        float bottomHeight = 200f;
        BottomMenuRectTransform.sizeDelta = new Vector2(BottomMenuSize.x, bottomHeight);
    }

    // void AdjustSafeAreaAndMargins()
    // {
    //     if (SafeAreaRectTransform == null || TopAreaRectTransform == null || BottomAreaRectTransform == null)
    //     {
    //         Debug.LogError("One or more RectTransform references are missing!");
    //         return;
    //     }
    //
    //     // Get the screen safe area
    //     Rect safeArea = Screen.safeArea;
    //     float screenWidth = Screen.width;
    //     float screenHeight = Screen.height;
    //
    //     // Safe Area as normalized anchors
    //     Vector2 safeAreaMin = safeArea.position;
    //     Vector2 safeAreaMax = safeArea.position + safeArea.size;
    //     safeAreaMin.x /= screenWidth;
    //     safeAreaMin.y /= screenHeight;
    //     safeAreaMax.x /= screenWidth;
    //     safeAreaMax.y /= screenHeight;
    //
    //     // Apply Safe Area to SafeAreaRectTransform
    //     SafeAreaRectTransform.anchorMin = safeAreaMin;
    //     SafeAreaRectTransform.anchorMax = safeAreaMax;
    //
    //     // Calculate the size of top and bottom margins
    //     float bottomMargin = safeArea.yMin;
    //     float topMargin = screenHeight - safeArea.yMax;
    //
    //     // Normalize margins to screen height
    //     bottomMargin /= screenHeight;
    //     topMargin /= screenHeight;
    //
    //     // Adjust TopAreaRectTransform and BottomAreaRectTransform
    //     TopAreaRectTransform.anchorMin = new Vector2(0, 1 - topMargin);
    //     TopAreaRectTransform.anchorMax = new Vector2(1, 1);
    //     TopAreaRectTransform.offsetMin = Vector2.zero;
    //     TopAreaRectTransform.offsetMax = Vector2.zero;
    //
    //     BottomAreaRectTransform.anchorMin = new Vector2(0, 0);
    //     BottomAreaRectTransform.anchorMax = new Vector2(1, bottomMargin);
    //     BottomAreaRectTransform.offsetMin = Vector2.zero;
    //     BottomAreaRectTransform.offsetMax = Vector2.zero;
    //
    //     // Optionally log for debugging
    //     Debug.Log($"Top Margin: {topMargin * screenHeight}px, Bottom Margin: {bottomMargin * screenHeight}px");
    // }

    void AdjustSafeAreaAndMargins()
    {
        if (SafeAreaRectTransform == null || TopAreaRectTransform == null || BottomAreaRectTransform == null ||
            MainAreaRectTransform == null)
        {
            Debug.LogError("One or more RectTransform references are missing!");
            return;
        }

        // Get the screen safe area
        Rect safeArea = Screen.safeArea;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Safe Area as normalized anchors
        Vector2 safeAreaMin = safeArea.position;
        Vector2 safeAreaMax = safeArea.position + safeArea.size;
        safeAreaMin.x /= screenWidth;
        safeAreaMin.y /= screenHeight;
        safeAreaMax.x /= screenWidth;
        safeAreaMax.y /= screenHeight;

        // Apply Safe Area to SafeAreaRectTransform
        SafeAreaRectTransform.anchorMin = safeAreaMin;
        SafeAreaRectTransform.anchorMax = safeAreaMax;

        // Calculate the size of top and bottom margins
        float bottomMargin = safeArea.yMin;
        float topMargin = screenHeight - safeArea.yMax;

        // Normalize margins to screen height
        bottomMargin /= screenHeight;
        topMargin /= screenHeight;

        // Adjust TopAreaRectTransform and BottomAreaRectTransform
        TopAreaRectTransform.anchorMin = new Vector2(0, 1 - topMargin);
        TopAreaRectTransform.anchorMax = new Vector2(1, 1);
        TopAreaRectTransform.offsetMin = Vector2.zero;
        TopAreaRectTransform.offsetMax = Vector2.zero;

        BottomAreaRectTransform.anchorMin = new Vector2(0, 0);
        BottomAreaRectTransform.anchorMax = new Vector2(1, bottomMargin);
        BottomAreaRectTransform.offsetMin = Vector2.zero;
        BottomAreaRectTransform.offsetMax = Vector2.zero;

        // Adjust MainAreaRectTransform to fit between Top and Bottom areas
        MainAreaRectTransform.anchorMin = new Vector2(0, bottomMargin);
        MainAreaRectTransform.anchorMax = new Vector2(1, 1 - topMargin);
        MainAreaRectTransform.offsetMin = Vector2.zero;
        MainAreaRectTransform.offsetMax = Vector2.zero;

        // Optionally log for debugging
        Debug.Log($"Top Margin: {topMargin * screenHeight}px, Bottom Margin: {bottomMargin * screenHeight}px");
    }

    void AdjustSafeAreaAndMenus()
    {
        if (SafeAreaRectTransform == null || TopAreaRectTransform == null || BottomAreaRectTransform == null ||
            MainAreaRectTransform == null
            || TopMenuRectTransform == null || BottomMenuRectTransform == null)
        {
            Debug.LogError("One or more RectTransform references are missing!");
            return;
        }

        // Get the screen safe area
        Rect safeArea = Screen.safeArea;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Safe Area as normalized anchors
        Vector2 safeAreaMin = safeArea.position;
        Vector2 safeAreaMax = safeArea.position + safeArea.size;
        safeAreaMin.x /= screenWidth;
        safeAreaMin.y /= screenHeight;
        safeAreaMax.x /= screenWidth;
        safeAreaMax.y /= screenHeight;

        // Apply Safe Area to SafeAreaRectTransform
        SafeAreaRectTransform.anchorMin = safeAreaMin;
        SafeAreaRectTransform.anchorMax = safeAreaMax;

        // Calculate the size of top and bottom margins
        float topMargin = screenHeight - safeArea.yMax + TopMenuRectTransform.rect.height +0.0148726f;
        float bottomMargin = safeArea.yMin;

        float topMenuHeightPx = TopMenuRectTransform.rect.height;
        
        // Normalize margins to screen height
        topMargin /= screenHeight;
        bottomMargin /= screenHeight;
        

        // Adjust TopAreaRectTransform and BottomAreaRectTransform
        TopAreaRectTransform.anchorMin = new Vector2(0, 1 - topMargin );
        TopAreaRectTransform.anchorMax = new Vector2(1, 1);
        TopAreaRectTransform.offsetMin = Vector2.zero;
        TopAreaRectTransform.offsetMax = Vector2.zero;
        
        TopMenuRectTransform.anchorMin = new Vector2(0.5f, 0); // Center horizontally, align to bottom
        TopMenuRectTransform.anchorMax = new Vector2(0.5f, 0); // Center horizontally, align to bottom
        TopMenuRectTransform.pivot = new Vector2(0.5f, 0); // Set pivot to bottom center
        TopMenuRectTransform.offsetMin = new Vector2(-topMenuHeightPx / 2, 0); // Center horizontally by half of height
        TopMenuRectTransform.offsetMax = new Vector2(topMenuHeightPx / 2, topMenuHeightPx); // Center horizontally by half of height and height

        BottomAreaRectTransform.anchorMin = new Vector2(0, 0);
        BottomAreaRectTransform.anchorMax = new Vector2(1, bottomMargin);
        BottomAreaRectTransform.offsetMin = Vector2.zero;
        BottomAreaRectTransform.offsetMax = Vector2.zero;
        
        
        
        // Adjust MainAreaRectTransform to fit between the top and bottom menus
        MainAreaRectTransform.anchorMin = new Vector2(0, bottomMargin);
        MainAreaRectTransform.anchorMax = new Vector2(1, 1 - topMargin);
        MainAreaRectTransform.offsetMin = Vector2.zero;
        MainAreaRectTransform.offsetMax = Vector2.zero;

        // Optionally log for debugging
        Debug.Log($"Top Margin: {topMargin * screenHeight}px");
        // Debug.Log(
        // $"Bottom Margin: {bottomMargin * screenHeight}px, Bottom Menu Height: {bottomMenuHeight * screenHeight}px");
    }
}