using Kaisa.CircularTypes;
using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class AppDatabase : MonoBehaviour, IDigiviceApp {
        private enum ScreenDatabase {
            Menu,
            Menu_Spirit,
            Gallery,
            Pages,
            DDockList,
            DDockDisplay
        }

        [Header("UI Elements")]
        [SerializeField]
        private Image screenDisplay;

        private GameManager gm;
        private AudioManager audioMgr;

        private ScreenDatabase currentScreen = ScreenDatabase.Menu;
        private int menuIndex = 0;
        //Gallery viewer
        private bool digimonIsInDDock = false;
        private List<string> galleryList = new List<string>(); //Stores the names of all Pokémon that must be shown in this gallery.
        private int galleryIndex = 0;
        //Hybrid gallery menu
        private int spiritMenuIndex = 0;
        //Data pages
        private int pageIndex = 0; //This is restricted to 0 or 1, or sometimes 2 when the player can see the code of the Digimon.
        private Digimon pageDigimon;
        private GameObject digimonNameSign;
        //DDock list / display
        private int ddockIndex = 0;

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
            StartApp();
        }
        public void InputA() {
            if (currentScreen == ScreenDatabase.Menu) {
                if (menuIndex < 6) {
                    if (gm.Database.OwnsDigimonInStage((Stage)menuIndex)) {
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
            else if (currentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonA();
                OpenPages();
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                //TODO: Let the player choose DDock for a Digimon already in a DDock, and make it just swap those DDocks.
                //Armor and Hybrid Digimon can't be put in DDocks.
                if(menuIndex >= 5 || digimonIsInDDock) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    OpenDDockList();
                }
            }
            else if (currentScreen == ScreenDatabase.DDockList) {
                audioMgr.PlayButtonA();
                OpenDDockDisplay();
            }
            else if (currentScreen == ScreenDatabase.DDockDisplay) {
                ChooseDDock();
            }
        }
        public void InputB() {
            if (currentScreen == ScreenDatabase.Menu) {
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
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonB();
                ClosePages();
            }
            else if (currentScreen == ScreenDatabase.DDockList) {
                audioMgr.PlayButtonB();
                CloseDDockList();
            }
            else if (currentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonB();
                CloseDDockDisplay();
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
                if (galleryList.Count > 1) {
                    audioMgr.PlayButtonA();
                    NavigateGallery(Direction.Left);
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Left);
            }
            else if (currentScreen == ScreenDatabase.DDockList || currentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonA();
                NavigateDDock(Direction.Left);
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
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Right);
            }
            else if (currentScreen == ScreenDatabase.DDockList || currentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonA();
                NavigateDDock(Direction.Right);
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
            //Stop all coroutines, except if the digimon name sign has a value and we are still in the 'Pages' screen.
            if(!(digimonNameSign != null && currentScreen == ScreenDatabase.Pages)) {
                StopAllCoroutines();
            }
            //Destroy all children, except the ones called 'NameSign' if we are in the 'Pages' screen.
            foreach (Transform child in screenDisplay.transform) {
                if (!(currentScreen == ScreenDatabase.Pages && child.name == "NameSign")) {
                    Destroy(child.gameObject);
                }
            }

            if (currentScreen == ScreenDatabase.Menu) {
                screenDisplay.sprite = gm.spriteDB.database_sections[menuIndex];
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if (spiritMenuIndex < 10) {
                    screenDisplay.sprite = gm.spriteDB.elements[spiritMenuIndex];
                }
                else {
                    screenDisplay.sprite = gm.spriteDB.database_spirit_fusion;
                }
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                string displayDigimon = galleryList[galleryIndex];
                digimonIsInDDock = gm.Database.IsInDock(displayDigimon);
                screenDisplay.sprite = (digimonIsInDDock) ? gm.spriteDB.invertedArrowsSmall : gm.spriteDB.arrowsSmall;

                Sprite spriteRegular = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Default);
                Sprite spriteAlt = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Attack);

                if (digimonIsInDDock) {
                    spriteRegular = gm.spriteDB.GetInvertedSprite(spriteRegular);
                    spriteAlt = gm.spriteDB.GetInvertedSprite(spriteAlt);
                }

                SpriteBuilder builder = gm.CreateSprite("DigimonDisplay", screenDisplay.transform, 24, 24, 4, 4, spriteRegular);

                StartCoroutine(AnimateSprite(builder, spriteRegular, spriteAlt));
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                if(digimonNameSign == null) {
                    string name = string.Format("{0:000} {1}", pageDigimon.number, pageDigimon.name);
                    TextBoxBuilder nameBuilder = gm.CreateTextBox("NameSign", screenDisplay.transform, name, DFont.Big, 32, 7, 32, 0);
                    nameBuilder.SetFitSizeToContent(true);
                    digimonNameSign = nameBuilder.gameObject;
                    StartCoroutine(AnimateName(nameBuilder));
                }

                int playerLevel = gm.LoadedGame.PlayerLevel;
                int digimonExtraLevel = gm.LoadedGame.GetDigimonLevel(pageDigimon.name);

                string level = pageDigimon.GetFriendlyLevel(digimonExtraLevel, playerLevel).ToString();
                CombatStats stats = pageDigimon.GetFriendlyStats(playerLevel, digimonExtraLevel);
                int element = (int)pageDigimon.element;

                if (pageIndex == 0) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[0];
                    gm.CreateTextBox("Level", screenDisplay.transform, level, DFont.Regular, 15, 5, 16, 9, TextAnchor.UpperRight);
                    gm.CreateTextBox("HP", screenDisplay.transform, stats.HP.ToString(), DFont.Regular, 15, 5, 16, 17, TextAnchor.UpperRight);
                    gm.CreateSprite("Element", screenDisplay.transform, 30, 5, -1, 25, gm.spriteDB.elementNames[element]);
                }
                else if (pageIndex == 1) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[1];
                    gm.CreateTextBox("Energy", screenDisplay.transform, stats.EN.ToString(), DFont.Regular, 15, 5, 16, 9, TextAnchor.UpperRight);
                    gm.CreateTextBox("Crush", screenDisplay.transform, stats.CR.ToString(), DFont.Regular, 15, 5, 16, 17, TextAnchor.UpperRight);
                    gm.CreateTextBox("Ability", screenDisplay.transform, stats.AB.ToString(), DFont.Regular, 15, 5, 16, 25, TextAnchor.UpperRight);
                }
                else if (pageIndex == 2) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[2];
                    gm.Database.TryGetCodeOfDigimon(pageDigimon.name, out string code);
                    gm.CreateTextBox("Code", screenDisplay.transform, code, DFont.Big, 30, 8, 2, 23, TextAnchor.UpperRight);
                }
            }
            else if (currentScreen == ScreenDatabase.DDockList) {
                screenDisplay.sprite = gm.spriteDB.database_ddocks[ddockIndex];
            }
            else if (currentScreen == ScreenDatabase.DDockDisplay) {
                screenDisplay.sprite = gm.spriteDB.status_ddock[ddockIndex];
                gm.CreateSpriteForDDock(ddockIndex, screenDisplay.transform);
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
                foreach (Digimon d in gm.Database.Digimons) {
                    if ((int)d.stage == menuIndex && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                        galleryList.Add(d.name);
                    }
                }
            }
            //The gallery for Spirit-stage Digimon.
            else if (menuIndex == 6) {
                //If an element is chosen.
                if(spiritMenuIndex < 10) {
                    foreach (Digimon d in gm.Database.Digimons) {
                        if ((int)d.stage == menuIndex && (int)d.element == spiritMenuIndex && d.spiritType != SpiritType.Fusion && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
                //If fusion is chosen.
                if (spiritMenuIndex == 10) {
                    foreach (Digimon d in gm.Database.Digimons) {
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

        private void OpenPages() {
            currentScreen = ScreenDatabase.Pages;
            pageIndex = 0;

            string displayDigimon = galleryList[galleryIndex];
            pageDigimon = gm.Database.GetDigimon(displayDigimon);

            DrawScreen();
        }

        private void NavigatePages(Direction dir) {
            int upperBound = (gm.Database.IsDigimonCodeUnlocked(pageDigimon.name)) ? 2 : 1;

            if (dir == Direction.Left) pageIndex = pageIndex.CircularAdd(-1, upperBound);
            else pageIndex = pageIndex.CircularAdd(1, upperBound);

            DrawScreen();
        }

        private void ClosePages() {
            currentScreen = ScreenDatabase.Gallery;
            DrawScreen();
        }

        private void OpenDDockList() {
            currentScreen = ScreenDatabase.DDockList;
            ddockIndex = 0;

            DrawScreen();
        }

        private void NavigateDDock(Direction dir) {
            if (dir == Direction.Left) ddockIndex = ddockIndex.CircularAdd(-1, 3);
            else ddockIndex = ddockIndex.CircularAdd(1, 3);

            DrawScreen();
        }

        private void CloseDDockList() {
            currentScreen = ScreenDatabase.Pages;
            DrawScreen();
        }

        private void OpenDDockDisplay() {
            currentScreen = ScreenDatabase.DDockDisplay;
            DrawScreen();
        }

        private void CloseDDockDisplay() {
            currentScreen = ScreenDatabase.DDockList;
            DrawScreen();
        }
        private void ChooseDDock() {
            gm.PlayAnimation(SwapDDock(ddockIndex, pageDigimon.name));
            gm.Database.SetDDockDigimon(ddockIndex, pageDigimon.name);
            CloseDDockDisplay();
            CloseDDockList();
            ClosePages();
            CloseGallery();
            DrawScreen();
        }

        #region Animations
        private IEnumerator AnimateSprite(SpriteBuilder builder, Sprite spriteRegular, Sprite spriteAlt) {
            while (true) {
                yield return new WaitForSeconds(2.5f);
                builder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteRegular);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteRegular);
            }
        }

        private IEnumerator AnimateName(TextBoxBuilder builder) {
            yield return null; //Wait for the next frame so the builder's text fitter has adjusted the component's Width.
            int goWidth = builder.Width;

            while (true) {
                builder.SetPosition(32, 0);
                for (int i = 0; i < goWidth + 32; i++) {
                    builder.MoveSprite(Direction.Left);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }

        private IEnumerator SwapDDock(int ddock, string newDigimon) {
            gm.LockInput();

            float animDuration = 1.5f;
            Sprite newDigimonSprite = gm.spriteDB.GetDigimonSprite(newDigimon);
            Sprite newDigimonSpriteCr = gm.spriteDB.GetDigimonSprite(newDigimon, SpriteAction.Crush);

            audioMgr.PlaySound(audioMgr.changeDock);

            SpriteBuilder bBlackBars = gm.CreateSprite("BlackBars", gm.MainScreenTransform, sprite: gm.spriteDB.blackBars);
            bBlackBars.PlaceOutside(Direction.Down);
            SpriteBuilder bDDock = gm.CreateSprite("DDock", gm.MainScreenTransform, sprite: gm.spriteDB.status_ddock[ddock]);
            SpriteBuilder bDDockSprite = gm.CreateSpriteForDDock(ddock, bDDock.transform);

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Up);
                bDDock.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            bDDockSprite.SetSprite(newDigimonSprite);
            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Down);
                bDDock.MoveSprite(Direction.Down);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(audioMgr.PlaySoundAfterDelay(audioMgr.charHappy, 0.175f));
            for(int i = 0; i < 5; i++) {
                bDDockSprite.SetSprite(null);
                yield return new WaitForSeconds(0.175f);
                bDDockSprite.SetSprite(newDigimonSpriteCr);
                yield return new WaitForSeconds(0.175f);
            }

            yield return new WaitForSeconds(0.5f);

            bBlackBars.Dispose();
            bDDock.Dispose();

            gm.UnlockInput();
        }
        #endregion
    }
}