using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class RectangleBuilder : ScreenElement {
    [SerializeField] private Image rectangle;

    //Flick variables:
    private float timePassed = 0f;
    private bool isEnabled = true;
    private float flickPeriod = 0f;

    public float FlickPeriod => flickPeriod;

    //Overrides:
    protected override void BaseInvertColors(bool val) {
        if (val) {
            rectangle.color = Constants.BACKGROUND_COLOR;
            background.color = Constants.ACTIVE_COLOR;
        }
        else {
            rectangle.color = Constants.ACTIVE_COLOR;
            background.color = Constants.BACKGROUND_COLOR;
        }
    }

    //Chained base methods:
    public RectangleBuilder Center() {
        BaseCenter();
        return this;
    }
    public RectangleBuilder InvertColors(bool val) {
        BaseInvertColors(val);
        return this;
    }
    public RectangleBuilder Move(Direction direction, int amount = 1) {
        BaseMove(direction, amount);
        return this;
    }
    public RectangleBuilder PlaceOutside(Direction direction) {
        BasePlaceOutside(direction);
        return this;
    }
    public RectangleBuilder SetActive(bool active) {
        BaseSetActive(active);
        return this;
    }
    public RectangleBuilder SetPosition(int x, int y) {
        BaseSetPosition(x, y);
        return this;
    }
    public RectangleBuilder SetPosition(Vector2Int pos) {
        BaseSetPosition(pos);
        return this;
    }
    public RectangleBuilder SetSize(int width, int length) {
        BaseSetSize(width, length);
        return this;
    }
    public RectangleBuilder SetTransparent(bool val) {
        BaseSetTransparent(val);
        return this;
    }
    public RectangleBuilder SetX(int x) {
        BaseSetX(x);
        return this;
    }
    public RectangleBuilder SetY(int y) {
        BaseSetY(y);
        return this;
    }

    //Specific methods:
    /// <summary>
    /// Sets whether the rectangle pixels' are active (true) or inactive (false).
    /// </summary>
    public RectangleBuilder SetColor(bool activeColor) {
        if(activeColor) {
            rectangle.color = Constants.ACTIVE_COLOR;
        }
        else {
            rectangle.color = Constants.BACKGROUND_COLOR;
        }
        return this;
    }
    /// <summary>
    /// Sets the speed (in seeconds) at which this rectangle flicks. A flick speed of 0 means that it will not flick.
    /// </summary>
    public RectangleBuilder SetFlickPeriod(float flickPeriod, bool startEnabled = true) {
        this.flickPeriod = flickPeriod;
        rectangle.enabled = startEnabled;
        isEnabled = startEnabled;
        return this;
    }

    public RectangleBuilder ResetFlick(bool startEnabled = true) {
        timePassed = 0f;
        rectangle.enabled = startEnabled;
        isEnabled = startEnabled;
        return this;
    }

    private void Update() {
        if (flickPeriod == 0) {
            if(isEnabled == false) {
                rectangle.enabled = true;
            }
        }
        else {
            timePassed += Time.deltaTime;
            if(timePassed >= flickPeriod) {
                timePassed = 0f;

                if(isEnabled) {
                    rectangle.enabled = false;
                    isEnabled = false;
                }
                else {
                    rectangle.enabled = true;
                    isEnabled = true;
                }
            }
        }
    }
}
