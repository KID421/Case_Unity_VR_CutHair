using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {

    private Animator animator;

	void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        animator.SetBool("isGrabbing", Input.GetKey(KeyCode.F));
	}
}
