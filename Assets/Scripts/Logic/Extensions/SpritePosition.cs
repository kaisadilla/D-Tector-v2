using UnityEngine;

namespace Kaisa.Digivice.Extensions {
    public static class SpritePosition {
        public static void PlaceInPosition(this GameObject go, Vector2 pos) {
            go.PlaceInPosition((int)pos.x, (int)pos.y);
        }
        public static void PlaceInPosition(this GameObject go, int x, int y) {
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * Constants.PIXEL_SIZE, -y * Constants.PIXEL_SIZE);
        }
        /*
        public static void MovePosition(this GameObject go, int x, int y) {
            go.GetComponent<RectTransform>().anchoredPosition += new Vector2(x * Constants.PixelSize, -y * Constants.PixelSize);
        }*/
        public static void MoveSprite(this GameObject go, Direction direction, int amount = 1) {
            int displacement = Constants.PIXEL_SIZE * amount;
            switch(direction) {
                case Direction.Up:
                    go.transform.localPosition += new Vector3(0, displacement, 0);
                    break;
                case Direction.Down:
                    go.transform.localPosition += new Vector3(0, -displacement, 0);
                    break;
                case Direction.Left:
                    go.transform.localPosition += new Vector3(-displacement, 0, 0);
                    break;
                case Direction.Right:
                    go.transform.localPosition += new Vector3(displacement, 0, 0);
                    break;
            }
        }
        public static SpriteBuilder[] Move(this SpriteBuilder[] scrArray, Direction direction, int amount = 1) {
            foreach (SpriteBuilder scr in scrArray) scr.Move(direction, amount);
            return scrArray;
        }
    }
}