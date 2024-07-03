using UnityEngine;

public class BallControl : MonoBehaviour
{
    private Vector3 paddleToBallVector;
    private Rigidbody rb;

    private int startingStage = 0;

    [SerializeField] private LineRenderer lineRenderer;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        paddleToBallVector = transform.position - FindObjectOfType<PaddleControl>().transform.position;
        lineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        //[遊戲開始前檢測]
        if (!GameData.gameStarted && startingStage <= 1)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }
    }


    //遊戲開始前鎖定位置
    private void LockBallToPaddle()
    {
        Vector3 paddlePos = FindObjectOfType<PaddleControl>().transform.position;
        transform.position = paddlePos + paddleToBallVector;
    }


    //點擊滑鼠時開始遊戲
    private void LaunchOnMouseClick()
    {
        if (startingStage == 0)
        {
            //選擇發射位置、鎖定
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameData.gameRunning = false;
                startingStage = 1;
                lineRenderer.gameObject.SetActive(true);
            }
        }
        else if (startingStage == 1)
        {
            //選擇發射角度、發射

            //讀取滑鼠在螢幕上的位置 轉換為世界座標
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            //更新預覽軌跡
            UpdatePreview(transform.position, worldPosition);

            //發射! 開始遊戲
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.GameStarted();
                lineRenderer.gameObject.SetActive(false);
                Vector3 vector3 = worldPosition - transform.position;
                rb.velocity = vector3.normalized * GameData.initialSpeed;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //取消
                GameData.gameRunning = true;
                startingStage = 0;
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }


    //更新預覽軌跡
    public void UpdatePreview(Vector3 startPosition, Vector3 endPosition)
    {
        // 設置Line Renderer的起始和終止點
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

}
