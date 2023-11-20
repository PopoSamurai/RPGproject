using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class MusicManager : MonoBehaviour
    {
        public AudioSource bgMuscic;
        public AudioClip[] music;

        private void Start()
        {
            bgMuscic.clip = music[0];
            bgMuscic.Play();
        }
    }
}