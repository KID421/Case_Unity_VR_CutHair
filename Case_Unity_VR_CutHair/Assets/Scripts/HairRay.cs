using UnityEngine;
using System.Collections.Generic;

public class HairRay : MonoBehaviour
{
    public bool Up;
    public HairRay UpHairRay;
    public List<GameObject> Hairs = new List<GameObject>();

    [Header("0 前, 1 左, 2 右, 3 後")]
    public int HairType;

    private string[] TagName_Before = { "未檢查前髮", "未檢查左髮", "未檢查右髮", "未檢查後髮" };
    private string[] TagName_After = { "已檢查前髮", "已檢查左髮", "已檢查右髮", "已檢查後髮" };

    [Header("前：19, 左：0, 右：0")]
    public int TotalHair = 19;

    public static float Score;

    private bool IsCheckHair;
    private int index;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "已檢查頭髮") return;

        if (!Up && collision.gameObject.tag == TagName_Before[HairType])
        {
            Hairs.Add(collision.gameObject);
            collision.gameObject.tag = TagName_After[HairType];
            index++;
        }
        else if (Up && collision.gameObject.tag == TagName_After[HairType])
        {
            Hairs.Add(collision.gameObject);
            collision.gameObject.tag = "已檢查頭髮";
            index++;
        }
    }

    private void Update()
    {
        CheckHair();
    }

    private void CheckHair()
    {
        if (!IsCheckHair && !Up)
        {
            IsCheckHair = true;
            UpHairRay.gameObject.SetActive(true);

            Invoke("DelayCheckHair", 0.5f);
        }
    }

    private void DelayCheckHair()
    {
        string total = "";
        string totalUp = "";
        for (int i = 0; i < Hairs.Count; i++)
        {
            total += (Hairs[i].name + ", ");
        }
        for (int i = 0; i < UpHairRay.Hairs.Count; i++)
        {
            totalUp += (UpHairRay.Hairs[i] + ", ");
        }
        Debug.Log("上方檢查：" + Up + "\n" + total + "\n上方檢查：" + UpHairRay.Up + "\n" + totalUp);
        
        
        if (Hairs.Count == 0)
        {
            if (HairType == 1 || HairType == 2) Score += 0.5f;
            else Score += 1;
            Debug.Log("剪髮位置：" + HairType + " - 整齊： + 1 分，總分：" + Score);

            if (UpHairRay.Hairs.Count == TotalHair)
            {
                Score += 1;
                Debug.Log("剪髮位置：" + HairType + " - 長度正確： + 1 分，總分：" + Score);
            }
        }
        if (Score == 7 && HairType == 0)
        {
            Score += 4;
            Debug.Log("外型輪廓正確： + 4 分，總分：" + Score);
        }
        if (HairType == 1)
        {
            Score += 5;
            Debug.Log("左右兩側頭髮齊長： + 5 分，總分：" + Score);
        }

        if (HairType == 0)
        {
            Score += 3;
            Debug.Log("側部耳下無缺角： + 3 分，總分：" + Score);
            Score += 3;
            Debug.Log("剪髮技能熟練： + 3 分，總分：" + Score);
            Score += 4;
            Debug.Log("符合衛生標準： + 4 分，總分：" + Score);
        }
    }
}
