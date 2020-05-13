using UnityEngine;

namespace Kaisa.Digivice {
    public class Digimon {
        public readonly bool disabled; //If this is true, the Digimon does not appear anywhere in the game.

        public readonly int number; //The number of the Digimon.
        public readonly int order; //An index used to order Digimons in the database.
        public readonly string name;

        public readonly Stage stage;
        public readonly SpiritType spiritType; //Used for Digimons of stage Spirit.

        public readonly string abilityName; //The name of the sprite this Digimon uses as its ability.
        public readonly Element element;
        public readonly string evolution;
        public readonly string[] extraEvolutions;

        public readonly int baseLevel;
        public readonly CombatStats stats; //The regular stats used normally by the Digimon.
        //Stats used only to calculate the stats of a Digimon as a boss. At level 100, a boss Digimon's stats will be exactly those in bossStats.
        public readonly CombatStats bossStats;

        public readonly bool isPseudo; //If true, the Digimon can't be obtained and is not counted as a Digimon.
        public readonly string code;

        public Digimon(
                int number, int order, string name, Stage stage, SpiritType spiritType,
                string abilityName, Element element, string evolution, string[] extraEvolutions, bool disabled,
                int baseLevel, CombatStats stats, CombatStats bossStats,
                bool isPseudo, string code)
        {
            this.number = number;
            this.order = order;
            this.name = name.ToLower();
            this.stage = stage;
            this.spiritType = spiritType;
            this.abilityName = abilityName;
            this.element = element;
            this.evolution = evolution;
            this.extraEvolutions = extraEvolutions;
            this.disabled = disabled;
            this.baseLevel = baseLevel;
            this.stats = stats;
            this.bossStats = bossStats;
            this.bossStats = this.bossStats?? stats; //If the database does not have boss stats for this Digimon, use regular stats instead.
            this.isPseudo = isPseudo;
            this.code = code;
        }

