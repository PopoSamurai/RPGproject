using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public enum AddtonStyle
    {
        Key,
        Coin,
        HpPotion,
        Ammo,
        Bomb,
        Heart
    }
    public class Addton : MonoBehaviour
    {
        public AddtonStyle addStyle;
        public int addSt = 0;
        GameObject gameM;
        GameObject heart;
        private void Start()
        {
            gameM = GameObject.FindGameObjectWithTag("GameM");
            heart = GameObject.FindGameObjectWithTag("HpBar");
        }
        void Update()
        {
            switch(addSt)
            {
                case 1:
                    gameM.GetComponent<GameManage>().Keys += 1;
                    Destroy(transform.parent.gameObject);
                    break;
                case 2:
                    gameM.GetComponent<GameManage>().Coin += 1;
                    Destroy(transform.parent.gameObject);
                    break;
                case 3:
                    if (heart.GetComponent<Health>().health < heart.GetComponent<Health>().maxHp)
                    {
                        heart.GetComponent<Health>().health += 1;
                        Destroy(transform.parent.gameObject);
                    }
                    else
                    {
                        Debug.Log("masz full hp");
                    }
                    break;
                case 4:
                    gameM.GetComponent<Ammo>().shootAmmoNumber += 10;
                    Destroy(transform.parent.gameObject);
                    break;
                case 5:
                    gameM.GetComponent<GameManage>().Bomb += 1;
                    Destroy(transform.parent.gameObject);
                    break;
                case 6:
                    heart.GetComponent<Health>().health += 1;
                    heart.GetComponent<Health>().heartsCost += 1;
                    heart.GetComponent<Health>().maxHp += 1;
                    Destroy(transform.parent.gameObject);
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && addStyle == AddtonStyle.Key)
            {
                addSt = 1;
            }
            if (other.CompareTag("Player") && addStyle == AddtonStyle.Coin)
            {
                addSt = 2;
            }
            if (other.CompareTag("Player") && addStyle == AddtonStyle.HpPotion)
            {
                addSt = 3;
            }
            if (other.CompareTag("Player") && addStyle == AddtonStyle.Ammo)
            {
                addSt = 4;
            }
            if (other.CompareTag("Player") && addStyle == AddtonStyle.Bomb)
            {
                addSt = 5;
            }
            if (other.CompareTag("Player") && addStyle == AddtonStyle.Heart)
            {
                addSt = 6;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                addSt = 0;
            }
        }
    }
}
