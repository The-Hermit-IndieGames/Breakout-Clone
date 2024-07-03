using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;    //����
    [SerializeField] private RectTransform content;     //�e��
    [SerializeField] private GameObject contentPrefab;  //���e���ت��w�s��

    [SerializeField] private int itemCount;             //���e����
    [SerializeField] private float itemHeight;          //�C�Ӷ��ت�����
    [SerializeField] private float spacing;             //���ؤ��������Z
    [SerializeField] private float scrollSpeed = 200f;  //�u�ʳt��

    private Vector2 contentStartPos;

    void Start()
    {
        //�ɮ׿�ܦC��M��
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
        //�ھڶ��ؼƶq�Ψ䰪�׭p��e��������
        float contentHeight = (itemHeight + spacing) * itemCount - spacing;

        //�]�w�e�����j�p
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

        //��ҤƤ��e��
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(contentPrefab, content, false);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            float anchoredPositionY = (contentHeight * 0.5f) - i * (itemHeight + spacing) - (itemHeight * 0.5f);
            itemRect.anchoredPosition = new Vector2(0, anchoredPositionY);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemHeight);


            //�ɮ׿�ܦC��M��
            var buttonScript = itemRect.GetComponent<FileButton>();
            buttonScript.FileID = i;
        }

        //�N���e��m���m�쳻��
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