using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

enum EndMenuSelect
{
    None,
    Retry,
    Continue,
}

public class LevelEnder : MonoBehaviour
{
    public float scrollSpeed;

    private Image songIcon;
    private TMP_Text songTitle;
    private TMP_Text bandName;
    private TMP_Text difficulty;
    private TMP_Text score;
    private TMP_Text record;
    private TMP_Text max;

    private RectTransform selector;
    private RectTransform replayButton;
    private RectTransform continueButton;
    private Vector2 selectorTarget;

    private EndMenuSelect selection;

    public void Populate(int scoreValue, int maxValue)
    {
        Transform songInfo = transform.Find("SongInfo");
        songIcon = songInfo.Find("SongIcon").GetComponent<Image>();
        songTitle = songInfo.Find("SongTitle").GetComponent<TMP_Text>();
        bandName = songInfo.Find("BandName").GetComponent<TMP_Text>();
        difficulty = songInfo.Find("Difficulty").Find("Value").GetComponent<TMP_Text>();
        score = songInfo.Find("Score").Find("Value").GetComponent<TMP_Text>();
        record = songInfo.Find("Record").Find("Value").GetComponent<TMP_Text>();
        max = songInfo.Find("Max").Find("Value").GetComponent<TMP_Text>();

        selector = songInfo.Find("ButtonRow").Find("Selector").GetComponent<RectTransform>();
        selector.gameObject.SetActive(false);
        replayButton = songInfo.Find("ButtonRow").Find("Buttons").Find("Replay").GetComponent<RectTransform>();
        continueButton = songInfo.Find("ButtonRow").Find("Buttons").Find("Continue").GetComponent<RectTransform>();

        songIcon.sprite = LevelHolder.SelectedSong.songIcon;
        songTitle.SetText(LevelHolder.SelectedSong.songName);
        bandName.SetText(LevelHolder.SelectedSong.bandName);
        difficulty.SetText(LevelHolder.SelectedDifficulty.ToString());
        score.SetText(scoreValue.ToString());
        // TODO record
        max.SetText(maxValue.ToString());

        selection = EndMenuSelect.None;
    }

    // TODO use selector object and rectransform
    private void Update()
    {
        if (selector.gameObject.activeSelf)
        {
            selector.anchoredPosition = Vector2.MoveTowards(selector.anchoredPosition, selectorTarget, scrollSpeed * Time.deltaTime);
        } 

        if (selection == EndMenuSelect.Retry)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectorTarget = continueButton.anchoredPosition;
                selection = EndMenuSelect.Continue;
            }
            else if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (selection == EndMenuSelect.Continue)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectorTarget = replayButton.anchoredPosition;
                selection = EndMenuSelect.Retry;
            }
            else if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                selector.gameObject.SetActive(true);
                selector.anchoredPosition = replayButton.anchoredPosition;
                selectorTarget = selector.anchoredPosition;
                selection = EndMenuSelect.Retry;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                selector.gameObject.SetActive(true);
                selector.anchoredPosition = continueButton.anchoredPosition;
                selectorTarget = selector.anchoredPosition;
                selection = EndMenuSelect.Continue;
            }
        }
    }
}
