using UnityEngine;
using UnityEngine.UI;

public enum CutHairType { 水平, 逆斜, 正斜 }
public enum Room { 髮廊, 教室, 試場 }

public class CutHairExam : MonoBehaviour
{
    public CutHairType m_CutHairType;
    public Room m_Room;
    public GameObject[] CheckHairArea;
    public Text T_Cut, T_Wind;

    public void GetType(int number)
    {
        m_CutHairType = (CutHairType)number;
        Debug.Log("剪吹髮 - 目前選擇剪髮類型：" + m_CutHairType);
    }

    public void GetRoom(int number)
    {
        m_Room = (Room)number;
        Debug.Log("剪吹髮 - 目前選擇房間類型：" + m_Room);
    }

    public void CheckHair()
    {
        CheckHairArea[(int)m_CutHairType].SetActive(true);
        T_Cut.text = "剪髮分數：取得分數中...";
        Invoke("HairScore", 1f);
        StartWindHair();
    }

    public void HairScore()
    {
        HairRay.Score = Random.Range(5, 20);
        T_Cut.text = "剪髮分數：" + HairRay.Score;
    }

    public void StartWindHair()
    {
        GameManager.Instance.ChangeState(5);
    }

    public void CheckWind()
    {
        T_Wind.text = "吹髮分數：取得分數中...";
        Invoke("WindScore", 1f);
    }

    public void WindScore()
    {
        T_Wind.text = "吹髮分數：" + WindHair.Score;
    }
}
