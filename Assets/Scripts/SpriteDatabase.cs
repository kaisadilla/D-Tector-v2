using Kaisa.Digivice;
using UnityEngine;

public class SpriteDatabase : MonoBehaviour {
    /* 0-3: idle
     * 4-5: walking
     * 6: happy
     * 7: sad
     * 8: event
     * 9: evolving
     */
    [Header("Characters")]
    public Sprite[] takuya = new Sprite[10];
    public Sprite[] koji = new Sprite[10];
    public Sprite[] zoe = new Sprite[10];
    public Sprite[] jp = new Sprite[10];
    public Sprite[] tommy = new Sprite[10];
    public Sprite[] koichi = new Sprite[10];
    [Header("Generic")]
    public Sprite emptySprite;
    public Sprite blackScreen;
    public Sprite blackBars;
    public Sprite arrows;
    public Sprite arrowsSmall;
    public Sprite invertedArrowsSmall;
    public Sprite[] rewardBackground = new Sprite[4];
    public Sprite[] rewards = new Sprite[5]; //0: Level
    [Header("Menus")]
    public Sprite[] mainMenu = new Sprite[7];
    [Header("Map")]
    public Sprite map_distanceScreen;
    public Sprite[] map0_sectors = new Sprite[4];
    public Sprite[] map0_areas = new Sprite[12];
    [Header("Status")]
    public Sprite status_distance;
    public Sprite status_level;
    public Sprite status_victories;
    public Sprite[] status_ddock = new Sprite[4];
    public Sprite status_ddockEmpty;
    [Header("Games")]
    public Sprite[] game_sections = new Sprite[3];
    public Sprite[] games_reward = new Sprite[3];
    public Sprite[] games_travel = new Sprite[4];
    public Sprite games_score;
    public Sprite games_distance;
    [Header("Database")]
    [Tooltip("The last element of this array is the \"search tab\" of the database")]
    public Sprite[] database_sections = new Sprite[8];
    public Sprite database_spirit_fusion;
    public Sprite[] elements = new Sprite[10];
    public Sprite[] elementNames = new Sprite[10];
    public Sprite[] database_pages = new Sprite[3];
    public Sprite[] database_ddocks = new Sprite[4];
    public Sprite[] database_searchOptions = new Sprite[3];
    [Header("Digits")]
    public Sprite digits_ok;
    public Sprite digits_error;
    [Header("Animations")]
    public Sprite givePower;
    public Sprite givePowerInverted;
    public Sprite giveMassivePower;
    public Sprite giveMassivePowerIverted;
    public Sprite acquireDigimon;
    public Sprite dTector;
    public Sprite animDistance;
    [Header("Battle")]
    public Sprite[] battle_mainMenu = new Sprite[4];
    public Sprite[] battle_combatMenu = new Sprite[5];
    public Sprite[] battle_attackMenu = new Sprite[3];
    [Header("Battle Particles")]
    public Sprite[] battle_energy = new Sprite[20];
    public Sprite battle_disobey;
    public Sprite battle_attackCollision; //The particles of the attack collision
    public Sprite battle_attackCollisionBig;
    public Sprite[] battle_explosion = new Sprite[2];
    public Sprite battle_callPoints_screen;
    public Sprite battle_callPoints_chooser;
    public Sprite[] battle_gainingSP = new Sprite[2];
    [Header("Game - SpeedRunner")]
    public Sprite speedRunner_rocket;
    public Sprite speedRunner_rocketExplosion;
    public Sprite speedRunner_rocketAsteroid;
    public Sprite speedRunner_rocketSpeedMark;
    public Sprite speedRunner_rocketFinish;

    public Sprite[] GetCharacterSprites(GameChar character) {
        switch(character) {
            case GameChar.Takuya: return takuya;
            case GameChar.Koji: return koji;
            case GameChar.Zoe: return zoe;
            case GameChar.JP: return jp;
            case GameChar.Tommy: return tommy;
            case GameChar.Koichi: return koichi;
        }
        return null;
    }

    /// <summary>
    /// Returns the sprite associated with the digimon and state given.
    /// </summary>
    /// <param name="name">The name of the digimon.</param>
    /// <param name="state">The state of the digimon.</param>
    public Sprite GetDigimonSprite(string name, SpriteAction state = SpriteAction.Default) {
        switch(state) {
            case SpriteAction.Default:
                return Resources.Load<Sprite>("Sprites/Digimon/" + name);
            case SpriteAction.Attack:
                return Resources.Load<Sprite>("Sprites/Digimon/" + name + "_at");
            case SpriteAction.Crush:
                return Resources.Load<Sprite>("Sprites/Digimon/" + name + "_cr");
            case SpriteAction.Spirit:
                return Resources.Load<Sprite>("Sprites/Digimon/" + name + "_sp");
            case SpriteAction.Black:
                return Resources.Load<Sprite>("Sprites/Digimon/" + name + "_bl");
            default:
                return null;
        }
    }
    public Sprite GetDigimonSprite(Digimon digimon, SpriteAction state = SpriteAction.Default) => GetDigimonSprite(digimon.name, state);
    /// <summary>
    /// Returns the sprite associated with the ability given.
    /// </summary>
    /// <param name="abilityName">The name of the ability – this is not the name of the digimon who has said ability.</param>
    public Sprite GetAbilitySprite(string abilityName) {
        return Resources.Load<Sprite>("Sprites/Abilities/" + abilityName);
    }

    public Sprite[] GetMapSectorSprites(int map) {
        if (map == 0) return map0_sectors;
        return null;
    }
    public Sprite[] GetMapAreaSprites(int map) {
        if (map == 0) return map0_areas;
        return null;
    }

    public Sprite GetInvertedSprite(Sprite sprite) {
        Texture2D texture = sprite.texture;
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        for(int x = 0; x < texture.width; x++) {
            for(int y = 0; y < texture.height; y++) {
                if(texture.GetPixel(x, y) == Color.black) {
                    newTexture.SetPixel(x, y, Color.clear);
                }
                else {
                    newTexture.SetPixel(x, y, Color.black);
                }
            }
        }

        newTexture.Apply();

        return Sprite.Create(newTexture, sprite.rect, sprite.pivot);
    }
}
