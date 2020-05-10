using Kaisa.Digivice;
using UnityEngine;

namespace Kaisa.Digivice {
    public class SpriteDatabase : MonoBehaviour {
        /* 0-3: idle
         * 4-5: walking
         * 6: happy
         * 7: sad
         * 8: event
         * 9: evolving
         */
        [Header("Characters")]
        public Sprite[] takuya = new Sprite[10];
        public Sprite[] koji = new Sprite[10];
        public Sprite[] zoe = new Sprite[10];
        public Sprite[] jp = new Sprite[10];
        public Sprite[] tommy = new Sprite[10];
        public Sprite[] koichi = new Sprite[10];
        [Header("Generic")]
        public Sprite emptySprite;
        public Sprite blackScreen;
        public Sprite blackBars;
        public Sprite arrows;
        public Sprite arrowsSmall;
        public Sprite invertedArrowsSmall;
        public Sprite triggerEvent;
        public Sprite loading;
        public Sprite loadingComplete;
        public Sprite[] pressAButton = new Sprite[2];
        public Sprite hourglass;
        public Sprite error;
        public Sprite[] rewardBackground = new Sprite[4];
        public Sprite[] rewards = new Sprite[5]; //0: Level, 1: Distance down, 2: Distance up, 3: Spirits
        public Sprite bubble;
        public Sprite[] digistorm = new Sprite[2];
        [Header("Game start")]
        public Sprite gameStart_clouds;
        public Sprite gameStart_trailmon;
        public Sprite gameStart_spiritPlatform;
        [Header("Menus")]
        public Sprite[] mainMenu = new Sprite[7];
        [Header("Map")]
        public Sprite map_distanceScreen;
        public Sprite[] map0_sectors = new Sprite[4];
        public Sprite[] map0_areas = new Sprite[12];
        [Header("Status")]
        public Sprite status_distance;
        public Sprite status_level;
        public Sprite status_victories;
        public Sprite[] status_ddock = new Sprite[4];
        public Sprite status_ddockEmpty;
        [Header("Games")]
        public Sprite[] game_sections = new Sprite[3];
        public Sprite[] games_reward = new Sprite[3];
        public Sprite[] games_travel = new Sprite[4];
        public Sprite games_score;
        public Sprite games_distance;
        [Header("Database")]
        [Tooltip("The last element of this array is the \"search tab\" of the database")]
        public Sprite[] database_sections = new Sprite[8];
        public Sprite database_spirit_fusion;
        public Sprite[] elements = new Sprite[10];
        public Sprite[] elementNames = new Sprite[10];
        public Sprite[] database_pages = new Sprite[3];
        public Sprite[] database_ddocks = new Sprite[4];
        public Sprite[] database_searchOptions = new Sprite[3];
        [Header("Digits")]
        public Sprite digits_ok;
        public Sprite digits_error;
        [Header("Animations")]
        public Sprite givePower;
        public Sprite givePowerInverted;
        public Sprite giveMassivePower;
        public Sprite giveMassivePowerInverted;
        public Sprite curtain;
        public Sprite[] curtainSpecial = new Sprite[2];
        public Sprite dTector;
        public Sprite animDistance;
        public Sprite[] ancientSpiral = new Sprite[2];
        public Sprite[] ancientCircle = new Sprite[3];
        public Sprite stealSpiritAttractor;
        [Header("Battle")]
        public Sprite[] battle_mainMenu = new Sprite[4];
        public Sprite[] battle_combatMenu = new Sprite[5];
        public Sprite[] battle_attackMenu = new Sprite[3];
        [Header("Battle Particles")]
        public Sprite[] battle_energy = new Sprite[20];
        public Sprite battle_disobey;
        public Sprite battle_attackCollision; //The particles of the attack collision
        public Sprite battle_attackCollisionBig;
        public Sprite battle_attackCollisionSmall;
        public Sprite[] battle_explosion = new Sprite[2];
        public Sprite battle_callPoints_screen;
        public Sprite battle_callPoints_chooser;
        public Sprite[] battle_gainingSP = new Sprite[2];
        [Header("Game - SpeedRunner")]
        public Sprite speedRunner_rocket;
        public Sprite speedRunner_rocketExplosion;
        public Sprite speedRunner_rocketAsteroid;
        public Sprite speedRunner_rocketSpeedMark;
        public Sprite speedRunner_rocketFinish;
        [Header("Game - Jackpot Box")]
        public Sprite jackpot_box;
        public Sprite jackpot_pad;
        public Sprite[] jackpot_keys = new Sprite[4];

