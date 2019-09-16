using UnityEngine;

public class PhysicalObjec_Obj : MonoBehaviour
{
    /* STEP 1 : 水洗 */
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("物理消毒 - 水洗");
        PhysicalObject.Instance.CheckStep("水洗");
    }

    /* STEP 3 : 蓋上 - 蓋子 Cover 腳本 */

    /* STEP 2 : 完全浸泡 && STEP 4 : 濾網 && STEP 5 : 櫥櫃 */
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("物理消毒 - 物件碰撞：" + gameObject + " | 碰到：" + other.name);
        if (other.name == "煮沸水")
            PhysicalObject.Instance.CheckStep("完全浸泡");
        if (other.name == "濾網")
            PhysicalObject.Instance.CheckStep("濾網");
        if (other.name == "櫥櫃")
            PhysicalObject.Instance.CheckStep("櫥櫃");

        /* 蒸氣消毒法：毛巾 STEP 3 : 直立置入 */
        if (gameObject.name == "毛巾類（白色）" && other.name == "蒸氣箱內部")
            PhysicalObject.Instance.CheckStep("直立置入");

        /* 紫外線消毒法：金屬 STEP 3 : 放入紫外線 */
        if (other.name == "紫外線消毒箱內部")
            PhysicalObject.Instance.CheckStep("放入紫外線");
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("物理消毒 - 物件碰撞：" + gameObject + " | 碰到：" + other.gameObject.name);
        /* STEP 2 : 面紙 */
        if (other.gameObject.tag == "抽起來的衛生紙")
            PhysicalObject.Instance.CheckStep("面紙");
    }

    /* 蒸氣消毒法：毛巾 */
    /* STEP 1 : 水洗 行 5 */
    /* STEP 2 &&  STEP 4 : Hand Object 100 */
    /* STEP 5 : PhysicalObject 行 167 */

    /* 紫外線消毒法：金屬 */
    /* STEP 1 : 水洗 行 5 */
    /* STEP 4 : Hand Object 行 107 */
}
