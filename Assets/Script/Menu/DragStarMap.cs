using UnityEngine;
using UnityEngine.EventSystems;

public class DragStarMap : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Camera mainCamera;

    // 設置攝影機位置和目標平面
    public Vector3 cameraPosition = new Vector3(0, 0, -20);
    public float targetPlaneZ = 10f;

    // 螢幕邊界
    public Vector2 screenMax = new Vector2(80f, 45f);

    // 物件大小
    public Vector3 objectSize = new Vector3(320f, 180f, 1f);

    // 與螢幕邊界距離
    public float xBoundsDistance = 2.5f;
    public float yBoundsDistance = 2.5f;

    // 當前拖曳邊界
    private Vector2 currentBoundsMin;
    private Vector2 currentBoundsMax;

    // 偏移量閾值，用於限制初始錯誤位移
    public float maxAllowedOffset = 1f;

    // 縮放參數
    public float zoomSpeed = 1f;
    public float minScale = 0.5f;
    public float maxScale = 2f;

    private float nowScale = 1f;

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = cameraPosition;
        CalculateBounds();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }

        if (isDragging)
        {
            OnMouseDrag();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            OnMouseScroll(Input.mouseScrollDelta.y);
        }
    }

    void OnMouseDown()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        if (IsMouseOverObject(mouseWorldPosition))
        {
            offset = transform.position - mouseWorldPosition;

            // 檢查偏移量是否超過閾值
            if (offset.magnitude <= maxAllowedOffset)
            {
                isDragging = true;
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void OnMouseDrag()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 targetPosition = mouseWorldPosition + offset;

        // 限制目標位置在當前邊界內
        ClampPosition(ref targetPosition);

        transform.position = targetPosition;
    }

    void OnMouseScroll(float scrollAmount)
    {
        float scaleFactor = 1 + scrollAmount * zoomSpeed * Time.deltaTime;
        nowScale = Mathf.Clamp(transform.localScale.x * scaleFactor, minScale, maxScale);

        scaleFactor = nowScale / transform.localScale.x;

        transform.localScale *= scaleFactor;

        // 確保物件位置在新的邊界內
        CalculateBounds();
        Vector3 clampedPosition = transform.position;
        ClampPosition(ref clampedPosition);
        transform.position = clampedPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = targetPlaneZ - mainCamera.transform.position.z; // 計算攝影機到目標平面的距離
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private bool IsMouseOverObject(Vector3 mouseWorldPosition)
    {
        // 檢查滑鼠位置是否在物件的碰撞範圍內
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null)
        {
            return collider2D.OverlapPoint(mouseWorldPosition);
        }

        Collider collider3D = GetComponent<Collider>();
        if (collider3D != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            return collider3D.Raycast(ray, out hit, Mathf.Infinity);
        }

        return false;
    }

    private void ClampPosition(ref Vector3 position)
    {
        // 限制目標位置在邊界內
        position.x = Mathf.Clamp(position.x, currentBoundsMin.x, currentBoundsMax.x);
        position.y = Mathf.Clamp(position.y, currentBoundsMin.y, currentBoundsMax.y);
    }

    private void CalculateBounds()
    {
        // 計算邊界
        float xDistance = (objectSize.x * nowScale - screenMax.x) * 0.5f + xBoundsDistance;
        float yDistance = (objectSize.y * nowScale - screenMax.y) * 0.5f + yBoundsDistance;

        // 更新邊界
        currentBoundsMin = new Vector2(-xDistance, -yDistance);
        currentBoundsMax = new Vector2(xDistance, yDistance);
    }
}