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
        CharSelection,
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
    public enum Reward {
        none,
        Empty,
        IncreaseDistance300,
        IncreaseDistance500,
        IncreaseDistance2000,
        ReduceDistance500,
        ReduceDistance1000,
        PunishDigimon,
        RewardDigimon,
        UnlockDigicodeOwned,
        UnlockDigicodeNotOwned,
        DataStorm,
        LoseSpiritPower10,
        LoseSpiritPower50,
        GainSpiritPower10,
        GainSpiritPowerMax,
        LevelDown,
        LevelUp,
        ForceLevelDown,
        ForceLevelUp,
        TriggerBattle
    }
}