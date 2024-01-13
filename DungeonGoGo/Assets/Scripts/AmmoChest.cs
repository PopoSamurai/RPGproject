using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public enum ChestStyle
    {
        box,
        KeyChest,
        IronChest
    }
    public class AmmoChest : MonoBehaviour
    {
        public ChestStyle cheststyle;
        public int chestSt = 0;
        public SpriteRenderer sr;
        public Sprite newImg;
        public Sprite oldImg;
        public int stateNumber = 0;
        public bool active = true;
        public GameObject interactIcon;
        public GameObject addtonIcon;
        public Sprite inside;
        public BoxCollider2D collT;
        GameObject player;
        GameObject ammo;
        public int ammoNr = 0;
        public int weamponStyle = 0;
        public ParticleSystem effectOpen;
        public bool set = false;
        void Start()
        {
            effectOpen.Stop();
            sr = GetComponent<SpriteRenderer>();
            player = GameObject.FindGameObjectWithTag("Player");
            ammo = GameObject.FindGameObjectWithTag("GameM");
        }

        void Update()
        {
            if (player.GetComponent<Interact>().openChest == true && set == true)
            {
                stateNumber = 1;
                player.GetComponent<Interact>().openChest = false;
                set = false;
                player.GetComponent<Interact>().playerIsRange = false;
            }

            switch (chestSt)
            {
                case 0:
                    cheststyle = ChestStyle.box;
                    break;
                case 1:
                    cheststyle = ChestStyle.KeyChest;
                    break;
                case 2:
                    cheststyle = ChestStyle.IronChest;
                    break;
            }

            switch (stateNumber)
            {
                case 0:
                    //close
                    break;
                case 1:
                    //open
                    if(active == true)
                    {
                        interactIcon.SetActive(false);
                        sr.sprite = newImg;
                        addtonIcon.SetActive(true);
                        addtonIcon.GetComponent<SpriteRenderer>().sprite = inside;
                        ammo.GetComponent<Ammo>().shootAmmoNumber += ammoNr;
                        ammo.GetComponent<Ammo>().weamponNumber = weamponStyle;
                        StartCoroutine(czekajNaIkone());
                        active = false;
                    }
                    break;
                case 2:
                    //complete
                    sr.sprite = newImg;
                    active = false;
                    collT.enabled = false;
                    break;
                case 3:
                    if (active == true)
                    {
                        interactIcon.SetActive(false);
                        sr.sprite = newImg;
                        addtonIcon.SetActive(true);
                        addtonIcon.GetComponent<SpriteRenderer>().sprite = inside;
                        ammo.GetComponent<Ammo>().shootAmmoNumber += ammoNr;
                        ammo.GetComponent<Ammo>().weamponNumber = weamponStyle;
                        StartCoroutine(czekajNaDest());
                        active = false;
                    }
                    break;
                case 4:
                    //open
                    if (active == true)
                    {
                        interactIcon.SetActive(false);
                        sr.sprite = newImg;
                        addtonIcon.SetActive(true);
                        addtonIcon.GetComponent<SpriteRenderer>().sprite = inside;
                        ammo.GetComponent<Ammo>().shootAmmoNumber += ammoNr;
                        ammo.GetComponent<Ammo>().weamponNumber = weamponStyle;
                        StartCoroutine(czekajNaIkone());
                        active = false;
                    }
                    break;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && active == true && cheststyle == ChestStyle.IronChest)
            {
                player.GetComponent<Interact>().playerIsRange = true;
                interactIcon.SetActive(true);
                set = true;
            }
            if (other.CompareTag("Bullet") && cheststyle == ChestStyle.box)
            {
                stateNumber = 3;
            }
            if (other.CompareTag("Player") && active == true && cheststyle == ChestStyle.box)
            {
                player.GetComponent<Interact>().playerIsRange = true;
                interactIcon.SetActive(true);
                set = true;
            }
            if (other.CompareTag("Player") && cheststyle == ChestStyle.KeyChest)
            {
                player.GetComponent<Interact>().playerIsRange = true;
                interactIcon.SetActive(true);

                if (ammo.GetComponent<GameManage>().Keys >= 1)
                {
                    ammo.GetComponent<GameManage>().Keys -= 1;
                    set = true;
                }
                else
                {
                    Debug.Log("brak klucza");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player.GetComponent<Interact>().playerIsRange = false;
                interactIcon.SetActive(false);
                set = false;
            }
        }

        IEnumerator czekajNaIkone()
        {
            effectOpen.Play();
            yield return new WaitForSeconds(1f);
            stateNumber = 2;
            effectOpen.Stop();
            addtonIcon.SetActive(false);
            if (chestSt == 0)
            {
                Destroy(this.gameObject);
            }
        }
        IEnumerator czekajNaDest()
        {
            effectOpen.Play();
            yield return new WaitForSeconds(1f);
            stateNumber = 2;
            effectOpen.Stop();
            addtonIcon.SetActive(false);
            active = false;
            Destroy(this.gameObject);
        }
    }
}
