using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PaddleControl : MonoBehaviour
{
    public Camera mainCamera;   // 在Inspector視窗中指定主攝影機
    private float targetX = 0f; // 儲存X分量的目標值


    //移動控制
    private void Update()
    {
        //只允許在遊戲運行時移動，否則返回
        if (GameData.gameRunning && !GameData.gameOver)
        {

            // 獲取滑鼠位置
            Vector3 mousePosition = Input.mousePosition;

            // 將Z座標設置為45，以使焦點平面為Z=0 (相機位置為Z=45)
            mousePosition.z = 45;

            // 使用Camera.ScreenToWorldPoint將滑鼠位置轉換為世界座標
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // 將X分量儲存到targetX，並限制在指定範圍內
            targetX = Mathf.Clamp(worldPosition.x, -GameData.boundaryX, GameData.boundaryX);

            // 更新滑板的位置
            Vector3 paddlePosition = transform.position;
            paddlePosition.x = targetX;
            transform.position = paddlePosition;
        }
    }
}
