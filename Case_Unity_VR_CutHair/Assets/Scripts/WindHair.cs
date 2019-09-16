using UnityEngine;

public class WindHair : MonoBehaviour
{
    public bool[] Hairs = new bool[3];
    public static int Score;
    public Material MHair;

    public static WindHair Instance;

    private void Start()
    {
        Instance = this;
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("吹風機粒子：" + other);

        if (!Hairs[0] && other.name == "吹髮檢查 - 前")
        {
            Hairs[0] = true;
            Score += 2;
        }
        if (!Hairs[1] && other.name == "吹髮檢查 - 中")
        {
            Hairs[1] = true;
            Score += 3;
            Score += 3;
        }
        if (!Hairs[2] && other.name == "吹髮檢查 - 後")
        {
            Hairs[2] = true;
            Score += 3;
            Score += 3;
        }

        if (other.tag == "未檢查頭髮")
        {
            Score = Random.Range(7, 15);
            if (other.transform.localScale.x <= 0.23f) other.transform.localScale += new Vector3(0.0001f, 0, 0.0001f);
            other.GetComponent<Renderer>().material.color -= new Color(0.01f, 0.01f, 0.01f);
        }
        if (other.tag == "頂部頭髮")
        {
            Score += 2;
            Score = Mathf.Clamp(Score, 0, 15);
            MHair.color -= new Color(10f, 10f, 10f);
            Debug.Log("吹頭髮分數：" + Score);
        }
    }
}
