using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class World {
        public readonly int number; //The number of the world. Note that the game will crash if two worlds have the same number.
        public readonly bool multiMap; //Whether the world has 1 or 4 maps.
        public readonly string worldSprite; //The sprite(s) used for this world.
        public readonly bool shuffle; //Whether the bosses are shuffled randomly on game creation.
        public readonly bool removePlayer; //Whether the initial Digimon of the player should be removed from the list of bosses, if able.
        public readonly bool lockTravel; //If true, the player can't travel between areas, areas will be traveled in order, and datastorms can't happen.
        public readonly Area[] areas;
        public readonly string[] bosses;
        public readonly BossMode bossMode;
        public readonly bool showEyes;
        public readonly SemibossMode semibossMode;
        public readonly string[][] semibosses; //An array of groups of semibosses (arrays of semibosses).

        public int AreaCount => areas.Length;
        public int? SemibossGroupsCount => semibosses?.Length;

        public World(int number, bool multiMap, string worldSprite, bool shuffle, bool removePlayer, bool lockTravel,
                Area[] areas, string[] bosses, BossMode bossMode, bool showEyes, SemibossMode semibossMode,
                string[][] semibosses)
        {
            this.number = number;
            this.multiMap = multiMap;
            this.worldSprite = worldSprite;
            this.shuffle = shuffle;
            this.removePlayer = removePlayer;
            this.lockTravel = lockTravel;
            this.areas = areas;
            this.bosses = bosses;
            this.bossMode = bossMode;
            this.showEyes = showEyes;
            this.semibossMode = semibossMode;
            this.semibosses = semibosses;
        }

        public int[] GetAreasInMap(int map) {
            List<int> areasInMap = new List<int>();
            foreach(Area a in areas) {
                if (a.map == map) areasInMap.Add(a.number);
            }
            return areasInMap.ToArray();
        }
    }

    public class Area {
        public readonly int number;
        public readonly int map;
        public readonly int distance;
        public readonly Vector2Int coords;

        public Area(int number, int map, int distance, Vector2Int coords) {
            this.number = number;
            this.map = map;
            this.distance = distance;
            this.coords = coords;
        }
    }

    /*public struct Coordinate {
        public int x, y;

        [JsonConstructor]
        public Coordinate(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }*/

    public enum BossMode {
        Evolve,
        UseBurst
    }

    public enum SemibossMode {
        none = 0,
        Fill,
        Gank,
        Pseudo,
        Deva
    }
}