using UnityEngine;

namespace Kaisa.Digivice {
    public class Digimon {
        public readonly int number; //The number of the Digimon.
        public readonly int order; //An index used to order Digimons in the database.
        public readonly string name;

        public readonly Stage stage;
        public readonly SpiritType spiritType; //Used for Digimons of stage Spirit.

        public readonly string abilityName; //The name of the sprite this Digimon uses as its ability.
        public readonly Element element;
        public readonly string evolution;

        public readonly int weight; //A number between 0 and 10, used to calculate how commonly a Digimon appears. A weight of 0 means the Digimon never appears.
        public readonly Rarity rarity; //Rarity is used to calculate the chance of unlocking a Digimon.
        public readonly bool disabled; //If this is true, the Digimon does not appear anywhere in the game.

        public readonly int baseLevel;
        public readonly CombatStats stats; //The regular stats used normally by the Digimon.
        //Stats used only to calculate the stats of a Digimon as a boss. At level 100, a boss Digimon's stats will be exactly those in bossStats.
        public readonly CombatStats bossStats;

        public Digimon(
                int number, int order, string name, Stage stage, SpiritType spiritType,
                string abilityName, Element element, string evolution, int weight, Rarity rarity, bool disabled,
                int baseLevel, CombatStats stats, CombatStats bossStats)
        {
            this.number = number;
            this.order = order;
            this.name = name.ToLower();
            this.stage = stage;
            this.spiritType = spiritType;
            this.abilityName = abilityName;
            this.element = element;
            this.evolution = evolution;
            this.weight = weight;
            this.rarity = rarity;
            this.disabled = disabled;
            this.baseLevel = baseLevel;
            this.stats = stats;
            this.bossStats = bossStats;
            this.bossStats = this.bossStats?? stats; //If the database does not have boss stats for this Digimon, use regular stats instead.
        }

        /// <summary>
        /// Returns the maximum amount of extra levels a Digimon may have.
        /// </summary>
        public int MaxExtraLevel {
            get {
                int maxLevel;
                if (stage == Stage.Rookie) maxLevel = baseLevel * 2;
                else if (stage == Stage.Armor) maxLevel = 100;
                else maxLevel = Mathf.CeilToInt(baseLevel * 1.5f);
                return maxLevel - baseLevel;
            }
        }
        /// <summary>
        /// Returns the Spirit Cost of this Digimon, based on the player level.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        public int GetSpiritCost(int playerLevel) {
            int baseCost;
            float decay;

            if (stage == Stage.Armor) {
                baseCost = 10;
                decay = 20;
            }
            else if (stage == Stage.Spirit) {
                if (spiritType == SpiritType.Human) {
                    baseCost = 20;
                    decay = 30;
                }
                else if (spiritType == SpiritType.Ancient) {
                    baseCost = 40;
                    decay = 50;
                }
                else {
                    baseCost = 30;
                    decay = 30;
                }
            }
            else return 0;

            float currentCost = baseCost * Mathf.Pow(0.5f, playerLevel / decay);
            return Mathf.FloorToInt(currentCost);
        }
        /// <summary>
        /// Returns the cost to call this Digimon from a D-Dock, based on the player level.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        public int GetCallCost(int playerLevel) {
            float percLevelDiff = baseLevel / (float)playerLevel; //The proportional difference between the level of the player and the Digimon.
            int levelDiff = playerLevel - baseLevel; //The absolute difference between those levels.

