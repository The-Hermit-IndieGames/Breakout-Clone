using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MakingManager : MonoBehaviour
{
    //序列化定義
    [Serializable]
    public class BricksData
    {
        public int xPoint;
        public int yPoint;
        public int pointValue;
        public int brickLevel;
        public int powerUpType;
    }

    [Serializable]
    public class LevelConfig
    {
        public string level;
        public List<BricksData> bricksData;
    }

    [Serializable]
    public class Root
    {
        public List<LevelConfig> levelConfig;
    }

    //參數
    public TMP_InputField inputLevelsName;                                  //輸入格-關卡名
    public string levelsName = "Null";                                      //關卡名
    public string exportFileName;                                           //保存的JSON文件名
    public GameObject brickPrefab;                                          //磚塊的預置體
    public TMP_InputField inputBrickLevel;                                  //輸入格-磚塊等級
    public TMP_InputField inputPointValue;                                  //輸入格-破壞分數
    public TextMeshProUGUI textBrickLevel;                                  //數值-磚塊等級
    public TextMeshProUGUI textPointValue;                                  //數值-破壞分數

    //運行
    private Vector3 brickPosition;                                      //磚塊座標
    private List<GameObject> bricksInScene = new List<GameObject>();    //場景中的磚塊列表
    private BrickMake nowBrick;


    void Start()
    {
        GenerateBricks();
    }


    void Update()
    {
        // 檢查滑鼠按鍵點擊事件
        if (Input.GetMouseButtonDown(0)) // 左鍵
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButtonDown(1)) // 右鍵
        {
            HandleRightClick();
        }
    }

    //左鍵
    void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 使用射線檢測滑鼠點擊的位置
        if (Physics.Raycast(ray, out hit))
        {
            // 檢查是否點擊到了 Brick 物件
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                //指定nowBrick用於後續更改
                nowBrick = brick;

                //更新 brickLevel
                brick.brickLevel += 1;
                if (brick.brickLevel >= 6)
                {
                    brick.brickLevel = 0;
                }
                brick.UpdateLevel();

                //輸入條
                textBrickLevel.text = brick.brickLevel.ToString();
                textPointValue.text = brick.pointValue.ToString();
                inputBrickLevel.text = "";
                inputPointValue.text = "";
            }
        }
    }

    //輸入條(等級)
    public void OnInputBrickLevel()
    {
        //讀取 TMP Input Field 中的文本
        string inputText = inputBrickLevel.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.brickLevel = inputNumber;
        inputPointValue.text = "";
        nowBrick.UpdateLevel();
        textBrickLevel.text = nowBrick.brickLevel.ToString();
        textPointValue.text = nowBrick.pointValue.ToString();
    }

    //輸入條(分數)
    public void OnInputBrickScore()
    {
        //讀取 TMP Input Field 中的文本
        string inputText = inputPointValue.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.pointValue = inputNumber;
        textPointValue.text = nowBrick.pointValue.ToString();
    }

    //右鍵
    void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 使用射線檢測滑鼠點擊的位置
        if (Physics.Raycast(ray, out hit))
        {
            // 檢查是否點擊到了 Brick 物件
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                brick.UpdateItem();     //更新 powerUpType
            }
        }
    }


    //磚塊生成器
    void GenerateBricks()
    {
        for (int x = 1; x <= 12; x++)
        {
            for (int y = 1; y <= 16; y++)
            {
                brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity);
                bricksInScene.Add(brick);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    brickScript.pointValue = 0;
                    brickScript.brickLevel = 0;
                    brickScript.powerUpType = 0;
                }
            }
        }
    }


    //重新開始按鈕
    public void RestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //輸入條(關卡名)
    public void OnInputLevelsName()
    {
        //讀取 TMP Input Field 中的文本
        levelsName = inputLevelsName.text;
    }


    //匯出 JSON 按鈕
    public void ExportLevelConfigToJson()
    {
        // 創建一個新的Root對象
        Root root = new Root();
        root.levelConfig = new List<LevelConfig>();

        // 創建一個新的LevelConfig對象
        LevelConfig levelConfig = new LevelConfig();
        levelConfig.bricksData = new List<BricksData>();
        levelConfig.level = levelsName;

        // 收集場景中的磚塊數據
        foreach (GameObject brick in bricksInScene)
        {
            // 獲取磚塊腳本
            var brickScript = brick.GetComponent<BrickMake>();
            if (brickScript.brickLevel != 0)
            {
                BricksData brickData = new BricksData();
                brickData.xPoint = Mathf.RoundToInt((26 - brick.transform.position.x) / 4);
                brickData.yPoint = Mathf.RoundToInt(24.5f - brick.transform.position.y);
                brickData.pointValue = brickScript.pointValue;
                brickData.brickLevel = brickScript.brickLevel;
                brickData.powerUpType = brickScript.powerUpType;

                levelConfig.bricksData.Add(brickData);
            }
        }

        // 添加LevelConfig到Root
        root.levelConfig.Add(levelConfig);

        // 轉換為JSON字符串
        string json = JsonUtility.ToJson(root, true);

        // 寫入JSON文件
        exportFileName = levelsName + ".json";
        File.WriteAllText(exportFileName, json);

        Debug.Log("關卡配置已保存到 " + exportFileName);
    }


    //退出按鈕
    public void BackButtonClick()
    {
        // 載入 MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}
