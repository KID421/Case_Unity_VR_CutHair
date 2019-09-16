using UnityEngine;

public class PSControl : MonoBehaviour
{
    public bool WashL, WashR;

    private ParticleSystem PS;

    private void Start()
    {
        PS = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (gameObject.name == "水")
        {
            if (other.name == "HandLeft")
            {
                WashL = true;
            }
            if (other.name == "HandRight")
            {
                WashR = true;
            }
            if (WashL || WashR)
            {
                WashL = false;
                WashR = false;
                HandExam.Instance.StartWashHand("洗手");
            }
        }
        else if (gameObject.name == "沐浴乳液體")
        {
            if (other.name == "HandLeft" || other.name == "HandRight")
            {
                HandExam.Instance.StartWashHand("沐浴乳液體");
            }
        }
    }
}
