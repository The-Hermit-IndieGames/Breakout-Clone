using UnityEngine;
using UnityEngine.EventSystems;

public class MapControl : MonoBehaviour, IDragHandler, IScrollHandler
{
    [SerializeField] private RectTransform mapRectTransform;
    public float dragSpeed = 2f;
    public float zoomSpeed = -0.25f;
    public float minZoom = 0.75f;
    public float maxZoom = 1.5f;
    public Vector2 boundaryMin;
    public Vector2 boundaryMax;

    private Vector2 oldPosition = Vector2.zero;

    private Vector2 nowBoundaryMin;
    private Vector2 nowBoundaryMax;

    private void Start()
    {
        nowBoundaryMin = boundaryMin + new Vector2(800, 450) + new Vector2(20, 20);
        nowBoundaryMax = boundaryMax - new Vector2(800, 450) - new Vector2(20, 20);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mapRectTransform)
            return;


        if (oldPosition != Vector2.zero)
        {
            Vector2 positionFix;

            //計算變化量
            positionFix = eventData.position - oldPosition;

            if (positionFix.magnitude > 32)
            {
                Debug.Log("positionFix: " + positionFix + " magnitude=" + positionFix.magnitude);
                positionFix = Vector2.zero;
            }

            //計算物件的新位置
            Vector2 newPosition = ClampToCanvas(mapRectTransform.anchoredPosition + (positionFix * dragSpeed));

            //更新物件的座標
            mapRectTransform.anchoredPosition = newPosition;
        }

        oldPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        oldPosition = Vector2.zero;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!mapRectTransform)
            return;

        float scrollDelta = -eventData.scrollDelta.y * zoomSpeed;
        float newScale = Mathf.Clamp(mapRectTransform.localScale.x + scrollDelta, minZoom, maxZoom);
        mapRectTransform.localScale = Vector3.one * newScale;

        nowBoundaryMin = boundaryMin * newScale + new Vector2(800, 450) + new Vector2(20, 20);
        nowBoundaryMax = boundaryMax * newScale - new Vector2(800, 450) - new Vector2(20, 20);
        Debug.Log("Object Scale: " + newScale);

        mapRectTransform.anchoredPosition = ClampToCanvas(mapRectTransform.anchoredPosition);
    }

    private Vector2 ClampToCanvas(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, nowBoundaryMin.x, nowBoundaryMax.x);
        position.y = Mathf.Clamp(position.y, nowBoundaryMin.y, nowBoundaryMax.y);
        return position;
    }
}
