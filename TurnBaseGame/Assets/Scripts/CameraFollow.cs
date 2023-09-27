using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        [Range(1, 10)]
        public float smooth;
        public Vector3 minVal, maxVal;
        public int inHouse = 0;
        public GameObject water;
        private void FixedUpdate()
        {
            switch (inHouse)
            {
                //house
                case 0:
                    water.SetActive(true);
                    Follow();
                    break;
                case 1:
                    water.SetActive(false);
                    transform.position = new Vector3(15, -36, -10);
                    break;
                //cave
                case 2:
                    water.SetActive(true);
                    Follow();
                    break;
                case 3:
                    break;

            }
        }

        void Follow()
        {
            Vector3 targetPos = target.position + offset;
            Vector3 boundPos = new Vector3(
                Mathf.Clamp(targetPos.x, minVal.x, maxVal.x),
                Mathf.Clamp(targetPos.y, minVal.y, maxVal.y),
                Mathf.Clamp(targetPos.z, minVal.z, maxVal.z)
                );
            Vector3 smoothPos = Vector3.Lerp(transform.position, boundPos, smooth * Time.fixedDeltaTime);
            transform.position = smoothPos;
        }
    }
}
