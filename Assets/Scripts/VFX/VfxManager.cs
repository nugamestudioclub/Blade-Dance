using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Handles all VFX for any given level - has ability to parse an effect string ("BG bad-apple 3 1") 
 * for its effect type, as well as its parameters. Only supports parameters of types string, float, and int
 */
public class VfxManager : MonoBehaviour
{

    void Start()
    {
        //PlayEffect("BG VIDEO bad-apple 3 1");
        //PlayEffect("BG COLOR 220 20 60");
        StartCoroutine(TestWait());
    }

    IEnumerator TestWait()
    {
        yield return new WaitForSeconds(2);
        PlayEffect("BG COLOR 260 20 2");
        //PlayEffect("BG VIDEO bad-apple 1 5");

    }

    void PlayEffect(string effectString)
    {
        EffectCommand command = EffectStringToCommand(effectString);
        command.Execute();
    }

    //  Converts an effect string into an EffectCommand object, denoted as a string in the following format:
    //  TYPE param1 param2 param3
    EffectCommand EffectStringToCommand(string effectString)
    {
        //parses the string for tokens, splits into effect type and its parameters
        List<string> tokens = new List<string>(effectString.Split(' '));
        string effectType = "";

        if (tokens.Count == 0) {
            Debug.Log("Could not find effect type");
        } else
        {
            effectType = tokens[0];
            tokens.RemoveAt(0);
        }


        //creates an EffectCommand object from the effectType and params provided
        EffectCommand command = null;

        if (effectType == "BG")
        {
            //BackgroundEffectCommand takes (string filename, float duration, float playbackSpeed)
            if (tokens[0] == "VIDEO")
            {
                command = new BackgroundEffectCommand(tokens[1], float.Parse(tokens[2]), float.Parse(tokens[3]));
            }
            else if (tokens[0] == "COLOR")
            {
                command = new BackgroundEffectCommand(int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
            } else
            {
                Debug.Log("Could not find BG effect type");
            }
        } else
        {
            Debug.Log("Could not find effect type: " + effectType);
        }

        return command;
    }
    
}
