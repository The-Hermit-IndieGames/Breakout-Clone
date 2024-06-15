using UnityEngine;
using UnityEngine.EventSystems;

public class DragStarMap : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Camera mainCamera;

    // �]�m��v����m�M�ؼХ���
    public Vector3 cameraPosition = new Vector3(0, 0, -20);
    public float targetPlaneZ = 10f;

    // �ù����
    public Vector2 screenMax = new Vector2(80f, 45f);

    // ����j�p
    public Vector3 objectSize = new Vector3(320f, 180f, 1f);

    // �P�ù���ɶZ��
    public float xBoundsDistance = 2.5f;
    public float yBoundsDistance = 2.5f;

    // ��e�즲���
    private Vector2 currentBoundsMin;
    private Vector2 currentBoundsMax;

    // �����q�H�ȡA�Ω󭭨��l���~�첾
    public float maxAllowedOffset = 1f;

    // �Y��Ѽ�
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

            // �ˬd�����q�O�_�W�L�H��
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

        // ����ؼЦ�m�b��e��ɤ�
        ClampPosition(ref targetPosition);

        transform.position = targetPosition;
    }

    void OnMouseScroll(float scrollAmount)
    {
        float scaleFactor = 1 + scrollAmount * zoomSpeed * Time.deltaTime;
        nowScale = Mathf.Clamp(transform.localScale.x * scaleFactor, minScale, maxScale);

        scaleFactor = nowScale / transform.localScale.x;

        transform.localScale *= scaleFactor;

        // �T�O�����m�b�s����ɤ�
        CalculateBounds();
        Vector3 clampedPosition = transform.position;
        ClampPosition(ref clampedPosition);
        transform.position = clampedPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = targetPlaneZ - mainCamera.transform.position.z; // �p����v����ؼХ������Z��
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private bool IsMouseOverObject(Vector3 mouseWorldPosition)
    {
        // �ˬd�ƹ���m�O�_�b���󪺸I���d��
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
        // ����ؼЦ�m�b��ɤ�
        position.x = Mathf.Clamp(position.x, currentBoundsMin.x, currentBoundsMax.x);
        position.y = Mathf.Clamp(position.y, currentBoundsMin.y, currentBoundsMax.y);
    }

    private void CalculateBounds()
    {
        // �p�����
        float xDistance = (objectSize.x * nowScale - screenMax.x) * 0.5f + xBoundsDistance;
        float yDistance = (objectSize.y * nowScale - screenMax.y) * 0.5f + yBoundsDistance;

        // ��s���
        currentBoundsMin = new Vector2(-xDistance, -yDistance);
        currentBoundsMax = new Vector2(xDistance, yDistance);

        Debug.Log(currentBoundsMin + " , " + currentBoundsMax);
    }
}