using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public enum Direction {
        Left,
        Right,
        Up,
        Down,
        none
    }

    public enum Screen {
        Character,
        MainMenu,
        App,
        GamesMenu,
        GamesRewardMenu,
        GamesTravelMenu
    }
    public enum MainMenu {
        Map,
        Status,
        Game,
        Database,
        Digits,
        Camp,
        Connect
    }
    public enum GameMenu {
        Reward,
        Travel
    }
    public enum GameRewardMenu {
        FindBattle,
        JackpotBox,
        EnergyWars,
        DigiCatch
    }
    public enum GameTravelMenu {
        SpeedRunner,
        Asteroids,
        DigiHunter,
        Maze
    }
}