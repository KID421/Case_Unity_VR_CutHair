using UnityEngine;

public class Tissue : MonoBehaviour
{
    public GameObject m_Tissue;
    public Transform Pos;
    public GameObject CurrentTissue;

    public static Tissue Instance;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (CurrentTissue == null)
        {
            CurrentTissue = Instantiate(m_Tissue, Pos.position, Pos.rotation);
            CurrentTissue.transform.SetParent(Pos);
            CurrentTissue.transform.localScale = Vector3.one;
        }
    }

    public void NoTissue()
    {
        CurrentTissue = null;
    }
}
