using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HudPanel : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Text hpText;
    public Text spText;
    public Image hpBar;
    public Image spBar;
    public int maxHp;
    public int maxSp;
    private void Update()
    {
        hpText.text = "HP " + hpBar.fillAmount + "/" + maxHp;
        spText.text = "SP " + spBar.fillAmount + "/" + maxSp;
    }
    public void setHud(CharacterClass unit)
    {
        nameText.text = unit.nameP;
        levelText.text = "" + unit.level;
        maxHp = unit.health;
        hpBar.fillAmount = unit.currentHp;
        maxSp = unit.mana;
        spBar.fillAmount = unit.currentSp;
    }

    public void SetHp(int hp)
    {
        hpBar.fillAmount = hp;
    }
    public void SetSp(int sp)
    {
        spBar.fillAmount = sp;
    }
}