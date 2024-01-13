using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeongo
{
    public class GameManage : MonoBehaviour
    {
        public int Keys = 0;
        public Text KeysCost;
        public int Coin = 0;
        public Text CoinsCost;
        public int Bomb = 0;
        public Text BombCost;

        void Update()
        {
            KeysCost.text = "x " + Keys;
            CoinsCost.text = "x " + Coin;
            BombCost.text = "x " + Bomb;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}