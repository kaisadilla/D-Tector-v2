using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        Coroutine screenAnimation;
        //Current screen
        private ScreenDatabase currentScreen = ScreenDatabase.Menu;
        private int menuIndex = 0;

        //Gallery viewer
        private bool digimonIsInDDock = false;
        private List<string> galleryList = new List<string>(); //Stores the names of all Pokémon that must be shown in this gallery.
        private int galleryIndex = 0;

        //Hybrid gallery menu
        private List<int> availableElements;
        private int elementIndex = 0; //This points which int in availableElements is used as the current element selected.
        private int SelectedElement => availableElements[elementIndex];

        //Data pages
        private int pageIndex = 0; //This is restricted to 0 or 1, or sometimes 2 when the player can see the code of the Digimon.
        private Digimon pageDigimon;
        private GameObject digimonNameSign;

        //DDock list/display
        private int ddockIndex = 0;

        #region Input
        public override void InputA() {
            if (currentScreen == ScreenDatabase.Menu) {
                galleryList = gm.GetAllUnlockedDigimonInStage((Stage)menuIndex);
                if (galleryList.Count > 0) {
                    audioMgr.PlayButtonA();
                    if (menuIndex < 6) {
                        OpenGallery();
                    }
                    else if (menuIndex == 6) {
                        OpenSpiritMenu();
                    }
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if (SelectedElement < 10) {
                    galleryList = gm.GetAllUnlockedSpiritsOfElement((Element)SelectedElement);
                    if (galleryList.Count > 0) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
                else {
                    galleryList = gm.GetAllUnlockedFusionDigimon();
                    if (galleryList.Count > 0) {
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
                if (menuIndex >= 5 || digimonIsInDDock) {
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
        public override void InputB() {
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
        public override void InputLeft() {
            if (currentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Left);
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Left);
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
        public override void InputRight() {
            if (currentScreen == ScreenDatabase.Menu) {
                audioMgr.PlayButtonA();
                NavigateStageMenu(Direction.Right);
            }
            else if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Right);
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
        public override void InputLeftDown() {
            if(currentScreen == ScreenDatabase.Gallery) {
                if(galleryList.Count <= 1) {
                    audioMgr.PlayButtonB();
                }
                else {
                    StartNavigation(Direction.Left);
                }
            }
        }
        public override void InputRightDown() {
            if (currentScreen == ScreenDatabase.Gallery) {
                if (galleryList.Count <= 1) {
                    audioMgr.PlayButtonB();
                }
                else {
                    StartNavigation(Direction.Right);
                }
            }
        }
        public override void InputLeftUp() {
            StopNavigation();
        }
        public override void InputRightUp() {
            StopNavigation();
        }
        protected override IEnumerator AutoNavigateDir(Direction dir) {
            audioMgr.PlayButtonA();
            NavigateGallery(dir);
            yield return new WaitForSeconds(0.35f);
            while (true) {
                yield return new WaitForSeconds(0.15f);
                audioMgr.PlayButtonA();
                NavigateGallery(dir);
            }
        }
        #endregion

        protected override void StartApp() {
            DrawScreen();
        }

        private void DrawScreen() {
            //Stop all coroutines, except if the digimon name sign has a value and we are still in the 'Pages' screen.
            if(!(digimonNameSign != null && currentScreen == ScreenDatabase.Pages)) {
                if (screenAnimation != null) StopCoroutine(screenAnimation);
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
                if (SelectedElement < 10) {
                    screenDisplay.sprite = gm.spriteDB.elements[SelectedElement];
                }
                else {
                    screenDisplay.sprite = gm.spriteDB.database_spirit_fusion;
                }
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                string displayDigimon = galleryList[galleryIndex];
                digimonIsInDDock = gm.IsInDock(displayDigimon);
                screenDisplay.sprite = (digimonIsInDDock) ? gm.spriteDB.invertedArrowsSmall : gm.spriteDB.arrowsSmall;

                Sprite spriteRegular = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Default);
                Sprite spriteAlt = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Attack);

                if (digimonIsInDDock) {
                    spriteRegular = gm.spriteDB.GetInvertedSprite(spriteRegular);
                    spriteAlt = gm.spriteDB.GetInvertedSprite(spriteAlt);
                }

                SpriteBuilder builder = ScreenElement.BuildSprite("DigimonDisplay", screenDisplay.transform).SetSize(24, 24).Center().SetSprite(spriteRegular);

                screenAnimation = StartCoroutine(AnimateSprite(builder, spriteRegular, spriteAlt));
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                if(digimonNameSign == null) {
                    string name = string.Format("{0:000} {1}", pageDigimon.number, pageDigimon.name);
                    TextBoxBuilder nameBuilder = ScreenElement.BuildTextBox("NameSign", screenDisplay.transform, DFont.Big).SetText(name).SetSize(32, 7).SetPosition(32, 0);
                    nameBuilder.SetFitSizeToContent(true);
                    digimonNameSign = nameBuilder.gameObject;
                    screenAnimation = StartCoroutine(AnimateName(nameBuilder));
                }

                int playerLevel = gm.logicMgr.GetPlayerLevel();
                int digimonExtraLevel = gm.logicMgr.GetDigimonExtraLevel(pageDigimon.name);
                int realLevel;

                MutableCombatStats stats;
                //If the Digimon is Spirit- or Armor-Stage.
                if (menuIndex == 5 || menuIndex == 6) {
                    realLevel = pageDigimon.GetBossLevel(playerLevel);
                    stats = pageDigimon.GetBossStats(playerLevel);
                }
                else {
                    realLevel = pageDigimon.GetFriendlyLevel(digimonExtraLevel);
                    stats = pageDigimon.GetFriendlyStats(digimonExtraLevel);
                } 


                int element = (int)pageDigimon.element;

                if (pageIndex == 0) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[0];
                    ScreenElement.BuildTextBox("Level", screenDisplay.transform, DFont.Regular)
                        .SetText(realLevel.ToString()).SetSize(15, 5).SetPosition(16, 9).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("HP", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.HP.ToString()).SetSize(15, 5).SetPosition(16, 17).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildSprite("Element", screenDisplay.transform).SetSize(30, 5).SetPosition(1, 25).SetSprite(gm.spriteDB.elementNames[element]);
                }
                else if (pageIndex == 1) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[1];
                    ScreenElement.BuildTextBox("Energy", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.EN.ToString()).SetSize(15, 5).SetPosition(16, 9).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("Crush", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.CR.ToString()).SetSize(15, 5).SetPosition(16, 17).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("Ability", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.AB.ToString()).SetSize(15, 5).SetPosition(16, 25).SetAlignment(TextAnchor.UpperRight);
                }
                else if (pageIndex == 2) {
                    screenDisplay.sprite = gm.spriteDB.database_pages[2];
                    gm.DatabaseMgr.TryGetCodeOfDigimon(pageDigimon.name, out string code);
                    ScreenElement.BuildTextBox("Code", screenDisplay.transform, DFont.Big)
                        .SetText(code).SetSize(30, 8).SetPosition(2, 23).SetAlignment(TextAnchor.UpperRight);
                }
            }
            else if (currentScreen == ScreenDatabase.DDockList) {
                screenDisplay.sprite = gm.spriteDB.database_ddocks[ddockIndex];
            }
            else if (currentScreen == ScreenDatabase.DDockDisplay) {
                screenDisplay.sprite = gm.spriteDB.status_ddock[ddockIndex];
                gm.GetDDockScreenElement(ddockIndex, screenDisplay.transform);
            }
        }

        private void NavigateStageMenu(Direction dir) {
            if (dir == Direction.Left) menuIndex = menuIndex.CircularAdd(-1, 7);
            else menuIndex = menuIndex.CircularAdd(1, 7);
            DrawScreen();
        }

        private void OpenSpiritMenu() {
            availableElements = new List<int>();

            HashSet<int> elementsFound = new HashSet<int>(); //a list of elements found that will contain only 1 of each.

            foreach(string d in galleryList) {
                elementsFound.Add((int)gm.DatabaseMgr.GetDigimon(d).element);
            }
            if(gm.GetAllUnlockedFusionDigimon().Count > 0) {
                elementsFound.Add(10);
            }

            availableElements = elementsFound.ToList();
            availableElements.Sort();

            elementIndex = 0;
            currentScreen = ScreenDatabase.Menu_Spirit;
            DrawScreen();
        }
        private void NavigateSpiritMenu(Direction dir) {
            if (dir == Direction.Left) elementIndex = elementIndex.CircularAdd(-1, availableElements.Count - 1);
            else elementIndex = elementIndex.CircularAdd(1, availableElements.Count - 1);
            DrawScreen();
        }
        private void CloseSpiritMenu() {
            currentScreen = ScreenDatabase.Menu;
            DrawScreen();
        }

        private void OpenGallery() {
            if (menuIndex == 6) {
                galleryList.Clear();
                //If an element is chosen.
                if(elementIndex < 10) {
                    foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                        if ((int)d.stage == menuIndex && (int)d.element == elementIndex && d.spiritType != SpiritType.Fusion && gm.logicMgr.GetDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
                //If fusion is chosen.
                if (elementIndex == 10) {
                    foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                        if ((int)d.stage == menuIndex && d.spiritType == SpiritType.Fusion && gm.logicMgr.GetDigimonUnlocked(d.name)) {
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
            pageDigimon = gm.DatabaseMgr.GetDigimon(displayDigimon);

            DrawScreen();
        }

        private void NavigatePages(Direction dir) {
            int upperBound = (gm.logicMgr.GetDigimonCodeUnlocked(pageDigimon.name)) ? 2 : 1;

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
            gm.EnqueueAnimation(gm.screenMgr.ASwapDDock(ddockIndex, pageDigimon.name));
            gm.logicMgr.SetDDockDigimon(ddockIndex, pageDigimon.name);
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
                    builder.Move(Direction.Left);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        #endregion
    }
}