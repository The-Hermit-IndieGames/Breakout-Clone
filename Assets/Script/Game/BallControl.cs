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
        //[�C���}�l�e�˴�]
        if (!GameData.gameStarted && startingStage <= 1)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }
    }


    //�C���}�l�e��w��m
    private void LockBallToPaddle()
    {
        Vector3 paddlePos = FindObjectOfType<PaddleControl>().transform.position;
        transform.position = paddlePos + paddleToBallVector;
    }


    //�I���ƹ��ɶ}�l�C��
    private void LaunchOnMouseClick()
    {
        if (startingStage == 0)
        {
            //��ܵo�g��m�B��w
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameData.gameRunning = false;
                startingStage = 1;
                lineRenderer.gameObject.SetActive(true);
            }
        }
        else if (startingStage == 1)
        {
            //��ܵo�g���סB�o�g

            //Ū���ƹ��b�ù��W����m �ഫ���@�ɮy��
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            //��s�w���y��
            UpdatePreview(transform.position, worldPosition);

            //�o�g! �}�l�C��
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
                //����
                GameData.gameRunning = true;
                startingStage = 0;
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }


    //��s�w���y��
    public void UpdatePreview(Vector3 startPosition, Vector3 endPosition)
    {
        // �]�mLine Renderer���_�l�M�פ��I
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

}
