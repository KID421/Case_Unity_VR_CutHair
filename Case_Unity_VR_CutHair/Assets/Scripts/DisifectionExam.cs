using System;
using UnityEngine;
using UnityEngine.UI;

public class DisifectionExam : MonoBehaviour
{
    public static DisifectionExam Instance;

    public Animator AniScore, AniEndorsement;
    public Text TextDate, TextName, TextNumber, TextScore, TextEndorsementChemistry, TextEndorsementPhysical;
    public Button BtnChemistry, BtnPhysical, BtnCStart;
    public Toggle[] Toggles;
    public Dropdown DropPhysical;
    public int CurrentAnswerChemistryThing;
    public bool[] CurrentAnswerChemistry = new bool[4];
    public int CurrentAnswerPhysical;
    public int CurrentAnswerPhysicalThing;

    public static int Score;

    public GameObject[] ThingsObj;
    public GameObject[] ThingsObjPhy;
    [Header("籤筒")]
    public GameObject Signing;
    [Header("所有化學消毒物件")]
    public GameObject AllThings;

    private string[] Things = { "修眉刀", "剪刀", "挖杓", "鑷子", "髮夾", "塑膠挖杓", "含金屬塑膠髮夾", "睫毛捲曲器", "毛巾類（白色）" };
    private int CurrentAnswerChemistryIndex;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.onStart += StartDisinfection;
        GameManager.Instance.onEnd += Finish;
    }

    private void StartDisinfection()
    {
        TextChange();
        ResetDisinfection();
    }

    private void Finish()
    {
        if (GameManager.Instance.m_ExamState != ExamState.Disinfection) return;
        AniScore.SetTrigger("In");
        CheckAnswer();
    }

    private void ResetDisinfection()
    {
        BtnChemistry.interactable = true;
        BtnPhysical.interactable = false;
        BtnCStart.interactable = false;
        TextEndorsementChemistry.text = "抽籤內容";
        TextEndorsementPhysical.text = "抽籤內容";
        for (int i = 0; i < Toggles.Length; i++)
        {
            Toggles[i].isOn = false;
        }
        DropPhysical.value = 0;
        CurrentAnswerChemistryThing = 0;
        for (int i = 0; i < CurrentAnswerChemistry.Length; i++)
        {
            CurrentAnswerChemistry[i] = false;
        }
        CurrentAnswerPhysical = 0;
        CurrentAnswerPhysicalThing = 0;
        AniEndorsement.enabled = true;
        Invoke("DelayClose", 0.1f);
    }

    private void DelayClose()
    {
        AniEndorsement.enabled = false;
        AniEndorsement.gameObject.SetActive(false);
    }

    private void TextChange()
    {
        TextDate.text = DateTime.Now.ToString("yyyy/MM/dd");
        TextName.text = GameManager.Instance.ID;
        TextNumber.text = PlayerPrefs.GetInt("Number").ToString("000000");
    }

    private void CheckAnswer()
    {
        for (int i = 0; i < Things.Length; i++)
        {
            if (TextEndorsementChemistry.text == Things[i])
            {
                if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 6 || i == 7)
                {
                    if (Toggles[2].isOn && Toggles[3].isOn)
                    {
                        Score += 15;
                    }
                }
                if (i == 5 )
                {
                    if (Toggles[0].isOn && Toggles[1].isOn && Toggles[2].isOn && Toggles[3].isOn)
                    {
                        Score += 15;
                    }
                }
                if (i == 8)
                {
                    if (Toggles[2].isOn)
                    {
                        Score += 15;
                    }
                }
                if (i == 9)
                {
                    if (Toggles[0].isOn && Toggles[1].isOn)
                    {
                        Score += 15;
                    }
                }
            }
        }
        if (CurrentAnswerPhysical == 1 || CurrentAnswerPhysical == 3)
        {
            if (CurrentAnswerPhysicalThing == 0 || CurrentAnswerPhysicalThing == 1 || CurrentAnswerPhysicalThing == 2 || CurrentAnswerPhysicalThing == 3 || CurrentAnswerPhysicalThing == 4)
            {
                Score += 10;
            }
        }
        if (CurrentAnswerPhysicalThing == 5)
        {
            if (CurrentAnswerPhysical == 1 || CurrentAnswerPhysical == 2)
            {
                Score += 10;
            }
        }
    }

    public void Endorsement(string name)
    {
        if (!BtnChemistry.interactable && TextEndorsementChemistry.text == "抽籤內容")
        {
            TextEndorsementChemistry.text = name;
            for (int i = 0; i < Things.Length; i++)
            {
                if (name == Things[i])
                {
                    CurrentAnswerChemistryThing = i;
                    break;
                }
            }
        }
        BtnCStart.interactable = true;
    }

    public void AnswerChemistryIndex(int index)
    {
        CurrentAnswerChemistryIndex = index;
    }

    public void AnswerChemistry(bool choose)
    {
        CurrentAnswerChemistry[CurrentAnswerChemistryIndex] = choose;
    }

    public void EndorsementPhysical()
    {
        CurrentAnswerPhysical = UnityEngine.Random.Range(1, 4);
        PhysicalObject.Instance.GetPhysicalType(CurrentAnswerPhysical);
        switch (CurrentAnswerPhysical)
        {
            case 1:
                TextEndorsementPhysical.text = "煮沸消毒法";
                break;
            case 2:
                TextEndorsementPhysical.text = "蒸氣消毒法";
                break;
            case 3:
                TextEndorsementPhysical.text = "紫外線消毒法";
                break;
        }
    }

    public void ChoosePhysicalThing(int index)
    {
        CurrentAnswerPhysicalThing = index;
    }
    
    public void StartChemistry()
    {
        Debug.Log("化學消毒 - 目前的化學消毒物品：" + ThingsObj[CurrentAnswerChemistryThing]);
        ThingsObj[CurrentAnswerChemistryThing].SetActive(true);
        ChemistryObject.Instance.GetObjectType(ThingsObj[CurrentAnswerChemistryThing].name);
    }

    public void StartPhysical()
    {
        Debug.Log("物理消毒 - 目前的物理消毒物品：" + ThingsObjPhy[CurrentAnswerPhysicalThing]);
        ThingsObjPhy[CurrentAnswerPhysicalThing].SetActive(true);
        PhysicalObject.Instance.GetObjectType(ThingsObjPhy[CurrentAnswerPhysicalThing].name);
    }
}
