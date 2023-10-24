using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BreakAble : MonoBehaviour
{
    SpriteRenderer sr;
    private Material orMat;
    [SerializeField] private Material flashMat;
    private Animator anim; 
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        orMat = sr.material;
        flashMat = new Material(flashMat);
        anim = GetComponent<Animator>();
    }
    public void Smash()
    {
        sr.material = flashMat;
        anim.SetBool("break", true);
    }
    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
