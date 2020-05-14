using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Kaisa.Digivice {
    public class PlayerCharacter : MonoBehaviour {
        private GameManager gm;

        public GameChar currentChar;

        /// <summary>
        /// 0-3: idle, 4-5: walking, 6: happy, 7: sad, 8: event, 9: evolving
        /// </summary>
        public int CurrentSprite { get; private set; }

        private bool usedAltSprite = false;
        private int lastValue = 0;

        public void Initialize(GameManager gm, GameChar currentChar) {
            this.gm = gm;
            this.currentChar = currentChar;
            CurrentSprite = 0;
            InvokeRepeating("UpdateSprite", 0.5f, 0.5f);
        }

        public void UpdateSprite() {
            if(gm.IsCharacterDefeated) {
                CurrentSprite = 7;
            }
            else if(gm.IsEventActive) {
                if (usedAltSprite) {
                    usedAltSprite = false;
                    CurrentSprite = 0;
                }
                else {
                    usedAltSprite = true;
                    CurrentSprite = 8;
                }
            }
            else if (gm.isCharacterWalking) {
                if (usedAltSprite) {
                    usedAltSprite = false;
                    CurrentSprite = 4;
                }
                else {
                    usedAltSprite = true;
                    CurrentSprite = 5;
                }
            }
            else {
                if (usedAltSprite) {
                    usedAltSprite = false;
                    return;
                }
                usedAltSprite = true;
                if (Random.Range(0, 4) == 0) return;

                lastValue = Random.Range(0, 4);
                CurrentSprite = lastValue;
            }
        }
    }
    /// <summary>
    /// A list of the characters available in-game.
    /// </summary>
    public enum GameChar {
        none = -1,
        Takuya,
        Koji,
        Zoe,
        JP,
        Tommy,
        Koichi
    }
}