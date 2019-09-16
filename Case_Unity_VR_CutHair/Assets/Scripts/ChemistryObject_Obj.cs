using UnityEngine;

public class ChemistryObject_Obj : MonoBehaviour
{
    /* STEP 1 : 水洗 && STEP 4 : 沖水 */
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("化學消毒 - 水洗 || 沖水");
        ChemistryObject.Instance.CheckStep("水洗");
        ChemistryObject.Instance.CheckStep("沖水");
    }

    /* STEP 2 : 放入消毒液 */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "盤子 - 酒精消毒法")
            ChemistryObject.Instance.CheckStep("放入消毒液", "酒精");
        if (collision.collider.name == "盤子 - 氯液消毒液")
            ChemistryObject.Instance.CheckStep("放入消毒液", "氯液");
        if (collision.collider.name == "盤子 - 陽性肥皂液消毒法")
            ChemistryObject.Instance.CheckStep("放入消毒液", "陽性肥皂液");
        if (collision.collider.name == "盤子 - 煤餾油酚肥皂溶液消毒法")
            ChemistryObject.Instance.CheckStep("放入消毒液", "煤餾油酚肥皂溶液");
    }

    /* STEP 3 : 蓋上 - 蓋子 Cover 腳本 */

    /* STEP 5 : 濾網 && STEP 6 : 櫥櫃 */
    private void OnTriggerEnter(Collider other)
    {
        if (ChemistryObject.Instance.Step == 4 && other.name == "濾網")
            ChemistryObject.Instance.CheckStep("濾網");
        if (ChemistryObject.Instance.Step == 5 && other.name == "櫥櫃")
            ChemistryObject.Instance.CheckStep("櫥櫃");
    }

}
