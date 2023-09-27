using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class BattleHUD : MonoBehaviour
    {
        public Text nameText;
        public Text levelText;
        public Text hpText;
        public Text spText;
        public Slider hpBar;
        public Slider spBar;
        public GameObject color;
        private void Update()
        {
            hpText.text = "HP " + hpBar.value + "/" + hpBar.maxValue;
            spText.text = "SP " + spBar.value + "/" + spBar.maxValue;
        }
        public void setHud(CharacterClass unit)
        {
            nameText.text = unit.nameP;
            levelText.text = "" + unit.level;
            hpBar.maxValue = unit.health;
            hpBar.value = unit.currentHp;
            spBar.maxValue = unit.mana;
            spBar.value = unit.currentSp;
        }

        public void SetHp(int hp)
        {
            hpBar.value = hp;
        }
        public void SetSp(int sp)
        {
            spBar.value = sp;
        }
    }
}
