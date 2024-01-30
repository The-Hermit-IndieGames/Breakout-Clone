using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PaddleControl : MonoBehaviour
{
    public Camera mainCamera;   // �bInspector���������w�D��v��
    private float targetX = 0f; // �x�sX���q���ؼЭ�


    //���ʱ���
    private void Update()
    {
        //�u���\�b�C���B��ɲ��ʡA�_�h��^
        if (GameData.gameRunning && !GameData.gameOver)
        {

            // ����ƹ���m
            Vector3 mousePosition = Input.mousePosition;

            // �NZ�y�г]�m��45�A�H�ϵJ�I������Z=0 (�۾���m��Z=45)
            mousePosition.z = 45;

            // �ϥ�Camera.ScreenToWorldPoint�N�ƹ���m�ഫ���@�ɮy��
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // �NX���q�x�s��targetX�A�í���b���w�d��
            targetX = Mathf.Clamp(worldPosition.x, -GameData.boundaryX, GameData.boundaryX);

            // ��s�ƪO����m
            Vector3 paddlePosition = transform.position;
            paddlePosition.x = targetX;
            transform.position = paddlePosition;
        }
    }
}
