﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Kaisa.Digivice {
    public class PlayerCharacter {
        public GameChar currentChar;
        public CharState currentState = CharState.Idle;

        /// <summary>
        /// 0-3: idle, 4-5: walking, 6: happy, 7: sad, 8: event, 9: evolving
        /// </summary>
        public int CurrentSprite { get; private set; }

        private bool usedAltSprite = false;
        private int lastValue = 0;

        public PlayerCharacter(GameChar currentChar) {
            this.currentChar = currentChar;
            CurrentSprite = 0;
        }

        public void UpdateSprite() {
            switch(currentState) {
                case CharState.Idle:
                    if(usedAltSprite) {
                        usedAltSprite = false;
                        break;
                    }
                    usedAltSprite = true;
                    if (Random.Range(0, 4) == 0) break;

                    lastValue = Random.Range(0, 4);
                    CurrentSprite = lastValue;
                    break;
                case CharState.Walking:
                    if(usedAltSprite) {
                        usedAltSprite = false;
                        CurrentSprite = 4;
                    }
                    else {
                        usedAltSprite = true;
                        CurrentSprite = 5;
                    }
                    break;
                case CharState.Event:
                    if(usedAltSprite) {
                        usedAltSprite = false;
                        CurrentSprite = 0;
                    }
                    else {
                        usedAltSprite = true;
                        CurrentSprite = 8;
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// A list of the characters available in-game.
    /// </summary>
    public enum GameChar {
        Takuya,
        Koji,
        Zoe,
        JP,
        Tommy,
        Koichi
    }

    public enum CharState {
        Idle,
        Walking,
        Event
    }
}