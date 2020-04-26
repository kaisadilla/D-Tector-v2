using Kaisa.Digivice;
using System.Collections;
using System.Collections.Generic;
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
    public Sprite[] game_sections = new Sprite[2];
    public Sprite[] games_reward = new Sprite[4];
    public Sprite[] games_travel = new Sprite[4];
    [Header("Database")]
    [Tooltip("The last element of this array is the \"search tab\" of the database")]
    public Sprite[] database_sections = new Sprite[8];
    public Sprite database_spirit_fusion;
    public Sprite[] elements = new Sprite[10];
    public Sprite arrows;
    public Sprite invertedArrows;
    public Sprite[] database_pages = new Sprite[3];
    public Sprite[] database_ddocks = new Sprite[4];

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
                if(texture.GetPixel(x, y) == new Color(0, 0, 0, 1)) {
                    newTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
                else {
                    newTexture.SetPixel(x, y, new Color(0, 0, 0, 1));
                }
            }
        }

        newTexture.Apply();

        return Sprite.Create(newTexture, sprite.rect, sprite.pivot);
    }
}
