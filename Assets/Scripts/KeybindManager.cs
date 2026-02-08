using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeybindManager
{
    // Primary and secondary keybinds for directional controls
    private static KeyCode up_p = KeyCode.W;
    private static KeyCode up_s = KeyCode.UpArrow;
    private static KeyCode down_p = KeyCode.S;
    private static KeyCode down_s = KeyCode.DownArrow;
    private static KeyCode left_s = KeyCode.A;
    private static KeyCode left_p = KeyCode.LeftArrow;
    private static KeyCode right_p = KeyCode.D;
    private static KeyCode right_s = KeyCode.RightArrow;

    // Other keybinds
    private static KeyCode menuConfirm = KeyCode.Return;
    private static KeyCode exitKey = KeyCode.Backspace;

    public static bool HoldingUp() {
        return Input.GetKey(up_p) || Input.GetKey(up_s);
    }

    public static bool HoldingDown() {
        return Input.GetKey(down_p) || Input.GetKey(down_s);
    }

    public static bool HoldingLeft() {
        return Input.GetKey(left_p) || Input.GetKey(left_s);
    }

    public static bool HoldingRight() {
        return Input.GetKey(right_p) || Input.GetKey(right_s);
    }

    public static bool PressedUp() {
        return Input.GetKeyDown(up_p) || Input.GetKeyDown(up_s);
    }

    public static bool PressedDown() {
        return Input.GetKeyDown(down_p) || Input.GetKeyDown(down_s);
    }

    public static bool PressedLeft() {
        return Input.GetKeyDown(left_p) || Input.GetKeyDown(left_s);
    }

    public static bool PressedRight() {
        return Input.GetKeyDown(right_p) || Input.GetKeyDown(right_s);
    }

    public static bool PressedConfirm() {
        return Input.GetKeyDown(menuConfirm);
    }

    public static bool PressedExit() {
        return Input.GetKeyDown(exitKey);
    }
}
