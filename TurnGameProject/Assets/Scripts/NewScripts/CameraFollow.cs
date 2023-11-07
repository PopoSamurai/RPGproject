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
        public Vector3 CaveminVal, CavemaxVal;
        public int inHouse = 0;
        public GameObject water;
        public int set = 0;
        private void FixedUpdate()
        {
            switch (inHouse)
            {
                //house
                case 0:
                    water.transform.position = new Vector2(this.transform.position.x, -6.54f);
                    water.SetActive(true);
                    Follow();
                    break;
                case 1:
                    water.SetActive(false);
                    transform.position = new Vector3(0, -25.75f, -10);
                    break;
                //cave
                case 2:
                    water.SetActive(true);
                    FollowCave();
                    break;
                case 3:
                    water.SetActive(true);
                    transform.position = new Vector3(-87.25f, 3.08f, -10);
                    break;
                //boss
                case 4:
                    water.SetActive(true);
                    transform.position = new Vector3(206.1f, -0.25f, -10);
                    break;

            }
        }
        private void Update()
        {
            if(set == 1)
            {
                interactOn();
            }
            else if(set == 2)
            {
                interactOff();
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
        void FollowCave()
        {
            Vector3 targetPos = target.position + offset;
            Vector3 boundPos = new Vector3(
                Mathf.Clamp(targetPos.x, CaveminVal.x, CavemaxVal.x),
                Mathf.Clamp(targetPos.y, CaveminVal.y, CavemaxVal.y),
                Mathf.Clamp(targetPos.z, CaveminVal.z, CavemaxVal.z)
                );
            Vector3 smoothPos = Vector3.Lerp(transform.position, boundPos, smooth * Time.fixedDeltaTime);
            transform.position = smoothPos;
        }

        public void interactOn()
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 1 * Time.deltaTime * 7; 

            if (GetComponent<Camera>().orthographicSize < 5)
            {
                GetComponent<Camera>().orthographicSize = 5;
            }
        }
        public void interactOff()
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 1 * Time.deltaTime * 7;
            if (GetComponent<Camera>().orthographicSize > 8)
            {
                GetComponent<Camera>().orthographicSize = 8;
            }
        }
    }
}
