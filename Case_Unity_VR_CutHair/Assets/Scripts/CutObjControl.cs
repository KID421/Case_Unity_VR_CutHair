using UnityEngine;

public class CutObjControl : MonoBehaviour
{
    private string OriName;
    private bool Down;

    private void Start()
    {
        OriName = name;
    }

    private void Update()
    {
        HandleCutDown();
    }

    private void HandleCutDown()
    {
        if ((name.Contains("_neg") || name.Contains("_pos")) && !Down)
        {
            Debug.Log("原始名稱：" + OriName);
            GameObject obj = GameObject.Find(OriName + (name.Contains("_neg") ? "_pos" : "_neg"));
            Debug.Log("找 POS 物件名稱：" + OriName + (name.Contains("_neg") ? "_pos" : "_neg"));
            float y = GetComponent<Rigidbody>().position.y;
            float obj_y = obj.GetComponent<Rigidbody>().position.y;
            Debug.Log("_neg 高度：" + y);
            Debug.Log("_pos 高度：" + obj_y);
            if (y < obj_y)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                //obj.name = OriName;
                obj.SetActive(false);
                obj.SetActive(true);
                Down = true;
                obj.GetComponent<CutObjControl>().Down = false;
                Destroy(gameObject, 1.5f);
            }
            else
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
                //name = OriName;
                gameObject.SetActive(false);
                gameObject.SetActive(true);
                Down = false;
                obj.GetComponent<CutObjControl>().Down = true;
                Destroy(obj, 1.5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        //if (other.name == "Slicer") Down = false;
    }
}
