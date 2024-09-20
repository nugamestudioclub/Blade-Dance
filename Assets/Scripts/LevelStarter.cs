using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    public LevelRunner runner;

    private Dictionary<string, Vector3> directionConversion;

    void Start()
    {
        directionConversion = new Dictionary<string, Vector3>();

        directionConversion.Add("u", Vector3.up);
        directionConversion.Add("d", Vector3.down);
        directionConversion.Add("l", Vector3.left);
        directionConversion.Add("r", Vector3.right);

        directionConversion.Add("ul", (Vector3.up + Vector3.left));
        directionConversion.Add("ur", (Vector3.up + Vector3.right));
        directionConversion.Add("dl", (Vector3.down + Vector3.left));
        directionConversion.Add("dr", (Vector3.down + Vector3.right));

        directionConversion.Add("lu", (Vector3.up + Vector3.left));
        directionConversion.Add("ru", (Vector3.up + Vector3.right));
        directionConversion.Add("ld", (Vector3.down + Vector3.left));
        directionConversion.Add("rd", (Vector3.down + Vector3.right));

        //LevelHolder.LevelContentFilepath = "INeedYou3"; // debug line for testing purposes, Filepath should be persistent from menu screen
        TextAsset levelText = Resources.Load<TextAsset>("LevelContents/" + LevelHolder.LevelContentFilepath);
        if (levelText == null)
        {
            Debug.Log("Failed to load text asset for filepath " + LevelHolder.LevelContentFilepath + ". Level load failed.");
            return;
        }

        string[] lineSeparators = { "\n", "\r\n" };
        string[] levelLines = levelText.ToString().Split(lineSeparators, System.StringSplitOptions.RemoveEmptyEntries);

        int separatorIndex = -1;

        for (int i = 0; i < levelLines.Length; i++)
        {
            if (levelLines[i][0].Equals('-'))
            {
                separatorIndex = i;
                break;
            }
        }

        if (separatorIndex == -1)
        {
            Debug.Log("Separator for song parameters not found. Level load failed.");
            return;
        }

        if (separatorIndex == levelLines.Length - 1)
        {
            Debug.Log("Note content for level not found. Level load failed.");
            return;
        }

        string songAudioFilename = "default";
        float bpm = -1f;
        int countin = 8;
        float notespeed = 5;
        float camerasize = 7;

        for (int i = 0; i < separatorIndex; i++)
        {
            string[] setupLine = levelLines[i].Split(" ", System.StringSplitOptions.RemoveEmptyEntries);

            if (setupLine.Length < 1)
            {
                Debug.Log("Error: Empty setup line encountered. Skipping.");
                continue;
            }

            if (setupLine.Length < 2)
            {
                Debug.Log("Unfilled song parameter [" + setupLine[0] + "] found. Skipping.");
                continue;
            }

            if (setupLine[0].Equals("bpm"))
            {
                try
                {
                    bpm = float.Parse(setupLine[1]);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Failed to parse song parameter [" + setupLine[1] + "]. Skipping.");
                }
            }
            else if (setupLine[0].Equals("count"))
            {
                try
                {
                    countin = int.Parse(setupLine[1]);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Failed to parse song parameter [" + setupLine[1] + "]. Skipping.");
                }
            }
            else if (setupLine[0].Equals("notespeed"))
            {
                try
                {
                    notespeed = float.Parse(setupLine[1]);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Failed to parse song parameter [" + setupLine[1] + "]. Skipping.");
                }
            }
            else if (setupLine[0].Equals("song"))
            {
                songAudioFilename = setupLine[1];
            }
            else if (setupLine[0].Equals("camerasize"))
            {
                try
                {
                    camerasize = int.Parse(setupLine[1]);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Failed to parse song parameter [" + setupLine[1] + "]. Skipping.");
                }
            }
            else
            {
                Debug.Log("Unrecognized song parameter [" + setupLine[0] + "] found. Skipping.");
            }
        }

        if (songAudioFilename.Equals("default"))
        {
            Debug.Log("No filename for song audio found. Level load failed.");
            return;
        }

        if (bpm == -1)
        {
            Debug.Log("No BPM for song found. Level load failed.");
            return;
        }

        runner.enabled = true;
        runner.songBpm = bpm;
        runner.beatsDelay = countin;
        runner.noteSpeed = notespeed;
        Camera.main.orthographicSize = camerasize;
             
        AudioClip currentSong = Resources.Load<AudioClip>("Songs/" + songAudioFilename);
        if (currentSong == null)
        {
            Debug.Log("Failed to acquire song file [" + songAudioFilename + "]. Level load failed.");
            return;
        }
        gameObject.GetComponent<AudioSource>().clip = currentSong;

        

        List<Note> levelContent = new List<Note>();

        for (int i = separatorIndex + 1; i < levelLines.Length; i++)
        {
            string[] setupLine = levelLines[i].Split(" ", System.StringSplitOptions.RemoveEmptyEntries);

            if (setupLine[0].Length >= 2 && setupLine[0].Substring(0, 2).Equals("//"))
            {
                continue;
            }

            if (setupLine.Length < 2 || setupLine.Length > 3)
            {
                Debug.Log("Improper note command found on line " + (i + 1) + ". Skipping.");
                continue;
            }

            float beat = -1f;
            Vector3 direction = Vector3.zero;
            int offset = -2;

            try
            {
                beat = float.Parse(setupLine[0]);
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed to parse beat number for line " + (i + 1) + ". Skipping.");
                continue;
            }

            if (levelContent.Count > 0 && levelContent[levelContent.Count-1].targetBeat > beat)
            {
                Debug.Log("Encountered beat number which was less than previous beat number at line " + (i + 1) + ". Level load failed.");
                return;
            }

            if (!directionConversion.TryGetValue(setupLine[1], out direction))
            {
                Debug.Log("Failed to parse note direction for line " + (i + 1) + ". Skipping.");
                continue;
            }

            if (setupLine.Length == 2)
            {
                levelContent.Add(new Enemy(beat, direction));
                continue;
            }

            try
            {
                offset = int.Parse(setupLine[2]);
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed to parse bullet offset for line " + (i+1) + ". Skipping.");
                continue;
            }

            if (offset < -1 || offset > 1)
            {
                Debug.Log("Commanded note offset was invalid on line " + (i + 1) + ". Skipping.");
                continue;
            }

            levelContent.Add(new Bullet(beat, direction, offset));
        }

        runner.LaunchRunner(levelContent);
    }
}
