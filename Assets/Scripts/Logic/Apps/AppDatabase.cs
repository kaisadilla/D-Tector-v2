using Kaisa.CircularTypes;
using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class AppDatabase : MonoBehaviour, IDigiviceApp {
        private enum ScreenDatabase {
            Menu,
            Menu_Spirit,
            Gallery,
        }

        [Header("UI Elements")]
        [SerializeField]
        private Image screenDisplay;

        private GameManager gm;
        private AudioManager audioMgr;
        private SpriteDatabase spriteDB;

        private ScreenDatabase currentScreen = ScreenDatabase.Menu;
        private int menuIndex = 0;
        //Gallery viewer
        private bool highlightDigimon = false;
        private List<string> galleryList = new List<string>(); //Stores the names of all Pokémon that must be shown in this gallery.
        private int galleryIndex = 0;
        //Hybrid gallery menu
        private int spiritMenuIndex = 0;
        //Data pages
        private int pageIndex = 0; //This is restricted to 0 or 1, or sometimes 2 when the player can see the code of the Digimon.

        //App Loader
        public static AppDatabase LoadApp(GameManager gm) {
            GameObject appGO = Instantiate(gm.pAppDatabase, gm.mainScreen.transform);
            AppDatabase appMap = appGO.GetComponent<AppDatabase>();
            appMap.Initialize(gm);
            return appMap;
        }

        //IDigiviceApp Methods:
        public void Dispose() {
            Destroy(gameObject);
        }
        public void Initialize(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            spriteDB = gm.spriteDB;
            StartApp();
        }
        public void InputA() {
            if (currentScreen == ScreenDatabase.Menu) {
                if(menuIndex < 6) {
                    if(gm.Database.OwnsDigimonInStage((Stage)menuIndex)) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
                else if (menuIndex == 6) {
                    audioMgr.PlayButtonA();
                    OpenSpiritMenu();
                }
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if (spiritMenuIndex < 10) {
                    if (gm.Database.OwnsSpiritDigimonOfElement((Element)spiritMenuIndex)) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
                else {
                    if (gm.Database.OwnsFusionSpiritDigimon()) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
            }
        }
        public void InputB() {
            if(currentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonB();
                CloseSpiritMenu();
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonB();
                CloseGallery();
            }
        }
        public void InputLeft() {
            if (currentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Left);
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Left);
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                if(galleryList.Count > 1) {
                    audioMgr.PlayButtonA();
                    NavigateGallery(Direction.Left);
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
        }
        public void InputRight() {
            if (currentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Right);
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Right);
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                if (galleryList.Count > 1) {
                    audioMgr.PlayButtonA();
                    NavigateGallery(Direction.Right);
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
        }

        //Specific methods:
        private void StartApp() {
            DrawScreen();
        }
        private void CloseApp() {
            gm.logicMgr.FinalizeApp(false);
        }

        private void DrawScreen() {
            StopAllCoroutines(); //This has to be called before the SpriteBuilder of the digimon being animated is destroyed.
            foreach (Transform child in screenDisplay.transform) {
                Destroy(child.gameObject);
            }

            if (currentScreen == ScreenDatabase.Menu) {
                screenDisplay.sprite = gm.spriteDB.database_sections[menuIndex];
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if(spiritMenuIndex < 10) {
                    screenDisplay.sprite = gm.spriteDB.elements[spiritMenuIndex];
                }
                else {
                    screenDisplay.sprite = gm.spriteDB.database_spirit_fusion;
                }
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                string displayDigimon = galleryList[galleryIndex];
                highlightDigimon = gm.Database.IsInDock(displayDigimon);
                screenDisplay.sprite = (highlightDigimon) ? gm.spriteDB.invertedArrows : gm.spriteDB.arrows;

                Sprite spriteRegular = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Default);
                Sprite spriteAlt = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Attack);

                if (highlightDigimon) {
                    spriteRegular = gm.spriteDB.GetInvertedSprite(spriteRegular);
                    spriteAlt = gm.spriteDB.GetInvertedSprite(spriteAlt);
                }

                SpriteBuilder sBuilder = gm.CreateSprite("DigimonDisplay", screenDisplay.transform, 24, 24, 4, 4, spriteRegular);

                StartCoroutine(AnimateSprite(sBuilder, spriteRegular, spriteAlt));
            }
        }

        private IEnumerator AnimateSprite(SpriteBuilder sBuilder, Sprite spriteRegular, Sprite spriteAlt) {
            while(true) {
                yield return new WaitForSeconds(2.5f);
                sBuilder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                sBuilder.SetSprite(spriteRegular);
                yield return new WaitForSeconds(0.4f);
                sBuilder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                sBuilder.SetSprite(spriteRegular);
            }
        }

        private void NavigateStageMenu(Direction dir) {
            if (dir == Direction.Left) menuIndex = menuIndex.CircularAdd(-1, 7);
            else menuIndex = menuIndex.CircularAdd(1, 7);
            DrawScreen();
        }

        private void OpenSpiritMenu() {
            spiritMenuIndex = 0;
            currentScreen = ScreenDatabase.Menu_Spirit;
            DrawScreen();
        }
        private void NavigateSpiritMenu(Direction dir) {
            if (dir == Direction.Left) spiritMenuIndex = spiritMenuIndex.CircularAdd(-1, 10);
            else spiritMenuIndex = spiritMenuIndex.CircularAdd(1, 10);
            DrawScreen();
        }
        private void CloseSpiritMenu() {
            currentScreen = ScreenDatabase.Menu;
            DrawScreen();
        }

        private void OpenGallery() {
            galleryList.Clear();
            //Regular galleries.
            if (menuIndex < 6) {
                foreach (Digimon d in gm.Database.DigimonDB) {
                    if ((int)d.stage == menuIndex && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                        galleryList.Add(d.name);
                    }
                }
            }
            //The gallery for Spirit-stage Digimon.
            else if (menuIndex == 6) {
                //If an element is chosen.
                if(spiritMenuIndex < 10) {
                    foreach (Digimon d in gm.Database.DigimonDB) {
                        if ((int)d.stage == menuIndex && (int)d.element == spiritMenuIndex && d.spiritType != SpiritType.Fusion && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
                //If fusion is chosen.
                if (spiritMenuIndex == 10) {
                    foreach (Digimon d in gm.Database.DigimonDB) {
                        if ((int)d.stage == menuIndex && d.spiritType == SpiritType.Fusion && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
            }

            galleryIndex = 0;
            currentScreen = ScreenDatabase.Gallery;
            DrawScreen();
        }

        private void NavigateGallery(Direction dir) {
            int maxIndex = galleryList.Count - 1;
            if (dir == Direction.Left) galleryIndex = galleryIndex.CircularAdd(-1, maxIndex);
            else galleryIndex = galleryIndex.CircularAdd(1, maxIndex);
            DrawScreen();
        }

        private void CloseGallery() {
            if (menuIndex < 6) {
                currentScreen = ScreenDatabase.Menu;
            }
            else if (menuIndex == 6) {
                currentScreen = ScreenDatabase.Menu_Spirit;
            }
            DrawScreen();
        }
    }
}