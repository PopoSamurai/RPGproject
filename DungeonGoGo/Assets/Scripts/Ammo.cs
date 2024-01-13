using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeongo
{
    public enum WeamponStyle
    {
        pistol,
        shothun,
        rife,
        bazooka,
        laserPistol,
        laserrife
    }

    public class Ammo : MonoBehaviour
    {
        public WeamponStyle weamponCheck;
        public Image sr;
        public int ammoCost;
        public Text ammoText;
        public Sprite[] icons;
        public int weamponNumber = 0;
        public int shootAmmoNumber;
        public ParticleSystem particles;
        void Start()
        {
            particles = FindObjectOfType<ParticleSystem>();
            shootAmmoNumber = 20;
            weamponNumber = 1;
        }

        void Update()
        {
            ammoText.text = "x " + shootAmmoNumber;

            switch (weamponNumber)
            {
                case 1:
                    weamponCheck = WeamponStyle.pistol;
                    sr.sprite = icons[0];
                    particles.textureSheetAnimation.SetSprite(0, icons[0]);
                    break;
                case 2:
                    weamponCheck = WeamponStyle.shothun;
                    sr.sprite = icons[1]; 
                    particles.textureSheetAnimation.SetSprite(0, icons[1]);
                    break;
                case 3:
                    weamponCheck = WeamponStyle.rife;
                    sr.sprite = icons[2];
                    particles.textureSheetAnimation.SetSprite(0, icons[2]);
                    break;
                case 4:
                    weamponCheck = WeamponStyle.bazooka;
                    sr.sprite = icons[3];
                    particles.textureSheetAnimation.SetSprite(0, icons[3]);
                    break;
                case 5:
                    weamponCheck = WeamponStyle.laserPistol;
                    sr.sprite = icons[4];
                    particles.textureSheetAnimation.SetSprite(0, icons[4]);
                    break;
                case 6:
                    weamponCheck = WeamponStyle.laserrife;
                    sr.sprite = icons[5];
                    particles.textureSheetAnimation.SetSprite(0, icons[5]);
                    break;
            }
        }
    }
}
