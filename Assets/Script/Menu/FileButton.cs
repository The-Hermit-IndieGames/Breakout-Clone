using UnityEngine;
using TMPro;


public class FileButton : MonoBehaviour
{
    public int FileID;

    private string nameString;
    private string makerString;
    private string versionString;
    private string descriptionString;

    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private TextMeshProUGUI makerTMP;
    [SerializeField] private TextMeshProUGUI versionTMP;
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    void Start()
    {
        nameString = MainManager.levelConfigFiles[FileID].name;
        makerString = MainManager.levelConfigFiles[FileID].maker;
        versionString = MainManager.levelConfigFiles[FileID].version;
        descriptionString = MainManager.levelConfigFiles[FileID].description;


        nameTMP.text = nameString;
        makerTMP.text = "by "+makerString;
        versionTMP.text = "Ver."+versionString;
        descriptionTMP.text = "Description: \n" + descriptionString;
    }

    public void ClickButton()
    {
        MainManager.nowFileId = FileID;

        var mainUIManager = GameObject.Find("MainUIManager").GetComponent<MainUIManager>();
        mainUIManager.OpenLevelsStarMap();
    }
}
