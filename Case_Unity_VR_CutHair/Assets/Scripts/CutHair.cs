using UnityEngine;
using VRTK;

public class CutHair : MonoBehaviour
{
    private Material CapMaterial;
    public VRTK_ControllerEvents CE;

    private void Update()
    {
        if (GameManager.Instance.m_ExamState == ExamState.CutHair)
        {
            if (CE.triggerClicked)
            {
                Cut();
            }
        }
    }

    public void Cut()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {

            GameObject victim = hit.collider.gameObject;

            GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, CapMaterial);

            if (!pieces[1].GetComponent<Rigidbody>())
                pieces[1].AddComponent<Rigidbody>();

            Destroy(pieces[1], 1);
        }
    }
}
