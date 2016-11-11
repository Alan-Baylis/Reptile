using UnityEngine;
using System.Collections;

[System.Serializable]
public class Binding {
    public string name;
    public KeyCode key;
    public int btn = -1;
    public string axis;
    public bool negateAxis;
    public bool shift;
    public bool ctrl;
    public bool alt;

    public bool IsPressed()
    {
        bool res = true;
        if (key != KeyCode.None) res = res && Input.GetKeyDown(key);
        if (btn >= 0) res = res && Input.GetMouseButtonDown(btn);
        res = res && AreModifiersHeld();
        return res;
    }

    public bool IsHeld()
    {
        bool res = true;
        if (key != KeyCode.None) res = res && Input.GetKey(key);
        if (btn >= 0) res = res && Input.GetMouseButton(btn);
        if (axis != "") res = res && ((negateAxis) ? Input.GetAxis(axis) < 0 : Input.GetAxis(axis) > 0);
        res = res && AreModifiersHeld();
        return res;
    }

    public bool IsReleased()
    {
        bool res = true;
        if (key != KeyCode.None) res = res && Input.GetKeyUp(key);
        if (btn >= 0) res = res && Input.GetMouseButtonUp(btn);
        res = res && AreModifiersHeld();
        return res;
    }

    bool AreModifiersHeld()
    {
        bool res = true;
        res = res && shift == (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        res = res && ctrl == (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                                Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand));
        res = res && alt == (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
        return res;
    }
}
