using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeongo
{
    public class Stamina : MonoBehaviour
    {
        public Image[] staminaIcon;
        public Sprite fullStam;
        public Sprite emptySta;
        public int staminaCost;
        public int maxSta;
        //respawn sta
        [SerializeField] private float timer;
        [SerializeField] private float timeBetweenFiring;
        void Start()
        {
            staminaCost = maxSta;
        }

        void Update()
        {
            if(staminaCost < maxSta)
            {
                timer += Time.deltaTime;
                if (timer > timeBetweenFiring)
                {
                    timer = 0;
                    staminaCost++;
                }
            }

            foreach (Image img in staminaIcon)
            {
                img.sprite = emptySta;
            }
            for (int i = 0; i < staminaCost; i++)
            {
                staminaIcon[i].sprite = fullStam;
            }
        }
    }
}
