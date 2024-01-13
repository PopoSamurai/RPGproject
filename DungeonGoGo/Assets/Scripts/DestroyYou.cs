using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public class DestroyYou : MonoBehaviour
    {
        public float timer;
        public float timeBetweenFiring;
        public GameObject explose;
        Shaking shake;
        private void Start()
        {
            shake = FindObjectOfType<Shaking>();
        }
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                Destroy(this.gameObject);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Wall"))
            {
                shake.start = true;
                Destroy(this.gameObject);
                Instantiate(explose, this.gameObject.transform.position, this.gameObject.transform.rotation);
            }
            if (other.CompareTag("Enemy"))
            {
                shake.start = true;
                Destroy(this.gameObject);
                Instantiate(explose, this.gameObject.transform.position, this.gameObject.transform.rotation);
            }
            if (other.CompareTag("Chest"))
            {
                shake.start = true;
                Destroy(this.gameObject);
                Instantiate(explose, this.gameObject.transform.position, this.gameObject.transform.rotation);
            }
        }
    }
}