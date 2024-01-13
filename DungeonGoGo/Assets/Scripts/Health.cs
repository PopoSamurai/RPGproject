using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Dungeongo
{
    public class Health : MonoBehaviour
    {
        public int health;
        public int heartsCost;
        public Image[] hearts;
        public Sprite fullHeart;
        public Sprite emptyHeart;
        public int maxHp;

        void Update()
        {
            for(int i = 0; i < hearts.Length; i++)
            {
                if (i < health)
                {
                    hearts[i].sprite = fullHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }

                if(i < heartsCost)
                {
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }
        }
    }
}
