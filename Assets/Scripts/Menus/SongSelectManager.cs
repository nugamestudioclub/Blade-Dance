using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

enum MenuSection
{
    SongSelect,
    LevelSelect,
    LevelConfirm
}

public class SongSelectManager : MonoBehaviour
{
    public TracklistSO track;

    public GameObject songPanelPrefab;

    public GameObject settingsKeybindPrefab;

    public GameObject selectPanel;

    // measured in pixels per second
    public float scrollSpeed = 100;

    public AudioClip selectSFX;

    public GameObject difficultyButtonPrefab;

    private Transform scrollAreaContent;
    private RectTransform scrollAreaContentRect;

    private int selectedTrack = 0;

    private Vector2 scrollAreaTargetPosition;
    private Vector2 levelSelectTargetPosition;

    private Image songDetailImage;
    private TMP_Text songDetailName;
    private TMP_Text songDetailBand;
    private Transform difficultyButtons;

    private MenuSection menuSection = MenuSection.SongSelect;

    private int selectedLevel = 0;
    private RectTransform levelSelectPanel;
    private bool cancelReleasedFromLevelSelect = true;

    private GameObject startButton;

    private Transform settingsMenuObj;
    private Transform settingsAreaContent;
    CanvasGroup settingsMenuCanvasGroup;
    private bool settingsMenuOpen = false;

    private bool awaitingSetKeybind = false; // If true, next key press will be used to set a keybind.
    private string bindNameToSet = "";       // Name of the keybind currently being set (if applicable).
    private string previousBindName = "";    // Previous keybind name, used to revert back if needed.
    private TMP_Text buttonTextToSet = null; // Button text we need to update after setting the keybind
    private Dictionary<string, TMP_Text> keybindNamesToButtons = new Dictionary<string, TMP_Text>();

    // Start is called before the first frame update
    void Start()
    {
        //Screen.fullScreen = false;
        //Screen.fullScreenMode = FullScreenMode.Windowed;
        //Screen.SetResolution(1920, 1080, Screen.fullScreen);
        //Cursor.lockState = CursorLockMode.Locked;

        scrollAreaContent = this.transform.Find("ScrollArea").Find("Content");
        scrollAreaContentRect = scrollAreaContent.GetComponent<RectTransform>();

        settingsMenuObj = this.transform.Find("SettingsArea");
        settingsAreaContent = settingsMenuObj.Find("Content");
        settingsMenuCanvasGroup = settingsMenuObj.GetComponent<CanvasGroup>();

        Transform songDetail = this.transform.Find("SongDetail");
        songDetailImage = songDetail.Find("Image").GetComponent<Image>();
        songDetailName = songDetail.Find("SongTitle").GetComponent<TMP_Text>();
        songDetailBand = songDetail.Find("BandName").GetComponent<TMP_Text>();
        difficultyButtons = songDetail.Find("DifficultyButtons");
        levelSelectPanel = songDetail.Find("Selection").GetComponent<RectTransform>();
        levelSelectPanel.gameObject.SetActive(false);

        startButton = songDetail.Find("LevelPanel").Find("Selection").gameObject;
        startButton.SetActive(false);

        foreach (SongSO song in track.songs)
        {
            GameObject nextPanel = Instantiate(songPanelPrefab, scrollAreaContent);
            nextPanel.transform.Find("SongImage").GetComponent<Image>().sprite = song.songIcon;
            Transform infoPanel = nextPanel.transform.Find("InfoPanel");
            infoPanel.Find("SongTitle").GetComponent<TMP_Text>().SetText(song.songName);
            infoPanel.Find("BandName").GetComponent<TMP_Text>().SetText(song.bandName);
            infoPanel.Find("Difficulties").GetComponent<TMP_Text>().SetText("Difficulty: " + song.GetDifficultyList());
        }

        // Settings menu setup
        int idx = 0;
        string[] keybindNames = {
            "up_p", "up_s", "down_p", "down_s", "left_p", "left_s", "right_p", "right_s", "settingsMenu", "menuConfirm", "exitKey", "pauseKey"
        };
        foreach (var (name, keyCode) in KeybindManager.Keybinds)
        {
            // Init from prefab
            GameObject nextPanel = Instantiate(settingsKeybindPrefab, settingsAreaContent);
            nextPanel.transform.Find("BindName").GetComponent<TMP_Text>().SetText(name);
            TMP_Text buttonTmpElem = nextPanel.transform.Find("Button").transform.Find("KeyName").GetComponent<TMP_Text>();
            buttonTmpElem.SetText(keyCode.ToString());
            keybindNamesToButtons.Add(name, buttonTmpElem); // Store reference to the button text for this keybind for easy updating when the keybind is changed

            // Positioning
            RectTransform rect = nextPanel.GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            pos.y += 30 + ((5 - idx) * 80);
            rect.anchoredPosition = pos;
            
            // Button click listener
            string argument = keybindNames[idx];
            Button button = nextPanel.transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => startSetKeybind(argument, buttonTmpElem));

            idx++;
        }
        // Reset to defaults button listener
        settingsAreaContent.transform.Find("KeybindDefaults").GetComponent<Button>().onClick.AddListener(() => {
            if (KeybindManager.ResetKeybindsToDefault()) {
                // Update all button texts to reflect default keybinds
                int i = 0;
                foreach (var (name, keyCode) in KeybindManager.Keybinds)
                {
                    keybindNamesToButtons[name].SetText(keyCode.ToString());
                    i++;
                }
            }
            else {
                Debug.LogError("Failed to reset keybinds to default. JSON save failure.");
                NotificationManager.NotifyError("Failed to reset keybinds to default. JSON save failure.");
            }
        });

