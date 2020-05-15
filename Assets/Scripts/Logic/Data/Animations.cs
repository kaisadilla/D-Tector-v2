using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public static class Animations {
        private static GameManager gm;
        private static AudioManager audioMgr;
        private static SpriteDatabase spriteDB;
        private static Transform AnimParent => gm.screenMgr.animParent;

        public static void Initialize(GameManager gm, AudioManager audioMgr, SpriteDatabase spriteDB) {
            Animations.gm = gm;
            Animations.audioMgr = audioMgr;
            Animations.spriteDB = spriteDB;
        }

        private static void ClearAnimParent() {
            foreach (Transform child in AnimParent) GameObject.Destroy(child.gameObject);
        }

        //WARNING: This is about to get very ugly.
        public static IEnumerator LoadCharacterSelection() {
            Sprite sCharacter = spriteDB.takuya[0];
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sEmptyScreen = spriteDB.emptySprite;
            Sprite sCurtain = spriteDB.curtain;
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(sCharacter);
            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("Character", AnimParent);

            for (int i = 0; i < 2; i++) {
                sbCurtain.SetSprite(sBlackScreen);
                yield return new WaitForSeconds(0.15f);
                sbCurtain.SetSprite(sEmptyScreen);
                yield return new WaitForSeconds(0.25f);
            }
            sbCurtain.SetSprite(sCurtain);

            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.15f);
                sbCurtain.SetSprite(sEmptyScreen);
                yield return new WaitForSeconds(0.25f);
                sbCurtain.SetSprite(sCurtain);
            }

            for (int i = 0; i < 32; i++) {
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(2f / 32);
            }
            yield return new WaitForSeconds(1f);
        }

        public static IEnumerator StartGameAnimation(GameChar character, string spirit, int spiritEnergy, string enemyDigimon, int enemyEnergy) {
            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            Sprite[] sSpirit = spriteDB.GetAllDigimonSprites(spirit);
            Sprite[] sEnemyDigimon = spriteDB.GetAllDigimonSprites(enemyDigimon);
            Sprite sSpiritEnergy = spriteDB.GetEnergySprite(spiritEnergy);
            Sprite sEnemyEnergy = spriteDB.GetEnergySprite(enemyEnergy);
            Sprite sClouds = spriteDB.gameStart_clouds;
            Sprite sTrailmon = spriteDB.gameStart_trailmon;
            Sprite sSpiritPlatform = spriteDB.gameStart_spiritPlatform;
            Sprite sExclamationMark = spriteDB.battle_disobey;
            Sprite sCollision = spriteDB.battle_attackCollisionSmall;
            Sprite sExplosionBig = spriteDB.battle_explosion[0];
            Sprite sExplosionSmall = spriteDB.battle_explosion[1];
            Sprite sCurtain = spriteDB.curtain;
            Sprite sDTector = spriteDB.dTector;
            Sprite sPowerBlack = spriteDB.giveMassivePowerInverted;

            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(sCharacter[0]).PlaceOutside(Direction.Right);

            for (int i = 0; i < 32; i++) {
                if (Mathf.FloorToInt(i / 2f) % 2 == 0) {
                    sbCharacter.SetSprite(sCharacter[4]);
                }
                else {
                    sbCharacter.SetSprite(sCharacter[5]);
                }

                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(2f / 32);
            }

            yield return CharHappy();
            sbCharacter.PlaceOutside(Direction.Right);

            audioMgr.PlaySound(audioMgr.gameStart);

            SpriteBuilder sbClouds = ScreenElement.BuildSprite("Character", AnimParent).SetSize(76, 32).SetSprite(sClouds);
            SpriteBuilder sbTrailmon = ScreenElement.BuildSprite("Character", AnimParent).SetSize(118, 15).SetSprite(sTrailmon).SetY(9).PlaceOutside(Direction.Right);
            for (int i = 0; i < 75; i++) {
                if (i % 2 == 0) sbClouds.Move(Direction.Left);
                sbTrailmon.Move(Direction.Left);
                yield return new WaitForSeconds(4.2f / 75);
            }
            for (int i = 0; i < 32; i++) {
                if (i < 12 && i % 2 == 0) sbClouds.Move(Direction.Left);
                sbTrailmon.Move(Direction.Left);
                yield return new WaitForSeconds(3.4f / 32);
            }
            for (int i = 0; i < 11; i++) {
                sbTrailmon.Move(Direction.Left);
                yield return new WaitForSeconds(2.6f / 11);
            }
            RectangleBuilder sbWindow1 = ScreenElement.BuildRectangle("Window1", AnimParent).SetSize(0, 5).SetPosition(7, 14);
            RectangleBuilder sbWindow2 = ScreenElement.BuildRectangle("Window1", AnimParent).SetSize(0, 5).SetPosition(17, 14);
            RectangleBuilder sbWindow3 = ScreenElement.BuildRectangle("Window1", AnimParent).SetSize(0, 5).SetPosition(27, 14);
            for (int i = 0; i < 2; i++) {
                sbWindow1.SetSize(i + 1, 5).Move(Direction.Left);
                sbWindow2.SetSize(i + 1, 5).Move(Direction.Left);
                sbWindow3.SetSize(i + 1, 5).Move(Direction.Left);
                yield return new WaitForSeconds(0.8f / 2);
            }
            sbClouds.Dispose();
            sbTrailmon.Dispose();
            sbWindow1.Dispose();
            sbWindow2.Dispose();
            sbWindow3.Dispose();
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 32; i++) {
                if (Mathf.FloorToInt(i / 2f) % 2 == 0) {
                    sbCharacter.SetSprite(sCharacter[4]);
                }
                else {
                    sbCharacter.SetSprite(sCharacter[5]);
                }

                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(2f / 32);
            }
            sbCharacter.SetSprite(sCharacter[0]);
            SpriteBuilder sbDisobey = ScreenElement.BuildSprite("Disobey", AnimParent).SetSize(3, 9).SetPosition(1, 1).SetSprite(sExclamationMark);
            yield return new WaitForSeconds(0.5f);
            sbDisobey.Dispose();
            sbCharacter.SetActive(false);
            yield return new WaitForSeconds(0.4f);

            SpriteBuilder sbAttack = ScreenElement.BuildSprite("Attack", AnimParent)
                .SetSize(24, 24)
                .SetSprite(sEnemyEnergy)
                .Center()
                .Move(Direction.Left, 3)
                .FlipHorizontal(true)
                .SetActive(false);
            SpriteBuilder sbEnemy = ScreenElement.BuildSprite(enemyDigimon, AnimParent).SetSize(24, 24).SetSprite(sEnemyDigimon[0]).FlipHorizontal(true).Center();
            yield return new WaitForSeconds(0.15f);
            sbEnemy.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            sbEnemy.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbEnemy.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            sbEnemy.SetSprite(sEnemyDigimon[1]).Move(Direction.Left, 3).SetActive(true);
            sbAttack.SetActive(true);

            for (int i = 0; i < 38; i++) {
                yield return new WaitForSeconds(1.7f / 38);
                sbAttack.Move(Direction.Right);
            }
            sbEnemy.SetActive(false);
            sbAttack.PlaceOutside(Direction.Left);
            for (int i = 0; i < 32; i++) {
                yield return new WaitForSeconds(1.5f / 32);
                sbAttack.Move(Direction.Right);
            }
            sbAttack.PlaceOutside(Direction.Left);
            ContainerBuilder cbSpirit = ScreenElement.BuildContainer("Spirit", AnimParent).SetSize(24, 24).SetPosition(8, 4).SetMaskActive(true);
            SpriteBuilder sbSpiritEmerging = ScreenElement.BuildSprite(enemyDigimon, cbSpirit.transform).SetSize(24, 24).SetPosition(0, 21).SetSprite(sSpirit[3]);
            ScreenElement.BuildSprite("Platform", cbSpirit.transform).SetSize(22, 3).SetPosition(1, 21).SetSprite(sSpiritPlatform);
            for (int i = 0; i < 21; i++) {
                sbSpiritEmerging.Move(Direction.Up);
                yield return new WaitForSeconds(1f / 21);
            }
            for (int i = 0; i < 8; i++) {
                yield return new WaitForSeconds(0.4f / 8);
                sbAttack.Move(Direction.Right);
            }
            sbAttack.Dispose();
            sbAttack = ScreenElement.BuildSprite("Collision", AnimParent).SetSprite(sCollision).SetSize(7, 15).SetPosition(0, 8);
            yield return new WaitForSeconds(0.4f);
            sbAttack.Dispose();
            cbSpirit.SetPosition(4, 4);
            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i < 24; i++) {
                yield return new WaitForSeconds(1.5f / 24);
                cbSpirit.Move(Direction.Up);
            }
            cbSpirit.Dispose();
            SpriteBuilder sbSpirit = ScreenElement.BuildSprite("Spirit", AnimParent)
                .SetSize(24, 24).SetSprite(sSpirit[3]).Center().PlaceOutside(Direction.Left).SetTransparent(true);
            sbCharacter.Center().PlaceOutside(Direction.Right).SetTransparent(true).SetActive(true);
            for (int i = 0; i < 30; i++) {
                sbSpirit.Move(Direction.Right);
                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(2.8f / 30);
            }
            sbSpirit.SetTransparent(false);
            yield return new WaitForSeconds(0.25f);
            sbSpirit.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            sbSpirit.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            sbSpirit.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            sbSpirit.Dispose();
            sbCharacter.SetSize(24, 24).Center().SetSprite(sSpirit[0]);
            yield return new WaitForSeconds(0.5f);
            sbCharacter.Move(Direction.Right, 3).SetSprite(sSpirit[1]);
            yield return new WaitForSeconds(0.3f);
            sbCharacter.Move(Direction.Left, 3).SetSprite(sSpirit[0]);
            yield return new WaitForSeconds(0.3f);
            sbCharacter.Move(Direction.Right, 3);
            yield return new WaitForSeconds(0.15f);
            sbCharacter.Move(Direction.Right, 3).SetSprite(sSpirit[1]);

            sbAttack = ScreenElement.BuildSprite("Attack", AnimParent).SetSize(24, 24).SetSprite(sSpiritEnergy).SetPosition(10, 4);
            sbCharacter.SetTransparent(false);
            sbCharacter.transform.SetAsLastSibling();
            for (int i = 0; i < 38; i++) {
                yield return new WaitForSeconds(1.5f / 38);
                sbAttack.Move(Direction.Left);
            }
            sbCharacter.SetActive(false);
            sbAttack.PlaceOutside(Direction.Right);
            for (int i = 0; i < 32; i++) {
                yield return new WaitForSeconds(1.4f / 32);
                sbAttack.Move(Direction.Left);
            }
            sbAttack.PlaceOutside(Direction.Right);
            sbEnemy.SetActive(true).Center().SetSprite(sEnemyDigimon[0]);
            for (int i = 0; i < 4; i++) {
                yield return new WaitForSeconds(0.4f / 4);
                sbAttack.Move(Direction.Left);
            }
            sbAttack.Dispose();
            sbEnemy.FlipHorizontal(false);
            for (int i = 0; i < 2; i++) {
                sbEnemy.SetSprite(sExplosionBig);
                yield return new WaitForSeconds(0.5f);
                sbEnemy.SetSprite(sExplosionSmall);
                yield return new WaitForSeconds(0.5f);
            }
            sbEnemy.SetActive(false);
            sbCharacter.SetSize(32, 32).SetPosition(0, 0).SetSprite(sCharacter[0]).SetActive(true);

            yield return new WaitForSeconds(0.3f);
            sbSpirit = ScreenElement.BuildSprite("Spirit", AnimParent).SetSize(24, 24).Center().SetSprite(sSpirit[0]);
            yield return new WaitForSeconds(0.3f);
            sbSpirit.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbSpirit.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            sbSpirit.SetActive(false);
            yield return new WaitForSeconds(0.3f);

            sbEnemy.FlipHorizontal(true).SetSprite(sEnemyDigimon[0]).PlaceOutside(Direction.Left).SetActive(true);
            sbCharacter.SetSprite(sCharacter[9]);
            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i < 26; i++) {
                sbEnemy.Move(Direction.Right);
                sbCharacter.Move(Direction.Right);
                yield return new WaitForSeconds(3.3f / 32f);
            }
            sbEnemy.SetActive(false);
            yield return new WaitForSeconds(0.6f);
            sbCharacter.SetActive(false);
            sbEnemy.FlipHorizontal(false).Center().SetActive(true);
            yield return new WaitForSeconds(0.45f);

            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("Curtain", AnimParent).SetSprite(sCurtain).PlaceOutside(Direction.Down).SetTransparent(true);
            for (int i = 0; i < 64; i++) {
                if (i > 31) sbEnemy.Move(Direction.Up);
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(3.4f / 64);
            }
            ScreenElement.BuildSprite("DTector", AnimParent).SetSprite(sDTector);
            SpriteBuilder sbPower = ScreenElement.BuildSprite("Power", AnimParent).SetSprite(sPowerBlack).SetTransparent(true);

            for (int i = 0; i < 5; i++) {
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
                yield return new WaitForSeconds(0.15f);
            }

            yield return CharHappy();
        }

        public static IEnumerator SummonDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonCr = spriteDB.GetDigimonSprite(digimon, SpriteAction.Crush);
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sPowerBlack = spriteDB.givePowerInverted;
            Sprite sPowerWhite = spriteDB.givePower;

            audioMgr.PlaySound(audioMgr.summonDigimon);

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite(digimon, AnimParent).SetSize(24, 24).Center().SetActive(false);
            SpriteBuilder sbBlackScreen = ScreenElement.BuildSprite("BlackScreen", AnimParent).SetSprite(sBlackScreen);

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
            sbDigimon.SetSprite(sDigimon).SetActive(true);
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
            sbDigimon.SetSprite(sDigimon);
            yield return new WaitForSeconds(0.15f);
        }
        public static IEnumerator UnlockDigimon(string digimon, bool useSpiritForm = false) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon, useSpiritForm ? SpriteAction.Spirit : default);
            Sprite sCurtain = spriteDB.curtain;
            Sprite sDTector = spriteDB.dTector;
            Sprite sPowerBlack = spriteDB.giveMassivePowerInverted;

            audioMgr.PlaySound(audioMgr.unlockDigimon);

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite(digimon, AnimParent).SetSize(24, 24).Center().SetSprite(sDigimon);
            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("BlackScreen", AnimParent).SetSprite(sCurtain).SetTransparent(true);
            sbCurtain.PlaceOutside(Direction.Down);

            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 32; i++) {
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);

            }
            for (int i = 0; i < 32; i++) {
                sbCurtain.Move(Direction.Up);
                sbDigimon.Move(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);
            }

            yield return new WaitForSeconds(0.75f);
            sbDigimon.Dispose();
            sbCurtain.Dispose();

            ScreenElement.BuildSprite("DTector", AnimParent).SetSprite(sDTector);
            SpriteBuilder sbPower = ScreenElement.BuildSprite("Power", AnimParent).SetSprite(sPowerBlack).SetTransparent(true);
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
        public static IEnumerator ReceiveSpirit(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon, SpriteAction.Spirit);

            //audioMgr.PlaySound(audioMgr.unpleasantBeep);

            RectangleBuilder rbPlatform = ScreenElement.BuildRectangle("Platform", AnimParent).SetSize(26, 1).SetPosition(3, 29);
            SpriteBuilder sbDigimon = ScreenElement.BuildSprite(digimon, AnimParent).SetSize(24, 24)
                .FlipHorizontal(true).Center().SetSprite(sDigimon).PlaceOutside(Direction.Up);

            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 28; i++) {
                sbDigimon.Move(Direction.Down);
                yield return new WaitForSeconds(2.5f / 28);
            }

            yield return new WaitForSeconds(0.75f);
            sbDigimon.FlipHorizontal(false);
            yield return new WaitForSeconds(0.75f);
        }
        public static IEnumerator LoseSpirit(string spiritLost, string enemyDigimon) {
            Sprite sSpiritLost = spriteDB.GetDigimonSprite(spiritLost, SpriteAction.Spirit);
            Sprite sEnemyDigimon = spriteDB.GetDigimonSprite(enemyDigimon);
            Sprite sAttractor = spriteDB.stealSpiritAttractor;

            SpriteBuilder sbSpiritLost = ScreenElement.BuildSprite("Enemy", AnimParent)
                .SetSize(24, 24).Center().SetX(8).SetSprite(sSpiritLost).SetActive(false);
            SpriteBuilder sbAttractor = ScreenElement.BuildSprite("Attractor", AnimParent)
                .SetSize(3, 24).Center().SetX(20).SetSprite(sAttractor);
            SpriteBuilder sbEnemyDigimon = ScreenElement.BuildSprite("Enemy", AnimParent)
                .SetSize(24, 24).Center().SetX(0).SetSprite(sEnemyDigimon).FlipHorizontal(true);

            audioMgr.PlaySound(audioMgr.attackTravelVeryLong);

            for (int i = 0; i < 12; i++) {
                sbAttractor.Move(Direction.Right);
                yield return new WaitForSeconds(2f / 32);
            }

            sbEnemyDigimon.SetActive(false);
            sbSpiritLost.SetActive(true);
            sbAttractor.PlaceOutside(Direction.Left);

            for (int i = 0; i < 8; i++) {
                sbAttractor.Move(Direction.Right);
                yield return new WaitForSeconds(2f / 32);
            }

            for (int i = 0; i < 32; i++) {
                sbAttractor.Move(Direction.Left);
                sbSpiritLost.Move(Direction.Left);
                yield return new WaitForSeconds(2f / 32);
            }
            sbEnemyDigimon.SetActive(true);
            sbSpiritLost.Move(Direction.Right, 40 + 28);
            sbAttractor.Move(Direction.Right, 40 + 28);

            for (int i = 0; i < 42; i++) {
                sbAttractor.Move(Direction.Left);
                sbSpiritLost.Move(Direction.Left);
                yield return new WaitForSeconds(2f / 32);
            }

            sbSpiritLost.SetActive(false);
            sbAttractor.SetActive(false);

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 16; i++) {
                sbEnemyDigimon.Move(Direction.Up, 2);
                yield return new WaitForSeconds(0.6f / 16);
            }

            yield return new WaitForSeconds(0.2f);
        }
        public static IEnumerator LevelUpDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDTector = spriteDB.dTector;
            Sprite sPower = spriteDB.givePower;
            Sprite sPowerInverted = spriteDB.givePowerInverted;
            Sprite sMassivePowerInverted = spriteDB.giveMassivePowerInverted;

            audioMgr.PlaySound(audioMgr.unlockDigimon);

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite(digimon, AnimParent).SetSize(24, 24).Center().SetSprite(sDigimon);
            SpriteBuilder sbPower = ScreenElement.BuildSprite("Power", AnimParent).SetSprite(sPower).SetTransparent(true).SetActive(false);

            //Give power to Digimon
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.40f);
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
            }
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.20f);
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
            }
            sbPower.SetSprite(sPowerInverted);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbPower.SetActive(false);

            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(true);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetSprite(sMassivePowerInverted);

            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(false);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(true);

            yield return new WaitForSeconds(0.15f);
            sbPower.SetActive(false);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbPower.SetActive(false);

            //Give power to D-Tector
            sbDigimon.SetSize(32, 32).Center().SetSprite(sDTector);
            sbPower.SetSprite(sMassivePowerInverted);
            sbPower.SetActive(false);

            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < 5; i++) {
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator EraseDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sGivePower = spriteDB.givePower;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Digimon", AnimParent).SetSize(24, 24).SetSprite(sDigimon);
            sbDigimon.Center();
            sbDigimon.SetActive(false);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("Digimon", AnimParent).SetSprite(sGivePower);
            sbGivePower.SetTransparent(true);
            sbGivePower.SetActive(false);

            audioMgr.PlaySound(audioMgr.loseDigimon);

            yield return new WaitForSeconds(0.2f);
            sbDigimon.SetActive(true);
            yield return new WaitForSeconds(1f);

            sbGivePower.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            sbGivePower.SetActive(false);
            yield return new WaitForSeconds(0.9f);
            sbGivePower.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            sbGivePower.SetActive(false);
            sbDigimon.SetActive(false);

            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(0.3f);
                sbDigimon.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbDigimon.SetActive(false);
            }

            sbGivePower.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            sbGivePower.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }
        public static IEnumerator LevelDownDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sMassivePower = spriteDB.giveMassivePower;
            Sprite sMassivePowerInverted = spriteDB.giveMassivePowerInverted;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite(digimon, AnimParent).SetSize(24, 24).Center().SetSprite(sDigimon);
            SpriteBuilder sbPower = ScreenElement.BuildSprite("Power", AnimParent).SetTransparent(true).SetActive(false);

            audioMgr.PlaySound(audioMgr.levelDownDigimon);

            //Give power to Digimon
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.20f);
                sbDigimon.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(true);
            }
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(true);
            }
            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.20f);
                sbPower.SetSprite(sMassivePower).SetActive(true);
                yield return new WaitForSeconds(0.20f);
                sbPower.SetSprite(sMassivePowerInverted).SetActive(true);
                yield return new WaitForSeconds(0.20f);
                sbPower.SetActive(false);
            }
            yield return new WaitForSeconds(0.40f);
            sbPower.SetSprite(sMassivePower).SetActive(true);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetSprite(sMassivePowerInverted).SetActive(true);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(false);
            yield return new WaitForSeconds(0.60f);
            sbPower.SetSprite(sMassivePower).SetActive(true);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetSprite(sMassivePowerInverted).SetActive(true);
            yield return new WaitForSeconds(0.20f);
            sbPower.SetActive(false);
            yield return new WaitForSeconds(0.30f);
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(true);
            }
            yield return new WaitForSeconds(0.50f);
        }
        public static IEnumerator CharHappy() {
            yield return CharHappyShort();
            yield return CharHappyShort();
        }
        public static IEnumerator CharHappyShort() {
            Sprite charIdle = gm.PlayerCharSprites[0];
            Sprite charHappy = gm.PlayerCharSprites[6];

            audioMgr.PlaySound(audioMgr.charHappy);

            SpriteBuilder sbChar = ScreenElement.BuildSprite("CharHappy", AnimParent).SetSprite(charIdle);

            for (int i = 0; i < 2; i++) {
                sbChar.SetSprite(charIdle);
                yield return new WaitForSeconds(0.5f);
                sbChar.SetSprite(charHappy);
                yield return new WaitForSeconds(0.5f);
            }
            sbChar.Dispose();
        }
        public static IEnumerator CharSad() {
            yield return CharSadShort();
            yield return CharSadShort();
        }
        public static IEnumerator CharSadShort() {
            Sprite charIdle = gm.PlayerCharSprites[0];
            Sprite charSad = gm.PlayerCharSprites[7];

            audioMgr.PlaySound(audioMgr.charSad);

            SpriteBuilder sbChar = ScreenElement.BuildSprite("CharSad", AnimParent).SetSprite(charIdle);

            for (int i = 0; i < 2; i++) {
                sbChar.SetSprite(charIdle);
                yield return new WaitForSeconds(0.475f);
                sbChar.SetSprite(charSad);
                yield return new WaitForSeconds(0.475f);
            }
            sbChar.Dispose();
        }
        //TODO: Make the screen exit after the player presses A.
        public static IEnumerator AwardDistance(int score, int distanceBefore, int distanceAfter) {
            //yield return null;
            //gm.inputMgr.ConsumeLastKey(Direction.Up, Direction.Down);
            Sprite sScore = spriteDB.games_score;
            Sprite sDistance = spriteDB.games_distance;

            audioMgr.PlayButtonA();
            ScreenElement.BuildSprite("Score", AnimParent).SetSize(32, 5).SetPosition(1, 1).SetSprite(sScore);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            ScreenElement.BuildTextBox("ScoreText", AnimParent, DFont.Regular)
                .SetText(score.ToString()).SetSize(31, 5).SetPosition(1, 9).SetAlignment(TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            ScreenElement.BuildSprite("Distance", AnimParent).SetSize(32, 5).SetPosition(1, 18).SetSprite(sDistance);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            TextBoxBuilder tbDistance = ScreenElement.BuildTextBox("DistanceText", AnimParent, DFont.Regular)
                .SetText(distanceBefore.ToString()).SetSize(31, 5).SetPosition(1, 26).SetAlignment(TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayCharHappy();
            tbDistance.Text = distanceAfter.ToString();
            yield return new WaitForSeconds(2f);
            //gm.UnlockInput();
            //yield return new WaitUntil(() => gm.inputMgr.ConsumeLastKey(Direction.Up, Direction.Down));
        }
        public static IEnumerator TravelMap(int world, int mapBefore, int mapAfter, float animDuration) {
            ContainerBuilder cbMap = gm.BuildMapScreen(world, AnimParent);

            if (mapBefore == 0) cbMap.SetPosition(0, 0);
            else if (mapBefore == 1) cbMap.SetPosition(0, -32);
            else if (mapBefore == 2) cbMap.SetPosition(-32, -32);
            else if (mapBefore == 3) cbMap.SetPosition(-32, 0);

            //If both areas are the same:
            if (mapBefore == mapAfter) {
                yield return new WaitForSeconds(animDuration);
            }
            //If the areas are consecutive of one another:
            else if (Mathf.Abs(mapBefore - mapAfter) == 1
                    || mapBefore == 0 && mapAfter == 3
                    || mapBefore == 3 && mapAfter == 0) {
                Direction animationDir = Direction.Right;
                if (mapBefore == 0 && mapAfter == 3) animationDir = Direction.Left;
                else if (mapBefore == 0 && mapAfter == 1) animationDir = Direction.Up;
                else if (mapBefore == 1 && mapAfter == 0) animationDir = Direction.Down;
                else if (mapBefore == 1 && mapAfter == 2) animationDir = Direction.Left;
                else if (mapBefore == 2 && mapAfter == 1) animationDir = Direction.Right;
                else if (mapBefore == 2 && mapAfter == 3) animationDir = Direction.Down;
                else if (mapBefore == 3 && mapAfter == 2) animationDir = Direction.Up;
                else if (mapBefore == 3 && mapAfter == 0) animationDir = Direction.Right;

                for (int i = 0; i < 32; i++) {
                    cbMap.Move(animationDir);
                    yield return new WaitForSeconds(animDuration / 32);
                }
            }
            else {
                if (mapBefore == 0 && mapAfter == 2) {
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Up);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Left);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                }
                else if (mapBefore == 2 && mapAfter == 0) {
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Down);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Right);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                }
                else if (mapBefore == 1 && mapAfter == 3) {
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Left);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Down);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                }
                else if (mapBefore == 3 && mapAfter == 1) {
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Right);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                    for (int i = 0; i < 32; i++) {
                        cbMap.Move(Direction.Up);
                        yield return new WaitForSeconds(animDuration / 64);
                    }
                }
            }
            cbMap.Dispose();
        }
        public static IEnumerator DisplayNewArea(int world, int area, int distance) {
            ContainerBuilder cbMap = gm.BuildMapScreen(world, AnimParent);

            int map = Database.Worlds[world].areas[area].map;

            if (map == 0) cbMap.SetPosition(0, 0);
            else if (map == 1) cbMap.SetPosition(0, -32);
            else if (map == 2) cbMap.SetPosition(-32, -32);
            else if (map == 3) cbMap.SetPosition(-32, 0);

            //Spawn the area name and marker.
            int areaPosY = (map == 0 || map == 3) ? 1 : 26;
            ScreenElement.BuildTextBox("AreaName", AnimParent, DFont.Small).SetText(string.Format("area{0:00}", area + 1))
                .SetPosition(2, areaPosY);
            ScreenElement.BuildRectangle("OptionMarker", AnimParent).SetSize(2, 2).SetFlickPeriod(0.25f)
                .SetPosition(Database.Worlds[world].areas[area].coords);

            //Draw the completed area markers.

            int[] areasInCurrentMap = Database.Worlds[world].GetAreasInMap(map);

            foreach (int i in areasInCurrentMap) {
                if (gm.WorldMgr.GetAreaCompleted(0, i)) {
                    Vector2Int markerPos = Database.Worlds[world].areas[i].coords;
                    ScreenElement.BuildRectangle("Area" + i + "Marker", AnimParent).SetSize(2, 2).SetPosition(markerPos.x, markerPos.y);
                }
            }
            yield return new WaitForSeconds(2.25f);

            //Display distance.
            SpriteBuilder sbDistance = ScreenElement.BuildSprite("DistanceBackground", AnimParent).SetSprite(spriteDB.map_distanceScreen);
            ScreenElement.BuildTextBox("Distance", AnimParent, DFont.Regular)
                .SetText(distance.ToString()).SetSize(25, 5).SetPosition(6, 25).SetAlignment(TextAnchor.UpperRight);
            yield return new WaitForSeconds(2.5f);
        }
        public static IEnumerator ForcedTravelMap(int world, int areaBefore, int areaAfter, int newDistance) {
            int mapBefore = Database.Worlds[world].areas[areaBefore].map;
            int mapAfter = Database.Worlds[world].areas[areaAfter].map;

            audioMgr.PlaySound(audioMgr.travelMap);
            yield return TravelMap(world, mapBefore, mapAfter, 3.5f);
            yield return DisplayNewArea(world, areaAfter, newDistance);
        }
        public static IEnumerator SwapDDock(int ddock, string newDigimon) {
            float animDuration = 1.5f;
            Sprite newDigimonSprite = spriteDB.GetDigimonSprite(newDigimon);
            Sprite newDigimonSpriteCr = spriteDB.GetDigimonSprite(newDigimon, SpriteAction.Crush);

            audioMgr.PlaySound(audioMgr.changeDock);

            SpriteBuilder bBlackBars = ScreenElement.BuildSprite("BlackBars", AnimParent).SetSprite(gm.spriteDB.blackBars).PlaceOutside(Direction.Down);
            SpriteBuilder bDDock = ScreenElement.BuildSprite("DDock", AnimParent).SetSprite(gm.spriteDB.status_ddock[ddock]);
            SpriteBuilder bDDockSprite = gm.GetDDockScreenElement(ddock, bDDock.transform);

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.Move(Direction.Up);
                bDDock.Move(Direction.Up);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            bDDockSprite.SetSprite(newDigimonSprite);
            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.Move(Direction.Down);
                bDDock.Move(Direction.Down);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            yield return new WaitForSeconds(0.5f);

            audioMgr.PlaySound(audioMgr.charHappy); //Originally this started after 0.175 seconds.
            bDDockSprite.SetSprite(newDigimonSpriteCr);
            for (int i = 0; i < 5; i++) {
                bDDockSprite.SetActive(false);
                yield return new WaitForSeconds(0.175f);
                bDDockSprite.SetActive(true);
                yield return new WaitForSeconds(0.175f);
            }

            yield return new WaitForSeconds(0.5f);

            bBlackBars.Dispose();
            bDDock.Dispose();
        }
        public static IEnumerator EncounterEnemy(string digimon, float finalDelay = 1f) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonAt = spriteDB.GetDigimonSprite(digimon, SpriteAction.Attack);
            Sprite sGivePower = spriteDB.givePower;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24).Center().SetSprite(sDigimon);
            sbDigimon.FlipHorizontal(true);
            sbDigimon.SetActive(false);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("Power", AnimParent).SetSprite(sGivePower).SetTransparent(true);
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
            yield return new WaitForSeconds(finalDelay);
        }
        public static IEnumerator EncounterBoss(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonAt = spriteDB.GetDigimonSprite(digimon, SpriteAction.Attack);
            Sprite sGivePower = spriteDB.giveMassivePower;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24).Center().SetSprite(sDigimon);
            sbDigimon.FlipHorizontal(true);
            sbDigimon.SetActive(false);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("MassivePower", AnimParent).SetSprite(sGivePower).SetTransparent(true);
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
        public static IEnumerator SpendCallPoints(int pointsBefore, int pointsAfter) {
            SpriteBuilder sbCallPoints = ScreenElement.BuildSprite("CallPointScreen", AnimParent).SetSprite(spriteDB.battle_callPoints_screen);
            RectangleBuilder[] callPoints = new RectangleBuilder[pointsBefore];
            for (int i = 0; i < callPoints.Length; i++) {
                callPoints[i] = ScreenElement.BuildRectangle($"CallPoint{i}", sbCallPoints.transform).SetSize(2, 5).SetPosition(1 + (3 * i), 25);
            }
            yield return new WaitForSeconds(1f);
            audioMgr.PlayButtonA();
            for (int i = callPoints.Length - 1; i > pointsAfter - 1; i--) {
                callPoints[i].Dispose();
            }
            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator DeportDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            yield return DeportSprite(sDigimon);
        }
        public static IEnumerator DeportSprite(Sprite sprite, int dim = 24) {
            SpriteBuilder[] spDigimon = new SpriteBuilder[4];

            for (int i = 0; i < 4; i++) {
                spDigimon[i] = ScreenElement.BuildSprite($"Deport{i}", AnimParent)
                    .SetSize(dim, dim).SetSprite(sprite).SetTransparent(true).SetActive(false).Center();
            }
            spDigimon[0].SetActive(true);
            audioMgr.PlaySound(audioMgr.deport);

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.25f);
                spDigimon[0].SetActive(false);
                yield return new WaitForSeconds(0.25f);
                spDigimon[0].SetActive(true);
            }

            spDigimon[1].SetActive(true);
            spDigimon[2].SetActive(true);
            spDigimon[3].SetActive(true);

            yield return new WaitForSeconds(0.75f);
            for (int i = 0; i < 32; i++) {
                spDigimon[0].Move(Direction.Left);
                spDigimon[1].Move(Direction.Right);
                spDigimon[2].Move(Direction.Up);
                spDigimon[3].Move(Direction.Down);
                yield return new WaitForSeconds(1.5f / 32);
            }
            yield return new WaitForSeconds(0.2f);
        }
        public static IEnumerator DeportSpirit(string digimon, GameChar character) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sGivePower = spriteDB.giveMassivePowerInverted;
            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);

            SpriteBuilder[] spDigimon = new SpriteBuilder[2];

            audioMgr.PlaySound(audioMgr.deportSpirit);

            for (int i = 0; i < 2; i++) {
                spDigimon[i] = ScreenElement.BuildSprite($"Deport{i}", AnimParent)
                    .SetSize(24, 24).SetSprite(sDigimon).SetTransparent(true).SetActive(true).Center();
            }

            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < 16; i++) {
                spDigimon[0].Move(Direction.Up);
                spDigimon[1].Move(Direction.Down);
                yield return new WaitForSeconds(1.75f / 16);
            }

            for (int i = 0; i < 3; i++) {
                spDigimon[0].SetActive(true);
                spDigimon[1].SetActive(true);
                yield return new WaitForSeconds(0.1f);
                spDigimon[0].SetActive(false);
                spDigimon[1].SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            spDigimon[0].Dispose();
            spDigimon[1].Dispose();
            yield return new WaitForSeconds(0.3f);

            SpriteBuilder spCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(sCharacter[0]).SetActive(false);
            SpriteBuilder spGivePower = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sGivePower).SetTransparent(true);
            for (int i = 0; i < 3; i++) {
                if (i == 2) spCharacter.SetActive(true);
                spGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                spGivePower.SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(0.2f);
            spGivePower.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            spGivePower.SetActive(false);
            yield return new WaitForSeconds(1f);
            spGivePower.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            spGivePower.SetActive(false);
            spCharacter.SetSprite(sCharacter[9]);
            yield return new WaitForSeconds(0.9f);
        }
        public static IEnumerator LevelUp(int levelBefore, int levelAfter) {
            Sprite[] sLevelUpBG = spriteDB.rewardBackground;
            Sprite sLevelUpIcon = spriteDB.rewards[0];
            SpriteBuilder sbLevelUpBG = ScreenElement.BuildSprite("LevelUpBackground", AnimParent).SetSprite(sLevelUpBG[0]);
            SpriteBuilder sbLevelUpIcon = ScreenElement.BuildSprite("LevelUpIcon", AnimParent)
                .SetSize(16, 16).SetSprite(sLevelUpIcon).Center().SetActive(false).SetTransparent(false);

            audioMgr.PlaySound(audioMgr.levelUp);

            for (int i = 0; i < 2; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            for (int cycle = 0; cycle < 4; cycle++) {
                sbLevelUpIcon.SetActive(cycle % 2 == 0);
                sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                yield return new WaitForSeconds(0.125f);
            }
            sbLevelUpIcon.SetActive(true);
            for (int i = 0; i < 4; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            sbLevelUpBG.SetActive(false);
            for (int i = 0; i < 9; i++) {
                sbLevelUpIcon.Move(Direction.Up);
                yield return new WaitForSeconds(0.5f / 9);
            }

            ScreenElement.BuildTextBox("Level", AnimParent, DFont.Regular)
                .SetText("LEVEL").SetSize(32, 5).SetPosition(0, 17).SetAlignment(TextAnchor.UpperCenter);
            TextBoxBuilder tbLevel = ScreenElement.BuildTextBox("LevelNumber", AnimParent, DFont.Regular)
                .SetText(levelBefore.ToString()).SetSize(29, 5).SetPosition(2, 24).SetAlignment(TextAnchor.UpperRight);

            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
            tbLevel.Text = (levelAfter).ToString();
            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator LevelDown(int levelBefore, int levelAfter) {
            Sprite[] sLevelUpBG = spriteDB.rewardBackground.ReorderedAs(0, 3, 2, 1);
            Sprite sLevelUpIcon = spriteDB.rewards[0];
            SpriteBuilder sbLevelUpBG = ScreenElement.BuildSprite("LevelUpBackground", AnimParent).SetSprite(sLevelUpBG[0]);
            SpriteBuilder sbLevelUpIcon = ScreenElement.BuildSprite("LevelUpIcon", AnimParent).SetSize(16, 16).SetSprite(sLevelUpIcon);
            sbLevelUpIcon.Center(); //Center: 9, 9
            sbLevelUpIcon.SetActive(false);
            sbLevelUpIcon.SetTransparent(false);

            audioMgr.PlaySound(audioMgr.levelDown);

            for (int i = 0; i < 2; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            for (int cycle = 0; cycle < 4; cycle++) {
                sbLevelUpIcon.SetActive(cycle % 2 == 0);
                sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                yield return new WaitForSeconds(0.125f);
            }
            sbLevelUpIcon.SetActive(true);
            for (int i = 0; i < 4; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbLevelUpBG.SetSprite(sLevelUpBG[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            sbLevelUpBG.SetActive(false);
            for (int i = 0; i < 9; i++) {
                sbLevelUpIcon.Move(Direction.Up);
                yield return new WaitForSeconds(0.5f / 9);
            }

            ScreenElement.BuildTextBox("Level", AnimParent, DFont.Regular)
                .SetText("LEVEL").SetSize(32, 5).SetPosition(0, 17).SetAlignment(TextAnchor.UpperCenter);
            TextBoxBuilder tbLevel = ScreenElement.BuildTextBox("LevelNumber", AnimParent, DFont.Regular)
                .SetText(levelBefore.ToString()).SetSize(29, 5).SetPosition(2, 24).SetAlignment(TextAnchor.UpperRight);

            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
            tbLevel.Text = levelAfter.ToString();
            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
        }
        //Note: this is not the same as ARewardDistance
        public static IEnumerator ChangeDistance(int distanceBefore, int distanceAfter) {
            ScreenElement.BuildSprite("Distance", AnimParent).SetSize(32, 5).SetPosition(0, 17).SetSprite(spriteDB.animDistance);
            TextBoxBuilder tbLevel = ScreenElement.BuildTextBox("DistanceNumber", AnimParent, DFont.Regular)
                .SetText(distanceBefore.ToString()).SetSize(29, 5).SetPosition(2, 24).SetAlignment(TextAnchor.UpperRight);

            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
            tbLevel.Text = distanceAfter.ToString();
            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator DisplayTurn(
                string friendlyDigimon, int friendlyAttack, int friendlyEnergyRank,
                string enemyDigimon, int enemyAttack, int enemyEnergyRank,
                int winner, bool disobeyed, int loserHPbefore, int loserHPnow) {
            Sprite[] friendlySprites = spriteDB.GetAllDigimonBattleSprites(friendlyDigimon, friendlyEnergyRank);
            Sprite[] enemySprites = spriteDB.GetAllDigimonBattleSprites(enemyDigimon, enemyEnergyRank);

            yield return LaunchAttack(friendlySprites, friendlyAttack, false, disobeyed);
            yield return LaunchAttack(enemySprites, enemyAttack, true, false);
            yield return AttackCollision(friendlyAttack, friendlySprites, enemyAttack, enemySprites, winner);
            if (winner == 0) {
                //If the enemy used crush (and you, ability), skip the ability animation.
                Sprite abilitySprite = (enemyAttack == 1) ? null : friendlySprites[4];
                yield return DestroyLoser(enemySprites, friendlyAttack, abilitySprite, true, loserHPbefore, loserHPnow);
            }
            else if (winner == 1) {
                Sprite abilitySprite = (friendlyAttack == 1) ? null : enemySprites[4];
                yield return DestroyLoser(friendlySprites, enemyAttack, abilitySprite, false, loserHPbefore, loserHPnow);
            }
        }
        public static IEnumerator AWardSpiritPower(int SPbefore) {
            SpriteBuilder sbSPBackground = ScreenElement.BuildSprite("SPBackground", AnimParent);
            Coroutine bgAnimation = gm.StartCoroutine(AnimateSPScreen(sbSPBackground));

            TextBoxBuilder tbSpirits = ScreenElement.BuildTextBox("SPAmount", AnimParent, DFont.Small)
                .SetText(SPbefore.ToString()).SetSize(32, 11).SetPosition(0, 21).SetAlignment(TextAnchor.UpperRight);
            tbSpirits.InvertColors(true);
            tbSpirits.SetComponentSize(28, 5);
            tbSpirits.SetComponentPosition(2, 3);
            tbSpirits.SetActive(false);

            yield return new WaitForSeconds(0.4f);
            tbSpirits.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < 3; i++) {
                IncreaseSP();
                yield return new WaitForSeconds(0.4f);
            }
            yield return new WaitForSeconds(0.4f);

            void IncreaseSP() {
                SPbefore += (SPbefore >= 99) ? 0 : 1;
                tbSpirits.Text = SPbefore.ToString();
            }
            gm.StopCoroutine(bgAnimation);
        }
        public static IEnumerator PaySpiritPower(int SPbefore, int SPafter) {
            SpriteBuilder sbSPBackground = ScreenElement.BuildSprite("SPBackground", AnimParent);
            Coroutine bgAnimation = gm.StartCoroutine(AnimateSPScreen(sbSPBackground));

            TextBoxBuilder tbSpirits = ScreenElement.BuildTextBox("SPAmount", AnimParent, DFont.Small)
                .SetText(SPbefore.ToString()).SetSize(32, 11).SetPosition(0, 21).SetAlignment(TextAnchor.UpperRight);
            tbSpirits.InvertColors(true);
            tbSpirits.SetComponentSize(28, 5);
            tbSpirits.SetComponentPosition(2, 3);
            tbSpirits.SetActive(false);

            yield return new WaitForSeconds(1f);
            audioMgr.PlayButtonA();
            tbSpirits.SetActive(true);

            yield return new WaitForSeconds(1f);
            audioMgr.PlayButtonA();
            tbSpirits.Text = SPafter.ToString();

            yield return new WaitForSeconds(1f);

            gm.StopCoroutine(bgAnimation);
        }
        public static IEnumerator RegularEvolution(string digimonBefore, string digimonAfter) {
            Sprite sDigimonBefore = spriteDB.GetDigimonSprite(digimonBefore);
            Sprite sDigimonAfter = spriteDB.GetDigimonSprite(digimonAfter);
            Sprite sGivePower = spriteDB.givePower;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Digimon", AnimParent).SetSize(24, 24).Center().SetSprite(sDigimonBefore);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("Digimon", AnimParent).SetSprite(sGivePower).SetTransparent(true).SetActive(false);

            audioMgr.PlaySound(audioMgr.evolutionRegular);
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.5f);
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGivePower.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 5; i++) {
                if (i == 2) sbDigimon.SetSprite(sDigimonAfter);
                sbDigimon.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                sbDigimon.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(0.25f);
            sbGivePower.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            sbGivePower.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
        public static IEnumerator SpiritEvolution(GameChar character, string digimon) {
            Sprite sGivePower = spriteDB.givePower;
            Sprite sGiveMassivePowerBlack = spriteDB.giveMassivePowerInverted;
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sCurtain = spriteDB.curtain;

            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            Sprite[] sDigimon = spriteDB.GetAllDigimonSprites(digimon);

            SpriteBuilder sbBackground = ScreenElement.BuildSprite("BlackBackground", AnimParent).SetSprite(sBlackScreen).SetActive(false);
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Char", AnimParent).SetSprite(sCharacter[0]);
            audioMgr.PlaySound(audioMgr.evolutionSpirit);
            yield return new WaitForSeconds(0.5f);
            SpriteBuilder sbGiveMassivePower = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sGiveMassivePowerBlack).SetTransparent(true);

            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.2f);
                sbGiveMassivePower.SetActive(false);
                yield return new WaitForSeconds(0.4f);
                sbGiveMassivePower.SetActive(true);
            }

            sbCharacter.SetSprite(sCharacter[9]);

            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbGiveMassivePower.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.2f);

            sbCharacter.PlaceOutside(Direction.Down);
            sbCharacter.SetSprite(sCharacter[0]);

            SpriteBuilder[] sbDigimon = new SpriteBuilder[4];
            sbDigimon[0] = ScreenElement.BuildSprite("Spirit", AnimParent).SetSize(24, 24).SetSprite(sDigimon[3]).Center();

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.15f);
                sbDigimon[0].SetActive(false);
                yield return new WaitForSeconds(0.25f);
                sbDigimon[0].SetActive(true);
            }
            yield return new WaitForSeconds(0.7f);

            for (int i = 1; i < 4; i++) {
                sbDigimon[i] = ScreenElement.BuildSprite("Spirit", AnimParent).SetSize(24, 24).SetSprite(sDigimon[3]).Center();
                sbDigimon[i].SetTransparent(true);
            }

            for (int i = 0; i < 32; i++) {
                sbDigimon[0].Move(Direction.Left);
                sbDigimon[1].Move(Direction.Right);
                sbDigimon[2].Move(Direction.Up);
                sbDigimon[3].Move(Direction.Down);
                yield return new WaitForSeconds(3f / 32);
            }

            for (int i = 1; i < 4; i++) sbDigimon[i].Dispose();

            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < 64; i++) {
                sbCharacter.Move(Direction.Up);
                yield return new WaitForSeconds(1f / 64);
            }
            yield return new WaitForSeconds(0.7f);

            sbBackground.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 3; i++) {
                sbBackground.SetSprite(sGivePower);
                yield return new WaitForSeconds(0.1f);
                sbBackground.SetSprite(sBlackScreen);
                yield return new WaitForSeconds(0.5f);
            }

            SpriteBuilder sbBlackSprite = ScreenElement.BuildSprite("BlackSprite", AnimParent).SetSprite(sDigimon[4]);
            yield return new WaitForSeconds(0.1f);

            sbBlackSprite.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            sbBlackSprite.SetActive(true);
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++) {
                sbBlackSprite.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                sbBlackSprite.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            sbBlackSprite.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbBlackSprite.SetActive(true);
            yield return new WaitForSeconds(0.1f);

            sbBackground.SetActive(false);
            sbBlackSprite.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            sbDigimon[0].SetSprite(sDigimon[0]).Center();
            yield return new WaitForSeconds(0.2f);

            sbBlackSprite.PlaceOutside(Direction.Down);
            sbBlackSprite.SetSprite(sCurtain).SetTransparent(true);
            sbBlackSprite.SetActive(true);

            for (int i = 0; i < 64; i++) {
                sbBlackSprite.Move(Direction.Up);
                yield return new WaitForSeconds(3f / 64);
            }

            yield return new WaitForSeconds(0.6f);
            sbDigimon[0].SetSprite(sDigimon[1]);
            yield return new WaitForSeconds(0.8f);
            sbDigimon[0].SetSprite(sDigimon[0]);
            yield return new WaitForSeconds(0.6f);
        }
        public static IEnumerator FusionSpiritEvolution(GameChar character, string digimon) {
            Sprite sGiveMassivePowerBlack = spriteDB.giveMassivePowerInverted;
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sCurtain = spriteDB.curtain;
            Sprite sCurtainSpecial = spriteDB.curtainSpecial[0];

            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            Sprite[] sDigimon = spriteDB.GetAllDigimonSprites(digimon);
            Sprite[] sHumans = new Sprite[5];
            Sprite[] sAnimals = new Sprite[5];
            if (digimon == "kaisergreymon") {
                sHumans[0] = spriteDB.GetDigimonSprite("agunimon", SpriteAction.SpiritSmall);
                sHumans[1] = spriteDB.GetDigimonSprite("kazemon", SpriteAction.SpiritSmall);
                sHumans[2] = spriteDB.GetDigimonSprite("kumamon", SpriteAction.SpiritSmall);
                sHumans[3] = spriteDB.GetDigimonSprite("grumblemon", SpriteAction.SpiritSmall);
                sHumans[4] = spriteDB.GetDigimonSprite("arbormon", SpriteAction.SpiritSmall);
                sAnimals[0] = spriteDB.GetDigimonSprite("burninggreymon", SpriteAction.SpiritSmall);
                sAnimals[1] = spriteDB.GetDigimonSprite("zephyrmon", SpriteAction.SpiritSmall);
                sAnimals[2] = spriteDB.GetDigimonSprite("korikakumon", SpriteAction.SpiritSmall);
                sAnimals[3] = spriteDB.GetDigimonSprite("gigasmon", SpriteAction.SpiritSmall);
                sAnimals[4] = spriteDB.GetDigimonSprite("petaldramon", SpriteAction.SpiritSmall);
            }
            else {
                sHumans[0] = spriteDB.GetDigimonSprite("lobomon", SpriteAction.SpiritSmall);
                sHumans[1] = spriteDB.GetDigimonSprite("beetlemon", SpriteAction.SpiritSmall);
                sHumans[2] = spriteDB.GetDigimonSprite("loweemon", SpriteAction.SpiritSmall);
                sHumans[3] = spriteDB.GetDigimonSprite("mercurymon", SpriteAction.SpiritSmall);
                sHumans[4] = spriteDB.GetDigimonSprite("lanamon", SpriteAction.SpiritSmall);
                sAnimals[0] = spriteDB.GetDigimonSprite("kendogarurumon", SpriteAction.SpiritSmall);
                sAnimals[1] = spriteDB.GetDigimonSprite("metalkabuterimon", SpriteAction.SpiritSmall);
                sAnimals[2] = spriteDB.GetDigimonSprite("kaiserleomon", SpriteAction.SpiritSmall);
                sAnimals[3] = spriteDB.GetDigimonSprite("sephirothmon", SpriteAction.SpiritSmall);
                sAnimals[4] = spriteDB.GetDigimonSprite("calmaramon", SpriteAction.SpiritSmall);
            }

            //Common animation.
            SpriteBuilder sbBackground = ScreenElement.BuildSprite("BlackBackground", AnimParent).SetSprite(sBlackScreen).SetActive(false);
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Char", AnimParent).SetSprite(sCharacter[0]);
            audioMgr.PlaySound(audioMgr.evolutionSpirit);
            yield return new WaitForSeconds(0.5f);
            SpriteBuilder sbGiveMassivePower = ScreenElement.BuildSprite("Char", AnimParent).SetSprite(sGiveMassivePowerBlack).SetTransparent(true);

            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.2f);
                sbGiveMassivePower.SetActive(false);
                yield return new WaitForSeconds(0.4f);
                sbGiveMassivePower.SetActive(true);
            }

            sbCharacter.SetSprite(sCharacter[9]);

            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbGiveMassivePower.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.2f);

            //Small spirits display - total animation duration: 3.0 s.
            sbCharacter.Dispose();
            SpriteBuilder sbSmallHuman = ScreenElement.BuildSprite("Human", AnimParent).SetSize(14, 16);
            SpriteBuilder sbSmallAnimal = ScreenElement.BuildSprite("Animal", AnimParent).SetSize(14, 16);
            for (int i = 0; i < 5; i++) {
                sbSmallHuman.SetY(16).PlaceOutside(Direction.Left).Move(Direction.Left);
                sbSmallAnimal.SetY(16).PlaceOutside(Direction.Right).Move(Direction.Right);
                sbSmallHuman.SetSprite(sHumans[i]);
                sbSmallAnimal.SetSprite(sAnimals[i]);
                for (int j = 0; j < 4; j++) {
                    sbSmallHuman.Move(Direction.Right, 4);
                    sbSmallAnimal.Move(Direction.Left, 4);
                    yield return new WaitForSeconds(0.6f / 10);
                }
                for (int j = 0; j < 6; j++) {
                    sbSmallHuman.Move(Direction.Up, 4);
                    sbSmallAnimal.Move(Direction.Up, 4);
                    yield return new WaitForSeconds(0.6f / 10);
                }
            }
            sbSmallHuman.Dispose();
            sbSmallAnimal.Dispose();
            //Create Transcendent spirit - total animation duration: 3.2 s
            SpriteBuilder sbTranscendent = ScreenElement.BuildSprite("Transcendent", AnimParent).SetSize(24, 24).SetSprite(sDigimon[3]).Center();
            RectangleBuilder sbCover = ScreenElement.BuildRectangle("Cover", AnimParent).SetSize(32, 32).SetColor(false);
            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("Curtain", AnimParent).SetSprite(sCurtainSpecial).PlaceOutside(Direction.Up);
            sbCurtain.SetTransparent(true);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("MassivePower", AnimParent).SetSprite(sGiveMassivePowerBlack);
            sbGivePower.SetTransparent(true);
            sbGivePower.SetActive(false);
            for (int i = 0; i < 32; i++) {
                if (i == 14 || i == 28) sbGivePower.SetActive(true);
                if (i == 15 || i == 29) sbGivePower.SetActive(false);
                sbCover.Move(Direction.Down);
                sbCurtain.Move(Direction.Down);
                yield return new WaitForSeconds(3.2f / 32);
            }
            sbCurtain.PlaceOutside(Direction.Down);
            sbGivePower.Dispose();
            //Flash spirit
            for (int i = 0; i < 3; i++) {
                sbTranscendent.SetActive(false);
                yield return new WaitForSeconds(0.35f);
                sbTranscendent.SetActive(true);
                yield return new WaitForSeconds(0.35f);
            }
            for (int i = 0; i < 32; i++) {
                sbTranscendent.Move(Direction.Up);
                yield return new WaitForSeconds(1.4f / 32);
            }
            //Digimon runs twice.
            sbTranscendent.SetSprite(sDigimon[2]);
            sbTranscendent.SetY(4).PlaceOutside(Direction.Right);
            for (int i = 0; i < 12; i++) {
                sbTranscendent.Move(Direction.Left, 6);
                yield return new WaitForSeconds(1f / 12);
            }
            yield return new WaitForSeconds(0.7f);
            sbTranscendent.SetSprite(sDigimon[2]);
            sbTranscendent.PlaceOutside(Direction.Right);
            for (int i = 0; i < 14; i++) {
                sbTranscendent.Move(Direction.Left, 2);
                yield return new WaitForSeconds(0.7f / 14);
            }
            sbTranscendent.SetSprite(sDigimon[0]);
            //Final Curtain
            sbCurtain.SetSprite(sCurtain).SetTransparent(true);
            sbCurtain.SetActive(true);

            for (int i = 0; i < 64; i++) {
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(3f / 64);
            }

            yield return new WaitForSeconds(0.6f);
            sbTranscendent.SetSprite(sDigimon[1]);
            yield return new WaitForSeconds(0.8f);
            sbTranscendent.SetSprite(sDigimon[0]);
            yield return new WaitForSeconds(0.6f);
        }
        public static IEnumerator SusanoomonEvolution(GameChar character) {
            Sprite sGiveMassivePowerBlack = spriteDB.giveMassivePowerInverted;
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sCurtainSpecial = spriteDB.curtainSpecial[1];

            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            Sprite[] sSusanoomon = spriteDB.GetAllDigimonSprites("susanoomon");
            Sprite sKaiserGreymon = spriteDB.GetDigimonSprite("kaisergreymon", SpriteAction.Spirit);
            Sprite sMagnaGarurumon = spriteDB.GetDigimonSprite("magnagarurumon", SpriteAction.Spirit);
            Sprite[] sHumans = new Sprite[10];
            Sprite[] sAnimals = new Sprite[10];
            {
                sHumans[0] = spriteDB.GetDigimonSprite("agunimon", SpriteAction.SpiritSmall);
                sHumans[1] = spriteDB.GetDigimonSprite("kazemon", SpriteAction.SpiritSmall);
                sHumans[2] = spriteDB.GetDigimonSprite("kumamon", SpriteAction.SpiritSmall);
                sHumans[3] = spriteDB.GetDigimonSprite("grumblemon", SpriteAction.SpiritSmall);
                sHumans[4] = spriteDB.GetDigimonSprite("arbormon", SpriteAction.SpiritSmall);
                sAnimals[0] = spriteDB.GetDigimonSprite("burninggreymon", SpriteAction.SpiritSmall);
                sAnimals[1] = spriteDB.GetDigimonSprite("zephyrmon", SpriteAction.SpiritSmall);
                sAnimals[2] = spriteDB.GetDigimonSprite("korikakumon", SpriteAction.SpiritSmall);
                sAnimals[3] = spriteDB.GetDigimonSprite("gigasmon", SpriteAction.SpiritSmall);
                sAnimals[4] = spriteDB.GetDigimonSprite("petaldramon", SpriteAction.SpiritSmall);
                sHumans[5] = spriteDB.GetDigimonSprite("lobomon", SpriteAction.SpiritSmall);
                sHumans[6] = spriteDB.GetDigimonSprite("beetlemon", SpriteAction.SpiritSmall);
                sHumans[7] = spriteDB.GetDigimonSprite("loweemon", SpriteAction.SpiritSmall);
                sHumans[8] = spriteDB.GetDigimonSprite("mercurymon", SpriteAction.SpiritSmall);
                sHumans[9] = spriteDB.GetDigimonSprite("lanamon", SpriteAction.SpiritSmall);
                sAnimals[5] = spriteDB.GetDigimonSprite("kendogarurumon", SpriteAction.SpiritSmall);
                sAnimals[6] = spriteDB.GetDigimonSprite("metalkabuterimon", SpriteAction.SpiritSmall);
                sAnimals[7] = spriteDB.GetDigimonSprite("kaiserleomon", SpriteAction.SpiritSmall);
                sAnimals[8] = spriteDB.GetDigimonSprite("sephirothmon", SpriteAction.SpiritSmall);
                sAnimals[9] = spriteDB.GetDigimonSprite("calmaramon", SpriteAction.SpiritSmall);
            }

            //Common animation.
            SpriteBuilder sbBackground = ScreenElement.BuildSprite("BlackBackground", AnimParent).SetSprite(sBlackScreen).SetActive(false);
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Char", AnimParent).SetSprite(sCharacter[0]);
            audioMgr.PlaySound(audioMgr.evolutionSpirit);
            yield return new WaitForSeconds(0.5f);
            SpriteBuilder sbGiveMassivePower = ScreenElement.BuildSprite("Char", AnimParent).SetSprite(sGiveMassivePowerBlack).SetTransparent(true);

            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.2f);
                sbGiveMassivePower.SetActive(false);
                yield return new WaitForSeconds(0.4f);
                sbGiveMassivePower.SetActive(true);
            }

            sbCharacter.SetSprite(sCharacter[9]);

            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbGiveMassivePower.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            sbGiveMassivePower.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            sbCharacter.PlaceOutside(Direction.Down);
            sbCharacter.SetSprite(sCharacter[0]);

            SpriteBuilder sbTranscendent = ScreenElement.BuildSprite("Transcendent", AnimParent).SetSize(24, 24).SetSprite(sKaiserGreymon).Center();
            SpriteBuilder sbSmallSpirit = ScreenElement.BuildSprite("SmallSpirit", AnimParent).SetSize(14, 16).SetActive(false);
            //KaiserGreymon's small spirits - total animation duration: 3.1 s.
            for (int i = 0; i < 4; i++) {
                yield return new WaitForSeconds(0.15f);
                sbTranscendent.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbTranscendent.SetActive(true);
            }
            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(0.1f);
                sbTranscendent.SetActive(false);
                sbSmallSpirit.SetSprite(sHumans[i]);
                if (i == 0) sbSmallSpirit.SetPosition(9, 0);
                else if (i == 1) sbSmallSpirit.SetPosition(0, 7);
                else if (i == 2) sbSmallSpirit.SetPosition(18, 7);
                else if (i == 3) sbSmallSpirit.SetPosition(2, 16);
                else if (i == 4) sbSmallSpirit.SetPosition(16, 16);
                sbSmallSpirit.SetActive(true);

                yield return new WaitForSeconds(0.28f);
                sbSmallSpirit.SetActive(false);
                sbTranscendent.SetActive(true);
            }
            //MagnaGarurumon's small spirits - total animation duration: 3.1 s
            sbTranscendent.SetSprite(sMagnaGarurumon);
            for (int i = 0; i < 4; i++) {
                yield return new WaitForSeconds(0.15f);
                sbTranscendent.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbTranscendent.SetActive(true);
            }
            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(0.1f);
                sbTranscendent.SetActive(false);
                sbSmallSpirit.SetSprite(sHumans[5 + i]);
                if (i == 0) sbSmallSpirit.SetPosition(9, 0);
                else if (i == 1) sbSmallSpirit.SetPosition(0, 7);
                else if (i == 2) sbSmallSpirit.SetPosition(18, 7);
                else if (i == 3) sbSmallSpirit.SetPosition(2, 16);
                else if (i == 4) sbSmallSpirit.SetPosition(16, 16);
                sbSmallSpirit.SetActive(true);

                yield return new WaitForSeconds(0.28f);
                sbSmallSpirit.SetActive(false);
                sbTranscendent.SetActive(true);
            }
            sbTranscendent.SetActive(false);
            //Quick player down-to-up
            for (int i = 0; i < 12; i++) {
                sbCharacter.Move(Direction.Up, 6);
                yield return new WaitForSeconds(1f / 12);
            }
            //Show all small spirits
            SpriteBuilder sbSmallHuman = ScreenElement.BuildSprite("Human", AnimParent).SetSize(14, 16);
            SpriteBuilder sbSmallAnimal = ScreenElement.BuildSprite("Animal", AnimParent).SetSize(14, 16);
            for (int i = 0; i < 10; i++) {
                sbSmallHuman.SetY(16).PlaceOutside(Direction.Left).Move(Direction.Left);
                sbSmallAnimal.SetY(16).PlaceOutside(Direction.Right).Move(Direction.Right);
                sbSmallHuman.SetSprite(sHumans[i]);
                sbSmallAnimal.SetSprite(sAnimals[i]);
                for (int j = 0; j < 4; j++) {
                    sbSmallHuman.Move(Direction.Right, 4);
                    sbSmallAnimal.Move(Direction.Left, 4);
                    yield return new WaitForSeconds(0.6f / 10);
                }
                for (int j = 0; j < 6; j++) {
                    sbSmallHuman.Move(Direction.Up, 4);
                    sbSmallAnimal.Move(Direction.Up, 4);
                    yield return new WaitForSeconds(0.6f / 10);
                }
            }
            sbSmallHuman.Dispose();
            sbSmallAnimal.Dispose();
            //Form Susanoomon
            sbTranscendent.SetSprite(sSusanoomon[0]).SetActive(true);
            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("Curtain", AnimParent).SetSprite(sCurtainSpecial);
            for (int i = 0; i < 32; i++) {
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(2.2f / 32);
            }
            sbCurtain.PlaceOutside(Direction.Down);

            sbTranscendent.SetSprite(sSusanoomon[1]);
            yield return new WaitForSeconds(0.8f);
            sbTranscendent.SetSprite(sSusanoomon[0]);
            yield return new WaitForSeconds(0.6f);
        }
        public static IEnumerator AncientEvolution(GameChar character, string digimon) {
            Sprite sAncient = spriteDB.GetDigimonSprite(digimon);
            Sprite sAncientAt = spriteDB.GetDigimonSprite(digimon, SpriteAction.Attack);
            Sprite sHumanSpirit = null, sAnimalSpirit = null;
            switch (digimon) {
                case "ancientgreymon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("agunimon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("burninggreymon", SpriteAction.Spirit);
                    break;
                case "ancientgarurumon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("lobomon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("kendogarurumon", SpriteAction.Spirit);
                    break;
                case "ancientbeetlemon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("beetlemon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("metalkabuterimon", SpriteAction.Spirit);
                    break;
                case "ancientirismon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("kazemon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("zephyrmon", SpriteAction.Spirit);
                    break;
                case "ancientmegatheriummon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("kumamon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("korikakumon", SpriteAction.Spirit);
                    break;
                case "ancientsphinxmon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("loweemon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("kaiserleomon", SpriteAction.Spirit);
                    break;
                case "ancientvolcamon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("grumblemon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("gigasmon", SpriteAction.Spirit);
                    break;
                case "ancienttrojamon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("arbormon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("petaldramon", SpriteAction.Spirit);
                    break;
                case "ancientwisemon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("mercurymon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("sephirothmon", SpriteAction.Spirit);
                    break;
                case "ancientmermaimon":
                    sHumanSpirit = spriteDB.GetDigimonSprite("lanamon", SpriteAction.Spirit);
                    sAnimalSpirit = spriteDB.GetDigimonSprite("calmaramon", SpriteAction.Spirit);
                    break;
            }
            Sprite sGiveMassivePowerBlack = spriteDB.giveMassivePowerInverted;
            Sprite[] sSpiral = spriteDB.ancientSpiral;
            Sprite[] sCircle = spriteDB.ancientCircle;
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);

            SpriteBuilder sbSpiral = ScreenElement.BuildSprite("Spiral", AnimParent);
            SpriteBuilder sbCircle = ScreenElement.BuildSprite("Circle", AnimParent).SetTransparent(true);
            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Digimon", AnimParent).SetSize(24, 24).Center().PlaceOutside(Direction.Down);
            SpriteBuilder sbGiveMassivePower = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sGiveMassivePowerBlack).SetTransparent(true);
            sbGiveMassivePower.SetActive(false);

            audioMgr.PlaySound(audioMgr.evolutionAncient);

            //Show human spirit
            sbDigimon.SetSprite(sHumanSpirit);
            for (int i = 0; i < 28; i++) {
                sbDigimon.Move(Direction.Up);
                yield return new WaitForSeconds(0.95f / 28);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++) {
                sbGiveMassivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGiveMassivePower.SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            for (int i = 0; i < 28; i++) {
                sbDigimon.Move(Direction.Up);
                yield return new WaitForSeconds(0.95f / 28);
            }
            //Show animal spirit
            sbDigimon.SetSprite(sAnimalSpirit);
            sbDigimon.PlaceOutside(Direction.Down);
            for (int i = 0; i < 28; i++) {
                sbDigimon.Move(Direction.Up);
                yield return new WaitForSeconds(0.95f / 28);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++) {
                sbGiveMassivePower.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbGiveMassivePower.SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            for (int i = 0; i < 28; i++) {
                sbDigimon.Move(Direction.Up);
                yield return new WaitForSeconds(0.95f / 28);
            }
            //Spiral and circle
            sbGiveMassivePower.SetSprite(sCharacter[0]).SetTransparent(false);
            for (int i = 0; i < 2; i++) {
                sbGiveMassivePower.SetActive(false);
                sbCircle.SetSprite(sCircle[0]);
                for (int j = 0; j < 3; j++) {
                    sbSpiral.SetSprite(sSpiral[0]);
                    yield return new WaitForSeconds(0.3f);
                    sbSpiral.SetSprite(sSpiral[1]);
                    yield return new WaitForSeconds(0.3f);
                }

                sbGiveMassivePower.SetActive(true);
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(0.15f);
            sbGiveMassivePower.SetSprite(sCharacter[9]);
            yield return new WaitForSeconds(0.4f);
            sbGiveMassivePower.Dispose();

            for (int j = 0; j < 3; j++) {
                if (j == 1) sbCircle.SetSprite(sCircle[1]);
                if (j == 2) sbCircle.SetSprite(sCircle[2]);
                sbSpiral.SetSprite(sSpiral[0]);
                yield return new WaitForSeconds(0.3f);
                sbSpiral.SetSprite(sSpiral[1]);
                yield return new WaitForSeconds(0.3f);
            }
            sbSpiral.Dispose();
            sbCircle.SetSprite(sBlackScreen);
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.2f);
                sbCircle.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                sbCircle.SetActive(true);
            }
            yield return new WaitForSeconds(0.2f);
            sbCircle.SetActive(false);
            sbDigimon.SetSprite(sAncient).Center();
            for (int i = 0; i < 2; i++) {
                sbDigimon.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbDigimon.SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            sbDigimon.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            sbDigimon.SetSprite(sAncientAt);
            yield return new WaitForSeconds(0.55f);
            sbDigimon.SetSprite(sAncient);
            yield return new WaitForSeconds(0.7f);
        }
        public static IEnumerator BoostFailed(string sacrifice) {
            Sprite sGivePower = spriteDB.givePower;
            Sprite sGivePowerBlack = spriteDB.givePowerInverted;
            Sprite sSacrifice = spriteDB.GetDigimonSprite(sacrifice);

            audioMgr.PlaySound(audioMgr.digiPowerFailed);

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("GivePower", AnimParent).SetSize(24, 24).SetSprite(sSacrifice).Center();
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sGivePowerBlack);

            for (int i = 0; i < 2; i++) {
                sbGivePower.SetSprite(sGivePowerBlack).SetActive(true);
                yield return new WaitForSeconds(0.2f);
                sbGivePower.SetSprite(sGivePower);
                yield return new WaitForSeconds(0.2f);
                sbGivePower.SetActive(false);
                yield return new WaitForSeconds(0.45f);
            }

            yield return new WaitForSeconds(0.4f);
            sbDigimon.SetActive(false);

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.60f);
                sbDigimon.SetActive(true);
                yield return new WaitForSeconds(0.45f);
                sbDigimon.SetActive(false);
            }

            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(0.30f);
                sbDigimon.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbDigimon.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);
        }
        public static IEnumerator BoostSucceed(string digimon, string sacrifice) {
            Sprite sGivePower = spriteDB.givePower;
            Sprite sGivePowerBlack = spriteDB.givePowerInverted;
            Sprite sGiveMassivePowerBlack = spriteDB.giveMassivePowerInverted;
            Sprite sCurtain = spriteDB.curtain;
            Sprite sSacrifice = spriteDB.GetDigimonSprite(sacrifice);
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);

            audioMgr.PlaySound(audioMgr.digiPowerSucceed);

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("GivePower", AnimParent).SetSize(24, 24).SetSprite(sSacrifice).Center();
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sGivePowerBlack);

            for (int i = 0; i < 2; i++) {
                sbGivePower.SetSprite(sGivePowerBlack).SetActive(true);
                yield return new WaitForSeconds(0.2f);
                sbGivePower.SetSprite(sGivePower);
                yield return new WaitForSeconds(0.2f);
                sbGivePower.SetActive(false);
                yield return new WaitForSeconds(0.45f);
            }

            yield return new WaitForSeconds(0.4f);

            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("GivePower", AnimParent).SetSprite(sCurtain).PlaceOutside(Direction.Up);

            for (int i = 0; i < 64; i++) {
                if (i == 32) sbDigimon.SetActive(false);
                sbCurtain.Move(Direction.Down);
                yield return new WaitForSeconds(3.5f / 64);
            }

            sbDigimon.SetSprite(sDigimon).SetActive(true);
            yield return new WaitForSeconds(0.5f);

            sbCurtain.SetTransparent(true);

            for (int i = 0; i < 64; i++) {
                if (i == 32) sbDigimon.SetActive(true);
                sbCurtain.Move(Direction.Up);
                yield return new WaitForSeconds(3.5f / 64);
            }
            yield return new WaitForSeconds(0.4f);
            sbCurtain.SetSprite(sGiveMassivePowerBlack).SetPosition(0, 0);

            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.1f);
                sbCurtain.SetActive(false);
                yield return new WaitForSeconds(0.4f);
                sbCurtain.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
            sbCurtain.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
        private static IEnumerator AnimateSPScreen(SpriteBuilder background) {
            Sprite sSpiritsA = spriteDB.battle_gainingSP[0];
            Sprite sSpiritsB = spriteDB.battle_gainingSP[1];
            while (background != null) {
                background.SetSprite(sSpiritsA);
                yield return new WaitForSeconds(0.2f);
                background.SetSprite(sSpiritsB);
                yield return new WaitForSeconds(0.2f);
            }
        }
        public static IEnumerator LaunchAttack(Sprite[] digimonSprites, int attack, bool isEnemy, bool disobeyed) {
            Direction launchDir = isEnemy ? Direction.Right : Direction.Left;
            SpriteBuilder sbAttack = ScreenElement.BuildSprite("Attack", AnimParent).SetSize(24, 24).Center();
            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Attacker", AnimParent).SetSize(24, 24).Center().SetSprite(digimonSprites[0]);
            sbAttack.SetComponentSize(24, 24);

            sbAttack.SnapComponentToSide(launchDir, true);
            sbDigimon.FlipHorizontal(isEnemy);
            sbAttack.FlipHorizontal(isEnemy);

            int extraPixels = 0;

            if (attack != 3) {
                //Show exclamation mark.
                if (disobeyed) {
                    yield return new WaitForSeconds(0.1f);
                    SpriteBuilder sbDisobey = ScreenElement.BuildSprite("Disobey", AnimParent).SetSize(3, 9).SetPosition(1, 1).SetSprite(spriteDB.battle_disobey);
                    yield return new WaitForSeconds(0.3f);
                    sbDisobey.Dispose();
                }
                yield return new WaitForSeconds(0.2f);
                sbDigimon.Move(launchDir.Opposite(), 3);
                sbAttack.Move(launchDir.Opposite(), 3);
            }
            if (attack == 0 || attack == 2) {
                sbDigimon.SetSprite(digimonSprites[1]);

                sbAttack.SetSprite((attack == 0) ? digimonSprites[3] : digimonSprites[4]);

                if(attack == 2 && digimonSprites[4].texture.width > 32) {
                    sbAttack.SetSize(digimonSprites[4].texture.width, 24).Move(launchDir.Opposite(), sbAttack.Width - 24).CenterComponent();
                    sbDigimon.Move(launchDir.Opposite());
                    extraPixels = sbAttack.Width - 24;
                    audioMgr.PlaySound(audioMgr.launchAttackLong);
                }
                else {
                    audioMgr.PlaySound(audioMgr.launchAttack);
                }

                for (int i = 0; i < 38; i++) {
                    yield return new WaitForSeconds(1.7f / 32f);
                    sbAttack.Move(launchDir);
                }
                for (int i = 0; i < extraPixels; i++) {
                    yield return new WaitForSeconds(1.7f / 32f);
                    sbAttack.Move(launchDir);
                }
                yield return new WaitForSeconds(0.3f);
            }
            else if (attack == 1) {
                sbDigimon.SetSprite(digimonSprites[2]);

                audioMgr.PlaySound(audioMgr.launchAttack);
                for (int i = 0; i < 7; i++) {
                    ScreenElement.BuildSprite($"Crush{i}", AnimParent).SetSize(24, 24).Center().SetSprite(digimonSprites[2]).FlipHorizontal(isEnemy).Move(launchDir, 4 * i);
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
            ClearAnimParent();
            //yield return new WaitForSeconds(0.2f);
        }
        public static IEnumerator AttackCollision(int friendlyAttack, Sprite[] friendlySprites, int enemyAttack, Sprite[] enemySprites, int winner) {

            SpriteBuilder sbFriendlyAttack = ScreenElement.BuildSprite("FriendlyAttack", AnimParent).SetSize(24, 24);
            sbFriendlyAttack.Center().PlaceOutside(Direction.Right);
            SpriteBuilder sbEnemyAttack = ScreenElement.BuildSprite("EnemyAttack", AnimParent).SetSize(24, 24);
            sbEnemyAttack.Center().PlaceOutside(Direction.Left);
            sbEnemyAttack.FlipHorizontal(true);

            int[] extraPixels = new int[2];

            if (winner == 0) sbFriendlyAttack.transform.SetAsLastSibling(); //Place the friendly attack above if he won.

            //Set attack sprite.
            if (friendlyAttack == 0) sbFriendlyAttack.SetSprite(friendlySprites[3]);
            else if (friendlyAttack == 1) sbFriendlyAttack.SetSprite(friendlySprites[2]);
            else if (friendlyAttack == 2) {
                sbFriendlyAttack.SetSprite(friendlySprites[4]);
                if(friendlySprites[4].texture.width > 32) {
                    extraPixels[0] = friendlySprites[4].texture.width - 24;
                    sbFriendlyAttack.SetSize(friendlySprites[4].texture.width, 24).Move(Direction.Right, extraPixels[0]).CenterComponent().PlaceOutside(Direction.Right);
                }
            }
            else if (friendlyAttack == 3) sbFriendlyAttack.SetActive(false);
            if (enemyAttack == 0) sbEnemyAttack.SetSprite(enemySprites[3]);
            else if (enemyAttack == 1) sbEnemyAttack.SetSprite(enemySprites[2]);
            else if (enemyAttack == 2) {
                sbEnemyAttack.SetSprite(enemySprites[4]);
                if (enemySprites[4].texture.width > 32) {
                    extraPixels[1] = enemySprites[4].texture.width - 24;
                    sbEnemyAttack.SetSize(enemySprites[4].texture.width, 24).Move(Direction.Left, extraPixels[1]).CenterComponent().PlaceOutside(Direction.Left);
                }
            }
            else if (enemyAttack == 3) sbEnemyAttack.SetActive(false);

            audioMgr.PlaySound(audioMgr.attackTravelVeryLong);
            //Attacks approach each other.
            for (int i = 0; i < 16; i++) {
                sbFriendlyAttack.Move(Direction.Left);
                sbEnemyAttack.Move(Direction.Right);
                yield return new WaitForSeconds(0.6f / 16f);
            }
            //Attacks collide. The pattern of this script is: tie, player win, player lose.
            if (friendlyAttack == 0) {
                if (enemyAttack == 0) yield return _EnergyOrAbilityCollides();
                else if (enemyAttack == 1) yield return _EnergyVsCrush();
                else if (enemyAttack == 2) yield return _EnergyVsAbility();
                else if (enemyAttack == 3) yield return _CrushVsAbility();
            }
            else if (friendlyAttack == 1) {
                if (enemyAttack == 0) yield return _EnergyVsCrush();
                else if (enemyAttack == 1) yield return _CrushCollides();
                else if (enemyAttack == 2) yield return _CrushVsAbility();
                else if (enemyAttack == 3) yield return _CrushVsAbility();
            }
            else if (friendlyAttack == 2) {
                if (enemyAttack == 0) yield return _EnergyVsAbility();
                else if (enemyAttack == 1) yield return _CrushVsAbility();
                else if (enemyAttack == 2) yield return _EnergyOrAbilityCollides();
                else if (enemyAttack == 3) yield return _CrushVsAbility();
            }
            else if (friendlyAttack == 3) {
                yield return _CrushVsAbility(); //Reuse this animation because it's identical.
            }

            yield return new WaitForSeconds(0.15f);

            ClearAnimParent();

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
                        sbFriendlyAttack.Move(Direction.Left);
                        yield return new WaitForSeconds(0.6f / 16f);
                    }
                    for (int i = 0; i < extraPixels[0]; i++) {
                        sbFriendlyAttack.Move(Direction.Left);
                        yield return new WaitForSeconds(0.6f / 16f);
                    }
                }
                else if (winner == 1) {
                    _TransformAttackIntoCollision(sbFriendlyAttack);
                    for (int i = 0; i < 40; i++) {
                        if (i == 3) sbFriendlyAttack.Dispose();
                        sbEnemyAttack.Move(Direction.Right);
                        yield return new WaitForSeconds(0.6f / 16f);
                    }
                    for (int i = 0; i < extraPixels[1]; i++) {
                        sbEnemyAttack.Move(Direction.Right);
                        yield return new WaitForSeconds(0.6f / 16f);
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
                    winnerSprite.Move(winnerDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
            }
            IEnumerator _EnergyVsAbility() {
                //The chunks of the ability.
                SpriteBuilder winnerSprite = (winner == 0) ? sbFriendlyAttack : sbEnemyAttack;
                SpriteBuilder loserSprite = (winner == 0) ? sbEnemyAttack : sbFriendlyAttack;
                winnerSprite.SetTransparent(true); //The energy panel is transparent.

                int brokenAbilityX = loserSprite.Position.x;
                Sprite brokenAbilitySprite = loserSprite.Sprite;
                Direction winnerDirection = (winner == 0) ? Direction.Left : Direction.Right;

                ContainerBuilder cbBrokenAbilityUp = ScreenElement.BuildContainer("AbilityUp", AnimParent)
                    .SetSize(24 + extraPixels[winner == 0? 1 : 0], 12).SetMaskActive(true);
                cbBrokenAbilityUp.SetPosition(brokenAbilityX, 4);
                ScreenElement.BuildSprite("AbilityUpSprite", cbBrokenAbilityUp.transform)
                    .SetSize(24 + extraPixels[winner == 0 ? 1 : 0], 24).CenterComponent()
                    .SetPosition(0, 0).SetSprite(brokenAbilitySprite).FlipHorizontal(winner == 0);

                ContainerBuilder cbBrokenAbilityDown = ScreenElement.BuildContainer("AbilityDown", AnimParent)
                    .SetSize(24 + extraPixels[winner == 0? 1 : 0], 12).SetMaskActive(true);
                cbBrokenAbilityDown.SetPosition(brokenAbilityX, 16);
                ScreenElement.BuildSprite("AbilityDownSprite", cbBrokenAbilityDown.transform)
                    .SetSize(24 + extraPixels[winner == 0 ? 1 : 0], 24).CenterComponent()
                    .SetPosition(0, -12).SetSprite(brokenAbilitySprite).FlipHorizontal(winner == 0);

                loserSprite.Dispose();
                winnerSprite.transform.SetAsLastSibling(); //Place the winning attack above everything else.

                for (int i = 0; i < 32; i++) { //Enemy crush - player loses
                    cbBrokenAbilityUp.Move(winnerDirection.Opposite()).Move(Direction.Up);
                    cbBrokenAbilityDown.Move(winnerDirection.Opposite()).Move(Direction.Down);
                    winnerSprite.Move(winnerDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
                for (int i = 0; i < extraPixels[winner]; i++) {
                    winnerSprite.Move(winnerDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
            }
            IEnumerator _CrushCollides() {
                if (winner == 2) {
                    for (int i = 0; i < 40; i++) {
                        sbFriendlyAttack.Move(Direction.Right);
                        sbEnemyAttack.Move(Direction.Left);
                        yield return new WaitForSeconds(0.6f / 16f);
                    }
                    audioMgr.StopSound();
                }
                else {
                    Direction pushDirection = (winner == 0) ? Direction.Left : Direction.Right;
                    for (int i = 0; i < 40; i++) {
                        sbFriendlyAttack.Move(pushDirection);
                        sbEnemyAttack.Move(pushDirection);
                        yield return new WaitForSeconds(0.6f / 16f);
                    }
                }
            }
            IEnumerator _CrushVsAbility() {
                SpriteBuilder winnerSprite = (winner == 0) ? sbFriendlyAttack : sbEnemyAttack;
                SpriteBuilder loserSprite = (winner == 0) ? sbEnemyAttack : sbFriendlyAttack;
                Direction winnerDirection = (winner == 0) ? Direction.Left : Direction.Right;
                for (int i = 0; i < 40; i++) {
                    if (i == 16) loserSprite.Dispose(); //Remove the crush at the exact frame the ability is completely covering it.
                    winnerSprite.Move(winnerDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
                for (int i = 0; i < extraPixels[winner]; i++) {
                    winnerSprite.Move(winnerDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
            }
        }
        //Note, if winningAbility is null, the animation assumes the caller does not want the ability win animation to play.
        public static IEnumerator DestroyLoser(Sprite[] loserSprites, int winningAttack, Sprite winningAbility, bool isEnemy, int loserHPbefore, int loserHPnow) {
            Sprite sLoser = loserSprites[0];
            Sprite sLoserCr = loserSprites[2];
            Sprite explosionBig = spriteDB.battle_explosion[0];
            Sprite explosionSmall = spriteDB.battle_explosion[1];
            SpriteBuilder sbLoser = ScreenElement.BuildSprite("Loser", AnimParent).SetSize(24, 24).Center();

            sbLoser.FlipHorizontal(isEnemy); //Flip the sprite if the loser is the enemy.
            Direction winningDirection = isEnemy ? Direction.Left : Direction.Right;

            if (winningAttack == 0) {
                sbLoser.SetSprite(sLoser);
                yield return new WaitForSeconds(0.5f);
                yield return _ExplodeLoser();
            }
            else if (winningAttack == 1) {
                sbLoser.SetSprite(sLoserCr);
                sbLoser.PlaceOutside(winningDirection.Opposite());
                for (int i = 0; i < 64; i++) {
                    sbLoser.Move(winningDirection);
                    yield return new WaitForSeconds(0.6f / 16f);
                }
                yield return _ExplodeLoser();
            }
            else if (winningAttack == 2 && winningAbility != null) {
                int extraPixels = 0;

                sbLoser.SetSprite(sLoser);
                SpriteBuilder sbAbility = ScreenElement.BuildSprite("Loser", AnimParent).SetSize(24, 24).SetSprite(winningAbility).Center();
                if (winningAbility.texture.width > 32) {
                    sbAbility.SetSize(winningAbility.texture.width, 24).CenterComponent();
                    extraPixels = sbAbility.Width - 24;
                }
                sbAbility.PlaceOutside(winningDirection.Opposite());
                sbAbility.FlipHorizontal(!isEnemy); //Flip the ability if the loser is the ally.
                for (int i = 0; i < 64; i++) {
                    if (i == 28) sbLoser.SetActive(false);
                    sbAbility.Move(winningDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
                for (int i = 0; i < extraPixels; i++) {
                    if (i == 28) sbLoser.SetActive(false);
                    sbAbility.Move(winningDirection);
                    yield return new WaitForSeconds(Constants.ATTACK_TRAVEL_SPEED);
                }
                audioMgr.StopSound();
            }
            else {
                audioMgr.StopSound();
            }
            //No animation is done when a digimon loses to an ability.

            sbLoser.SetActive(true).SetSprite(sLoser);

            yield return new WaitForSeconds(0.5f);
            ContainerBuilder cbLifeSign = ScreenElement.BuildStatSign("LIFE", AnimParent);

            yield return new WaitForSeconds(0.5f);
            audioMgr.PlayButtonA();
            ((TextBoxBuilder)cbLifeSign.GetChildBuilder(1)).Text = loserHPbefore.ToString();
            yield return new WaitForSeconds(0.75f);
            audioMgr.PlayButtonA();
            ((TextBoxBuilder)cbLifeSign.GetChildBuilder(1)).Text = loserHPnow.ToString();
            yield return new WaitForSeconds(0.75f);

            ClearAnimParent();

            IEnumerator _ExplodeLoser() {
                audioMgr.PlaySound(audioMgr.explosion);
                sbLoser.FlipHorizontal(false);
                sbLoser.Center();
                for (int i = 0; i < 2; i++) {
                    sbLoser.SetSprite(explosionBig);
                    yield return new WaitForSeconds(0.5f);
                    sbLoser.SetSprite(explosionSmall);
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.25f);
                sbLoser.FlipHorizontal(isEnemy);
            }
        }

        public static IEnumerator DestroyBox() {
            Sprite sBoxSprite = spriteDB.GetDigimonSprite("jackpot");
            Sprite sBoxSpriteBroken = spriteDB.GetDigimonSprite("jackpot", SpriteAction.Spirit);
            Sprite explosionBig = spriteDB.battle_explosion[0];
            Sprite explosionSmall = spriteDB.battle_explosion[1];

            SpriteBuilder sbBox = ScreenElement.BuildSprite("Loser", AnimParent).SetSize(24, 24).Center().FlipHorizontal(true).SetSprite(sBoxSprite);
            audioMgr.StopSound();
            yield return new WaitForSeconds(0.5f);
            audioMgr.PlaySound(audioMgr.explosion);
            sbBox.FlipHorizontal(false);
            sbBox.Center();
            for (int i = 0; i < 2; i++) {
                sbBox.SetSprite(explosionBig);
                yield return new WaitForSeconds(0.5f);
                sbBox.SetSprite(explosionSmall);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.15f);
            sbBox.FlipHorizontal(true).SetSprite(sBoxSpriteBroken);
            yield return new WaitForSeconds(0.85f);
        }
        public static IEnumerator BoxResists(string friendlyDigimon) {
            Sprite sBoxSprite = spriteDB.GetDigimonSprite("jackpot");
            Sprite sDigimonCr = spriteDB.GetDigimonSprite(friendlyDigimon, SpriteAction.Crush);

            Sprite sGivePower = spriteDB.giveMassivePowerInverted;
            SpriteBuilder sbBox = ScreenElement.BuildSprite("Loser", AnimParent).SetSize(24, 24).Center().SetSprite(sBoxSprite).FlipHorizontal(true);
            SpriteBuilder sbGivePower = ScreenElement.BuildSprite("Loser", AnimParent).SetSprite(sGivePower).SetTransparent(true).SetActive(false);

            audioMgr.StopSound();
            yield return new WaitForSeconds(0.5f);
            sbBox.Center();
            for (int i = 0; i < 2; i++) {
                sbGivePower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbGivePower.SetActive(false);
                yield return new WaitForSeconds(0.35f);
            }
            yield return new WaitForSeconds(0.25f);
            audioMgr.PlaySound(audioMgr.attackTravelVeryLong);
            sbBox.FlipHorizontal(false).SetSprite(sDigimonCr).PlaceOutside(Direction.Left);
            for (int i = 0; i < 64; i++) {
                sbBox.Move(Direction.Right);
                yield return new WaitForSeconds(0.6f / 16f);
            }
            audioMgr.StopSound();
        }

        //Rewards:
        public static IEnumerator RewardEmpty() {
            Sprite sEmpty = spriteDB.status_ddockEmpty;

            SpriteBuilder sbEmpty = ScreenElement.BuildSprite("Empty, sorry :(", AnimParent).SetSize(24, 24).Center().SetSprite(sEmpty);
            yield return new WaitForSeconds(2f);
        }
        public static IEnumerator RewardDistance(bool isPunishment, int distanceBefore, int distanceAfter) {
            Sprite[] sRewardBg = spriteDB.rewardBackground;
            Sprite sDistanceUpIcon = isPunishment ? spriteDB.rewards[2] : spriteDB.rewards[1];
            SpriteBuilder sbRewardBg = ScreenElement.BuildSprite("DistanceBackground", AnimParent).SetSprite(sRewardBg[0]);
            SpriteBuilder sbDistanceUp = ScreenElement.BuildSprite("DistanceIcon", AnimParent)
                .SetSize(16, 16).SetSprite(sDistanceUpIcon).Center().SetActive(false).SetTransparent(false);

            if (isPunishment) {
                audioMgr.PlaySound(audioMgr.punishment);
            }
            else {
                audioMgr.PlaySound(audioMgr.reward);
            }

            for (int i = 0; i < 2; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbRewardBg.SetSprite(sRewardBg[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            for (int cycle = 0; cycle < 4; cycle++) {
                sbDistanceUp.SetActive(cycle % 2 == 0);
                sbRewardBg.SetSprite(sRewardBg[cycle]);
                yield return new WaitForSeconds(0.125f);
            }
            sbDistanceUp.SetActive(true);
            for (int i = 0; i < 6; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbRewardBg.SetSprite(sRewardBg[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            sbRewardBg.SetActive(false);
            for (int i = 0; i < 9; i++) {
                sbDistanceUp.Move(Direction.Up);
                yield return new WaitForSeconds(0.5f / 9);
            }

            ScreenElement.BuildTextBox("Distance", AnimParent, DFont.Small)
                .SetText("DISTANCE").SetSize(32, 5).SetPosition(0, 17);
            TextBoxBuilder tbLevel = ScreenElement.BuildTextBox("DistanceNumber", AnimParent, DFont.Regular)
                .SetText(distanceBefore.ToString()).SetSize(29, 5).SetPosition(2, 24).SetAlignment(TextAnchor.UpperRight);

            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
            tbLevel.Text = distanceAfter.ToString();
            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator RewardSpiritPower(bool isPunishment, int distanceBefore, int distanceAfter) {
            Sprite[] sRewardBg = spriteDB.rewardBackground;
            Sprite sSpiritPowerIcon = spriteDB.rewards[3];
            SpriteBuilder sbRewardBg = ScreenElement.BuildSprite("LevelUpBackground", AnimParent).SetSprite(sRewardBg[0]);
            SpriteBuilder sbSpiritPower = ScreenElement.BuildSprite("LevelUpIcon", AnimParent)
                .SetSize(16, 16).SetSprite(sSpiritPowerIcon).Center().SetActive(false).SetTransparent(false);

            if (isPunishment) {
                audioMgr.PlaySound(audioMgr.punishment);
            }
            else {
                audioMgr.PlaySound(audioMgr.reward);
            }

            for (int i = 0; i < 2; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbRewardBg.SetSprite(sRewardBg[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            for (int cycle = 0; cycle < 4; cycle++) {
                sbSpiritPower.SetActive(cycle % 2 == 0);
                sbRewardBg.SetSprite(sRewardBg[cycle]);
                yield return new WaitForSeconds(0.125f);
            }
            sbSpiritPower.SetActive(true);
            for (int i = 0; i < 6; i++) {
                for (int cycle = 0; cycle < 4; cycle++) {
                    sbRewardBg.SetSprite(sRewardBg[cycle]);
                    yield return new WaitForSeconds(0.125f);
                }
            }
            sbRewardBg.SetActive(false);
            for (int i = 0; i < 9; i++) {
                sbSpiritPower.Move(Direction.Up);
                yield return new WaitForSeconds(0.5f / 9);
            }

            ScreenElement.BuildTextBox("SpiritPower", AnimParent, DFont.Small)
                .SetText("SPIRITS").SetSize(32, 5).SetPosition(1, 17);
            TextBoxBuilder tbLevel = ScreenElement.BuildTextBox("SpiritPowerNumber", AnimParent, DFont.Regular)
                .SetText(distanceBefore.ToString()).SetSize(29, 5).SetPosition(2, 24).SetAlignment(TextAnchor.UpperRight);

            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
            tbLevel.Text = distanceAfter.ToString();
            audioMgr.PlayButtonA();
            yield return new WaitForSeconds(1f);
        }

        public static IEnumerator RewardCode(string digimon, string code) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sBubble = spriteDB.bubble;
            Sprite sDTector = spriteDB.dTector;
            Sprite sPowerBlack = spriteDB.giveMassivePowerInverted;

            SpriteBuilder sbDigimon = ScreenElement.BuildSprite("Digimon", AnimParent).SetSize(24, 24)
                .Center().PlaceOutside(Direction.Up).Move(Direction.Up, 4).SetSprite(sDigimon);
            SpriteBuilder sbBubble = ScreenElement.BuildSprite("Bubble", AnimParent)
                .PlaceOutside(Direction.Up).SetSprite(sBubble);

            audioMgr.PlaySound(audioMgr.unlockCode);

            for (int i = 0; i < 64; i++) {
                sbBubble.SetActive(i % 2 == 1);
                if (i % 2 == 0) {
                    sbDigimon.Move(Direction.Down);
                    sbBubble.Move(Direction.Down);
                }
                yield return new WaitForSeconds(4f / 64);
            }
            sbBubble.SetActive(false).SetTransparent(true);
            yield return new WaitForSeconds(0.15f);
            sbBubble.SetActive(true);
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < 3; i++) {
                sbBubble.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                sbBubble.SetActive(true);
                yield return new WaitForSeconds(0.4f);
            }
            for (int i = 0; i < 3; i++) {
                sbBubble.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                sbBubble.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }
            sbBubble.SetActive(false);
            yield return new WaitForSeconds(1f);

            sbDigimon.Dispose();

            audioMgr.PlayButtonA();

            sbBubble.SetTransparent(false).SetSprite(gm.spriteDB.database_pages[2]).SetActive(true);
            TextBoxBuilder tbCode = ScreenElement.BuildTextBox("Code", AnimParent, DFont.Big)
                .SetText(code).SetSize(30, 8).SetPosition(2, 23).SetAlignment(TextAnchor.UpperRight);
            yield return new WaitForSeconds(3f);
            tbCode.Dispose();

            sbBubble.SetSprite(sDTector);
            SpriteBuilder sbPower = ScreenElement.BuildSprite("Power", AnimParent).SetSprite(sPowerBlack).SetTransparent(true);
            sbPower.SetActive(false);

            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < 5; i++) {
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
                yield return new WaitForSeconds(0.15f);
            }
        }

        public static IEnumerator DataStorm(Sprite[] character, bool moveToNewArea) {
            Sprite[] sDigistorm = spriteDB.digistorm;
            Sprite sExclamation = spriteDB.battle_disobey;

            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(character[2]).SetActive(false);
            SpriteBuilder[] sbDigistorm = new SpriteBuilder[2];
            sbDigistorm[0] = ScreenElement.BuildSprite("Digistorm", AnimParent).SetSize(40, 32).SetSprite(sDigistorm[0]).PlaceOutside(Direction.Right);
            sbDigistorm[1] = ScreenElement.BuildSprite("Digistorm", AnimParent).SetSize(40, 32).SetSprite(sDigistorm[0]).PlaceOutside(Direction.Right).Move(Direction.Right, 40);

            audioMgr.PlaySound(audioMgr.digistorm);

            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 40; j++) {
                    sbDigistorm.Move(Direction.Left);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(j / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(j / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
            }

            sbDigistorm[0].PlaceOutside(Direction.Right);
            sbDigistorm[1].PlaceOutside(Direction.Right).Move(Direction.Right, 40);

            SpriteBuilder sbExclamation = ScreenElement.BuildSprite("Exclamation", AnimParent).SetSize(3, 9).SetPosition(26, 1).SetSprite(sExclamation);
            sbCharacter.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            sbExclamation.Dispose();
            sbCharacter.SetSprite(character[0]);
            for (int i = 0; i < 4; i++) {
                sbCharacter.SetSprite(character[4 + (i % 2)]);
                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(0.1f);
            }

            for (int i = 0; i < 20; i++) {
                sbCharacter.SetSprite(character[4 + (i % 2)]);
                sbDigistorm.Move(Direction.Left);
                sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                yield return new WaitForSeconds(4f / 40);
            }

            int timeTryingToEscape = Random.Range(0, 40); //Default was 20.

            if (moveToNewArea) {
                for (int i = 0; i < timeTryingToEscape; i++) {
                    sbCharacter.SetSprite(character[4 + (i % 2)]);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
                for (int i = 0; i < 20; i++) {
                    sbCharacter.SetSprite(character[4 + (i % 2)]);
                    sbDigistorm.Move(Direction.Left);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
                for (int i = 0; i < 40; i++) {
                    sbDigistorm.Move(Direction.Left);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
            }
            else {
                for (int i = 0; i < timeTryingToEscape; i++) {
                    sbCharacter.SetSprite(character[4 + (i % 2)]);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
                for (int i = 0; i < 30; i++) {
                    sbCharacter.SetSprite(character[4 + (i % 2)]);
                    sbCharacter.Move(Direction.Left);
                    sbDigistorm.Move(Direction.Right);
                    sbDigistorm[0].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    sbDigistorm[1].SetSprite(sDigistorm[Mathf.FloorToInt(i / 2) % 2]);
                    yield return new WaitForSeconds(4f / 40);
                }
                yield return new WaitForSeconds(0.75f);
            }
            audioMgr.StopSound();
        }

        public static IEnumerator OpenCamp(Sprite[] character) {
            //TOTAL DURATION: 7.2s.
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).Center().SetSprite(character[0]);
            yield return new WaitForSeconds(0.2f);
            for(int i = 0; i < 32; i++) {
                sbCharacter.SetSprite(character[4 + (i % 2)]);
                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(3f / 32);
            }
            sbCharacter.SetSize(24, 24).SetY(4).Move(Direction.Right, 4).SetSprite(spriteDB.camp[0]);
            for (int i = 0; i < 32; i++) {
                sbCharacter.Move(Direction.Right);
                yield return new WaitForSeconds(3f / 32);
            }
            yield return new WaitForSeconds(1f);
        }

        public static IEnumerator CloseCamp(Sprite[] character) {
            //TOTAL DURATION: 7.2s.
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSize(24, 24).Center().SetSprite(spriteDB.camp[0]);
            for (int i = 0; i < 32; i++) {
                sbCharacter.Move(Direction.Left);
                yield return new WaitForSeconds(3f / 32);
            }
            sbCharacter.SetSize(32, 32).SetY(0).Move(Direction.Left, 4).FlipHorizontal(true).SetSprite(character[0]);
            for (int i = 0; i < 32; i++) {
                sbCharacter.SetSprite(character[4 + (i % 2)]);
                sbCharacter.Move(Direction.Right);
                yield return new WaitForSeconds(3f / 32);
            }
            sbCharacter.SetSprite(character[0]);
            yield return new WaitForSeconds(0.2f);
        }

        public static IEnumerator StartAppDigiHunter(System.Action<bool> markEnded) {
            Sprite[] sArrows = spriteDB.digiHunter_arrows;
            Sprite[] sFaces = spriteDB.digiHunter_faces;

            audioMgr.PlaySound(audioMgr.digiHunter_Start);
            yield return new WaitForSeconds(0.5f);

            RectangleBuilder rbLoadingBar = ScreenElement.BuildRectangle("Loading Bar", AnimParent).SetSize(1, 4).SetPosition(30, 0);

            for(int i = 0; i < 29; i++) {
                rbLoadingBar.Move(Direction.Left).SetSize(i + 2, 4);
                yield return new WaitForSeconds(1.25f / 29);
            }

            SpriteBuilder[] sbArrows = new SpriteBuilder[6];
            for(int i = 0; i < 3; i++) {
                sbArrows[i] = ScreenElement.BuildSprite("Vertical Arrow", AnimParent).SetSize(3, 6).SetPosition(2, 9 + (i * 8)).SetSprite(sArrows[0]);
            }
            for (int i = 0; i < 3; i++) {
                sbArrows[3 + i] = ScreenElement.BuildSprite("Horizontal Arrow", AnimParent).SetSize(6, 3).SetPosition(6 + (i * 8), 5).SetSprite(sArrows[1]);
            }

            for(int i = 0; i < 3; i++) {
                foreach (var s in sbArrows) s.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                foreach (var s in sbArrows) s.SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }

            SpriteBuilder[] sbWhiteFaces = new SpriteBuilder[4];
            SpriteBuilder[] sbBlackFaces = new SpriteBuilder[5];
            sbWhiteFaces[0] = ScreenElement.BuildSprite("White face", AnimParent).SetSize(8, 8).SetPosition(13, 8).SetSprite(sFaces[0]);
            sbWhiteFaces[1] = ScreenElement.BuildSprite("White face", AnimParent).SetSize(8, 8).SetPosition(5, 16).SetSprite(sFaces[0]);
            sbWhiteFaces[2] = ScreenElement.BuildSprite("White face", AnimParent).SetSize(8, 8).SetPosition(21, 16).SetSprite(sFaces[0]);
            sbWhiteFaces[3] = ScreenElement.BuildSprite("White face", AnimParent).SetSize(8, 8).SetPosition(13, 24).SetSprite(sFaces[0]);
            sbBlackFaces[0] = ScreenElement.BuildSprite("Black face", AnimParent).SetSize(8, 8).SetPosition(5, 8).SetSprite(sFaces[1]);
            sbBlackFaces[1] = ScreenElement.BuildSprite("Black face", AnimParent).SetSize(8, 8).SetPosition(21, 8).SetSprite(sFaces[1]);
            sbBlackFaces[2] = ScreenElement.BuildSprite("Black face", AnimParent).SetSize(8, 8).SetPosition(13, 16).SetSprite(sFaces[1]);
            sbBlackFaces[3] = ScreenElement.BuildSprite("Black face", AnimParent).SetSize(8, 8).SetPosition(5, 24).SetSprite(sFaces[1]);
            sbBlackFaces[4] = ScreenElement.BuildSprite("Black face", AnimParent).SetSize(8, 8).SetPosition(21, 24).SetSprite(sFaces[1]);

            for(int i = 0; i < 3; i++) {
                foreach (var s in sbWhiteFaces) s.SetActive(true);
                foreach (var s in sbBlackFaces) s.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                foreach (var s in sbWhiteFaces) s.SetActive(false);
                foreach (var s in sbBlackFaces) s.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            foreach (var s in sbBlackFaces) s.SetActive(false);
            sbArrows[0].SetActive(true);
            sbArrows[3].SetActive(true);

            TextBoxBuilder tbStart = ScreenElement.BuildTextBox("Start", AnimParent, DFont.Small).SetText("START").SetPosition(6, 17);
            yield return new WaitForSeconds(0.5f);
            tbStart.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            tbStart.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            markEnded(true);
        }

        //Game event animations:
        public static IEnumerator TransitionToMap1(GameChar character) {
            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(sCharacter[0]);
            SpriteBuilder sbBlackScreen = ScreenElement.BuildSprite("Black Screen", AnimParent).SetSprite(spriteDB.blackScreen).SetActive(false);
            SpriteBuilder sbCurtain = ScreenElement.BuildSprite("Curtain", AnimParent).SetSprite(spriteDB.curtain).PlaceOutside(Direction.Up);

            for(int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.5f);
                sbBlackScreen.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbBlackScreen.SetActive(false);
            }
            for (int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.3f);
                sbBlackScreen.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                sbBlackScreen.SetActive(false);
            }
            yield return new WaitForSeconds(0.2f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            audioMgr.PlaySound(audioMgr.unpleasantBeep);
            sbCharacter.SetActive(false);

            yield return new WaitForSeconds(7f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            sbBlackScreen.SetActive(true);
            for(int i = 0; i < 2; i++) {
                yield return new WaitForSeconds(0.2f);
                sbBlackScreen.SetActive(false);
                yield return new WaitForSeconds(0.1f);
                sbBlackScreen.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            sbBlackScreen.SetActive(false);

            for (int i = 0; i < 64; i++) {
                if (i == 32) sbCharacter.SetSprite(sCharacter[7]).SetActive(true);
                sbCurtain.Move(Direction.Down);
                yield return new WaitForSeconds(4f / 64);
            }
            yield return new WaitForSeconds(0.5f);
            yield return DisplayNewArea(gm.WorldMgr.CurrentWorld, gm.WorldMgr.CurrentArea, gm.WorldMgr.CurrentDistance);
        }

        public static IEnumerator EnemyEscapes(string enemyDigimon, string friendlyDigimon) {
            Sprite[] sEnemy = spriteDB.GetAllDigimonSprites(enemyDigimon);
            Sprite[] sFriendly = spriteDB.GetAllDigimonSprites(friendlyDigimon);
            Sprite exclamationMark = spriteDB.battle_disobey;

            SpriteBuilder sbEnemy = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24).Center().SetSprite(sEnemy[0]).FlipHorizontal(true);

            yield return new WaitForSeconds(0.65f);
            sbEnemy.FlipHorizontal(false);
            yield return new WaitForSeconds(0.65f);
            sbEnemy.FlipHorizontal(true);
            yield return new WaitForSeconds(0.3f);
            sbEnemy.FlipHorizontal(false);
            yield return new WaitForSeconds(0.3f);
            sbEnemy.FlipHorizontal(true);
            yield return new WaitForSeconds(0.3f);
            sbEnemy.FlipHorizontal(false);

            SpriteBuilder sbDisobey = ScreenElement.BuildSprite("Disobey", AnimParent).SetSize(3, 9).SetPosition(1, 1).SetSprite(exclamationMark);
            yield return new WaitForSeconds(0.3f);
            sbDisobey.SetActive(false);
            yield return new WaitForSeconds(0.3f);

            audioMgr.PlaySound(audioMgr.launchAttack);

            sbEnemy.SetSprite(sEnemy[2]);
            for(int i = 0; i < 32; i++) {
                sbEnemy.Move(Direction.Left);
                yield return new WaitForSeconds(0.8f / 32);
            }

            yield return new WaitForSeconds(0.75f);

            SpriteBuilder sbFriendly = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24).Center().SetSprite(sFriendly[0]);
            yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(0.45f);
            sbFriendly.FlipHorizontal(true);
            yield return new WaitForSeconds(0.45f);
            sbFriendly.FlipHorizontal(false);
            yield return new WaitForSeconds(0.85f);

            sbFriendly.SetSprite(sFriendly[1]);
            sbDisobey.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            sbFriendly.SetSprite(sFriendly[0]);
            yield return new WaitForSeconds(0.3f);
            sbDisobey.Dispose();
            yield return new WaitForSeconds(0.5f);
        }

        public static IEnumerator TransitionToMap3(GameChar character, string enemyDigimon, List<string> stolenSpirits) {
            Sprite[] sCharacter = spriteDB.GetCharacterSprites(character);
            Sprite[] sEnemyDigimon = spriteDB.GetAllDigimonSprites(enemyDigimon);
            Sprite[] sAbsorber = spriteDB.spirit_absorber;

            SpriteBuilder[] sbAllSpirits = new SpriteBuilder[stolenSpirits.Count];

            if (stolenSpirits.Count != 0) {
                for (int i = 0; i < sbAllSpirits.Length; i++) {
                    sbAllSpirits[i] = ScreenElement.BuildSprite("Spirit", AnimParent).SetSize(14, 16)
                        .SetSprite(spriteDB.GetDigimonSprite(stolenSpirits[i], SpriteAction.SpiritSmall));
                    sbAllSpirits[i].SetPosition(32 + (Mathf.FloorToInt(i / 2f) * 16), ((i % 2) * 16));
                }
            }

            SpriteBuilder sbCharacter = ScreenElement.BuildSprite("Character", AnimParent).SetSprite(sCharacter[4]);
            SpriteBuilder[] sbAbsorber = new SpriteBuilder[2];
            sbAbsorber[0] = ScreenElement.BuildSprite("Absorber0", AnimParent).SetSize(16, 32).SetPosition(0, 0).SetSprite(sAbsorber[0]).SetActive(false);
            sbAbsorber[1] = ScreenElement.BuildSprite("Absorber1", AnimParent).SetSize(16, 32).SetPosition(16, 0).SetSprite(sAbsorber[1]).SetActive(false);

            //Character runs.
            for(int i = 0; i < 5; i++) {
                sbCharacter.SetSprite(sCharacter[4]);
                yield return new WaitForSeconds(0.25f);
                sbCharacter.SetSprite(sCharacter[5]);
                yield return new WaitForSeconds(0.25f);
            }
            //Absorbers appear.
            audioMgr.PlaySound(audioMgr.stealAllSpirits);
            for (int i = 0; i < 5; i++) {
                sbAbsorber.SetActive(true);
                yield return new WaitForSeconds(0.35f);
                sbAbsorber.SetActive(false);
                yield return new WaitForSeconds(0.35f);
            }
            //Camera moves to show spirits being absorbed.
            for (int i = 0; i < 16; i++) {
                sbCharacter.Move(Direction.Right);
                yield return new WaitForSeconds(1f / 16);
            }
            sbAbsorber[1].SetSprite(sAbsorber[0]);
            for(int i = 0; i < 192; i++) {
                sbAbsorber[1].SetActive(Mathf.FloorToInt(i / 7) % 2 == 0);
                sbAllSpirits.Move(Direction.Left);
                yield return new WaitForSeconds(1.5f / 32);
            }
            yield return new WaitForSeconds(0.35f);
            sbAbsorber[1].SetActive(false);
            sbCharacter.SetActive(false);

            //Enemy appears.
            SpriteBuilder sbEnemyDigimon = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24)
                .FlipHorizontal(true).Center().SetSprite(sEnemyDigimon[0]);
            RectangleBuilder rbEnemyDigimonInverted = ScreenElement.BuildRectangle("Enemy inverted", AnimParent)
                .SetSize(32, 32).SetColor(true).SetActive(false);
            ScreenElement.BuildSprite("Enemy", rbEnemyDigimonInverted.transform).SetSize(24, 24)
                .FlipHorizontal(true).SetPosition(4, 4).SetSprite(spriteDB.GetInvertedSprite(sEnemyDigimon[1]));

            for (int i = 0; i < 3; i++) {
                sbEnemyDigimon.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                sbEnemyDigimon.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
            sbEnemyDigimon.SetSprite(sEnemyDigimon[1]);

            for(int i = 0; i < 3; i++) {
                rbEnemyDigimonInverted.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                rbEnemyDigimonInverted.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
            rbEnemyDigimonInverted.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            sbEnemyDigimon.SetActive(false);

            //Explode spirits:
            for (int i = 0; i < stolenSpirits.Count; i += 4) {
                Sprite sSpirit1, sSpirit2 = null, sSpirit3 = null, sSpirit4 = null;
                sSpirit1 = spriteDB.GetDigimonSprite(stolenSpirits[i], SpriteAction.SpiritSmall);
                if (stolenSpirits.Count > i + 1) sSpirit2 = spriteDB.GetDigimonSprite(stolenSpirits[i + 1], SpriteAction.SpiritSmall);
                if (stolenSpirits.Count > i + 2) sSpirit3 = spriteDB.GetDigimonSprite(stolenSpirits[i + 2], SpriteAction.SpiritSmall);
                if (stolenSpirits.Count > i + 3) sSpirit4 = spriteDB.GetDigimonSprite(stolenSpirits[i + 3], SpriteAction.SpiritSmall);

                SpriteBuilder[] sbSpirit = new SpriteBuilder[4];

                sbSpirit[0] = ScreenElement.BuildSprite("Spirit1", AnimParent)
                    .SetSize(14, 16).SetPosition(1, 0).SetSprite(sSpirit1);
                sbSpirit[1] = ScreenElement.BuildSprite("Spirit2", AnimParent)
                    .SetSize(14, 16).SetPosition(17, 0).SetSprite(sSpirit2 ?? spriteDB.emptySprite);
                sbSpirit[2] = ScreenElement.BuildSprite("Spirit3", AnimParent)
                    .SetSize(14, 16).SetPosition(1, 16).SetSprite(sSpirit3 ?? spriteDB.emptySprite);
                sbSpirit[3] = ScreenElement.BuildSprite("Spirit4", AnimParent)
                    .SetSize(14, 16).SetPosition(17, 16).SetSprite(sSpirit4 ?? spriteDB.emptySprite);

                yield return new WaitForSeconds(0.4f);

                audioMgr.PlaySound(audioMgr.destroySpirits);
                sbSpirit[0].SetSprite(spriteDB.spirit_explosion);
                yield return new WaitForSeconds(0.2f);
                sbSpirit[0].SetSprite(spriteDB.emptySprite);

                sbSpirit[3].SetSprite(spriteDB.spirit_explosion);
                yield return new WaitForSeconds(0.2f);
                sbSpirit[3].SetSprite(spriteDB.emptySprite);

                sbSpirit[1].SetSprite(spriteDB.spirit_explosion);
                yield return new WaitForSeconds(0.2f);
                sbSpirit[1].SetSprite(spriteDB.emptySprite);

                sbSpirit[2].SetSprite(spriteDB.spirit_explosion);
                yield return new WaitForSeconds(0.2f);
                sbSpirit[2].SetSprite(spriteDB.emptySprite);
            }
            yield return new WaitForSeconds(0.1f);

            ClearAnimParent();

            sbEnemyDigimon = ScreenElement.BuildSprite("Enemy", AnimParent).SetSize(24, 24)
                .FlipHorizontal(true).Center().SetSprite(sEnemyDigimon[1]);
            yield return new WaitForSeconds(0.4f);
            sbEnemyDigimon.SetSprite(sEnemyDigimon[0]);
            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < 16; i++) {
                sbEnemyDigimon.Move(Direction.Up, 2);
                yield return new WaitForSeconds(0.8f / 16);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}