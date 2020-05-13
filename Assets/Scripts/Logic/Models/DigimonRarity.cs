namespace Kaisa.Digivice {
    public class DigimonRarity {
        public readonly string digimon;
        public Rarity Rarity { get; private set; }
        public readonly bool exclusive;

        public DigimonRarity(string digimon, Rarity rarity, bool exclusive) {
            this.digimon = digimon;
            Rarity = rarity;
            this.exclusive = exclusive;
        }

        public void OverrideRarity(Rarity rarity) {
            Rarity = rarity;
        }

        public bool EligibleForBattle {
            get {
                if (Rarity == Rarity.Common
                    || Rarity == Rarity.Rare
                    || Rarity == Rarity.Epic
                    || Rarity == Rarity.Legendary) {
                    return true;
                }
                return false;
            }
        }
    }
}