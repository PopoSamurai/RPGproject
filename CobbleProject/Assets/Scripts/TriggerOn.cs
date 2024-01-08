using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TriggerOn : MonoBehaviour
{
    public Coins coin;
    GameObject score;
    private void Awake()
    {
        score = GameObject.FindGameObjectWithTag("gameM");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (coin.color == other.GetComponent<move>().id)
        {
            score.GetComponent<GameManage>().score += 1;
            score.GetComponent<GameManage>().UpdateScore();
            Destroy(this.gameObject);
        }
    }
}
