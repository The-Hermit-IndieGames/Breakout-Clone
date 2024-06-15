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
        // 獲取物件的 RectTransform
        objectRectTransform = GetComponent<RectTransform>();

        makingManager = GameObject.Find("MakingManager").GetComponent<MakingManager>();

        UpdateButtonName();
    }

    //關卡名稱----------------------------------------------------------------------------------
    [SerializeField] private TextMeshProUGUI MapButtonName;

    public void UpdateButtonName()
    {
        levelName = makingManager.GetLevelName(levelID);
        MapButtonName.text = levelName;
    }

    //按鈕部分----------------------------------------------------------------------------------
    public void OnClick()
    {
        makingManager.nowButton = gameObject;
        makingManager.MapLoadLevel(levelID);
    }


    //拖曳部分----------------------------------------------------------------------------------
    [SerializeField] private RectTransform canvasRectTransform;
    public float dragSpeed = 2f;

    [SerializeField] private Vector2 minPosition; // 最小座標限制
    [SerializeField] private Vector2 maxPosition; // 最大座標限制

    private Vector2 oldPosition = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        if (!canvasRectTransform)
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
            Vector2 newPosition = ClampToCanvas(canvasRectTransform.anchoredPosition + (positionFix * dragSpeed));

            //更新物件的座標
            canvasRectTransform.anchoredPosition = newPosition;
        }

        oldPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        oldPosition = Vector2.zero;

        // 回傳座標
        makingManager.UpDataPosition(levelID, objectRectTransform.anchoredPosition);
        makingManager.nowButton = gameObject;
    }

    // 將座標限制在畫布範圍內
    private Vector2 ClampToCanvas(Vector2 position)
    {
        // 將座標限制在指定範圍內
        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        return position;
    }

    // 更新物件的座標(調用)
    public void UpDataCoordinate(Vector2 position)
    {
        //計算物件的新位置
        Vector2 newPosition = ClampToCanvas(position);

        //更新物件的座標
        objectRectTransform.anchoredPosition = newPosition;
    }
}