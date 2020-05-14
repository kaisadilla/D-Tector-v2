using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice.Apps {
    public class JackpotBox : DigiviceApp {
        private const int MINIMUM_LENGTH = 4;
        private const int MAXIMUM_LENGTH = 10;
        private const float MINIMUM_TIME = 0.25f;
        private const float MAXIMUM_TIME = 0.75f;
        private const int THRESHOLD_FOR_MEGA_REWARD = 8;

        private int currentScreen = 0; //0: attack/exit, 1: receiving input, 2: end

        private string friendlyDigimon;
        private int[] pattern; //This ranges from 4 to 12 - 0: left, 1: right, 2: up, 3: down
        private float delay;
        private int timeRemaining = 12;
        private int[] playerSelection;
        private int currentKey = 0;

        private SpriteBuilder keypad;
        SpriteBuilder[] keys = new SpriteBuilder[4];
        private TextBoxBuilder tbTime, tbTimeCount;

        public override void InputLeft() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonB();
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                InputKey(0);
            }
        }
        public override void InputRight() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonB();
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                InputKey(1);
            }
        }
        public override void InputA() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonA();
                DisplayPattern();
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                InputKey(3);
            }
        }
        public override void InputB() {
            if(currentScreen == 0) {
                audioMgr.PlayButtonB();
                CloseApp(Screen.GamesRewardMenu);
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                InputKey(2);
            }
        }

        public override void StartApp() {
            friendlyDigimon = gm.logicMgr.GetAllDDockDigimon().GetRandomElement();
            gm.EnqueueAnimation(Animations.EncounterEnemy("jackpot", 0.5f));
            gm.EnqueueAnimation(Animations.SummonDigimon(friendlyDigimon));

            pattern = GeneratePattern(Random.Range(MINIMUM_LENGTH, MAXIMUM_LENGTH + 1));
            playerSelection = new int[pattern.Length];
            delay = Random.Range(MINIMUM_TIME, MAXIMUM_TIME);

            VisualDebug.WriteLine($"Generated pattern with {pattern.Length} keys, and a delay of {delay}");
        }

        private void Update() {
            DrawScreen();
        }
        private void DrawScreen() {
            if (currentScreen == 0) {
                SetScreen(gm.spriteDB.battle_combatMenu[0]);
            }
            else if (currentScreen == 1) {
                SetScreen(Constants.EMPTY_SPRITE);
            }
        }

        private int[] GeneratePattern(int length) {
            int[] input = new int[length];
            for(int i = 0; i < input.Length; i++) {
                input[i] = Random.Range(0, 4);
            }
            return input;
        }

        private void DisplayPattern() {
            StartCoroutine(PADisplayPattern());
            currentScreen = 1;
        }

        private void InputKey(int key) {
            playerSelection[currentKey] = key;
            StartCoroutine(PADisplayChosenKey(key));
            currentKey++;
            if(currentKey == playerSelection.Length) {
                DecideBattle();
            }
        }

        private void DecideBattle() {
            VisualDebug.WriteLine($"Original input: {string.Join(",", pattern)}");
            VisualDebug.WriteLine($"Player input:   {string.Join(",", playerSelection)}");
            currentScreen = 2;

            int energyRank = GetEnergyRank();
            int rewardCategory = GetRewardCategory();
            Reward reward = GetRandomReward(rewardCategory);

            //To prevent abusing jackpot to level up fast, level up/down is restricted to certain conditions.
            //If those conditions aren't met, they are replaced with increase/reduce distance.
            if (reward == Reward.LevelDown && gm.logicMgr.GetPlayerLevelProgression() > 0.5f
                || reward == Reward.ForceLevelDown && gm.logicMgr.GetPlayerLevelProgression() == 0f)
            {
                reward = Reward.IncreaseDistance500;
            }
            else if(reward == Reward.LevelUp && gm.logicMgr.GetPlayerLevelProgression() < 0.5f
                || reward == Reward.ForceLevelUp && gm.logicMgr.GetPlayerLevelProgression() == 0f)
            {
                reward = Reward.ReduceDistance500;
            }

            Sprite[] friendlySprites = gm.spriteDB.GetAllDigimonBattleSprites(friendlyDigimon, energyRank);

            //Play animations of the battle against the box.
            gm.EnqueueAnimation(Animations.LaunchAttack(friendlySprites, 0, false, false));
            gm.EnqueueAnimation(Animations.AttackCollision(0, friendlySprites, 3, null, 0));

            if(rewardCategory < 2) {
                gm.EnqueueAnimation(Animations.BoxResists(friendlyDigimon));
            }
            else {
                gm.EnqueueAnimation(Animations.DestroyBox());
                if(Random.Range(0, 20) > gm.JackpotValue) {
                    reward = Reward.Empty;
                }


            }

            //Apply the reward and play its animation.
            if (reward == Reward.Empty) {
                gm.EnqueueRewardAnimation(reward, null, null, null);
            }
            else if (reward == Reward.PunishDigimon) {
                gm.logicMgr.ApplyReward(reward, friendlyDigimon, out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, friendlyDigimon, resultBefore, resultAfter);
            }
            else if (reward == Reward.RewardDigimon) {
                Rarity rarity;
                float rng = Random.Range(0f, 1f);
                if (rng < 0.50f) rarity = Rarity.Common;
                else if (rng < 0.80f) rarity = Rarity.Rare;
                else if (rng < 0.95f) rarity = Rarity.Epic;
                else rarity = Rarity.Legendary;
                string rewardedDigimon = Database.GetRandomDigimonOfRarity(rarity, gm.logicMgr.GetPlayerLevel() + 20).name;
                gm.logicMgr.ApplyReward(reward, rewardedDigimon, out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, rewardedDigimon, resultBefore, resultAfter);
            }
            else if (reward == Reward.UnlockDigicodeOwned) {
                string[] ownedDigimon = gm.logicMgr.GetAllUnlockedDigimon();
                gm.logicMgr.ApplyReward(reward, ownedDigimon.GetRandomElement(), out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, ownedDigimon.GetRandomElement(), resultBefore, resultAfter);
            }
            else if (reward == Reward.UnlockDigicodeNotOwned) {
                Rarity rarity;
                float rng = Random.Range(0f, 1f);
                if (rng < 0.50f) rarity = Rarity.Common;
                else if (rng < 0.80f) rarity = Rarity.Rare;
                else if (rng < 0.95f) rarity = Rarity.Epic;
                else rarity = Rarity.Legendary;
                string rewardedDigimon = Database.GetRandomDigimonOfRarity(rarity, 100).name;
                gm.logicMgr.ApplyReward(reward, rewardedDigimon, out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, rewardedDigimon, resultBefore, resultAfter);
            }
            else if (reward == Reward.TriggerBattle) {
                //string enemyDigimon = Database.GetRandomDigimonForBattle(gm.logicMgr.GetPlayerLevel()).name;
                //gm.logicMgr.ApplyReward(reward, enemyDigimon, out object resultBefore, out object resultAfter);
                //gm.EnqueueRewardAnimation(reward, enemyDigimon, resultBefore, resultAfter);
                gm.EnqueueAnimation(TriggerBattle());
                return;
            }
            else {
                gm.logicMgr.ApplyReward(reward, null, out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, null, resultBefore, resultAfter);
            }

            CloseApp(Screen.GamesRewardMenu);
        }
        private IEnumerator TriggerBattle() {
            CloseApp();
            gm.logicMgr.CallRandomBattle();
            yield return null;
        }

        private IEnumerator PADisplayPattern() {
            gm.LockInput();

            keypad = ScreenElement.BuildSprite("Keypad", Parent).SetSize(24, 24).Center().SetSprite(gm.spriteDB.jackpot_pad);
            keys[0] = ScreenElement.BuildSprite("Key Left", Parent).SetSize(8, 12).SetPosition(4, 10)
                .SetSprite(gm.spriteDB.jackpot_keys[0]).SetTransparent(true).SetActive(false);
            keys[1] = ScreenElement.BuildSprite("Key Right", Parent).SetSize(8, 12).SetPosition(20, 10)
                .SetSprite(gm.spriteDB.jackpot_keys[1]).SetTransparent(true).SetActive(false);
            keys[2] = ScreenElement.BuildSprite("Key Up", Parent).SetSize(12, 8).SetPosition(10, 4)
                .SetSprite(gm.spriteDB.jackpot_keys[2]).SetTransparent(true).SetActive(false);
            keys[3] = ScreenElement.BuildSprite("Key Down", Parent).SetSize(12, 8).SetPosition(10, 20)
                .SetSprite(gm.spriteDB.jackpot_keys[3]).SetTransparent(true).SetActive(false);

            SpriteBuilder hourglass = ScreenElement.BuildSprite("Hourglass", Parent).SetSprite(gm.spriteDB.hourglass);

            yield return new WaitForSeconds(0.75f);
            hourglass.Dispose();

            for(int i = 0; i < pattern.Length; i++) {
                audioMgr.PlaySound(audioMgr.beepLow);
                keys[pattern[i]].SetActive(true);
                yield return new WaitForSeconds(delay);
                keys[pattern[i]].SetActive(false);
            }

            //Black screen:
            RectangleBuilder rbBlackScreen = ScreenElement.BuildRectangle("BlackScreen0", Parent).SetSize(32, 32);
            SpriteBuilder sbLoading = ScreenElement.BuildSprite("Loading", Parent).SetSprite(gm.spriteDB.loading).PlaceOutside(Direction.Up);

            for (int i = 0; i < 64; i++) {
                sbLoading.Move(Direction.Down);
                yield return new WaitForSeconds((delay * 2f) / 64);
            }
            rbBlackScreen.Dispose();
            sbLoading.Dispose();

            //Ready the player:
            keypad.Move(Direction.Down, 4);
            keys.Move(Direction.Down, 4);
            tbTime = ScreenElement.BuildTextBox("Time", screenDisplay.transform, DFont.Small)
                .SetText("TIME").SetSize(18, 5).SetPosition(1, 1);
            tbTimeCount = ScreenElement.BuildTextBox("TimeCount", screenDisplay.transform, DFont.Small)
                .SetText(timeRemaining.ToString()).SetSize(10, 5).SetPosition(22, 1);
            StartCoroutine(TimeCount());

            gm.UnlockInput();
        }

        private IEnumerator PADisplayChosenKey(int key) {
            //gm.LockInput();

            keys[key].SetActive(true);
            yield return new WaitForSeconds(0.25f);
            keys[key].SetActive(false);

            //gm.UnlockInput();
        }

        private IEnumerator TimeCount() {
            yield return new WaitForSeconds(1f);
            while (timeRemaining > -1) {
                timeRemaining--;
                tbTimeCount.Text = timeRemaining.ToString();
                yield return new WaitForSeconds(1f);
            }
            DecideBattle();
            tbTime.Dispose();
            tbTimeCount.Dispose();
        }

        private int GetCorrectInputCount() {
            int correctInputCount = 0;
            for (int i = 0; i < pattern.Length; i++) {
                if (playerSelection[i] == pattern[i]) correctInputCount++;
            }

            return correctInputCount;
        }
        
        private int GetRewardCategory() {
            int reward;

            float percCorrect = GetCorrectInputCount() / (float)pattern.Length;

            if (percCorrect < 0.26) reward = 0;
            else if (percCorrect < 0.51) reward = 1;
            else if (percCorrect < 0.76) reward = 2;
            else reward = 3;

            if (percCorrect == 1f && pattern.Length >= THRESHOLD_FOR_MEGA_REWARD) reward = 4;

            return reward;
        }

        private int GetEnergyRank() {
            float energyDiscountPerMiss = 12f / pattern.Length;
            int misses = pattern.Length - GetCorrectInputCount();
            int rank = Mathf.FloorToInt(12f - (energyDiscountPerMiss * misses));
            if (rank == 10) rank = 15;
            else if (rank == 11) rank = 16;
            else if (rank == 12) rank = 17;

            if (rank == 17 && pattern.Length >= THRESHOLD_FOR_MEGA_REWARD && misses == 0) rank = 19; //On perfect input when pattern length is 8 or more.

            return rank;
        }

        private Reward GetRandomReward(int category) {
            float rng = Random.Range(0f, 1f);
            switch(category) {
                case 0:
                    if (rng < 0.40f) return Reward.IncreaseDistance500;
                    else if (rng < 0.60f) return Reward.PunishDigimon;
                    else if (rng < 0.70f) return Reward.DataStorm;
                    else if (rng < 0.80f) return Reward.LoseSpiritPower10;
                    else if (rng < 0.90f) return Reward.ForceLevelDown;
                    else if (rng < 0.95f) return Reward.PunishDigimon;
                    else return Reward.IncreaseDistance2000;
                case 1:
                    if (rng < 0.40f) return Reward.IncreaseDistance500;
                    else if (rng < 0.65f) return Reward.TriggerBattle;
                    else if (rng < 0.75f) return Reward.PunishDigimon;
                    else if (rng < 0.85f) return Reward.DataStorm;
                    else if (rng < 0.95f) return Reward.LoseSpiritPower10;
                    else return Reward.LevelDown;
                case 2:
                    if (rng < 0.35f) return Reward.ReduceDistance500;
                    else if (rng < 0.65f) return Reward.TriggerBattle;
                    else if (rng < 0.80f) return Reward.IncreaseDistance300;
                    else if (rng < 0.90f) return Reward.GainSpiritPower10;
                    else return Reward.RewardDigimon;
                case 3:
                    if (rng < 0.30f) return Reward.ReduceDistance500;
                    else if (rng < 0.55f) return Reward.GainSpiritPower10;
                    else if (rng < 0.80f) return Reward.RewardDigimon;
                    else if (rng < 0.95f) return Reward.LevelUp;
                    else return Reward.UnlockDigicodeOwned;
                case 4:
                    if (rng < 0.55f) return Reward.RewardDigimon;
                    else if (rng < 0.65f) return Reward.ReduceDistance1000;
                    else if (rng < 0.75f) return Reward.ForceLevelUp;
                    else if (rng < 0.85f) return Reward.GainSpiritPowerMax;
                    else if (rng < 0.95f) return Reward.UnlockDigicodeOwned;
                    else return Reward.UnlockDigicodeNotOwned;
                default: return Reward.none;
            }
        }
    }
}