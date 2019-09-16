using UnityEngine;

public class NewKID_CutHair : MonoBehaviour
{
    [Header("要掉落的頭髮")]
    public Rigidbody Hair;
    [Header("分數")]
    public float Score;

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.SC_R.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && other.name == "剪刀")
        {
            Hair.useGravity = true;
            HairRay.Score += Score;
            Destroy(Hair.gameObject, 2.5f);
        }
    }
}
