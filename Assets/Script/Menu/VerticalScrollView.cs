using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;    //視窗
    [SerializeField] private RectTransform content;     //容器
    [SerializeField] private GameObject contentPrefab;  //內容項目的預製件

    [SerializeField] private int itemCount;             //內容項數
    [SerializeField] private float itemHeight;          //每個項目的高度
    [SerializeField] private float spacing;             //項目之間的間距
    [SerializeField] private float scrollSpeed = 200f;  //滾動速度

    private Vector2 contentStartPos;

    void Start()
    {
        //檔案選擇列表專用
        itemCount = MainManager.levelConfigFiles.Length;

        AdjustScrollView();
        contentStartPos = content.anchoredPosition;
    }

    void Update()
    {
        HandleScroll();
    }

    void AdjustScrollView()
    {
        //根據項目數量及其高度計算容器的高度
        float contentHeight = (itemHeight + spacing) * itemCount - spacing;

        //設定容器的大小
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

        //實例化內容項
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(contentPrefab, content, false);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            float anchoredPositionY = (contentHeight * 0.5f) - i * (itemHeight + spacing) - (itemHeight * 0.5f);
            itemRect.anchoredPosition = new Vector2(0, anchoredPositionY);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemHeight);


            //檔案選擇列表專用
            var buttonScript = itemRect.GetComponent<FileButton>();
            buttonScript.FileID = i;
        }

        //將內容位置重置到頂部
        content.anchoredPosition = ClampToBounds(new Vector2(content.anchoredPosition.x, contentHeight * -0.5f));
    }

    void HandleScroll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel") * (-scrollSpeed);
            Vector2 newPosition = content.anchoredPosition + new Vector2(0, scrollDelta);
            content.anchoredPosition = ClampToBounds(newPosition);
        }
    }

    Vector2 ClampToBounds(Vector2 position)
    {
        float maxY = viewport.rect.height - content.rect.height;
        return new Vector2(position.x, Mathf.Clamp(position.y, maxY * 0.5f, maxY * -0.5f));
    }
}