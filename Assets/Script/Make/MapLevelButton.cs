using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapLevelButton : MonoBehaviour, IDragHandler, IEndDragHandler
{
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
    private RectTransform objectRectTransform;
    private Vector2 oldPosition = Vector2.zero;

    public Vector2 minPosition; // �̤p�y�Э���
    public Vector2 maxPosition; // �̤j�y�Э���

    public void OnDrag(PointerEventData eventData)
    {
        if (oldPosition != Vector2.zero)
        {
            Vector2 positionFix;

            //�p���ܤƶq
            positionFix = eventData.position - oldPosition;

            //�p�⪫�󪺷s��m
            Vector2 newPosition = ClampToCanvas(objectRectTransform.anchoredPosition + positionFix);

            //��s���󪺮y��
            objectRectTransform.anchoredPosition = newPosition;
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
}