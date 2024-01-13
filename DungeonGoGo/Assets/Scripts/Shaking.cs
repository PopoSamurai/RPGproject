using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public class Shaking : MonoBehaviour
    {
        public bool start = false;
        public float duration;
        public float number;
        private void Update()
        {
            if (start)
            {
                start = false;
                StartCoroutine(ShakingOn());
            }
        }
        IEnumerator ShakingOn()
        {
            Vector3 startPos = transform.position;
            float elapseTime = 0f;
            while (elapseTime < duration)
            {
                elapseTime += Time.deltaTime;
                transform.position = startPos + Random.insideUnitSphere * number;
                yield return null;
            }
            transform.position = startPos;
        }
    }
}