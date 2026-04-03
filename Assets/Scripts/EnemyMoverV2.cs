using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyMoverV2 : MonoBehaviour
{
    public AudioClip hitSFX;
    public AudioClip missSFX;
    private LevelRunnerV2 levelRunner;

    // the direction the note will move over time
    private Vector3 direction;
    // the amount of units this note travels per beat of music
    private float unitsPerBeat;
    // the target location where the note becomes critical
    private Vector3 criticalPosition;
    // the target beat were the note becomes critical
    private float criticalBeat;

    private Bounds killBounds;

    //public AudioClip debugSound;
    //private bool debugCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        // levelRunner = GameObject.FindGameObjectWithTag("LevelRunner").GetComponent<LevelRunnerV2>();
    }

    public void FindRunner()
    {
        levelRunner = GameObject.FindGameObjectWithTag("LevelRunner").GetComponent<LevelRunnerV2>();
    }

    // Based on the provided beat, set this enemy's position accordingly
    public void UpdatePosition(float beat)
    {
        // secPerBeat * noteSpeed = units / beat
        // targetBeat, currentBeat, direction (Vector3), speed (units / beat), targetLocation
        // currentBeat -> position
        // position = targetLocation + (currentBeat - targetBeat) * speed * direction
        transform.position = criticalPosition + (beat - criticalBeat) * unitsPerBeat * direction;
    }

    public bool RemoveAtBounds()
    {
        if (killBounds.Contains(transform.position))
        {
            CenterHit();
            return true;
        }
        return false;
    }

    public void SetProperties(Vector3 setTargetPosition, Vector3 setDirection, float setSpeed, float setTargetBeat, Bounds bounds)
    {
        this.direction = setDirection;
        this.unitsPerBeat = setSpeed;
        this.criticalPosition = setTargetPosition;
        this.criticalBeat = setTargetBeat;
        killBounds = bounds;
        ResolveRotation();
    }

    private void ResolveRotation()
    {
        if (direction == Vector3.up)
        {
            transform.Rotate(new Vector3(0f, 0f, 0f));
        }
        else if (direction == Vector3.left)
        {
            transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if (direction == Vector3.down)
        {
            transform.Rotate(new Vector3(0f, 0f, 180f));
        }
        else if (direction == Vector3.right)
        {
            transform.Rotate(new Vector3(0f, 0f, 270f));
        }
    }

    public void CenterHit()
    {
        AudioSource.PlayClipAtPoint(missSFX, Camera.main.transform.position, 1.0f);
        levelRunner.AddMiss();
        Destroy(gameObject);
    }

    public void HitPlayer(int accuracy)
    {
        AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, 0.5f);
        levelRunner.AddHit(this, accuracy);
        Destroy(gameObject);
    }
}
