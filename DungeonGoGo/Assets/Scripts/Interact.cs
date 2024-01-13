using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeongo
{
    public class Interact : MonoBehaviour
    {
        public bool playerIsRange = false;
        public bool openChest = false;
        void Update()
        {
            if (Input.GetKey(KeyCode.F) && playerIsRange == false)
            {
                openChest = false;
            }
            if (Input.GetKey(KeyCode.F) && playerIsRange)
            {
                openChest = true;
            }
        }
    }
}
