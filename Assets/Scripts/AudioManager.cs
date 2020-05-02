using System.Collections;
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
        public AudioClip summonDigimon;
        public AudioClip unlockDigimon;
        public AudioClip levelUp;
        public AudioClip levelDown;
        [Header("Database")]
        public AudioClip changeDock;
        [Header("Game - Battle")]
        public AudioClip encounterDigimon;
        public AudioClip encounterDigimonBoss;
        public AudioClip launchAttack;
        public AudioClip attackTravel; //Not used
        public AudioClip attackTravelLong; //Not used
        public AudioClip attackTravelVeryLong;
        public AudioClip explosion;
        public AudioClip deport;
        [Header("Game - SpeedRunner")]
        public AudioClip speedRunner_Start;
        public AudioClip speedRunner_Asteroid;
        public AudioClip speedRunner_Finish;
        public AudioClip speedRunner_Crash;
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

        public void PlayCharHappy() {
            source.clip = charHappy;
            source.Play();
        }
        public void PlayCharSad() {
            source.clip = charSad;
            source.Play();
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

        public void StopSound() {
            source.Stop();
        }

        public bool IsSoundPlaying => source.isPlaying;
    }
}