using Kaisa.Digivice.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    /*IMPORTANT: Texture2D's origin (coords 0, 0) is in the lower left corner, but RectTransform's position origin is in the upper left corner.
     * Maze cells call "up" to the cell with a lower Y coordinate than them, but the Maze is rendered using the same origin as Texture2D.
     * This means that moving "up" in the maze is seen as moving "down" in the screen, and vice-versa.
     */
    public class Maze : DigiviceApp {
        new protected static string appName = "maze";

        //Current screen
        private int currentScreen = 0; //0: Start/cancel menu, 1: maze, 2: defeat, 3: victory
        private TextBoxBuilder[] tbOptions = new TextBoxBuilder[2];
        private int currentOption = 0;

        //Maze generation
        private const int MAZE_WIDTH = 10;
        private const int MAZE_HEIGHT = 8;
        private const int PATH_WIDTH = 2;

        private const int CELL_PATH_UP    = 0b00001; //0x01
        private const int CELL_PATH_LEFT  = 0b00010; //0x02;
        private const int CELL_PATH_DOWN  = 0b00100; //0x04;
        private const int CELL_PATH_RIGHT = 0b01000; //0x08;
        private const int CELL_VISITED    = 0b10000; //0x10, 0x20, 0x40, 0x80, 0x100, 0x200...;

        private int[] cellPaths = new int[MAZE_WIDTH * MAZE_HEIGHT]; //Stores the paths for each cell, and whether it's visited.
        private int visitedCells = 0;
        Stack<(int x, int y)> stack = new Stack<(int x, int y)>();

        //Player control
        private (int x, int y) playerPos = (-1, 0);
        private RectangleBuilder playerMarker;
        private int timeRemaining = 30; //After defeat, the time remaining must reach -1 for the player to close the app.
        private TextBoxBuilder tbTime;

        #region Input
        public override void InputA() {
            if(currentScreen == 0) {
                if (currentOption == 0) {
                    StartGame();
                }
                else if (currentOption == 1) {
                    audioMgr.PlayButtonA();
                    CloseApp();
                }
            }
            else if (currentScreen == 1) {
                if (!MovePlayer(Direction.Up)) {
                    audioMgr.PlayButtonB();
                }
            }
            else if (currentScreen == 2 && timeRemaining == -1) {
                audioMgr.PlayButtonA();
                CloseApp();
            }
            else if (currentScreen == 3) {
                audioMgr.PlayButtonA();
                gm.SubmitGameScore(CalculateScore());
                CloseApp();
            }
        }
        public override void InputB() {
            if(currentScreen == 0) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (currentScreen == 1) {
                if (!MovePlayer(Direction.Down)) {
                    audioMgr.PlayButtonB();
                }
            }
            else if (currentScreen == 2 && timeRemaining == -1) {
                audioMgr.PlayButtonA();
                CloseApp();
            }
            else if (currentScreen == 3) {
                audioMgr.PlayButtonA();
                gm.SubmitGameScore(CalculateScore());
                CloseApp();
            }
        }
        public override void InputLeft() {
            if(currentScreen == 0) {
                audioMgr.PlayButtonA();
                currentOption = (currentOption == 0) ? 1 : 0;
                HighlightSelection();
            }
            else if (currentScreen == 1) {
                if (!MovePlayer(Direction.Left)) {
                    audioMgr.PlayButtonB();
                }
            }
        }
        public override void InputRight() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonA();
                currentOption = (currentOption == 0) ? 1 : 0;
                HighlightSelection();
            }
            else if (currentScreen == 1) {
                if (!MovePlayer(Direction.Right)) {
                    audioMgr.PlayButtonB();
                }
            }
        }
        #endregion

        protected override void StartApp() {
            DrawStartMenu();
        }
        protected override void CloseApp(Screen goToMenu = Screen.MainMenu) {
            gm.SetTappingEnabled(Direction.Left, false);
            gm.SetTappingEnabled(Direction.Right, false);
            gm.SetTappingEnabled(Direction.Up, false);
            gm.SetTappingEnabled(Direction.Down, false);
            base.CloseApp(Screen.GamesTravelMenu);
        }

        private int CalculateScore() => 12 * timeRemaining;

        private void DrawStartMenu() {
            tbOptions[0] = ScreenElement.BuildTextBox("Start", screenDisplay.transform, DFont.Small)
                .SetText("start")
                .SetSize(28, 8)
                .SetPosition(2, 8)
                .SetAlignment(TextAnchor.UpperCenter)
                .SetComponentSize(28, 7)
                .SetComponentPosition(0, 1);
            tbOptions[1] = ScreenElement.BuildTextBox("Cancel", screenDisplay.transform, DFont.Small)
                .SetText("cancel")
                .SetSize(28, 8)
                .SetPosition(2, 16)
                .SetAlignment(TextAnchor.UpperCenter)
                .SetComponentSize(28, 7)
                .SetComponentPosition(0, 1);
            HighlightSelection();
        }
        
        private void HighlightSelection() {
            if(currentOption == 0) {
                tbOptions[0].InvertColors(true);
                tbOptions[1].InvertColors(false);
            }
            else {
                tbOptions[0].InvertColors(false);
                tbOptions[1].InvertColors(true);
            }
        }
        private void StartGame() {
            currentScreen = 1;
            gm.SetTappingEnabled(Direction.Left, true, 0.1f);
            gm.SetTappingEnabled(Direction.Right, true, 0.1f);
            gm.SetTappingEnabled(Direction.Up, true, 0.1f);
            gm.SetTappingEnabled(Direction.Down, true, 0.1f);
            tbOptions[0].Dispose();
            tbOptions[1].Dispose();
            GenerateMaze();
            DrawMaze();
            ScreenElement.BuildTextBox("Time", screenDisplay.transform, DFont.Small).SetText("TIME").SetSize(18, 5).SetPosition(1, 1);
            tbTime = ScreenElement.BuildTextBox("TimeCount", screenDisplay.transform, DFont.Small).SetText(timeRemaining.ToString()).SetSize(10, 5).SetPosition(22, 1);
            InvokeRepeating("CountDown", 1f, 1f);

            playerMarker = ScreenElement.BuildRectangle("Player", screenDisplay.transform).SetSize(2, 2).SetPosition(1, 29).SetFlickPeriod(0.2f);
            UpdateMarkerPos();
        }
        //Returns true if a movement is made.
        private bool MovePlayer(Direction dir) {
            //If player enters the winning cell.
            if(playerPos == (9, 7) && dir == Direction.Right) {
                playerPos.x += 1;
                UpdateMarkerPos();
                TriggerWin();
                return true;
            }
            //If the player is in the starting cell.
            if(playerPos == (-1, 0)) {
                if (dir == Direction.Right) {
                    playerPos.x += 1;
                    UpdateMarkerPos();
                    return true;
                }
                else return false;
            }
            //If the player is in the end cell.
            else if (playerPos == (10, 7)) {
                if (dir == Direction.Left) {
                    playerPos.x -= 1;
                    UpdateMarkerPos();
                    return true;
                }
                else return false;
            }
            //If the player is in a regular cell.
            else {
                switch (dir) {
                    case Direction.Left:
                        if (playerPos == (0, 0) || IsCellConnected(playerPos, dir)) {
                            playerPos.x -= 1;
                            UpdateMarkerPos();
                            return true;
                        }
                        return false;
                    case Direction.Right:
                        if (playerPos == (9, 7) || IsCellConnected(playerPos, dir)) {
                            playerPos.x += 1;
                            UpdateMarkerPos();
                            return true;
                        }
                        return false;
                    case Direction.Up:
                        if (IsCellConnected(playerPos, dir)) {
                            playerPos.y -= 1;
                            UpdateMarkerPos();
                            return true;
                        }
                        return false;
                    case Direction.Down:
                        if (IsCellConnected(playerPos, dir)) {
                            playerPos.y += 1;
                            UpdateMarkerPos();
                            return true;
                        }
                        return false;
                }
                return false;
            }
        }
        private void UpdateMarkerPos() {
            if (playerPos == (-1, 0)) playerMarker.SetPosition(0, 29); //Player has not left the start cell yet
            else if (playerPos == (10, 7)) playerMarker.SetPosition(30, 8); //Player has reached the end cell
            else {
                int posX = 1 + (3 * playerPos.x);
                int posY = 29 - (3 * playerPos.y);
                playerMarker.SetPosition(posX, posY);
            }
        }

        private void GenerateMaze() {
            //Initialize variables
            cellPaths.Fill(0);
            stack.Push((0, 0));
            cellPaths[0] = CELL_VISITED;
            visitedCells++;

            //cellState.Length is always equal to the total amount of cells.
            while (visitedCells < cellPaths.Length) {
                List<Direction> neighbors = new List<Direction>();
                //binary bitwise operator &: takes two numbers as operands and does AND on every bit of two numbers.
                if (stack.Peek().x > 0 && (cellPaths[NeighborIndex(stack.Peek(), Direction.Left)] & CELL_VISITED) == 0) {
                    neighbors.Add(Direction.Left);
                }
                if (stack.Peek().x < MAZE_WIDTH - 1 && (cellPaths[NeighborIndex(stack.Peek(), Direction.Right)] & CELL_VISITED) == 0) {
                    neighbors.Add(Direction.Right);
                }
                if (stack.Peek().y > 0 && (cellPaths[NeighborIndex(stack.Peek(), Direction.Up)] & CELL_VISITED) == 0) {
                    neighbors.Add(Direction.Up);
                }
                if (stack.Peek().y < MAZE_HEIGHT - 1 && (cellPaths[NeighborIndex(stack.Peek(), Direction.Down)] & CELL_VISITED) == 0) {
                    neighbors.Add(Direction.Down);
                }
                //binary bitwise operator |: takes two numbers as operands and does OR on every bit of two numbers.
                //Using | instead of + makes it so adding a path that was already added does not change anything.
                if (neighbors.Count != 0) {
                    Direction chosenNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                    //Create path between this cell and the chosen neighbor.
                    switch (chosenNeighbor) {
                        case Direction.Left:
                            cellPaths[CellIndex(stack.Peek())] |= CELL_PATH_LEFT;
                            cellPaths[NeighborIndex(stack.Peek(), Direction.Left)] |= CELL_PATH_RIGHT;
                            stack.Push((stack.Peek().x - 1, stack.Peek().y));
                            break;
                        case Direction.Right:
                            cellPaths[CellIndex(stack.Peek())] |= CELL_PATH_RIGHT;
                            cellPaths[NeighborIndex(stack.Peek(), Direction.Right)] |= CELL_PATH_LEFT;
                            stack.Push((stack.Peek().x + 1, stack.Peek().y));
                            break;
                        case Direction.Up:
                            cellPaths[CellIndex(stack.Peek())] |= CELL_PATH_UP;
                            cellPaths[NeighborIndex(stack.Peek(), Direction.Up)] |= CELL_PATH_DOWN;
                            stack.Push((stack.Peek().x, stack.Peek().y - 1));
                            break;
                        case Direction.Down:
                            cellPaths[CellIndex(stack.Peek())] |= CELL_PATH_DOWN;
                            cellPaths[NeighborIndex(stack.Peek(), Direction.Down)] |= CELL_PATH_UP;
                            stack.Push((stack.Peek().x, stack.Peek().y + 1));
                            break;
                    }
                    cellPaths[CellIndex(stack.Peek())] |= CELL_VISITED;
                    visitedCells++;
                }
                else {
                    stack.Pop();
                }

            }
        }

        private int CellIndex((int x, int y) coords) {
            return coords.x + (coords.y * MAZE_WIDTH);
        }
        private int NeighborIndex((int x, int y) coords, Direction dir) {
            switch (dir) {
                case Direction.Left:
                    return coords.x - 1 + (coords.y * MAZE_WIDTH);
                case Direction.Right:
                    return coords.x + 1 + (coords.y * MAZE_WIDTH);
                case Direction.Up:
                    return coords.x + ((coords.y - 1) * MAZE_WIDTH);
                case Direction.Down:
                    return coords.x + ((coords.y + 1) * MAZE_WIDTH);
                default:
                    return -1;
            }
        }
        private bool IsCellConnected((int x, int y) cell, Direction dir) {
            switch(dir) {
                case Direction.Left:
                    return (cellPaths[CellIndex(cell)] & CELL_PATH_LEFT) == CELL_PATH_LEFT;
                case Direction.Right:
                    return (cellPaths[CellIndex(cell)] & CELL_PATH_RIGHT) == CELL_PATH_RIGHT;
                case Direction.Up:
                    return (cellPaths[CellIndex(cell)] & CELL_PATH_UP) == CELL_PATH_UP;
                case Direction.Down:
                    return (cellPaths[CellIndex(cell)] & CELL_PATH_DOWN) == CELL_PATH_DOWN;
                default:
                    return false;
            }
        }
        private void DrawMaze() {
            screenDisplay.sprite = BuildSpriteFromMaze(gm.spriteDB.emptySprite);
        }
        private Sprite BuildSpriteFromMaze(Sprite sprite) {
            Texture2D texture = sprite.texture;
            Texture2D mazeTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            mazeTexture.filterMode = FilterMode.Point;

            //Set the transparent and black portions of the texture.
            mazeTexture.SetPixels32(0, 25, 32, 7, new Color32[32 * 7].Fill(Color.clear));
            mazeTexture.SetPixels32(0, 0, 32, 25, new Color32[32 * 25].Fill(Color.black));

            //Draw the maze.
            for (int x = 0; x < MAZE_WIDTH; x++) {
                for (int y = 0; y < MAZE_HEIGHT; y++) {
                    for (int px = 0; px < PATH_WIDTH; px++) {
                        for (int py = 0; py < PATH_WIDTH; py++) {
                            int pixelX = 1 + x * (PATH_WIDTH + 1) + px;
                            int pixelY = 1 + (y * (PATH_WIDTH + 1) + py);
                            mazeTexture.SetPixel(pixelX, pixelY, Color.clear);
                        }
                    }
                    for (int p = 0; p < PATH_WIDTH; p++) {
                        if (IsCellConnected((x, y), Direction.Down)) {
                            int pixelX = 1 + x * (PATH_WIDTH + 1) + p;
                            int pixelY = 1 + (y * (PATH_WIDTH + 1) + PATH_WIDTH);
                            mazeTexture.SetPixel(pixelX, pixelY, Color.clear);
                        }
                        if (IsCellConnected((x, y), Direction.Right)) {
                            int pixelX = 1 + x * (PATH_WIDTH + 1) + PATH_WIDTH;
                            int pixelY = 1 + (y * (PATH_WIDTH + 1) + p);
                            mazeTexture.SetPixel(pixelX, pixelY, Color.clear);
                        }
                    }
                }
            }

            //Draw the start and exit.
            mazeTexture.SetPixel(0, 1, Color.clear);
            mazeTexture.SetPixel(0, 2, Color.clear);
            mazeTexture.SetPixel(30, 22, Color.clear);
            mazeTexture.SetPixel(31, 22, Color.clear);
            mazeTexture.SetPixel(30, 23, Color.clear);
            mazeTexture.SetPixel(31, 23, Color.clear);

            mazeTexture.Apply();

            return Sprite.Create(mazeTexture, sprite.rect, sprite.pivot);
        }

        private void CountDown() {
            if(currentScreen == 1 || currentScreen == 2) {
                if (timeRemaining > 1) {
                    timeRemaining--;
                    tbTime.Text = timeRemaining.ToString();
                }
                else if (timeRemaining == 1) {
                    timeRemaining--;
                    audioMgr.PlayButtonB();
                    currentScreen = 2;

                    TextBoxBuilder tb = ScreenElement.BuildTextBox("Defeat", screenDisplay.transform, DFont.Small).SetText("DEFEAT").SetSize(32, 11).SetPosition(0, 14);
                    tb.SetComponentSize(29, 8);
                    playerMarker.SetFlickPeriod(0f);
                }
                else if (timeRemaining == 0) {
                    timeRemaining--;
                }
            }
        }

        private void TriggerWin() {
            audioMgr.PlayButtonB();
            currentScreen = 3;

            TextBoxBuilder tb = ScreenElement.BuildTextBox("Victory", screenDisplay.transform, DFont.Small).SetText("VICTORY").SetSize(32, 11).SetPosition(0, 14);
            tb.SetComponentSize(29, 8);
            playerMarker.SetFlickPeriod(0f);
        }
    }
}