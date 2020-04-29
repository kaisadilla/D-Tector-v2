using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class Database : DigiviceApp {
        new protected static string appName = "database";

        private enum ScreenDatabase { //TODO: Replace with an int
            Menu,
            Menu_Spirit,
            Gallery,
            Pages,
            DDockList,
            DDockDisplay
        }
        //Current screen
        private ScreenDatabase _currentScreen = ScreenDatabase.Menu;
        private ScreenDatabase CurrentScreen {
            get => _currentScreen;
            set {
                _currentScreen = value;
                if(value == ScreenDatabase.Gallery) {
                    gm.SetTappingEnabled(Direction.Left, true);
                    gm.SetTappingEnabled(Direction.Right, true);
                }
                else {
                    gm.SetTappingEnabled(Direction.Left, false);
                    gm.SetTappingEnabled(Direction.Right, false);
                }
            }
        }
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

        //DDock list/display
        private int ddockIndex = 0;

        #region Input
        public override void InputA() {
            if (CurrentScreen == ScreenDatabase.Menu) {
                if (menuIndex < 6) {
                    if (gm.DatabaseMgr.OwnsDigimonInStage((Stage)menuIndex)) {
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
            else if (CurrentScreen == ScreenDatabase.Menu_Spirit) {
                if (spiritMenuIndex < 10) {
                    if (gm.DatabaseMgr.OwnsSpiritDigimonOfElement((Element)spiritMenuIndex)) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
                else {
                    if (gm.DatabaseMgr.OwnsFusionSpiritDigimon()) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
            }
            else if (CurrentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonA();
                OpenPages();
            }
            else if (CurrentScreen == ScreenDatabase.Pages) {
                //TODO: Let the player choose DDock for a Digimon already in a DDock, and make it just swap those DDocks.
                //Armor and Hybrid Digimon can't be put in DDocks.
                if (menuIndex >= 5 || digimonIsInDDock) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    OpenDDockList();
                }
            }
            else if (CurrentScreen == ScreenDatabase.DDockList) {
                audioMgr.PlayButtonA();
                OpenDDockDisplay();
            }
            else if (CurrentScreen == ScreenDatabase.DDockDisplay) {
                ChooseDDock();
            }
        }
        public override void InputB() {
            if (CurrentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (CurrentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonB();
                CloseSpiritMenu();
            }
            else if (CurrentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonB();
                CloseGallery();
            }
            else if (CurrentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonB();
                ClosePages();
            }
            else if (CurrentScreen == ScreenDatabase.DDockList) {
                audioMgr.PlayButtonB();
                CloseDDockList();
            }
            else if (CurrentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonB();
                CloseDDockDisplay();
            }
        }
        public override void InputLeft() {
            if (CurrentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Left);
            }
            else if (CurrentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Left);
            }
            else if (CurrentScreen == ScreenDatabase.Gallery) {
                if (galleryList.Count > 1) {
                    audioMgr.PlayButtonA();
                    NavigateGallery(Direction.Left);
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
            else if (CurrentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Left);
            }
            else if (CurrentScreen == ScreenDatabase.DDockList || CurrentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonA();
                NavigateDDock(Direction.Left);
            }
        }
        public override void InputRight() {
            if (CurrentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Right);
            }
            else if (CurrentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Right);
            }
            else if (CurrentScreen == ScreenDatabase.Gallery) {
                if (galleryList.Count > 1) {
                    audioMgr.PlayButtonA();
                    NavigateGallery(Direction.Right);
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
            else if (CurrentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Right);
            }
            else if (CurrentScreen == ScreenDatabase.DDockList || CurrentScreen == ScreenDatabase.DDockDisplay) {
                audioMgr.PlayButtonA();
                NavigateDDock(Direction.Right);
            }
        }
        #endregion

        protected override void StartApp() {
            DrawScreen();
        }

        private void DrawScreen() {
            //Stop all coroutines, except if the digimon name sign has a value and we are still in the 'Pages' screen.
            if(!(digimonNameSign != null && CurrentScreen == ScreenDatabase.Pages)) {
                StopAllCoroutines();
            }
            //Destroy all children, except the ones called 'NameSign' if we are in the 'Pages' screen.
            foreach (Transform child in screenDisplay.transform) {
                if (!(CurrentScreen == ScreenDatabase.Pages && child.name == "NameSign")) {
                    Destroy(child.gameObject);
                }
            }

            if (CurrentScreen == ScreenDatabase.Menu) {
                screenDisplay.sprite = gm.spriteDB.database_sections[menuIndex];
            }
            else if (CurrentScreen == ScreenDatabase.Menu_Spirit) {
                if (spiritMenuIndex < 10) {
                    screenDisplay.sprite = gm.spriteDB.elements[spiritMenuIndex];
                }
                else {
                    screenDisplay.sprite = gm.spriteDB.database_spirit_fusion;
                }
            }
            else if (CurrentScreen == ScreenDatabase.Gallery) {
                string displayDigimon = galleryList[galleryIndex];
                digimonIsInDDock = gm.DatabaseMgr.IsInDock(displayDigimon);
                screenDisplay.sprite = (digimonIsInDDock) ? gm.spriteDB.invertedArrowsSmall : gm.spriteDB.arrowsSmall;

                Sprite spriteRegular = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Default);
                Sprite spriteAlt = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Attack);

                if (digimonIsInDDock) {
                    spriteRegular = gm.spriteDB.GetInvertedSprite(spriteRegular);
                    spriteAlt = gm.spriteDB.GetInvertedSprite(spriteAlt);
                }

                SpriteBuilder builder = gm.BuildSprite("DigimonDisplay", screenDisplay.transform, 24, 24, 4, 4, spriteRegular);

                StartCoroutine(AnimateSprite(builder, spriteRegular, spriteAlt));
            }
            else if (CurrentScreen == ScreenDatabase.Pages) {
                if(digimonNameSign == null) {
                    string name = string.Format("{0:000} {1}", pageDigimon.number, pageDigimon.name);
                    TextBoxBuilder nameBuilder = gm.BuildTextBox("NameSign", screenDisplay.transform, name, DFont.Big, 32, 7, 32, 0);
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
                    gm.BuildTextBox("Level", screenDisplay.transform, level, DFont.Regular, 15, 5, 16, 9, TextAnchor.UpperRight);
                    gm.BuildTextBox("HP", screenDisplay.transform, stats.HP.ToString(), DFont.Regular, 15, 5, 16, 17, TextAnchor.UpperRight);
                    gm.BuildSprite("Element", screenDisplay.transform, 30, 5, 1, 25, gm.spriteDB.elementNames[element]);
                }
                else if (pageIndex == 1) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[1];
                    gm.BuildTextBox("Energy", screenDisplay.transform, stats.EN.ToString(), DFont.Regular, 15, 5, 16, 9, TextAnchor.UpperRight);
                    gm.BuildTextBox("Crush", screenDisplay.transform, stats.CR.ToString(), DFont.Regular, 15, 5, 16, 17, TextAnchor.UpperRight);
                    gm.BuildTextBox("Ability", screenDisplay.transform, stats.AB.ToString(), DFont.Regular, 15, 5, 16, 25, TextAnchor.UpperRight);
                }
                else if (pageIndex == 2) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[2];
                    gm.DatabaseMgr.TryGetCodeOfDigimon(pageDigimon.name, out string code);
                    gm.BuildTextBox("Code", screenDisplay.transform, code, DFont.Big, 30, 8, 2, 23, TextAnchor.UpperRight);
                }
            }
            else if (CurrentScreen == ScreenDatabase.DDockList) {
                screenDisplay.sprite = gm.spriteDB.database_ddocks[ddockIndex];
            }
            else if (CurrentScreen == ScreenDatabase.DDockDisplay) {
                screenDisplay.sprite = gm.spriteDB.status_ddock[ddockIndex];
                gm.BuildDDockSprite(ddockIndex, screenDisplay.transform);
            }
        }

        private void NavigateStageMenu(Direction dir) {
            if (dir == Direction.Left) menuIndex = menuIndex.CircularAdd(-1, 7);
            else menuIndex = menuIndex.CircularAdd(1, 7);
            DrawScreen();
        }

        private void OpenSpiritMenu() {
            spiritMenuIndex = 0;
            CurrentScreen = ScreenDatabase.Menu_Spirit;
            DrawScreen();
        }
        private void NavigateSpiritMenu(Direction dir) {
            if (dir == Direction.Left) spiritMenuIndex = spiritMenuIndex.CircularAdd(-1, 10);
            else spiritMenuIndex = spiritMenuIndex.CircularAdd(1, 10);
            DrawScreen();
        }
        private void CloseSpiritMenu() {
            CurrentScreen = ScreenDatabase.Menu;
            DrawScreen();
        }

        private void OpenGallery() {
            galleryList.Clear();
            //Regular galleries.
            if (menuIndex < 6) {
                foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                    if ((int)d.stage == menuIndex && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                        galleryList.Add(d.name);
                    }
                }
            }
            //The gallery for Spirit-stage Digimon.
            else if (menuIndex == 6) {
                //If an element is chosen.
                if(spiritMenuIndex < 10) {
                    foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                        if ((int)d.stage == menuIndex && (int)d.element == spiritMenuIndex && d.spiritType != SpiritType.Fusion && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
                //If fusion is chosen.
                if (spiritMenuIndex == 10) {
                    foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                        if ((int)d.stage == menuIndex && d.spiritType == SpiritType.Fusion && gm.LoadedGame.IsDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
            }

            galleryIndex = 0;
            CurrentScreen = ScreenDatabase.Gallery;
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
                CurrentScreen = ScreenDatabase.Menu;
            }
            else if (menuIndex == 6) {
                CurrentScreen = ScreenDatabase.Menu_Spirit;
            }
            DrawScreen();
        }

        private void OpenPages() {
            CurrentScreen = ScreenDatabase.Pages;
            pageIndex = 0;

            string displayDigimon = galleryList[galleryIndex];
            pageDigimon = gm.DatabaseMgr.GetDigimon(displayDigimon);

            DrawScreen();
        }

        private void NavigatePages(Direction dir) {
            int upperBound = (gm.DatabaseMgr.IsDigimonCodeUnlocked(pageDigimon.name)) ? 2 : 1;

            if (dir == Direction.Left) pageIndex = pageIndex.CircularAdd(-1, upperBound);
            else pageIndex = pageIndex.CircularAdd(1, upperBound);

            DrawScreen();
        }

        private void ClosePages() {
            CurrentScreen = ScreenDatabase.Gallery;
            DrawScreen();
        }

        private void OpenDDockList() {
            CurrentScreen = ScreenDatabase.DDockList;
            ddockIndex = 0;

            DrawScreen();
        }

        private void NavigateDDock(Direction dir) {
            if (dir == Direction.Left) ddockIndex = ddockIndex.CircularAdd(-1, 3);
            else ddockIndex = ddockIndex.CircularAdd(1, 3);

            DrawScreen();
        }

        private void CloseDDockList() {
            CurrentScreen = ScreenDatabase.Pages;
            DrawScreen();
        }

        private void OpenDDockDisplay() {
            CurrentScreen = ScreenDatabase.DDockDisplay;
            DrawScreen();
        }

        private void CloseDDockDisplay() {
            CurrentScreen = ScreenDatabase.DDockList;
            DrawScreen();
        }
        private void ChooseDDock() {
            gm.PlayAnimation(gm.screenMgr.ASwapDDock(ddockIndex, pageDigimon.name));
            gm.DatabaseMgr.SetDDockDigimon(ddockIndex, pageDigimon.name);
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
        #endregion
    }
}