using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public enum CharClass
    {
        warrior,
        archer,
        mage,
        assassin,
        paladin,
        darkmage,
        hunter
    }
    public enum pOrE
    {
        player,
        enemy
    }

    public class CharacterClass : MonoBehaviour
    {
        public Animator anim;
        public CharClass classChar;
        public pOrE com;
        public int id;
        public int level = 1;
        public int health;
        public int currentHp;
        public int mana;
        public int currentSp;
        public string nameP;
        public int damage = 5;
        Vector3 firstPos;
        public Material whiteMaterial;
        public Material defaultMat;
        public SpriteRenderer sr;
        public Vector3 offset;
        public AudioSource attackSword;
        public AudioSource HealS;
        public GameObject[] skills;
        void Start()
        {
            firstPos = transform.position;
            currentHp = health;
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }

        public bool TakeDamage(int dmg)
        {
            currentHp -= dmg;
            sr.material = whiteMaterial;
            if(com == pOrE.enemy)
            {
                transform.position = transform.position - offset;
            }
            else
            {
                transform.position = transform.position + offset;
            }
            StartCoroutine(czekajNaHita());
            if (currentHp <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }  
        public IEnumerator czekajNaHita()
        {
            yield return new WaitForSeconds(1f);
            if (com == pOrE.enemy)
            {
                transform.position = transform.position + offset;
            }
            else
            {
                transform.position = transform.position - offset;
            }
            sr.material = defaultMat;
        }
        public void AttackCo()
        {
            StartCoroutine(czekaj());
        }
        IEnumerator czekaj()
        {
            attackSword.Play();
            anim.SetBool("attack", true);
            yield return new WaitForSeconds(1f);
            anim.SetBool("attack", false);
            transform.position = firstPos;
        }
        public void Heal(int amount, int mana)
        {
            currentSp -= mana;
            currentHp += amount;
            HealS.GetComponent<AudioSource>().Play();
            if (currentHp > health)
            {
                currentHp = health;
            }
            StartCoroutine(czekaj2());
        }
        IEnumerator czekaj2()
        {
            HealS.Play();
            anim.SetBool("heal", true);
            yield return new WaitForSeconds(1f);
            anim.SetBool("heal", false);
        }
    }
}
