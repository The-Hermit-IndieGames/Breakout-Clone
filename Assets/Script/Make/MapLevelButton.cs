using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapLevelButton : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform objectRectTransform;
    private MakingManager makingManager;
    public string levelID;
    public string levelName;

    private void Start()
    {
        // ������� RectTransform
        objectRectTransform = GetComponent<RectTransform>();

        makingManager = GameObject.Find("MakingManager").GetComponent<MakingManager>();

        UpdateButtonName();
    }

    //���d�W��----------------------------------------------------------------------------------
    [SerializeField] private TextMeshProUGUI MapButtonName;

    public void UpdateButtonName()
    {
        levelName = makingManager.GetLevelName(levelID);
        MapButtonName.text = levelName;
    }

    //���s����----------------------------------------------------------------------------------
    public void OnClick()
    {
        makingManager.nowButton = gameObject;
        makingManager.MapLoadLevel(levelID);
    }


    //�즲����----------------------------------------------------------------------------------
    [SerializeField] private RectTransform canvasRectTransform;
    public float dragSpeed = 2f;

    [SerializeField] private Vector2 minPosition; // �̤p�y�Э���
    [SerializeField] private Vector2 maxPosition; // �̤j�y�Э���

    private Vector2 oldPosition = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        if (!canvasRectTransform)
            return;


        if (oldPosition != Vector2.zero)
        {
            Vector2 positionFix;

            //�p���ܤƶq
            positionFix = eventData.position - oldPosition;

            if (positionFix.magnitude > 32)
            {
                Debug.Log("positionFix: " + positionFix + " magnitude=" + positionFix.magnitude);
                positionFix = Vector2.zero;
            }

            //�p�⪫�󪺷s��m
            Vector2 newPosition = ClampToCanvas(canvasRectTransform.anchoredPosition + (positionFix * dragSpeed));

            //��s���󪺮y��
            canvasRectTransform.anchoredPosition = newPosition;
        }

        oldPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        oldPosition = Vector2.zero;

        // �^�Ǯy��
        makingManager.UpDataPosition(levelID, objectRectTransform.anchoredPosition);
        makingManager.nowButton = gameObject;
    }

    // �N�y�Э���b�e���d��
    private Vector2 ClampToCanvas(Vector2 position)
    {
        // �N�y�Э���b���w�d��
        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        return position;
    }

    // ��s���󪺮y��(�ե�)
    public void UpDataCoordinate(Vector2 position)
    {
        //�p�⪫�󪺷s��m
        Vector2 newPosition = ClampToCanvas(position);

        //��s���󪺮y��
        objectRectTransform.anchoredPosition = newPosition;
    }
}