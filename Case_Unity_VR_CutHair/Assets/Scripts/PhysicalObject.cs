using UnityEngine;
using UnityEngine.UI;

public enum PhysicalType
{
    Boiled = 1, Vapor, UVRaysoil
}

public class PhysicalObject : MonoBehaviour
{
    /* 物理消毒 */
    public static PhysicalObject Instance;
    public int Step;                        // 目前步驟
    private ObjectType CurrentObjectType;   // 目前物件類型
    private int Score;                      // 分數紀錄
    private string StepName;                // 步驟名稱
    public PhysicalType m_PhysicalType;     // 物理消毒類型

    public GameObject[] CanTip;
    public Toggle[] TogBoiled, TogVapor, TogUVRays;
    public GameObject[] SayBoiled, SayVapor, SayUVRays;

    /* 步驟 - 煮沸消毒法：金屬、毛巾
     * 前處理 - 清洗乾淨
     * 操作要領 - 完全浸泡，水量一次加足
     * 消毒條件 - 水溫 100 度 C 以上，時間 5 分鐘以上
     * 假設消毒時間已到，瀝乾
     * 置於乾淨櫥櫃中，以備下次使用
     * */

    /* 步驟 - 蒸氣消毒法：毛巾
     * 前處理 - 清洗乾淨
     * 摺成弓字形
     * 毛巾直立置入，切勿擁擠
     * 蒸氣箱中心溫度達 80 度 C 以上，消毒時間 10 分鐘以上
     * 暫存蒸氣消毒箱，以備下次使用
     */

    /* 步驟 - 紫外線消毒法：金屬
     * 前處理 - 清洗乾淨
     * 以面紙將金屬剃刀擦拭乾淨
     * 刀剪類器具不可重疊，要打開或拆開
     * 光度強度每平方公分 85 微瓦特以上，消毒時間 20 分鐘以上，暫存紫外線消毒箱內，以備下次使用
     */

    private void Start()
    {
        Instance = this;
    }

    public void GetPhysicalType(int randomType)
    {
        CanTip[randomType - 1].SetActive(true);
        m_PhysicalType = (PhysicalType)randomType;
        Debug.Log("物理消毒 - 目前抽中的物理消毒法：" + m_PhysicalType);
    }

    public void GetObjectType(string obj)
    {
        if (obj == "毛巾類（白色）")
        {
            CurrentObjectType = ObjectType.毛巾;
        }
        else if (obj == "修眉刀" || obj == "剪刀" || obj == "挖杓" || obj == "鑷子" || obj == "髮夾")
        {
            CurrentObjectType = ObjectType.金屬;
        }

        Debug.Log("物理消毒 - 接收到的物件：" + obj + " | 類型：" + CurrentObjectType);
    }

    /// <summary>
    /// 檢查每個步驟
    /// </summary>
    public void CheckStep(string stepName)
    {
        StepName = stepName;

        switch (m_PhysicalType)
        {
            case PhysicalType.Boiled:
                switch (StepName)
                {
                    case "水洗":
                        if (Step == 0)
                        {
                            TogBoiled[Step].isOn = true;
                            SayBoiled[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                    case "完全浸泡":
                        if (Step == 1)
                        {
                            TogBoiled[Step].isOn = true;
                            SayBoiled[Step].SetActive(true);
                            Step++;
                            Score += 4;
                        }
                        break;
                    case "蓋上鍋蓋":
                        if (Step == 2)
                        {
                            TogBoiled[Step].isOn = true;
                            SayBoiled[Step].SetActive(true);
                            Step++;
                            Score += 4;
                        }
                        break;
                    case "濾網":
                        if (Step == 3)
                        {
                            TogBoiled[Step].isOn = true;
                            SayBoiled[Step].SetActive(true);
                            Step++;
                        }
                        break;
                    case "櫥櫃":
                        if (Step == 4)
                        {
                            TogBoiled[Step].isOn = true;
                            SayBoiled[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                }
                break;
            case PhysicalType.Vapor:
                if (CurrentObjectType != ObjectType.毛巾) return;
                switch (StepName)
                {
                    case "水洗":
                        if (Step == 0)
                        {
                            TogVapor[Step].isOn = true;
                            SayVapor[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                    case "開蒸氣門":    // 摺成弓字形
                        if (Step == 1)
                        {
                            TogVapor[Step].isOn = true;
                            SayVapor[Step].SetActive(true);
                            Step++;
                        }
                        break;
                    case "直立置入":
                        if (Step == 2)
                        {
                            TogVapor[Step].isOn = true;
                            SayVapor[Step].SetActive(true);
                            Step++;
                            Score += 4;
                        }
                        break;
                    case "關蒸氣門":
                        if (Step == 3)
                        {
                            TogVapor[Step].isOn = true;
                            SayVapor[Step].SetActive(true);
                            Step++;
                            Score += 4;
                            CheckStep("暫存蒸氣消毒箱");
                        }
                        break;
                    case "暫存蒸氣消毒箱":
                        if (Step == 4)
                        {
                            TogVapor[Step].isOn = true;
                            SayVapor[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                }
                break;
            case PhysicalType.UVRaysoil:
                if (CurrentObjectType == ObjectType.毛巾) return;
                switch (StepName)
                {
                    case "水洗":
                        if (Step == 0)
                        {
                            TogUVRays[Step].isOn = true;
                            SayUVRays[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                    case "面紙":
                        if (Step == 1)
                        {
                            TogUVRays[Step].isOn = true;
                            SayUVRays[Step].SetActive(true);
                            Step++;
                            Score += 4;
                        }
                        break;
                    case "放入紫外線":
                        if (Step == 2)
                        {
                            TogUVRays[Step].isOn = true;
                            SayUVRays[Step].SetActive(true);
                            Step++;
                            Score += 4;
                        }
                        break;
                    case "關閉紫外線":
                        if (Step == 3)
                        {
                            TogUVRays[Step].isOn = true;
                            SayUVRays[Step].SetActive(true);
                            Step++;
                            Score += 1;
                        }
                        break;
                }
                break;
        }

        Debug.Log("物理消毒 - 消毒法：" + m_PhysicalType + " | 步驟：" + Step + " | 收到：" + stepName);
        Debug.Log("物理消毒 - 分數：" + Score);
        DisifectionExam.Score += Score;
    }
}
