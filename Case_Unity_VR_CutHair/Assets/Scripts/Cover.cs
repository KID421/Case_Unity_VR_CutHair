using UnityEngine;

public class Cover : MonoBehaviour
{
    public string WaterName;

    private void OnCollisionEnter(Collision collision)
    {
        if (ChemistryObject.Instance.Step == 2 &&  collision.gameObject.name == WaterName)
        {
            if (transform.localEulerAngles.x > 0 && transform.localEulerAngles.x < 10)
            {
                ChemistryObject.Instance.CheckStep("蓋上");
            }
        }

        if (gameObject.name == "鍋蓋" && PhysicalObject.Instance.Step == 2)
        {
            Debug.Log(gameObject.name + " - 角度：" + transform.localEulerAngles.x);
            if (transform.localEulerAngles.x > 265 && transform.localEulerAngles.x < 275)
            {
                PhysicalObject.Instance.CheckStep("蓋上鍋蓋");
            }
        }
    }
}
