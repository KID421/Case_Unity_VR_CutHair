using UnityEngine;
using UnityEngine.UI;
using System;

public class HandExam : MonoBehaviour
{
    public static HandExam Instance;

    public Animator AniScore;
    public Text TextDate, TextName, TextNumber, TextScore;
    public static int Score;
    public InputField IF1_1, IF1_2, IF3;
    public Toggle[] Toggles;
    public ParticleSystem PSWater, PSBodyWash, PSBBL, PSBBR;

    public Toggle[] TogglesTip;
    public Toggle[] TogglesTipDisinfection;

    public string[] CurrentAnswer1 = new string[2];
    public string CurrentAnswer3;
    public int CurrentAnswer4;

    public Transform Tweezer, Cap, Cotton, Table;
    public Button BtnWash, BtnDisinfection;
    public GameObject TableWash, TableDisifection;

    public Collider HR, HL;

    private string[] Answer1 = { "工作後", "如廁後", "工作前", "手髒時", "打噴嚏後", "修剪指甲前", "修剪指甲後", "打掃環境後" };

    private string[] Answer3 = { "疑似發現顧客患有傳染性皮膚病時", "取器材前" };
    private int Index;
    private int WashHandCount;
    private bool UseWaterWashHand;
    private bool UseTissue;
    private int DesinfectionHandCount;
    private bool CapUp;
    private bool UseCotton;

