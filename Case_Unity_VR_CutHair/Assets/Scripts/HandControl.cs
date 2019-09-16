using UnityEngine;
using VRTK;

public class HandControl : MonoBehaviour
{
    public static bool HandL, HandR;
    public static HandControl instanc;

    public VRTK_ControllerEvents C;

    private void Start()
    {
        instanc = this;
        C = transform.parent.GetComponent<VRTK_ControllerEvents>();
        Physics.IgnoreLayerCollision(8, 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "HandLeft" || collision.gameObject.name == "Head")
        {
            HandL = true;
        }
        if (collision.gameObject.name == "HandRight" || collision.gameObject.name == "Head")
        {
            HandR = true;
        }
        if (HandL && HandR)
        {
            HandExam.Instance.StartWashHand("泡泡");
        }
    }
}
