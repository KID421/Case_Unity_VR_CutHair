using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnifeControl : MonoBehaviour
{
    //private string ObjCutDownName = "";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("刀子碰撞 - 開始 - 物件：" + other.transform.parent);

        StartCoroutine(DelayCutDown(other));
        //DelayCutDown();
        //StartCoroutine(CutDown(other));
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("刀子碰撞 - 結束 - 物件：" + other.transform.parent);

        Transform tmpObj1 = GameObject.Find(ObjCutDownName + "_neg").transform;
        Transform tmpObj2 = GameObject.Find(ObjCutDownName + "_pos").transform;
        Debug.Log("物件 1 ：" + tmpObj1 + " - Y 座標：" + tmpObj1.position.y);
        Debug.Log("物件 2 ：" + tmpObj2 + " - Y 座標：" + tmpObj2.position.y);

        ObjCutDown = tmpObj1.position.y > tmpObj2.position.y ? tmpObj1.GetComponent<Rigidbody>() : tmpObj2.GetComponent<Rigidbody>();
        Debug.Log("掉落的物件為：" + ObjCutDown.name);
        ObjCutDown.isKinematic = false;
    }
    */

    private IEnumerator DelayCutDown(Collider other)
    {
        //if (ObjCutDownName == "") ObjCutDownName = other.transform.parent.name;
        string ObjCutDownName = other.transform.parent.name;
        Rigidbody tmpObj1 = null;
        Rigidbody tmpObj2 = null;
        Rigidbody ObjCutDown;
        Rigidbody ObjNoDown;
        yield return new WaitForSeconds(0.07f);

        if (GameObject.Find(ObjCutDownName + "_neg")) { tmpObj1 = GameObject.Find(ObjCutDownName + "_neg").GetComponent<Rigidbody>(); }
        if (GameObject.Find(ObjCutDownName + "_pos")) { tmpObj2 = GameObject.Find(ObjCutDownName + "_pos").GetComponent<Rigidbody>(); }
        /*
        Debug.Log("物件 1 ：" + tmpObj1 + " - Y 座標：" + tmpObj1.position.y);
        Debug.Log("物件 2 ：" + tmpObj2 + " - Y 座標：" + tmpObj2.position.y);
        */
        Debug.Log("物件 1 ：" + tmpObj1 + " - Y 座標：" + tmpObj1.centerOfMass);
        Debug.Log("物件 2 ：" + tmpObj2 + " - Y 座標：" + tmpObj2.centerOfMass);

        ObjCutDown = tmpObj1.centerOfMass.y < tmpObj2.centerOfMass.y ? tmpObj1 : tmpObj2;
        ObjNoDown = tmpObj1.centerOfMass.y > tmpObj2.centerOfMass.y ? tmpObj1 : tmpObj2;
        Debug.Log("掉落的物件為：" + ObjCutDown.name);
        ObjCutDown.isKinematic = false;
        ObjCutDown.constraints = RigidbodyConstraints.None;
        ObjCutDownName = "";
        Destroy(ObjCutDown.gameObject, 1.5f);

        ObjNoDown.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ObjNoDown.gameObject.SetActive(true);

        //StartCoroutine(ReStart());
    }
    
    private IEnumerator CutDown(Collider other)
    {
        Transform p = other.transform.parent.parent;

        yield return new WaitForSeconds(0.05f);

        List<Rigidbody> r = new List<Rigidbody>();
        for (int i = 0; i < p.childCount; i++)
        {
            r.Add(p.GetChild(i).GetComponent<Rigidbody>());
        }
        for (int i = 0; i < p.childCount; i++)
        {
            Rigidbody tR = r[0];
            if (r[i].centerOfMass.y > tR.centerOfMass.y)
            {
                tR.isKinematic = false;
                Destroy(tR.gameObject, 0.1f);
                tR = p.GetChild(i).GetComponent<Rigidbody>();
            }
            else if (i != 0)
            {
                r[i].isKinematic = false;
                Destroy(r[i].gameObject, 0.1f);
            }
        }
    }
}
