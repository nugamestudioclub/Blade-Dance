using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Handles all VFX for any given level - has ability to parse an effect string ("BG bad-apple 3 1") 
 * for its effect type, as well as its parameters. Only supports parameters of types string, float, and int
 */
public class VfxManager : MonoBehaviour
{

    private Queue<EffectCommand> commandQueue;

    // Start is called before the first frame update
    void Start()
    {
        commandQueue = new Queue<EffectCommand>();

        ParseEffectLine("BG bad-apple 3 1");
        ParseEffectLine("BG bad-apple 3 2");
        PlayAllEffects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //code to handle an effect, denoted as a string in the following format:
    //  TYPE param1 param2 param3
    void ParseEffectLine(string line)
    {
        //parses the string for tokens, splits into effect type and its parameters
        List<string> tokens = new List<string>(line.Split(' '));
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
            command = new BackgroundEffectCommand(tokens[0], float.Parse(tokens[1]), float.Parse(tokens[2]));
        } else
        {
            Debug.Log("Could not find effect type: " + effectType);
        }

        //adds the EffectCommand to the command queue
        commandQueue.Enqueue(command);
    }


    //for testing purposes only - empties the entire command queue and plays all effects
    //TODO: Play effects at a given beat division or at a certain time
    void PlayAllEffects()
    {
        while (commandQueue.Count > 0)
        {
            EffectCommand command = commandQueue.Dequeue();
            command.Execute();
        }
    }

    
}
