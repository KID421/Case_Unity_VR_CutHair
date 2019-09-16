using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VRTK;

public class HandObject : VRTK_InteractableObject
{
    #region 化學消毒 - 夾子、水粒子
    private Transform PosSanpObj;   // 夾物件參考座標
    private Transform Obj;          // 夾子夾到的物件
    private ParticleSystem Water;   // 水龍頭的水
    #endregion

    #region 物理消毒 - 蒸氣箱、紫外線消毒箱
    private Animator AniVapor, AniUVRays;
    #endregion

    #region 剪刀
    public GameObject Knife;
    public GameObject Sissor_O, Sissor_C;
    #endregion

    private AudioSource Aud;

    private void Start()
    {
        if (gameObject.name == "夾子(化學)")
        {
            PosSanpObj = transform.Find("夾物件");
            isGrabbable = true;
        }
        if (gameObject.name == "夾子(物理)")
        {
            PosSanpObj = transform.Find("夾物件");
            isGrabbable = true;
        }
        else if (gameObject.name == "水桶")
        {
            Water = GetComponentInChildren<ParticleSystem>();
        }
        if (gameObject.name == "蒸氣箱門") AniVapor = transform.parent.GetComponent<Animator>();
        if (gameObject.name == "紫外線消毒箱門") AniUVRays = transform.parent.GetComponent<Animator>();

        if (name == "吹風機")
        {
            Aud = GetComponent<AudioSource>();
        }
    }

    private void DelayHideObject()
    {
        DisifectionExam.Instance.Signing.SetActive(false);
    }

