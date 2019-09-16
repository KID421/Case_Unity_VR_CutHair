using UnityEngine;
using UnityEngine.UI;

public enum ObjectType
{
    毛巾, 塑膠, 金屬, 金屬塑膠
}

public class ChemistryObject : MonoBehaviour
{
    /* 化學消毒 */
    public static ChemistryObject Instance;
    public int Step;                        // 目前步驟
    private ObjectType CurrentObjectType;   // 目前物件類型
    private int Score;                      // 分數紀錄
    private string StepName;                // 步驟名稱

    /* 步驟
     * 前處理 - 清洗乾淨
     * 操作要領 - 完全浸泡
     * 假設消毒時間已到，用夾子取出器具
     * 後處理 - 用水清洗 (金屬不用)
     * 瀝乾
     * 置於乾淨櫥櫃中，以備下次使用
     * */
    public Toggle[] TogGroup;
    public GameObject[] SayTip;               // 口述

    private void Start()
    {
        Instance = this;
    }

    // "修眉刀", "剪刀", "挖杓", "鑷子", "髮夾", "塑膠挖杓", "含金屬塑膠髮夾", "睫毛捲曲器", "毛巾類（白色）" 
    public void GetObjectType(string obj)
    {
        if (obj == "毛巾類（白色）")
        {
            CurrentObjectType = ObjectType.毛巾;
        }
        else if (obj == "塑膠挖杓")
        {
            CurrentObjectType = ObjectType.塑膠;
        }
        else if (obj == "修眉刀" || obj == "剪刀" || obj == "挖杓" || obj == "鑷子" || obj == "髮夾")
        {
            CurrentObjectType = ObjectType.金屬;
        }
        else if (obj == "含金屬塑膠髮夾" || obj == "睫毛捲曲器")
        {
            CurrentObjectType = ObjectType.金屬塑膠;
        }

        Debug.Log("化學消毒 - 接收到的物件：" + obj + " | 類型：" + CurrentObjectType);
    }

    /// <summary>
    /// 檢查每個步驟
    /// </summary>
    public void CheckStep(string stepName, string waterName = "")
    {
        StepName = stepName;

        switch (StepName)
        {
            case "水洗":
                if (Step == 0)
                {
                    TogGroup[Step].isOn = true;
                    SayTip[Step].SetActive(true);
                    Step++;
                    Score += 1;
                }
                break;

            case "放入消毒液":
                if (Step == 1)
                {
                    switch (CurrentObjectType)
                    {
                        case ObjectType.毛巾:
                            if (waterName == "氯液")
                            {
                                TogGroup[Step].isOn = true;
                                SayTip[Step].SetActive(true);
                                Step++;
                                Score += 3;
                            }
                            break;
                        case ObjectType.塑膠:
                            if (waterName == "氯液" || waterName == "陽性肥皂液" || waterName == "酒精" || waterName == "煤餾油酚肥皂溶液")
                            {
                                TogGroup[Step].isOn = true;
                                SayTip[Step].SetActive(true);
                                Step++;
                                Score += 3;
                            }
                            break;
                        case ObjectType.金屬:
                            if (waterName == "酒精" || waterName == "煤餾油酚肥皂溶液")
                            {
                                TogGroup[Step].isOn = true;
                                SayTip[Step].SetActive(true);
                                Step++;
                                Score += 3;
                            }
                            break;
                        case ObjectType.金屬塑膠:
                            if (waterName == "酒精" || waterName == "煤餾油酚肥皂溶液")
                            {
                                TogGroup[Step].isOn = true;
                                SayTip[Step].SetActive(true);
                                Step++;
                                Score += 3;
                            }
                            break;
                    }
                }
                break;
            case "蓋上":
                if (Step == 2)
                {
                    TogGroup[Step].isOn = true;
                    SayTip[Step].SetActive(true);
                    Step++;
                    if (CurrentObjectType == ObjectType.金屬) CheckStep("沖水");
                    Score += 4;
                }
                break;
            case "沖水":
                if (Step == 3)
                {
                    TogGroup[Step].isOn = true;
                    SayTip[Step].SetActive(true);
                    Step++;
                }
                break;
            case "濾網":
                if (Step == 4)
                {
                    TogGroup[Step].isOn = true;
                    SayTip[Step].SetActive(true);
                    Step++;
                }
                break;
            case "櫥櫃":
                if (Step == 5)
                {
                    TogGroup[Step].isOn = true;
                    SayTip[Step].SetActive(true);
                    Step++;
                    Score += 2;
                }
                break;
        }
        Debug.Log("化學消毒 - 分數：" + Score);
        DisifectionExam.Score = Score;
    }
}