        private void Awake() {
            Constants.SetEmptySprite(emptySprite);
        }

        public Sprite[] GetCharacterSprites(GameChar character) {
            switch (character) {
                case GameChar.Takuya: return takuya;
                case GameChar.Koji: return koji;
                case GameChar.Zoe: return zoe;
                case GameChar.JP: return jp;
                case GameChar.Tommy: return tommy;
                case GameChar.Koichi: return koichi;
            }
            return null;
        }

        /// <summary>
        /// Returns the sprite associated with the digimon and state given.
        /// </summary>
        /// <param name="name">The name of the digimon.</param>
        /// <param name="state">The state of the digimon.</param>
        public Sprite GetDigimonSprite(string name, SpriteAction state = SpriteAction.Default) {
            Sprite sprite = null;

            switch(state) {
                case SpriteAction.Default:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name);
                    break;
                case SpriteAction.Attack:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_at");
                    if (sprite != null) break;
                    else goto case SpriteAction.Default;
                case SpriteAction.Crush:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_cr");
                    if (sprite != null) break;
                    else goto case SpriteAction.Attack;
                case SpriteAction.Spirit:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_sp");
                    if (sprite != null) break;
                    else goto case SpriteAction.Default;
                case SpriteAction.SpiritSmall:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_sm");
                    if (sprite != null) break;
                    else goto case SpriteAction.Spirit;
                case SpriteAction.Black:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_bl");
                    if (sprite != null) break;
                    else goto case SpriteAction.Default;
                case SpriteAction.White:
                    sprite = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_wh");
                    if (sprite != null) break;
                    else goto case SpriteAction.Default;
            }

            return sprite;
        }
        public Sprite GetDigimonSprite(Digimon digimon, SpriteAction state = SpriteAction.Default) => GetDigimonSprite(digimon.name, state);
        /// <summary>
        /// Returns all the sprites associated with the digimon, in order: 0: default, 1: attack, 2: crush, 3: spirit, 4: black.
        /// </summary>
        /// <param name="name">The name of the digimon.</param>
        public Sprite[] GetAllDigimonSprites(string name) {
            Sprite[] sprites = new Sprite[5];
            sprites[0] = Resources.Load<Sprite>("Sprites/Digimon/" + name);
            sprites[1] = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_at");
            sprites[2] = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_cr");
            sprites[3] = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_sp");
            sprites[4] = Resources.Load<Sprite>("Sprites/Digimon/" + name + "_bl");
            return sprites;
        }
        /// <summary>
        /// Returns all the sprites used in combat, in order: 0: default, 1: attack, 2: crush, 3: energy, 4: ability.
        /// </summary>
        /// <returns></returns>
        public Sprite[] GetAllDigimonBattleSprites(string name, int energyRank) {
            return new Sprite[] {
                GetDigimonSprite(name),
                GetDigimonSprite(name, SpriteAction.Attack),
                GetDigimonSprite(name, SpriteAction.Crush),
                battle_energy[energyRank],
                GetAbilitySprite(Database.GetDigimon(name).abilityName)
            };
        }
        /// <summary>
        /// Returns the sprite associated with the ability given.
        /// </summary>
        /// <param name="abilityName">The name of the ability – this is not the name of the digimon who has said ability.</param>
        public Sprite GetAbilitySprite(string abilityName) {
            return Resources.Load<Sprite>("Sprites/Abilities/" + abilityName);
        }

        public Sprite GetWorldSprite(string worldName, int map) {
            return Resources.Load<Sprite>($"Sprites/Maps/{worldName}_{map}");
        }

        public Sprite GetInvertedSprite(Sprite sprite) {
            Texture2D texture = sprite.texture;
            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newTexture.filterMode = FilterMode.Point;

            for (int x = 0; x < texture.width; x++) {
                for (int y = 0; y < texture.height; y++) {
                    if (texture.GetPixel(x, y) == Color.white) {
                        newTexture.SetPixel(x, y, Color.clear);
                    }
                    else {
                        newTexture.SetPixel(x, y, Color.white);
                    }
                }
            }

            newTexture.Apply();

            return Sprite.Create(newTexture, sprite.rect, sprite.pivot);
        }
    }
}