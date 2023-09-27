using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class ButtonS : MonoBehaviour
    {
        GameObject battle;
        GameObject clickS;
        public GameObject color;
        public GameObject Nocolor;
        public Text nazwa;
        public Text nazwa2;
        public string nazwaText;
        public int manaCost;

        private void Start()
        {
            color.SetActive(false);
            Nocolor.SetActive(true);
            nazwa.text = nazwaText + "   " + manaCost;
            nazwa2.text = nazwaText + "   " + manaCost;
        }
        public void UseButtonAttack()
        {
            color.SetActive(true);
            Nocolor.SetActive(false);
            clickS = GameObject.FindGameObjectWithTag("Click");
            clickS.GetComponent<AudioSource>().Play();
            battle = GameObject.FindGameObjectWithTag("GameManager");
            battle.GetComponent<Battle>().AttackButton();
        }
        public void UseButtonHeal()
        {
            color.SetActive(true);
            Nocolor.SetActive(false);
            clickS = GameObject.FindGameObjectWithTag("Click");
            clickS.GetComponent<AudioSource>().Play();
            battle = GameObject.FindGameObjectWithTag("GameManager");
            battle.GetComponent<Battle>().HealButton();
        }
    }
}
