using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

public static class KeybindManager
{
    private static String saveFilePath = Path.Combine(Application.persistentDataPath, "keybinds.json");

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
    private static KeyCode settingsMenu = KeyCode.Tab;
    private static KeyCode menuConfirm = KeyCode.Return;
    private static KeyCode exitKey = KeyCode.Backspace;
    private static KeyCode pauseKey = KeyCode.Escape;

    // Getter for array of all keybinds
    public static (string bindName, KeyCode key)[] Keybinds => new[]
    {
        ("Up (primary)", up_p),
        ("Up (secondary)", up_s),
        ("Down (primary)", down_p),
        ("Down (secondary)", down_s),
        ("Left (primary)", left_p),
        ("Left (secondary)", left_s),
        ("Right (primary)", right_p),
        ("Right (secondary)", right_s),
        ("Open Settings", settingsMenu),
        ("Confirm", menuConfirm),
        ("Exit Level", exitKey),
        ("Pause", pauseKey),
    };

    // Static constructor, runs the first time something tries to use this class
    static KeybindManager() {
        JSONLoad();
    }

    // This just exists as a container for JSON serialization
    [Serializable]
    private class KeybindData
    {
        public KeyCode up_p;
        public KeyCode up_s;
        public KeyCode down_p;
        public KeyCode down_s;
        public KeyCode left_p;
        public KeyCode left_s;
        public KeyCode right_p;
        public KeyCode right_s;
        public KeyCode settingsMenu;
        public KeyCode menuConfirm;
        public KeyCode exitKey;
        public KeyCode pauseKey;

        public KeybindData(
            KeyCode up_p,
            KeyCode up_s,
            KeyCode down_p,
            KeyCode down_s,
            KeyCode left_p,
            KeyCode left_s,
            KeyCode right_p,
            KeyCode right_s,
            KeyCode settingsMenu,
            KeyCode menuConfirm,
            KeyCode exitKey,
            KeyCode pauseKey
        ) {
            this.up_p = up_p;
            this.up_s = up_s;
            this.down_p = down_p;
            this.down_s = down_s;
            this.left_p = left_p;
            this.left_s = left_s;
            this.right_p = right_p;
            this.right_s = right_s;
            this.settingsMenu = settingsMenu;
            this.menuConfirm = menuConfirm;
            this.exitKey = exitKey;
            this.pauseKey = pauseKey;
        }
    }

    // Load all keybinds from JSON, or do nothing if file doesn't exist (will use default keybinds in that case)
    private static void JSONLoad() {
        if (File.Exists(saveFilePath)) {
            string json;
            try {
                json = File.ReadAllText(saveFilePath);
            } catch (Exception e) {
                Debug.LogError("Failed to load keybinds from JSON: " + e.Message);
                return;
            }
            KeybindData container = JsonUtility.FromJson<KeybindData>(json);
            if (container.up_p != KeyCode.None) up_p = container.up_p;
            if (container.up_s != KeyCode.None) up_s = container.up_s;
            if (container.down_p != KeyCode.None) down_p = container.down_p;
            if (container.down_s != KeyCode.None) down_s = container.down_s;
            if (container.left_p != KeyCode.None) left_p = container.left_p;
            if (container.left_s != KeyCode.None) left_s = container.left_s;
            if (container.right_p != KeyCode.None) right_p = container.right_p;
            if (container.right_s != KeyCode.None) right_s = container.right_s;
            if (container.settingsMenu != KeyCode.None) settingsMenu = container.settingsMenu;
            if (container.menuConfirm != KeyCode.None) menuConfirm = container.menuConfirm;
            if (container.exitKey != KeyCode.None) exitKey = container.exitKey;
            if (container.pauseKey != KeyCode.None) pauseKey = container.pauseKey;
        }
    }

    // Save all keybinds to JSON
    private static bool JSONSave() {
        KeybindData container = new KeybindData(
            up_p, up_s, down_p, down_s, left_p, left_s, right_p, right_s, settingsMenu, menuConfirm, exitKey, pauseKey
        );
        string json = JsonUtility.ToJson(container);
        try {
            File.WriteAllText(saveFilePath, json);
            return true;
        } catch (Exception e) {
            Debug.LogError("Failed to save keybinds to JSON: " + e.Message);
        }
        return false;
    }

    // Used to set all keybinds at once, then save to JSON.
    public static bool SetKeybinds(
        KeyCode up_p,
        KeyCode up_s,
        KeyCode down_p,
        KeyCode down_s,
        KeyCode left_p,
        KeyCode left_s,
        KeyCode right_p,
        KeyCode right_s,
        KeyCode settingsMenu,
        KeyCode menuConfirm,
        KeyCode exitKey,
        KeyCode pauseKey
    ) {
        // Ensure no duplicates
        KeyCode[] arr = {
            up_p,
            up_s,
            down_p,
            down_s,
            left_p,
            left_s,
            right_p,
            right_s,
            settingsMenu,
            menuConfirm,
            exitKey,
            pauseKey
        };
        if (arr.Length != arr.Distinct().Count()) {
            return false;
        }

        // Set all
        KeybindManager.up_p = up_p;
        KeybindManager.up_s = up_s;
        KeybindManager.down_p = down_p;
        KeybindManager.down_s = down_s;
        KeybindManager.left_p = left_p;
        KeybindManager.left_s = left_s;
        KeybindManager.right_p = right_p;
        KeybindManager.right_s = right_s;
        KeybindManager.settingsMenu = settingsMenu;
        KeybindManager.menuConfirm = menuConfirm;
        KeybindManager.exitKey = exitKey;
        KeybindManager.pauseKey = pauseKey;

        return JSONSave();
    }

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

    public static bool PressedSettingsMenu() {
        return Input.GetKeyDown(settingsMenu);
    }

    public static bool PressedConfirm() {
        return Input.GetKeyDown(menuConfirm);
    }

    public static bool PressedExit() {
        return Input.GetKeyDown(exitKey);
    }

    public static bool PressedPause() {
        return Input.GetKeyDown(pauseKey);
    }
}