    private void DelayShowObject()
    {
        DisifectionExam.Instance.AllThings.SetActive(true);
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        if (GameManager.Instance.m_ExamState == ExamState.WashHand)
        {
            HandExam.Instance.StartWashHand(gameObject.name);
            HandExam.Instance.StartDesinfectionHand(gameObject.name);
        }
        /* 化學消毒 - 抽籤 */
        else if (GameManager.Instance.m_ExamState == ExamState.Disinfection && tag == "籤")
        {
            DisifectionExam.Instance.Endorsement(GetComponentInChildren<Text>().text);
            Invoke("DelayHideObject", 1f);
            Invoke("DelayShowObject", 2.5f);
        }

        /* 化學夾子夾物品與放物品 */
        if (gameObject.name == "夾子(化學)")
        {
            Debug.Log("化學消毒 - 使用夾子");
            if (Obj != null)
            {
                if (Obj.parent != PosSanpObj)
                {
                    Obj.SetParent(PosSanpObj);
                    Obj.GetComponent<Rigidbody>().useGravity = false;
                    Obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    Obj.localPosition = Vector3.zero;
                }
                else
                {
                    Obj.SetParent(null);
                    Obj.GetComponent<Rigidbody>().useGravity = true;
                    Obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }
        }

        /* 物理夾子夾物品與放物品 */
        if (gameObject.name == "夾子(物理)")
        {
            Debug.Log("物理消毒 - 使用夾子");
            if (Obj != null)
            {
                if (Obj.parent != PosSanpObj)
                {
                    Obj.SetParent(PosSanpObj);
                    Obj.GetComponent<Rigidbody>().useGravity = false;
                    Obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    Obj.localPosition = Vector3.zero;
                }
                else
                {
                    Obj.SetParent(null);
                    Obj.GetComponent<Rigidbody>().useGravity = true;
                    Obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }
        }

        /* 水桶使用水 */
        if (gameObject.name == "水桶")
        {
            if (Water.isPlaying) Water.Stop(); else Water.Play();
        }

        /* 蒸氣箱、紫外線消毒箱 */
        if (gameObject.name == "蒸氣箱門")
        {
            PhysicalObject.Instance.CheckStep(AniVapor.GetBool("開門") == false ? "開蒸氣門" : "關蒸氣門");
            AniVapor.SetBool("開門", !AniVapor.GetBool("開門"));
        }
        if (gameObject.name == "紫外線消毒箱門")
        {
            PhysicalObject.Instance.CheckStep(AniUVRays.GetBool("開門") == false ? "開啟紫外線" : "關閉紫外線");
            AniUVRays.SetBool("開門", !AniUVRays.GetBool("開門"));
        }

        /* 衛生紙 */
        if (gameObject.tag == "衛生紙")
        {
            Tissue.Instance.NoTissue();
            gameObject.tag = "抽起來的衛生紙";
        }

        /* 剪刀 */
        if (gameObject.name == "剪刀")
        {
            Debug.Log("剪刀");
            StopAllCoroutines();
            StartCoroutine(HandleKnife());
        }

        /* 吹風機 */
        if (gameObject.name == "吹風機")
        {
            if (Aud.isPlaying)
            {
                Aud.Stop();
                GetComponentInChildren<ParticleSystem>().Stop();
            }
            else
            {
                Aud.Play();
                GetComponentInChildren<ParticleSystem>().Play();
            }
        }
    }

    private IEnumerator HandleKnife()
    {
        Knife.SetActive(true);
        Sissor_O.SetActive(false);
        Sissor_C.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        Knife.SetActive(false);
        Sissor_O.SetActive(true);
        Sissor_C.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "衛生紙")
        {
            if (collision.gameObject.name == "HandLeft" || collision.gameObject.name == "HandRight")
            {
                HandExam.Instance.StartWashHand("擦完");
            }
            else if (collision.gameObject.name == "垃圾桶")
            {
                HandExam.Instance.StartWashHand("垃圾桶");
            }
        }
        if (gameObject.tag == "抽起來的衛生紙")
        {
            if (collision.gameObject.name == "HandLeft" || collision.gameObject.name == "HandRight")
            {
                HandExam.Instance.StartWashHand("擦完");
            }
            else if (collision.gameObject.name == "垃圾桶")
            {
                HandExam.Instance.StartWashHand("垃圾桶");
            }
        }
        if (gameObject.name == "棉球")
        {
            if (collision.gameObject.name == "鑷子")
            {
                collision.gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.SetParent(collision.transform);
                transform.localPosition = new Vector3(0.184f, 0.0065f, 0.022f);
                transform.SetParent(null);
                HandExam.Instance.StartDesinfectionHand("夾起棉球");
            }
            /*
            if (transform.parent.name == "鑷子")
            {
            */
            if (collision.gameObject.name == "HandLeft" || collision.gameObject.name == "HandRight")
            {
                HandExam.Instance.StartDesinfectionHand("手部消毒");
            }
            /*
            }
            */
            if (collision.gameObject.name == "垃圾桶")
            {
                HandExam.Instance.StartDesinfectionHand("垃圾桶");
            }
        }
        if (gameObject.name == "蓋子")
        {
            HandExam.Instance.StartDesinfectionHand("罐子");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!HandExam.Instance.DisinfectionHand[1] && gameObject.name == "蓋子")
        {
            if (collision.gameObject.name == "洗手檯消毒")
            {
                if (transform.rotation.eulerAngles.x <= 280 || transform.rotation.eulerAngles.x >= 260)
                {
                    HandExam.Instance.StartDesinfectionHand("蓋子向上");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " 碰到 " + other.name);

        if (gameObject.name == "衛生紙")
        {
            if (other.name == "Head" || other.name == "Head")
            {
                HandExam.Instance.StartWashHand("擦完");
            }
        }
        if (gameObject.tag == "抽起來的衛生紙")
        {
            if (other.name == "HandLeft" || other.name == "HandRight")
            {
                HandExam.Instance.StartWashHand("擦完");
            }
            else if (other.name == "垃圾桶")
            {
                HandExam.Instance.StartWashHand("垃圾桶");
            }
        }
        if (gameObject.name == "棉球")
        {
            if (other.gameObject.name == "HandLeft" || other.gameObject.name == "HandRight")
            {
                HandExam.Instance.StartDesinfectionHand("手部消毒");
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (gameObject.name == "夾子(化學)")
        {
            if (collision.transform.parent.name == "修眉刀" || collision.transform.parent.name == "剪刀" || collision.transform.parent.name == "挖杓" || collision.transform.parent.name == "鑷子" || collision.transform.parent.name == "髮夾" || collision.transform.parent.name == "塑膠挖杓" || collision.transform.parent.name == "含金屬塑膠髮夾" || collision.transform.parent.name == "睫毛捲曲器" || collision.transform.parent.name == "毛巾類（白色）")
            {
                Obj = collision.transform.parent;
            }

            Debug.Log("化學消毒 - 偵測夾子 - " + gameObject.name + " : " + collision.transform.parent.name + " | " + Obj);
        }

        if (gameObject.name == "夾子(物理)")
        {
            if (collision.transform.parent.name == "修眉刀" || collision.transform.parent.name == "剪刀" || collision.transform.parent.name == "挖杓" || collision.transform.parent.name == "鑷子" || collision.transform.parent.name == "髮夾" || collision.transform.parent.name == "毛巾類（白色）")
            {
                Obj = collision.transform.parent;
            }
            else Obj = null;

            Debug.Log("物理消毒 - 偵測夾子 - " + gameObject.name + " : " + collision.transform.parent.name + " | " + Obj);
        }
    }
}
