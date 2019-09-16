using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ExamState
{
    Wait, CutHair, Cosmetic, Disinfection, WashHand, WindHair
}
public delegate void OnStart();
public delegate void OnEnd();
public delegate void OnCosmetic();

[System.Serializable]
public struct SpriteTeach
{
    public Sprite[] S;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string ID;
    public ExamState m_ExamState = ExamState.Wait;
    public Text TextTimer, TextInputFild;
    public SteamVR_TrackedObject STO_L, STO_R;
    public Animator AniLeft, AniRight;
    public GameObject ObjTimer, ObjStart, ObjFinish;
    public OnStart onStart;
    public OnEnd onEnd;
    public OnCosmetic onCosmetic;

    public Text T_Score1, T_Score2, T_Score3, T_Score4;

    public SpriteTeach[] ST;
    public Image ImaTeach;

    public  SteamVR_Controller.Device SC_L { get { return SteamVR_Controller.Input((int)STO_L.index); } }
    public  SteamVR_Controller.Device SC_R { get { return SteamVR_Controller.Input((int)STO_R.index); } }
    private float[] TotalTime = { 0, 1800, 240 * 2, 480 * 2, 240 * 2, 600 * 2 };
    private float Timer;
    private bool IsStart;
    private int TeachIndex, SpriteIndex;

    private void Awake()
    {
        if (!Instance) Instance = this;

        PlayerPrefs.SetInt("Number", PlayerPrefs.GetInt("Number") + 1);
    }

    private void Update()
    {
        RefreshTime();
        ControlHand();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleTeachPic(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleTeachPic(-1);
        }
    }

    public void GetID()
    {
        ID = TextInputFild.text;
    }

    public void GetID(string ID)
    {
        this.ID = ID;
    }

    public void ChangeState(int state)
    {
        m_ExamState = (ExamState)state;
        Timer = TotalTime[state];

        switch (m_ExamState)
        {
            case ExamState.Wait:
                break;
            case ExamState.CutHair:
                break;
            case ExamState.Cosmetic:
                onCosmetic();
                break;
            case ExamState.Disinfection:
                break;
            case ExamState.WashHand:
                break;
            case ExamState.WindHair:
                break;
        }

        Restart();
    }

    public void TimerStart()
    {
        onStart();
        IsStart = true;
        RefreshTime();
    }

    public void TimerStop()
    {
        onEnd();
        IsStart = false;
    }

    private void RefreshTime()
    {
        if (!IsStart) return;
        Timer -= Time.deltaTime;
        TextTimer.text = "時間 " + ((int)Timer / 60).ToString("00") + " : " + ((int)Timer % 60).ToString("00");
        if (Timer <= 0) TimerStop();
    }

    private void ControlHand()
    {
        if (SC_L.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            AniLeft.SetBool("isGrabbing", true);
        }
        if (SC_L.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            AniLeft.SetBool("isGrabbing", false);
        }
        if (SC_R.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            AniRight.SetBool("isGrabbing", true);
        }
        if (SC_R.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            AniRight.SetBool("isGrabbing", false);
        }
    }

    public void Restart()
    {
        ObjTimer.SetActive(false);
        ObjStart.SetActive(true);
        ObjFinish.SetActive(false);
    }

    public void ShowScore_1()
    {
        T_Score1.text = "分數：" + (HairRay.Score + WindHair.Score);
    }

    public void ShowScore_2()
    {
        T_Score2.text = "分數：" + CosmeticExam.Score;
    }

    public void ShowScore_3()
    {
        T_Score3.text = "分數：" + DisifectionExam.Score;
    }

    public void ShowScore_4()
    {
        T_Score4.text = "分數：" + HandExam.Score;
    }

    public void Replay()
    {
        HairRay.Score = 0;
        WindHair.Score = 0;
        CosmeticExam.Score = 0;
        DisifectionExam.Score = 0;
        HandExam.Score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        WindHair.Instance.MHair.color = new Color(1, 0.8f, 0.8f);
    }

    public void HandleTeachIndex(int i)
    {
        TeachIndex = i;
        SpriteIndex = 0;
        ImaTeach.sprite = ST[TeachIndex].S[SpriteIndex];
    }

    public void HandleTeachPic(int d)
    {
        SpriteIndex += d;
        SpriteIndex = Mathf.Clamp(SpriteIndex, 0, ST[TeachIndex].S.Length - 1);
        ImaTeach.sprite = ST[TeachIndex].S[SpriteIndex];
    }
}