        // Start with settings hidden
        HideSettingsMenu();

        scrollAreaContentRect.anchoredPosition = new Vector2(0f, (track.songs.Count - 1) * -112.5f);
        SelectTrack(0);
    }

    public bool startSetKeybind(string bindName, TMP_Text buttonText) {
        if (awaitingSetKeybind) {
            return false; // Already waiting for a keybind, ignore this request
        }

        awaitingSetKeybind = true;
        bindNameToSet = bindName;
        buttonTextToSet = buttonText;

        previousBindName = buttonText.text;
        buttonText.SetText("Press a key...");
        return true;
    }

    // assumes that we want to treat index beyond the track list bounds as edge selections
    private void SelectTrack(int index)
    {
        if (index >= 0 && index <= track.songs.Count - 1)
        {
            
            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);
        }

        selectedTrack = Mathf.Clamp(index, 0, track.songs.Count - 1);
        float targetY = (track.songs.Count - 1) * -112.5f + selectedTrack * 225f;
        scrollAreaTargetPosition = new Vector2(0f, targetY);

        songDetailImage.sprite = track.songs[selectedTrack].songIcon;
        songDetailName.SetText(track.songs[selectedTrack].songName);
        songDetailBand.SetText(track.songs[selectedTrack].bandName);

        foreach (Transform child in difficultyButtons)
        {
            Destroy(child.gameObject);
        }

        int levelCount = track.songs[selectedTrack].levels.Count;
        for (int i = 0; i < levelCount; i++)
        {
            GameObject nextButton = Instantiate(difficultyButtonPrefab, difficultyButtons);
            nextButton.transform.GetChild(0).GetComponent<TMP_Text>().SetText(track.songs[selectedTrack].levels[i].difficulty.ToString());
        }
    }

    private void IncrementTrack()
    {
        SelectTrack(selectedTrack + 1);
    }

    private void DecrementTrack()
    {
        SelectTrack(selectedTrack - 1);
    }

    private void SelectLevel(int index)
    {
        int levelCount = track.songs[selectedTrack].levels.Count;

        if (index >= 0 && index <= levelCount - 1)
        {
            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);
        }

        selectedLevel = Mathf.Clamp(index, 0, levelCount - 1);
        float targetX = (levelCount - 1) * (-82.5f) + selectedLevel * 165f;
        levelSelectTargetPosition = new Vector2(targetX, -300f);
    }

    private void IncrementLevel()
    {
        SelectLevel(selectedLevel + 1);
    }

    private void DecrementLevel()
    {
        SelectLevel(selectedLevel - 1);
    }

    private void HideSettingsMenu()
    {
        settingsMenuCanvasGroup.alpha = 0f;
        settingsMenuCanvasGroup.interactable = false;
        settingsMenuCanvasGroup.blocksRaycasts = false;
    }

    private void ShowSettingsMenu()
    {
        settingsMenuCanvasGroup.alpha = 1f;
        settingsMenuCanvasGroup.interactable = true;
        settingsMenuCanvasGroup.blocksRaycasts = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Scrolling the song menu
        scrollAreaContentRect.anchoredPosition = Vector2.MoveTowards(scrollAreaContentRect.anchoredPosition, scrollAreaTargetPosition, scrollSpeed * Time.deltaTime);
        levelSelectPanel.anchoredPosition = Vector2.MoveTowards(levelSelectPanel.anchoredPosition, levelSelectTargetPosition, scrollSpeed * Time.deltaTime);

        // Setting keybinds, if currently in the set keybinds menu
        if (awaitingSetKeybind) {
            // Check for any key press
            if (Input.anyKeyDown) {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode))) {
                    if (Input.GetKeyDown(keyCode)) {
                        // Set the new keybind in the KeybindManager
                        bool success = KeybindManager.SetKeybind(bindNameToSet, keyCode);
                        awaitingSetKeybind = false;
                        bindNameToSet = "";

                        if (success) {
                            buttonTextToSet.SetText(keyCode.ToString());
                        } else {
                            Debug.LogError("Failed to set keybind. Possible duplicate keybind or JSON save failure.");
                            NotificationManager.NotifyError("Failed to set keybind. Possible duplicate keybind or JSON save failure.");
                            buttonTextToSet.SetText(previousBindName); // Revert to previous keybind name on failure
                        }
                        break;
                    }
                }
            }

            // Skip the rest of the update loop while waiting for keybind input
            return;
        }

        // Setting menu; prevents all other actions
        if (KeybindManager.PressedSettingsMenu()) {
            settingsMenuOpen = !settingsMenuOpen;
            if (settingsMenuOpen) {
                ShowSettingsMenu();
            } else {
                HideSettingsMenu();
            }
        }

        if (settingsMenuOpen) {
            // Nothing needed here for now since the buttons handle their own input
        }
        else if (menuSection == MenuSection.SongSelect)
        {
            InputSongSelect();
        }
        else if (menuSection == MenuSection.LevelSelect)
        {
            InputLevelSelect();
        }
        else
        {
            InputLevelConfirm();
        }
    }

    private void InputSongSelect()
    {
        if (KeybindManager.PressedUp() && !cancelReleasedFromLevelSelect)
        {
            cancelReleasedFromLevelSelect = true;
        }

        if (KeybindManager.PressedRight() || KeybindManager.PressedConfirm())
        {
            menuSection = MenuSection.LevelSelect;
            levelSelectPanel.gameObject.SetActive(true);
            Vector2 startPosition = new Vector2((track.songs[selectedTrack].levels.Count - 1) * (-82.5f), -300f);
            levelSelectPanel.anchoredPosition = startPosition;
            levelSelectTargetPosition = startPosition;
            SelectLevel(0);
        }
        else if (KeybindManager.PressedDown())
        {
            IncrementTrack();
        }
        else if (KeybindManager.PressedUp())
        {
            DecrementTrack();
        }
        else
        {
            if (scrollAreaContentRect.anchoredPosition == scrollAreaTargetPosition && cancelReleasedFromLevelSelect)
            {
                if (KeybindManager.HoldingDown())
                {
                    IncrementTrack();
                }
                else if (KeybindManager.HoldingUp())
                {
                    DecrementTrack();
                }
            }
        }
    }

    private void InputLevelSelect()
    {
        if (KeybindManager.PressedUp() || KeybindManager.PressedExit())
        {
            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);

            menuSection = MenuSection.SongSelect;
            levelSelectPanel.gameObject.SetActive(false);
            cancelReleasedFromLevelSelect = false;
        }
        else if (KeybindManager.PressedRight())
        {
            IncrementLevel();
        }
        else if (KeybindManager.PressedLeft())
        {
            DecrementLevel();
        }
        else if (KeybindManager.PressedDown() || KeybindManager.PressedConfirm())
        {
            menuSection = MenuSection.LevelConfirm;
            startButton.SetActive(true);

            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);
        }
    }

    private void InputLevelConfirm()
    {
        if (KeybindManager.PressedUp() || KeybindManager.PressedExit())
        {
            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);

            menuSection = MenuSection.LevelSelect;
            startButton.SetActive(false);
        }
        else if (Input.anyKeyDown)
        {
            // start the selected level
            LevelHolder.LevelContentFilepath = track.songs[selectedTrack].levels[selectedLevel].contentFilePath;
            LevelHolder.SelectedSong = track.songs[selectedTrack];
            LevelHolder.SelectedDifficulty = track.songs[selectedTrack].levels[selectedLevel].difficulty;

            AudioSource.PlayClipAtPoint(selectSFX, Camera.main.transform.position, 1.0f);

            SceneManager.LoadScene("Gameplay");
        }
    }
}