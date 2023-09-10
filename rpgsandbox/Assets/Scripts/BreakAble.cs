using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BreakAble : MonoBehaviour
{
    private Animator anim; 
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void Smash()
    {
        anim.SetBool("break", true);
    }
    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