        /// <summary>
        /// Returns the maximum amount of extra levels a Digimon may have.
        /// </summary>
        public int MaxExtraLevel {
            get {
                int maxLevel;
                if (stage == Stage.Rookie) maxLevel = baseLevel * 2;
                else if (stage == Stage.Spirit || stage == Stage.Armor) maxLevel = 0;
                else maxLevel = Mathf.CeilToInt(baseLevel * 1.5f);
                return maxLevel - baseLevel;
            }
        }
        /// <summary>
        /// Returns the Spirit Cost of this Digimon, based on the player level.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        public int GetSpiritCost(int playerLevel) {
            int baseCost = 20;
            float decay = 20;
            if (name == "susanoomon") {
                baseCost = 95;
                decay = 50;
            }
            else if (stage == Stage.Armor) {
                baseCost = 10;
                decay = 20;
            }
            else if (stage == Stage.Spirit) {
                if (spiritType == SpiritType.Human) {
                    baseCost = 20;
                    decay = 30;
                }
                else if (spiritType == SpiritType.Animal) {
                    baseCost = 30;
                    decay = 30;
                }
                else if (spiritType == SpiritType.Hybrid) {
                    baseCost = 35;
                    decay = 40;
                }
                else if (spiritType == SpiritType.Ancient) {
                    baseCost = 40;
                    decay = 50;
                }
                else if (spiritType == SpiritType.Fusion) {
                    baseCost = 55;
                    decay = 60;
                }
                else {
                    return 0;
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
            if (percLevelDiff < 1.30f) return 5;
            if (percLevelDiff < 1.60f) return 6;
            if (percLevelDiff < 2f) return 7;
            if (percLevelDiff < 3f) return 8;
            if (percLevelDiff < 4f) return 9;
            else return 10;
        }
        /// <summary>
        /// Returns the actual level of this Digimon controlled by the player, based and the extra level of the Digimon stored in the Saved Game.
        /// </summary>
        /// <param name="digimonExtraLevel">The extra level value of the Digimon stored in the Saved Game, not the actual level of the Digimon.</param>
        public int GetFriendlyLevel(int digimonExtraLevel) {
            return baseLevel + digimonExtraLevel;
        }
        /// Returns the level of the Digimon as a boss, based on the player level.
        /// <param name="playerLevel">The level of the player.</param>
        public int GetBossLevel(int playerLevel) {
            if (stage == Stage.Spirit) {
                if (spiritType == SpiritType.Ancient) {
                    float level = 20 + (playerLevel * 0.8f);
                    return Mathf.RoundToInt(level);
                }
                return playerLevel;
            }
            else if (stage == Stage.Armor) {
                return (playerLevel < 10) ? 10 : playerLevel; //Returns the level of the player, but at a minimum level of 10.
            }
            return playerLevel;
        }
        /// <summary>
        /// Returns the chance that this Digimon will obey (between 0f and 1f), based on the player level.
        /// Note that, for this calculation, the original Digimon called by the player should be used, even if it has evolved
        /// </summary>
        /// <param name="playerLevel">The level of the player</param>
        public float GetObeyChance(int playerLevel) {
            //If the Digimon is a Spirit or Armor digivolution, use its current level rather than its base level.
            int currentBaseLevel = (stage == Stage.Spirit || stage == Stage.Armor) ? GetBossLevel(playerLevel) : baseLevel;

            int levelDiff = currentBaseLevel - playerLevel;

            if (levelDiff <= 0) return 1f; //If the player's level is equal or greater than the digimon, it will always obey.
            if (levelDiff >= 10) return 0f; //If the player's level is 10 or more levels behind that of the digimon, it will never obey.

            float obeyChance = 1f - (Mathf.Pow(levelDiff, 2) / 100f);
            return obeyChance;
        }
        /// <summary>
        /// Returns the chance that this Digimon will even attack (between 0f and 1f).
        /// </summary>
        /// <param name="playerLevel">The level of the player</param>
        public float GetIdleChance(int playerLevel) {
            int currentBaseLevel = (stage == Stage.Spirit || stage == Stage.Armor) ? GetBossLevel(playerLevel) : baseLevel;

            int levelDiff = currentBaseLevel - playerLevel;

            if (levelDiff <= 0) return 1f;
            if (levelDiff >= 20) return 0f;

            float attackChance = (Mathf.Pow(10f, 1.5f) - Mathf.Pow((levelDiff / 2f), 1.5f)) / Mathf.Pow(10f, -0.5f);
            return attackChance;
        }
        /// <summary>
        /// Returns the stats (HP, EN, CR, AB) of this Digimon as a boss, based on its level. Notice that some friendly Digimons, such as those that
        /// are Spirit-Stage, use these stat.
        /// </summary>
        /// <param name="playerLevel">The level of the player</param>
        public MutableCombatStats GetBossStats(int playerLevel) {
            int bossLevel = GetBossLevel(playerLevel);
            int HP, EN, CR, AB;
            //If the Digimon is Hybrid, assign its stats using the formula(s) for Hybrid bosses.
            if (stage == Stage.Spirit) {
                HP = GetStatAsSpiritBoss(bossStats.HP, bossLevel);
                EN = GetStatAsSpiritBoss(bossStats.EN, bossLevel);
                CR = GetStatAsSpiritBoss(bossStats.CR, bossLevel);
                AB = GetStatAsSpiritBoss(bossStats.AB, bossLevel);
            }
            //Else, assign its stats using the formula for regular bosses.
            else {
                HP = GetStatAsRegularBoss(bossStats.HP, bossLevel);
                EN = GetStatAsRegularBoss(bossStats.EN, bossLevel);
                CR = GetStatAsRegularBoss(bossStats.CR, bossLevel);
                AB = GetStatAsRegularBoss(bossStats.AB, bossLevel);
            }

            return new MutableCombatStats(HP, EN, CR, AB);
        }
        /// <summary>
        /// Returns the stats of a Digimon based on its actual level. This shouldn't be used for special Digimon such as Hybrid- or Armor-Stage Digimon.
        /// </summary>
        /// <param name="digimonExtraLevel">The extra level of the Digimon stored in the Saved Game, not the actual level of the Digimon.</param>
        /// <returns></returns>
        public MutableCombatStats GetFriendlyStats(int digimonExtraLevel) {
            int HP = GetStatAsFriendly(stats.HP, digimonExtraLevel);
            int EN = GetStatAsFriendly(stats.EN, digimonExtraLevel);
            int CR = GetStatAsFriendly(stats.CR, digimonExtraLevel);
            int AB = GetStatAsFriendly(stats.AB, digimonExtraLevel);

            return new MutableCombatStats(HP, EN, CR, AB);
        }
        /// <summary>
        /// Returns a mutable copy of the regular stats of the Digimon.
        /// </summary>
        public MutableCombatStats GetRegularStats() {
            return new MutableCombatStats(stats.HP, stats.EN, stats.CR, stats.AB);
        }
        /// <summary>
        /// Returns the chance of any Digimon to evolve into this one.
        /// Note: This must be called on the Digimon that will result from the digivolution, rather than
        /// the Digimon that is trying to evolve into this.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        /// <param name="extraPoints">The call points spent for this attempt. It must be an integer between 1 and 10.</param>
        public float GetEvolveChance(int playerLevel, int extraPoints) {
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
            if (evolChance < 0.05f) evolChance = 0.05f;

            return evolChance;
        }
        private int GetStatAsSpiritBoss(int stat, int bossLevel) {
            float riggedStat;

            if(spiritType == SpiritType.Human) {
                //25% of the stat, increasing up to 75% when the Digimon reaches level 100.
                riggedStat = (0.25f + (0.005f * bossLevel)) * stat;
            }
            else if (spiritType == SpiritType.Ancient) {
                //0% of the stat, increasing up to 100% when the Digimon reaches level 100.
                riggedStat = (0.20f + (0.008f * bossLevel)) * stat;
                //riggedStat = (0.2f + (0.008f * bossLevel)) * stat;
            }
            else {
                //30% of the stat, increasing up to 100% when the Digimon reaches level 100.
                riggedStat = (0.30f + (0.007f * bossLevel)) * stat;
            }

            return Mathf.RoundToInt(riggedStat);
        }
        private int GetStatAsRegularBoss(int stat, int bossLevel) {
            float riggedStat = (0.20f + (0.008f * bossLevel)) * stat;
            return Mathf.RoundToInt(riggedStat);
        }
        private int GetStatAsFriendly(int stat, int currentExtraLevel) {
            if (currentExtraLevel == 0) return stat;
            //This formula makes every digimon have mostly round numbers at its max level.
            //This interpolates the stat between 100% (when it has 0 extra levels) and 150% (when it has the maximum amount of extra levels).
            float multiplier = 1f + (0.5f * (currentExtraLevel / (float)MaxExtraLevel));
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

        public override string ToString() {
            return $"HP: {HP}, EN: {EN}, CR: {CR}, AB: {AB}.";
        }
    }
    public class MutableCombatStats {
        public int HP;
        public int EN;
        public int CR;
        public int AB;

        public int maxHP;

        public MutableCombatStats(int HP, int EN, int CR, int AB) {
            this.HP = HP;
            this.EN = EN;
            this.CR = CR;
            this.AB = AB;

            maxHP = this.HP;
        }

        public override string ToString() {
            return $"HP: {HP}, EN: {EN}, CR: {CR}, AB: {AB}.";
        }
        /// <summary>
        /// Returns the amount of HP the Digimon is missing.
        /// </summary>
        public int GetMissingHP() => maxHP - HP;
        /// <summary>
        /// Tries to reduce HP in the necessary amoount to have this Digimon miss exactly that much HP. If the HP would be reduced below 1, it is set to 1.
        /// </summary>
        public void ApplyMissingHP(int amount) {
            HP = maxHP - amount;
            if (HP < 1) HP = 1;
        }

        /// <summary>
        /// Returns the damage of the attack, based solely on the index (0: energy, 1: crush, 2: ability).
        /// </summary>
        /// <param name="attackIndex">The index of the Attack.</param>
        /// <returns></returns>
        public int GetAttackDamage(int attackIndex) {
            switch (attackIndex) {
                case 0: return EN;
                case 1: return CR;
                case 2: return AB;
                default: return 0;
            }
        }
        /// <summary>
        /// Returns the type of the energy based on its power. This is useful to determine the energy Sprite that will be used.
        /// </summary>
        public int GetEnergyRank() {
            if (EN < 20) return 0;
            if (EN < 30) return 1;
            if (EN < 45) return 2;
            if (EN < 60) return 3;
            if (EN < 75) return 4;
            if (EN < 90) return 5;
            if (EN < 105) return 6;
            if (EN < 120) return 7;
            if (EN < 135) return 8;
            if (EN < 150) return 9;
            if (EN < 165) return 10;
            if (EN < 180) return 11;
            if (EN < 195) return 12;
            if (EN < 210) return 13;
            if (EN < 225) return 14;
            if (EN < 240) return 15;
            if (EN < 255) return 16;
            if (EN < 270) return 17;
            if (EN < 285) return 18;
            return 19;
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
        SpiritSmall,
        Black,
        White
    }
}