            if (percLevelDiff < 0.55f && levelDiff >= 10) return 0;
            if (percLevelDiff < 0.75f && levelDiff >= 5) return 1;
            if (percLevelDiff < 0.90f && levelDiff >= 2) return 2;
            if (percLevelDiff < 1f && levelDiff >= 1) return 3;
            if (percLevelDiff == 1f) return 4;
            if (percLevelDiff == 1.30f) return 5;
            if (percLevelDiff == 1.60f) return 6;
            if (percLevelDiff == 2f) return 7;
            if (percLevelDiff == 3f) return 8;
            if (percLevelDiff == 4f) return 9;
            else return 10;
        }
        /// <summary>
        /// Returns the actual level of this Digimon controlled by the player, based on the player level, and the extra level of the Digimon stored in the Saved Game.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        /// <param name="digimonExtraLevel">The extra level value of the Digimon stored in the Saved Game, not the actual level of the Digimon.</param>
        public int GetFriendlyLevel(int digimonExtraLevel, int playerLevel = 100) {
            if (stage == Stage.Spirit) {
                if (spiritType == SpiritType.Ancient) {
                    return (playerLevel < 20) ? 20 : playerLevel; //Returns the level of the player, but at a minimum level of 20.
                }
                return playerLevel;
            }
            else if (stage == Stage.Armor) {
                return (playerLevel < 10) ? 10 : playerLevel; //Returns the level of the player, but at a minimum level of 10.
            }

            return baseLevel + digimonExtraLevel;
        }
        /// <summary>
        /// Returns the chance that this Digimon will obey, based on the player level.
        /// Note that, for this calculation, the original Digimon called by the player should be used, even if it has evolved
        /// </summary>
        /// <param name="playerLevel">The level of the player</param>
        public float GetObeyChance(int playerLevel) {
            int levelDiff = baseLevel - playerLevel;

            if (levelDiff <= 0) return 1f; //If the player's level is equal or greater than the digimon, it will always obey.
            if (levelDiff >= 10) return 0f; //If the player's level is 10 or more levels behind that of the digimon, it will never obey.

            float obeyChance = 1f - (Mathf.Pow(levelDiff, 2) / 100f);
            return obeyChance;
        }
        /// <summary>
        /// Returns the chance that this Digimon will even attack.
        /// </summary>
        /// <param name="playerLevel">The level of the player</param>
        public float GetAttackChance(int playerLevel) {
            int levelDiff = baseLevel - playerLevel;

            if (levelDiff <= 0) return 1f;
            if (levelDiff >= 20) return 0f;

            float attackChance = (Mathf.Pow(10f, 1.5f) - Mathf.Pow((levelDiff / 2f), 1.5f)) / Mathf.Pow(10f, -0.5f);
            return attackChance;
        }
        /// <summary>
        /// Returns the stats (HP, EN, CR, AB) of this Digimon as a boss, based on its level.
        /// </summary>
        /// <param name="bossLevel">The level of the boss. Usually this is the same as the level of the player.</param>
        public CombatStats GetBossStats(int bossLevel) {
            int HP, EN, CR, AB;
            //If the Digimon is Hybrid, assign its stats using the formula(s) for Hybrid bosses.
            if (stage == Stage.Spirit) {
                HP = GetStatAsSpiritBoss(stats.HP, bossLevel);
                EN = GetStatAsSpiritBoss(stats.EN, bossLevel);
                CR = GetStatAsSpiritBoss(stats.CR, bossLevel);
                AB = GetStatAsSpiritBoss(stats.AB, bossLevel);
            }
            //Else, assign its stats using the formula for regular bosses.
            else {
                HP = GetStatAsRegularBoss(stats.HP, bossLevel);
                EN = GetStatAsRegularBoss(stats.EN, bossLevel);
                CR = GetStatAsRegularBoss(stats.CR, bossLevel);
                AB = GetStatAsRegularBoss(stats.AB, bossLevel);
            }

            return new CombatStats(HP, EN, CR, AB);
        }
        /// <summary>
        /// Returns the stats of a Digimon based on its actual level. This shouldn't be used for special Digimon such as Hybrid- or Armor-Stage Digimon.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        /// <param name="digimonExtraLevel">The extra level of the Digimon stored in the Saved Game, not the actual level of the Digimon.</param>
        /// <returns></returns>
        public CombatStats GetFriendlyStats(int playerLevel, int digimonExtraLevel) {
            int maxExtraLevel = MaxExtraLevel;

            int HP = GetStatAsFriendly(stats.HP, digimonExtraLevel, maxExtraLevel);
            int EN = GetStatAsFriendly(stats.EN, digimonExtraLevel, maxExtraLevel);
            int CR = GetStatAsFriendly(stats.CR, digimonExtraLevel, maxExtraLevel);
            int AB = GetStatAsFriendly(stats.AB, digimonExtraLevel, maxExtraLevel);

            return new CombatStats(HP, EN, CR, AB);
        }
        /// <summary>
        /// Returns the chance of this Digimon to evolve, based on the player level and the amount of call points they spent to attempt the evolution.
        /// Always returns 0f if the Digimon doesn't have an evolution.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        /// <param name="extraPoints">The call points spent for this attempt. It must be an integer between 1 and 10.</param>
        public float GetEvolveChance(int playerLevel, int extraPoints) {
            if (evolution == "") return 0f;
            //First, we calculate the amount of levels we'll add to the player level. This is based on the extra points:
            //The level multiplier ranges from 0 to 0.45 based on the amount of point spent (1 to 10).
            float multiplier = (extraPoints - 1) / 20f;
            //The amount of extra levels that will be added to the base player level to attemp the evolution.
            int extraLevel = Mathf.FloorToInt(playerLevel * multiplier);
            //The minimum amount of extra levels is the amount of extra points spent - 1.
            if (extraLevel < extraPoints - 1) extraLevel = extraPoints;

            playerLevel += extraLevel;
            int levelDiff = baseLevel - playerLevel;
            if (levelDiff <= 0) return 1f;
            if (levelDiff >= 10) return 0.05f;

            float a = Mathf.Pow(levelDiff, 2f);
            float b = levelDiff / 10f;
            float evolChance = 1.0f - (a / 100f) + (0.05f * b);

            return evolChance;
        }
        /// <summary>
        /// Returns the chance that this Digimon will be unlocked / leveled up.
        /// </summary>
        public float GetUnlockChance() {
            switch (rarity) {
                case Rarity.Common: return 0.50f;
                case Rarity.Rare: return 0.25f;
                case Rarity.Epic: return 0.10f;
                case Rarity.Legendary: return 0f;
                case Rarity.Boss: return 0f;
                case Rarity.none: return 0f;
                default: return 0f;
            }
        }

        private int GetStatAsSpiritBoss(int stat, int bossLevel) {
            float riggedStat;

            if(spiritType == SpiritType.Human) {
                riggedStat = (0.25f + (0.005f * bossLevel)) * stat;
            }
            else if (spiritType == SpiritType.Ancient) {
                riggedStat = (bossLevel / 100f) * stat;
            }
            else {
                riggedStat = (0.30f + (0.007f * bossLevel)) * stat;
            }

            return Mathf.RoundToInt(riggedStat);
        }
        private int GetStatAsRegularBoss(int stat, int bossLevel) {
            float riggedStat = (0.14f + (0.0086f * bossLevel)) * stat;
            return Mathf.RoundToInt(riggedStat);
        }
        private int GetStatAsFriendly(int stat, int currentExtraLevel, int maxExtraLevel) {
            if (currentExtraLevel == 0) return stat;
            int actualLevel = GetFriendlyLevel(currentExtraLevel);
            int actualExtraLevel = baseLevel + maxExtraLevel;
            //This formula makes every digimon have mostly round numbers at its max level.
            //This interpolates the stat between 100% (when it has 0 extra levels) and 150% (when it has the maximum amount of extra levels).
            float multiplier = 1.5f * (actualLevel / (float)actualExtraLevel);
            float riggedStat = stat * multiplier;

            return Mathf.CeilToInt(riggedStat);
        }
    }

    public class CombatStats {
        public readonly int HP;
        public readonly int EN;
        public readonly int CR;
        public readonly int AB;

        public CombatStats(int HP, int EN, int CR, int AB) {
            this.HP = HP;
            this.EN = EN;
            this.CR = CR;
            this.AB = AB;
        }

        /// <summary>
        /// Returns a mutable version of the CombatStats class. In this version, stats can be changed at runtime.
        /// </summary>
        public MutableStats MutableCopy() => new MutableStats(HP, EN, CR, AB);
    }

    public class MutableStats {
        public int HP;
        public int EN;
        public int CR;
        public int AB;

        public MutableStats(int HP, int EN, int CR, int AB) {
            this.HP = HP;
            this.EN = EN;
            this.CR = CR;
            this.AB = AB;
        }
    }

    public enum Stage {
        Rookie,
        Champion,
        Perfect,
        Mega,
        Ultimate,
        Armor,
        Spirit,
        none
    }

    public enum SpiritType {
        Human,
        Animal,
        Hybrid,
        Ancient,
        Fusion,
        Child,
        none
    }

    public enum Element {
        Fire,
        Light,
        Thunder,
        Wind,
        Ice,
        Dark,
        Earth,
        Wood,
        Metal,
        Water,
        none
    }

    public enum Rarity {
        Common,
        Rare,
        Epic,
        Legendary,
        Boss,
        none
    }

    public enum SpriteAction {
        Default,
        Attack,
        Crush,
        Spirit,
        Black
    }
}