using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    void Update()
    {
        if (Input.GetButton("Lean Left"))
            Animator.SetInteger("Lean", 1);
        else if (Input.GetButton("Lean Right"))
            Animator.SetInteger("Lean", -1);
        else
            Animator.SetInteger("Lean", 0);

        if (Input.GetMouseButton(1))
            Animator.SetBool("Aiming", true);
        else
            Animator.SetBool("Aiming", false);
    }
}