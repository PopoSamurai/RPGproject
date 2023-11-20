using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAnim : MonoBehaviour
{
    List<Animator> animators;
    public float wait = 0.2f;
    void Start()
    {
        animators = new List<Animator>(GetComponentsInChildren<Animator>());
        StartCoroutine(DoAnim());
    }

    IEnumerator DoAnim()
    {
        while(true)
        {
            foreach(var animator in animators)
            {
                animator.SetTrigger("on");
                yield return new WaitForSeconds(wait);
            }
        }
    }
}
