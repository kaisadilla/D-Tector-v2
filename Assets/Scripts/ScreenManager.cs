using Kaisa.Digivice.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ScreenManager : MonoBehaviour {
        private SpriteDatabase spriteDB;
        private GameManager gm;
        private AudioManager audioMgr;
        private LogicManager logicMgr;

        private Transform animParent;

        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            this.logicMgr = gm.logicMgr;
            this.spriteDB = gm.spriteDB;
        }
        [Header("UI Elements")]
        public Image screenDisplay;

        public Transform ScreenParent => screenDisplay.transform;

        /// <summary>
        /// Player any number of animations, in a sequence. Those animations are full screen and hide any other element in the screen.
        /// </summary>
        /// <param name="animations">The animations to be played.</param>
        public void PlayAnimation(params IEnumerator[] animations) => StartCoroutine(PlayAnimationCoroutine(animations));
        private IEnumerator PlayAnimationCoroutine(params IEnumerator[] animations) {
            gm.LockInput();
            foreach (IEnumerator a in animations) {
                animParent = gm.BuildContainer("Anim Parent", ScreenParent, 32, 32, transparent: false).transform;
                yield return a;
                Destroy(animParent.gameObject);
            }
            gm.UnlockInput();
        }

        private void ClearParent() {
            foreach (Transform child in animParent) Destroy(child.gameObject);
        }

        private void Update() {
            UpdateDisplay();
        }

        private void UpdateDisplay() {
            int index;
            Sprite sprite;
            switch (logicMgr.currentScreen) {
                case Screen.Character:
                    SetScreenSprite(gm.PlayerCharSprites[gm.CurrentPlayerCharSprite]);
                    break;
                case Screen.MainMenu:
                    index = (int)logicMgr.currentMainMenu;
                    sprite = spriteDB.mainMenu[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesMenu:
                    index = logicMgr.gamesMenuIndex;
                    sprite = spriteDB.game_sections[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesRewardMenu:
                    index = logicMgr.gamesRewardMenuIndex;
                    sprite = spriteDB.games_reward[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesTravelMenu:
                    index = logicMgr.gamesTravelMenuIndex;
                    sprite = spriteDB.games_travel[index];
                    SetScreenSprite(sprite);
                    break;
                default:
                    SetScreenSprite(null);
                    break;
            }
        }

        private void SetScreenSprite(Sprite sprite) {
            screenDisplay.sprite = sprite;
        }

        //WARNING: This is about to get very ugly.
        public IEnumerator ASummonDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonCr = spriteDB.GetDigimonSprite(digimon, SpriteAction.Crush);
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sPowerBlack = spriteDB.givePowerBlack;
            Sprite sPowerWhite = spriteDB.givePowerWhite;

            audioMgr.PlaySound(audioMgr.summonDigimon);

            SpriteBuilder sbDigimon = gm.BuildSprite(digimon, animParent, 24, 24, 4, 4);
            SpriteBuilder sbBlackScreen = gm.BuildSprite("BlackScreen", animParent, sprite: sBlackScreen);

            for (int i = 0; i < 4; i++) {
                sbBlackScreen.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetActive(true);
                yield return new WaitForSeconds(0.15f);
            }

            for (int i = 0; i < 4; i++) {
                sbBlackScreen.SetSprite(sPowerBlack);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetSprite(sBlackScreen);
                yield return new WaitForSeconds(0.15f);
            }

            sbBlackScreen.SetSprite(sPowerBlack);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetSprite(sPowerWhite);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetSprite(sPowerBlack);
            sbDigimon.SetSprite(sDigimon);
            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 2; i++) {
                sbBlackScreen.SetTransparent(true);
                sbBlackScreen.SetSprite(sPowerWhite);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetTransparent(false);
                sbBlackScreen.SetSprite(sPowerBlack);
                yield return new WaitForSeconds(0.15f);
            }

            sbBlackScreen.SetTransparent(true);
            sbBlackScreen.SetSprite(sPowerWhite);
            yield return new WaitForSeconds(0.20f);

            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.20f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.90f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(1.25f);

            sbDigimon.SetSprite(sDigimonCr);
            yield return new WaitForSeconds(0.75f);
            //yield return new WaitUntil(() => !audioMgr.IsSoundPlaying);
        }
        public IEnumerator AUnlockDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sCurtain = spriteDB.acquireDigimon;
            Sprite sDTector = spriteDB.dTector;
            Sprite sPowerWhite = spriteDB.giveMassivePowerWhite;

            audioMgr.PlaySound(audioMgr.unlockDigimon);

            SpriteBuilder sbDigimon = gm.BuildSprite(digimon, animParent, 24, 24, 4, 4, sprite: sDigimon);
            SpriteBuilder sbCurtain = gm.BuildSprite("BlackScreen", animParent, sprite: sCurtain, transparent: true);
            sbCurtain.PlaceOutside(Direction.Down);

            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 32; i++) {
                sbCurtain.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);

            }
            for (int i = 0; i < 32; i++) {
                sbCurtain.MoveSprite(Direction.Up);
                sbDigimon.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);
            }

            yield return new WaitForSeconds(0.75f);
            sbDigimon.Dispose();
            sbCurtain.Dispose();

            gm.BuildSprite("DTector", animParent, sprite: sDTector);
            SpriteBuilder sbPower = gm.BuildSprite("Power", animParent, sprite: sPowerWhite, transparent: true);
            sbPower.SetActive(false);

            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < 5; i++) {
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(0.45f);
        }
        public IEnumerator ACharHappy() {
            yield return ACharHappyShort();
            yield return ACharHappyShort();
        }
        public IEnumerator ACharHappyShort() {
            Sprite charIdle = gm.PlayerCharSprites[0];
            Sprite charHappy = gm.PlayerCharSprites[6];

            audioMgr.PlaySound(audioMgr.charHappy);

            SpriteBuilder sbChar = gm.BuildSprite("Char", animParent, sprite: charIdle);

            for (int i = 0; i < 2; i++) {
                sbChar.SetSprite(charIdle);
                yield return new WaitForSeconds(0.5f);
                sbChar.SetSprite(charHappy);
                yield return new WaitForSeconds(0.5f);
            }
        }
        //TODO: Make the screen exit after the player presses A.
        public IEnumerator AAwardDistance(int score, int distanceBefore, int distanceAfter) {
            //yield return null;
            //gm.inputMgr.ConsumeLastKey(Direction.Up, Direction.Down);
            Sprite sScore = spriteDB.games_score;
            Sprite sDistance = spriteDB.games_distance;

            audioMgr.PlayButtonA();
            gm.BuildSprite("Score", animParent, 32, 5, 0, 0, sScore);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            gm.BuildTextBox("ScoreText", animParent, score.ToString(), DFont.Regular, 31, 5, 0, 8, TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            gm.BuildSprite("Distance", animParent, 32, 5, 0, 17, sDistance);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            TextBoxBuilder tbDistance = gm.BuildTextBox("DistanceText", animParent, distanceBefore.ToString(), DFont.Regular, 31, 5, 0, 25, TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayCharHappy();
            tbDistance.Text = distanceAfter.ToString();
            yield return new WaitForSeconds(2f);
            //gm.UnlockInput();
            //yield return new WaitUntil(() => gm.inputMgr.ConsumeLastKey(Direction.Up, Direction.Down));
        }
        public IEnumerator ATravelMap(Direction dir, int currentMap, int currentSector, int newSector) {
            Sprite mapCurrentSprite = spriteDB.GetMapSectorSprites(currentMap)[currentSector];
            Sprite mapNewSprite = spriteDB.GetMapSectorSprites(currentMap)[newSector];

            SpriteBuilder sMapCurrent = gm.BuildSprite("AnimCurrentSector", animParent, posX: 0, posY: 0, sprite: mapCurrentSprite);
            SpriteBuilder sMapNew = gm.BuildSprite("AnimNewSector", animParent, sprite: mapNewSprite);
            sMapNew.PlaceOutside(dir.Opposite());

            float animDuration = 1.5f;
            for (int i = 0; i < 32; i++) {
                sMapCurrent.MoveSprite(dir);
                sMapNew.MoveSprite(dir);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            sMapCurrent.Dispose();
            sMapNew.Dispose();
        }
        public IEnumerator ASwapDDock(int ddock, string newDigimon) {
            float animDuration = 1.5f;
            Sprite newDigimonSprite = spriteDB.GetDigimonSprite(newDigimon);
            Sprite newDigimonSpriteCr = spriteDB.GetDigimonSprite(newDigimon, SpriteAction.Crush);

            audioMgr.PlaySound(audioMgr.changeDock);

            SpriteBuilder bBlackBars = gm.BuildSprite("BlackBars", animParent, sprite: gm.spriteDB.blackBars);
            bBlackBars.PlaceOutside(Direction.Down);
            SpriteBuilder bDDock = gm.BuildSprite("DDock", animParent, sprite: gm.spriteDB.status_ddock[ddock]);
            SpriteBuilder bDDockSprite = gm.BuildDDockSprite(ddock, bDDock.transform);

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Up);
                bDDock.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            bDDockSprite.SetSprite(newDigimonSprite);
            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Down);
                bDDock.MoveSprite(Direction.Down);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(audioMgr.PlaySoundAfterDelay(audioMgr.charHappy, 0.175f));
            for (int i = 0; i < 5; i++) {
                bDDockSprite.SetSprite(null);
                yield return new WaitForSeconds(0.175f);
                bDDockSprite.SetSprite(newDigimonSpriteCr);
                yield return new WaitForSeconds(0.175f);
            }

            yield return new WaitForSeconds(0.5f);

            bBlackBars.Dispose();
            bDDock.Dispose();
        }
        #region Battle animations
        public IEnumerator AEncounterEnemy(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonAt = spriteDB.GetDigimonSprite(digimon, SpriteAction.Attack);
            Sprite sGivePower = spriteDB.givePowerWhite;

            SpriteBuilder sbDigimon = gm.BuildSprite("Enemy", animParent, 24, 24, 4, 4, sDigimon);
            sbDigimon.FlipHorizontal(true);
            sbDigimon.SetActive(false);
            SpriteBuilder sbGivePower = gm.BuildSprite("Power", animParent, sprite: sGivePower, transparent: true);
            sbGivePower.SetActive(false);

            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.5f);
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGivePower.SetActive(false);
            }

            yield return new WaitForSeconds(0.5f);
            sbGivePower.SetActive(true);

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.25f);
                sbDigimon.SetActive(false);
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbDigimon.SetActive(true);
                sbGivePower.SetActive(false);
            }

            yield return new WaitForSeconds(0.25f);
            sbDigimon.SetActive(false);
            sbGivePower.SetActive(true);

            yield return new WaitForSeconds(0.1f);
            sbGivePower.SetActive(false);

            yield return new WaitForSeconds(0.35f);
            audioMgr.PlaySound(audioMgr.encounterDigimon);
            sbDigimon.SetActive(true);

            yield return new WaitForSeconds(0.35f);
            sbDigimon.SetSprite(sDigimonAt);
            yield return new WaitForSeconds(0.6f);
            sbDigimon.SetSprite(sDigimon);
            yield return new WaitForSeconds(0.6f);
            sbDigimon.SetSprite(sDigimonAt);
            yield return new WaitForSeconds(1f);
        }
        public IEnumerator AEncounterBoss(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonAt = spriteDB.GetDigimonSprite(digimon, SpriteAction.Attack);
            Sprite sGivePower = spriteDB.giveMassivePowerWhite;

            SpriteBuilder sbDigimon = gm.BuildSprite("Enemy", animParent, 24, 24, 4, 4, sDigimon);
            sbDigimon.FlipHorizontal(true);
            sbDigimon.SetActive(false);
            SpriteBuilder sbGivePower = gm.BuildSprite("MassivePower", animParent, sprite: sGivePower, transparent: true);
            sbGivePower.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            sbGivePower.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            sbGivePower.SetActive(false);
            audioMgr.PlaySound(audioMgr.encounterDigimonBoss);
            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.5f);
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGivePower.SetActive(false);
            }

            sbDigimon.SetActive(true);

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.25f);
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGivePower.SetActive(false);
            }

            yield return new WaitForSeconds(0.25f);
            sbDigimon.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            sbDigimon.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            sbDigimon.SetSprite(sDigimonAt);
            yield return new WaitForSeconds(0.5f);
            sbDigimon.SetSprite(sDigimon);
            yield return new WaitForSeconds(0.5f);
            sbDigimon.SetSprite(sDigimonAt);
            yield return new WaitForSeconds(0.75f);

        }
        public IEnumerator ADisplayTurn(
                string friendlyDigimon, int friendlyAttack, int friendlyEnergyRank,
                string enemyDigimon, int enemyAttack, int enemyEnergyRank,
                int winner, bool disobeyed, int loserHPbefore, int loserHPnow) {
            Sprite[] friendlySprites = new Sprite[] {
                spriteDB.GetDigimonSprite(friendlyDigimon),
                spriteDB.GetDigimonSprite(friendlyDigimon, SpriteAction.Attack),
                spriteDB.GetDigimonSprite(friendlyDigimon, SpriteAction.Crush),
                spriteDB.battle_energy[friendlyEnergyRank],
                spriteDB.GetAbilitySprite(gm.DatabaseMgr.GetDigimon(friendlyDigimon).abilityName)
            };
            Sprite[] enemySprites = new Sprite[] {
                spriteDB.GetDigimonSprite(enemyDigimon),
                spriteDB.GetDigimonSprite(enemyDigimon, SpriteAction.Attack),
                spriteDB.GetDigimonSprite(enemyDigimon, SpriteAction.Crush),
                spriteDB.battle_energy[enemyEnergyRank],
                spriteDB.GetAbilitySprite(gm.DatabaseMgr.GetDigimon(enemyDigimon).abilityName)
            };

            yield return PALaunchAttack(friendlySprites, friendlyAttack, false, disobeyed);
            yield return PALaunchAttack(enemySprites, enemyAttack, true, false);
            yield return PAAttackCollision(friendlyAttack, friendlySprites, enemyAttack, enemySprites, winner);
            if(winner == 0) {
                //If both attacks were ability, the loser needs to be 'hit' by the ability. Else, it has been already.
                Sprite abilitySprite = (friendlyAttack == enemyAttack) ? friendlySprites[4] : null;
                yield return PADestroyLoser(enemySprites, friendlyAttack, abilitySprite, true, loserHPbefore, loserHPnow);
            }
            else {
                Sprite abilitySprite = (friendlyAttack == enemyAttack) ? enemySprites[4] : null;
                yield return PADestroyLoser(friendlySprites, enemyAttack, abilitySprite, false, loserHPbefore, loserHPnow);
            }
        }
        //TODO: FIX VARIOUS INSTANCES OF ENEMIES ATTACKING AND LOOKING AT THE WRONG WAY.
        private IEnumerator PALaunchAttack(Sprite[] digimonSprites, int attack, bool isEnemy, bool disobeyed) {
            Direction launchDir = isEnemy ? Direction.Right : Direction.Left;
            SpriteBuilder sbAttack = gm.BuildSprite("Attack", animParent, 24, 24, 4, 4);
            SpriteBuilder sbDigimon = gm.BuildSprite("Attacker", animParent, 24, 24, 4, 4, digimonSprites[0]);
            sbAttack.SetComponentSize(24, 24);

            sbAttack.SnapComponentToSide(launchDir, true);
            sbDigimon.FlipHorizontal(isEnemy);
            sbAttack.FlipHorizontal(isEnemy);

            if (attack != 3) {
                //Show exclamation mark.
                if (disobeyed) {
                    yield return new WaitForSeconds(0.1f);
                    SpriteBuilder sbDisobey = gm.BuildSprite("Disobey", animParent, 3, 9, 1, 1, spriteDB.battle_disobey);
                    yield return new WaitForSeconds(0.3f);
                    sbDisobey.Dispose();
                }
                yield return new WaitForSeconds(0.2f);
                sbDigimon.MoveSprite(launchDir.Opposite(), 3);
                sbAttack.MoveSprite(launchDir.Opposite(), 3);
            }
            if (attack == 0 || attack == 2) {
                sbDigimon.SetSprite(digimonSprites[1]);
                sbAttack.SetSprite((attack == 0) ? digimonSprites[3] : digimonSprites[4]);

                audioMgr.PlaySound(audioMgr.launchAttack);
                for (int i = 0; i < 38; i++) {
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                    sbAttack.MoveSprite(launchDir);
                }
                yield return new WaitForSeconds(0.3f);
            }
            else if (attack == 1) {
                sbDigimon.SetSprite(digimonSprites[2]);

                audioMgr.PlaySound(audioMgr.launchAttack);
                for (int i = 0; i < 7; i++) {
                    gm.BuildSprite($"Crush{i}", animParent, 24, 24, 4, 4, digimonSprites[2]).FlipHorizontal(isEnemy).MoveSprite(launchDir, 4 * i);
                    yield return new WaitForSeconds(0.9f / 7);
                }
                yield return new WaitForSeconds(1.5f);
            }
            else if (attack == 3) {
                for (int i = 0; i < 2; i++) {
                    yield return new WaitForSeconds(0.65f);
                    sbDigimon.FlipHorizontal(true);
                    yield return new WaitForSeconds(0.65f);
                    sbDigimon.FlipHorizontal(false);
                }
            }
            ClearParent();
            //yield return new WaitForSeconds(0.2f);
        }
        private IEnumerator PAAttackCollision(int friendlyAttack, Sprite[] friendlySprites, int enemyAttack, Sprite[] enemySprites, int winner) {

            SpriteBuilder sbFriendlyAttack = gm.BuildSprite("FriendlyAttack", animParent, 24, 24);
            sbFriendlyAttack.Center<SpriteBuilder>().PlaceOutside(Direction.Right);
            SpriteBuilder sbEnemyAttack = gm.BuildSprite("EnemyAttack", animParent, 24, 24);
            sbEnemyAttack.Center<SpriteBuilder>().PlaceOutside(Direction.Left);
            sbEnemyAttack.FlipHorizontal(true);
            
            if (winner == 0) sbFriendlyAttack.transform.SetAsLastSibling(); //Place the friendly attack above if he won.

            //Set attack sprite.
            if (friendlyAttack == 0) sbFriendlyAttack.SetSprite(friendlySprites[3]);
            else if (friendlyAttack == 1) sbFriendlyAttack.SetSprite(friendlySprites[2]);
            else if (friendlyAttack == 2) sbFriendlyAttack.SetSprite(friendlySprites[4]);
            else if (friendlyAttack == 3) sbFriendlyAttack.SetSprite(null);
            if (enemyAttack == 0) sbEnemyAttack.SetSprite(enemySprites[3]);
            else if (enemyAttack == 1) sbEnemyAttack.SetSprite(enemySprites[2]);
            else if (enemyAttack == 2) sbEnemyAttack.SetSprite(enemySprites[4]);

            audioMgr.PlaySound(audioMgr.attackTravelVeryLong);
            //Attacks approach each other.
            for (int i = 0; i < 16; i++) {
                sbFriendlyAttack.MoveSprite(Direction.Left);
                sbEnemyAttack.MoveSprite(Direction.Right);
                yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
            }
            //Attacks collide. The pattern of this script is: tie, player win, player lose.
            if (friendlyAttack == 0) {
                if      (enemyAttack == 0) yield return _EnergyOrAbilityCollides();
                else if (enemyAttack == 1) yield return _EnergyVsCrush();
                else if (enemyAttack == 2) yield return _EnergyVsAbility();
            }
            else if (friendlyAttack == 1) {
                if      (enemyAttack == 0) yield return _EnergyVsCrush();
                else if (enemyAttack == 1) yield return _CrushCollides();
                else if (enemyAttack == 2) yield return _CrushVsAbility();
            }
            else if (friendlyAttack == 2) {
                if      (enemyAttack == 0) yield return _EnergyVsAbility();
                else if (enemyAttack == 1) yield return _CrushVsAbility();
                else if (enemyAttack == 2) yield return _EnergyOrAbilityCollides();
            }
            else if (friendlyAttack == 3) {
                yield return _CrushVsAbility(); //Reuse this animation because it's identical.
            }
 
            yield return new WaitForSeconds(0.2f);

            ClearParent();

            void _TransformAttackIntoCollision(SpriteBuilder sb) {
                Sprite collision = spriteDB.battle_attackCollision;
                sb.SetSprite(collision);
                sb.SetSize(7, 24);
            }
            void _TransformAttackIntoBigCollision(SpriteBuilder sb) {
                Sprite collisionBig = spriteDB.battle_attackCollisionBig;
                sb.SetSprite(collisionBig);
                sb.SetSize(15, 32);
                sb.SetPosition(8, 0);
            }
            IEnumerator _EnergyOrAbilityCollides() {
                if (winner == 0) {
                    _TransformAttackIntoCollision(sbEnemyAttack);
                    for (int i = 0; i < 40; i++) {
                        if (i == 3) sbEnemyAttack.Dispose();
                        sbFriendlyAttack.MoveSprite(Direction.Left);
                        yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                    }
                }
                else if (winner == 1) {
                    _TransformAttackIntoCollision(sbFriendlyAttack);
                    for (int i = 0; i < 40; i++) {
                        if (i == 3) sbFriendlyAttack.Dispose();
                        sbEnemyAttack.MoveSprite(Direction.Right);
                        yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                    }
                }
                else if (winner == 2) {
                    sbEnemyAttack.Dispose();
                    //Reuse the friendly attack as the big collision sprite.
                    _TransformAttackIntoBigCollision(sbFriendlyAttack);
                    yield return new WaitForSeconds(0.15f);
                    audioMgr.StopSound();
                }
            }
            IEnumerator _EnergyVsCrush() {
                SpriteBuilder winnerSprite = (winner == 0) ? sbFriendlyAttack : sbEnemyAttack;
                SpriteBuilder loserSprite = (winner == 0) ? sbEnemyAttack : sbFriendlyAttack;
                Direction winnerDirection = (winner == 0) ? Direction.Left : Direction.Right;
                _TransformAttackIntoCollision(loserSprite);
                for (int i = 0; i < 40; i++) {
                    if (i == 3) loserSprite.Dispose();
                    winnerSprite.MoveSprite(winnerDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
            }
            IEnumerator _EnergyVsAbility() {
                //The chunks of the ability.
                SpriteBuilder winnerSprite = (winner == 0) ? sbFriendlyAttack : sbEnemyAttack;
                SpriteBuilder loserSprite = (winner == 0) ? sbEnemyAttack : sbFriendlyAttack;
                winnerSprite.SetTransparent(true); //The energy panel is transparent.

                int brokenAbilityX = loserSprite.Position.x;
                Sprite brokenAbilitySprite = loserSprite.GetSprite();
                Direction winnerDirection = (winner == 0) ? Direction.Left : Direction.Right;

                ContainerBuilder cbBrokenAbilityUp = gm.BuildContainer("AbilityUp", animParent, 24, 12).SetMaskActive(true);
                cbBrokenAbilityUp.SetPosition(brokenAbilityX, 4);
                gm.BuildSprite("AbilityUpSprite", cbBrokenAbilityUp.transform, 24, 24, 0, 0, brokenAbilitySprite).FlipHorizontal(winner == 0);

                ContainerBuilder cbBrokenAbilityDown = gm.BuildContainer("AbilityDown", animParent, 24, 12).SetMaskActive(true);
                cbBrokenAbilityDown.SetPosition(brokenAbilityX, 16);
                gm.BuildSprite("AbilityDownSprite", cbBrokenAbilityDown.transform, 24, 24, 0, -12, brokenAbilitySprite).FlipHorizontal(winner == 0);

                loserSprite.Dispose();
                winnerSprite.transform.SetAsLastSibling(); //Place the winning attack above everything else.

                for (int i = 0; i < 32; i++) { //Enemy crush - player loses
                    cbBrokenAbilityUp.MoveSprite<ContainerBuilder>(winnerDirection.Opposite()).MoveSprite(Direction.Up);
                    cbBrokenAbilityDown.MoveSprite<ContainerBuilder>(winnerDirection.Opposite()).MoveSprite(Direction.Down);
                    winnerSprite.MoveSprite(winnerDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
            }
            IEnumerator _CrushCollides() {
                if (winner == 2) {
                    for (int i = 0; i < 40; i++) {
                        sbFriendlyAttack.MoveSprite(Direction.Right);
                        sbEnemyAttack.MoveSprite(Direction.Left);
                        yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                    }
                }
                else {
                    Direction pushDirection = (winner == 0) ? Direction.Left : Direction.Right;
                    for (int i = 0; i < 40; i++) {
                        sbFriendlyAttack.MoveSprite(pushDirection);
                        sbEnemyAttack.MoveSprite(pushDirection);
                        yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                    }
                }
            }
            IEnumerator _CrushVsAbility() {
                SpriteBuilder winnerSprite = (winner == 0) ? sbFriendlyAttack : sbEnemyAttack;
                SpriteBuilder loserSprite = (winner == 0) ? sbEnemyAttack : sbFriendlyAttack;
                Direction winnerDirection = (winner == 0) ? Direction.Left : Direction.Right;
                for (int i = 0; i < 40; i++) {
                    if (i == 16) loserSprite.Dispose(); //Remove the crush at the exact frame the ability is completely covering it.
                    winnerSprite.MoveSprite(winnerDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
            }
        }
        //Note, if winningAbility is null, the animation assumes the caller does not want the ability win animation to play.
        private IEnumerator PADestroyLoser(Sprite[] loserSprites, int winningAttack, Sprite winningAbility, bool isEnemy, int loserHPbefore, int loserHPnow) {
            Sprite sLoser = loserSprites[0];
            Sprite sLoserCr = loserSprites[2];
            Sprite explosionBig = spriteDB.battle_explosion[0];
            Sprite explosionSmall = spriteDB.battle_explosion[1];
            SpriteBuilder sbLoser = gm.BuildSprite("Loser", animParent, 24, 24).Center<SpriteBuilder>();

            sbLoser.FlipHorizontal(isEnemy); //Flip the sprite if the loser is the enemy.
            Direction winningDirection = isEnemy ? Direction.Left : Direction.Right;

            if(winningAttack == 0) {
                yield return _ExplodeLoser();
            }
            else if (winningAttack == 1) {
                sbLoser.SetSprite(sLoserCr);
                sbLoser.PlaceOutside(winningDirection.Opposite());
                for(int i = 0; i < 64; i++) {
                    sbLoser.MoveSprite(winningDirection);
                    yield return new WaitForSeconds(Constants.CRUSH_TRAVEL_SPEED);
                }
                yield return _ExplodeLoser();
            }
            else if (winningAttack == 2 && winningAbility != null) {
                sbLoser.SetSprite(sLoser);
                SpriteBuilder sbAbility = gm.BuildSprite("Loser", animParent, 24, 24, sprite: winningAbility).Center<SpriteBuilder>();
                sbAbility.PlaceOutside(winningDirection.Opposite());
                sbAbility.FlipHorizontal(!isEnemy); //Flip the ability if the loser is the ally.
                for (int i = 0; i < 64; i++) {
                    if (i == 28) sbLoser.SetSprite(null);
                    sbAbility.MoveSprite(winningDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
                audioMgr.StopSound();
            }
            else {
                audioMgr.StopSound();
            }
            //No animation is done when a digimon loses to an ability.

            sbLoser.SetSprite(sLoser);

            yield return new WaitForSeconds(0.5f);
            ContainerBuilder cbLifeSign = gm.BuildStatSign("LIFE", animParent);

            yield return new WaitForSeconds(0.5f);
            audioMgr.PlayButtonA();
            ((TextBoxBuilder)cbLifeSign.GetChildBuilder(1)).Text = loserHPbefore.ToString();
            yield return new WaitForSeconds(0.75f);
            audioMgr.PlayButtonA();
            ((TextBoxBuilder)cbLifeSign.GetChildBuilder(1)).Text = loserHPnow.ToString();
            yield return new WaitForSeconds(0.75f);

            ClearParent();

            IEnumerator _ExplodeLoser() {
                audioMgr.PlaySound(audioMgr.explosion);
                sbLoser.Center();
                for (int i = 0; i < 2; i++) {
                    sbLoser.SetSprite(explosionBig);
                    yield return new WaitForSeconds(0.5f);
                    sbLoser.SetSprite(explosionSmall);
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.25f);
            }
        }

        #endregion
    }
}