    public bool[] WashHand = new bool[7];
    public bool[] DisinfectionHand = new bool[3];

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.onStart += StartHand;
        GameManager.Instance.onEnd += Finish;
    }

    private void StartHand()
    {
        TextChange();
        ResetHand();
    }

    private void Finish()
    {
        if (GameManager.Instance.m_ExamState != ExamState.WashHand) return;
        AniScore.SetTrigger("In");
        CheckAnswer();
    }

    private void CheckAnswer()
    {
        Score = 0;
        for (int i = 0; i < CurrentAnswer1.Length; i++)
        {
            for (int j = 0; j < Answer1.Length; j++)
            {
                if ((i == CurrentAnswer1.Length - 1 && CurrentAnswer1[i] != CurrentAnswer1[i -1]) || i != CurrentAnswer1.Length - 1)
                {
                    if (CurrentAnswer1[i] == Answer1[j])
                    {
                        Score += 1;
                        continue;
                    }
                }
            }
        }
        for (int i = 0; i < Answer3.Length; i++)
        {
            if (CurrentAnswer3 == Answer3[i])
            {
                Score += 2;
                break;
            }
        }
        Score += CurrentAnswer4 == 1 ? 1 : 0;
        for (int i = 0; i < WashHand.Length; i++)
        {
            if (!WashHand[i])
            {
                break;
            }
            else if(i == WashHand.Length - 1 && WashHand[i])
            {
                Score += 8;
            }
        }
        for (int i = 0; i < DisinfectionHand.Length; i++)
        {
            if (!DisinfectionHand[i])
            {
                break;
            }
            else if (i == DisinfectionHand.Length - 1 && DisinfectionHand[i])
            {
                Score += 2;
            }
        }
        TextScore.text = "您本次的測驗分數為：" + Score;
    }

    private void ResetHand()
    {
        BtnWash.interactable = true;
        BtnDisinfection.interactable = true;
        TableWash.SetActive(false);
        TableDisifection.SetActive(false);
        IF1_1.text = "";
        IF1_2.text = "";
        IF3.text = "";
        for (int i = 0; i < Toggles.Length; i++)
        {
            Toggles[i].isOn = false;
        }
        for (int i = 0; i < CurrentAnswer1.Length; i++)
        {
            CurrentAnswer1[i] = "";
        }
        CurrentAnswer3 = "";
        CurrentAnswer4 = 0;
        WashHandCount = 0;
        UseWaterWashHand = false;
        UseTissue = false;
        for (int i = 0; i < TogglesTip.Length; i++)
        {
            TogglesTip[i].isOn = false;
        }
        for (int i = 0; i < WashHand.Length; i++)
        {
            WashHand[i] = false;
        }
        DesinfectionHandCount = 0;
        CapUp = false;
        UseCotton = false;
        for (int i = 0; i < TogglesTipDisinfection.Length; i++)
        {
            TogglesTipDisinfection[i].isOn = false;
        }
        for (int i = 0; i < DisinfectionHand.Length; i++)
        {
            DisinfectionHand[i] = false;
        }
        Cotton.SetParent(Table);
        Tweezer.localPosition = new Vector3(-0.295f, 0.3f, 0.113f);
        Tweezer.localEulerAngles = new Vector3(0, -90, 0);
        Cap.localPosition = new Vector3(0.226f, 0.563f, 0.178f);
        Cap.localEulerAngles = new Vector3(90, 0, -180);
        Cotton.localPosition = new Vector3(0.144f, 0.429f, 0.14f);
        Cotton.localEulerAngles = new Vector3(0, -180, 0);
    }

    private void TextChange()
    {
        TextDate.text = DateTime.Now.ToString("yyyy/MM/dd");
        TextName.text = GameManager.Instance.ID;
        TextNumber.text = PlayerPrefs.GetInt("Number").ToString("000000");
    }

    public void StartWashHand(string name)
    {
        Debug.Log(name);

        if (WashHandCount == WashHand.Length) return;

        if (name == "水龍頭")
        {
            if (PSWater.isPlaying) PSWater.Stop(); else PSWater.Play();
        }
        if (name == "沐浴乳開關")
        {
            if (PSBodyWash.isPlaying) PSBodyWash.Stop(); else PSBodyWash.Play();
        }

        if (!WashHand[0] || WashHand[WashHandCount - 1])
        {
            switch (WashHandCount)
            {
                case 0:
                    /*
                    if (name == "水龍頭")
                    {
                        PSWater.Play();
                    }
                    */
                    if (name == "洗手")
                    {
                        UseWaterWashHand = true;
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
                case 1:
                    if (name == "水龍頭")
                    {

                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
                case 2:
                    /*
                    if (name == "沐浴乳開關")
                    {
                        PSBodyWash.Play();
                    }
                    */
                    if (name == "沐浴乳液體")
                    {
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
                case 3:
                    if (name == "泡泡")
                    {
                        PSBBL.Play();
                        PSBBR.Play();
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
                case 4:
                    if (name == "水龍頭")
                    {
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
                case 5:
                    /*
                    if (name == "水龍頭")
                    {
                        PSWater.Play();
                    }
                    */
                    if (name == "洗手")
                    {
                        PSBBL.Stop();
                        PSBBR.Stop();
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                        HR.isTrigger = true;
                        HL.isTrigger = true;
                    }
                    break;
                case 6:
                    /*
                    if (name == "水龍頭")
                    {
                        PSWater.Stop();
                    }
                    */
                    if (name == "擦完")
                    {
                        UseTissue = true;
                    }
                    else if (UseTissue && name == "垃圾桶")
                    {
                        TogglesTip[WashHandCount].isOn = true;
                        WashHand[WashHandCount++] = true;
                    }
                    break;
            }
        }
    }

    public void StartDesinfectionHand(string name)
    {
        if (DesinfectionHandCount == DisinfectionHand.Length) return;
        if (!DisinfectionHand[0] || DisinfectionHand[DesinfectionHandCount - 1])
        {
            switch (DesinfectionHandCount)
            {
                case 0:
                    if (name == "罐子")
                    {
                        TogglesTipDisinfection[DesinfectionHandCount].isOn = true;
                        DisinfectionHand[DesinfectionHandCount++] = true;
                    }
                    break;
                case 1:
                    if (name == "蓋子向上")
                    {
                        CapUp = true;
                    }
                    if (name == "夾起棉球")
                    {
                        TogglesTipDisinfection[DesinfectionHandCount].isOn = true;
                        DisinfectionHand[DesinfectionHandCount++] = true;
                    }
                    break;
                case 2:
                    if (name == "手部消毒")
                    {
                        UseCotton = true;
                    }
                    if (name == "垃圾桶")
                    {
                        TogglesTipDisinfection[DesinfectionHandCount].isOn = true;
                        DisinfectionHand[DesinfectionHandCount++] = true;
                    }
                    break;
            }
        }
    }

    public void InputAnswerIndex1(int index)
    {
        Index = index;
    }

    public void InputAnswer1(string answer)
    {
        CurrentAnswer1[Index] = answer;
    }

    public void InputAnswer3(string answer)
    {
        CurrentAnswer3 = answer;
    }

    public void InputAnswer4(int answer)
    {
        CurrentAnswer4 = answer;
    }
}
