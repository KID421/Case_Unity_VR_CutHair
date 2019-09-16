using UnityEngine;
using UnityEngine.UI;
using System;

public class CosmeticExam : MonoBehaviour
{
    public Texture[] TextBox;
    public GameObject Box;
    [Header("化妝品辨識紙")]
    public GameObject ExamPaper;
    public Text TextDate, TextCardNumber, TextName, TextNumber, TextScore;
    public Animator AniScore;

    public CosmeticAnswer[] CA;

    private int Number;
    private int Index;

    public bool[] CurrentAnswer = new bool[12];
    public int[] CurrentAnswerNumber = new int[3];
    public bool CurrentFinal;

    public static int Score;

    private void Start()
    {
        GameManager.Instance.onStart += RandomExam;
        GameManager.Instance.onEnd += Finish;
        GameManager.Instance.onCosmetic += Initilize;
    }

    private void Initilize()
    {
        
    }

    private void Finish()
    {
        if (GameManager.Instance.m_ExamState != ExamState.Cosmetic) return;
            AniScore.SetTrigger("In");
        CheckAnswer();
    }

    public void RandomExam()
    {
        if (GameManager.Instance.m_ExamState == ExamState.Cosmetic)
        {
            /* 2019.01.16 開啟 */
            Number = UnityEngine.Random.Range(0, TextBox.Length);
            Box.GetComponent<MeshRenderer>().materials[0].mainTexture = TextBox[Number];
            Box.SetActive(true);
            
            /* 2019.01.16 關閉 化妝品辨識 - 測試
            ExamPaper.SetActive(true);
            */

            TextChange();
            ResetCosmetic();
        }
    }

    private void TextChange()
    {
        TextDate.text = DateTime.Now.ToString("yyyy/MM/dd");
        TextCardNumber.text = (Number + 1).ToString();
        TextName.text = GameManager.Instance.ID;
        TextNumber.text = PlayerPrefs.GetInt("Number").ToString("000000");
    }

    public void AnswerIndex(int index)
    {
        Index = index;
    }

    public void SaveAnswer(bool question)
    {
        CurrentAnswer[Index] = question;
    }

    public void SaveAnswerNumber(int question)
    {
        CurrentAnswerNumber[Index] = question;
    }

    public void SaveFinal(bool question)
    {
        CurrentFinal = question;
    }

    public void CheckAnswer()
    {
        Score = 0;
        bool Check = false;

        for (int i = 0; i < CurrentAnswer.Length; i++)
        {
            if ((i < 1 && CurrentAnswer[i] == CA[Number].Answer[i]) || (i > 6 && CurrentAnswer[i] == CA[Number].Answer[i]))
            {
                Score += 3;
            }
            else
            {
                if (!Check && CurrentAnswer[i] != CA[Number].Answer[i])
                {
                    Check = true;
                    continue;
                }
                else if (!Check && i == 6 && CurrentAnswer[i] == CA[Number].Answer[i])
                {
                    Score += 3;
                }
            }
        }
        for (int i = 0; i < CurrentAnswerNumber.Length; i++)
        {
            if (CurrentAnswerNumber[i] == CA[Number].AnswerNumber[i])
            {
                Score += 3;
            }
        }

        if (Score == 30) Score += CurrentFinal == CA[Number].Final ? 10 : 0;
        /* 2019.01.11 測試分數 
        Score = UnityEngine.Random.Range(5, 40);
        */
        TextScore.text = "您本次的測驗分數為：" + Score;
    }

    private void ResetCosmetic()
    {
        Toggle[] TGS = FindObjectsOfType<Toggle>();
        for (int i = 0; i < TGS.Length; i++)
        {
            if (TGS[i].gameObject.name == "NO" || TGS[i].gameObject.name == "3")
            {
                TGS[i].isOn = true;
            }
        }
        for (int i = 0; i < CurrentAnswer.Length; i++)
        {
            CurrentAnswer[i] = false;
        }
        for (int i = 0; i < CurrentAnswerNumber.Length; i++)
        {
            CurrentAnswerNumber[i] = 3;
        }
        CurrentFinal = false;
    }
}

[System.Serializable]
public class CosmeticAnswer
{
    public bool[] Answer = new bool[12];
    public int[] AnswerNumber = new int[3];
    public bool Final;
}
