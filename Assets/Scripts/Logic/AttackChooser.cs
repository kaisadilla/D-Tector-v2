using Kaisa.Digivice;
using System;

namespace Kaisa.Digivice {
    public class AttackChooser {
        Random rng;
        MutableCombatStats digimonStats;

        public AttackChooser(int seed, string digimon, MutableCombatStats digimonStats) {
            int hash = digimon.GetHashCode();
            int specificSeed = seed * hash;
            rng = new Random(specificSeed);
            this.digimonStats = digimonStats;
        }

        /// <summary>
        /// Returns the next attack a Digimon will choose.
        /// </summary>
        public int Next() {
            int chanceEN = 30 + digimonStats.EN;
            int chanceCR = 30 + digimonStats.CR;
            int chanceAB = 30 + digimonStats.AB;

            int total = chanceEN + chanceCR + chanceAB;
            int rngNumber = rng.Next(total);

            if (rngNumber < chanceEN) return 0;
            else if (rngNumber < (chanceEN + chanceCR)) return 1;
            else return 2;
        }
    }
}