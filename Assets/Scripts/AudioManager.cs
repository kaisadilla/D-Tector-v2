using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    //AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position); causes problems, as the AudioSource can be destroyed a bit earlier than the sound ends.
    public class AudioManager : MonoBehaviour {
        [SerializeField]
        private AudioSource source;
        public AudioClip buttonA;
        public AudioClip buttonB;
        [Header("Generic sounds")]
        public AudioClip charHappy;
        public AudioClip charHappyLong;
        public AudioClip charSad;
        public AudioClip charSadLong;
        [Header("Database clips")]
        public AudioClip changeDock;
        public void PlayButtonA() {
            source.clip = buttonA;
            source.Play();
            //AudioSource.PlayClipAtPoint(buttonA, Camera.main.transform.position);
        }
        public void PlayButtonB() {
            source.clip = buttonB;
            source.Play();
            //AudioSource.PlayClipAtPoint(buttonB, Camera.main.transform.position);
        }
        public void PlaySound(AudioClip sound) {
            source.clip = sound;
            source.Play();
            //AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        }
        /// <summary>
        /// Plays a sound after a delay (in seconds).
        /// </summary>
        public IEnumerator PlaySoundAfterDelay(AudioClip sound, float delay) {
            yield return new WaitForSeconds(delay);
            PlaySound(sound);
        }
    }
}