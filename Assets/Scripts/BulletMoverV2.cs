using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BulletMoverV2 : MonoBehaviour
{
    public AudioClip hitSFX;
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

    public Transform spriteTransform;
    // the SpriteRenderer of this bullet
    public SpriteRenderer spriteRender;
    // the array of sprites to the direction of movement: up, down, left, right
    public Sprite[] spriteDirections;

    // Start is called before the first frame update
    void Start()
    {
        // levelRunner = GameObject.FindGameObjectWithTag("LevelRunner").GetComponent<LevelRunner>();
    }

    public void FindRunner()
    {
        levelRunner = GameObject.FindGameObjectWithTag("LevelRunner").GetComponent<LevelRunnerV2>();
    }

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
        if (!killBounds.Contains(transform.position))
        {
            OutOfBounds();
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
        // RotationSprite();
    }

    private void ResolveRotation()
    {
        if (direction == Vector3.down)
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 0f));
        }
        else if (direction == Vector3.right)
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if (direction == Vector3.up)
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 180f));
        }
        else if (direction == Vector3.left)
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 270f));
        }
        else if (direction == (Vector3.down + Vector3.right))
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 45f));
        }
        else if (direction == (Vector3.up + Vector3.right))
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 135f));
        }
        else if (direction == (Vector3.up + Vector3.left))
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 225f));
        }
        else if (direction == (Vector3.down + Vector3.left))
        {
            spriteTransform.Rotate(new Vector3(0f, 0f, 315f));
        }
    }

    private void RotationSprite()
    {
        if (direction == Vector3.up)
        {
            spriteRender.sprite = spriteDirections[0];
        }
        else if (direction == Vector3.down)
        {
            spriteRender.sprite = spriteDirections[1];
        }
        else if (direction == Vector3.left)
        {
            spriteRender.sprite = spriteDirections[2];
        }
        else if (direction == Vector3.right)
        {
            spriteRender.sprite = spriteDirections[3];
        }
    }

    public void OutOfBounds()
    {
        levelRunner.AddHit();
        Destroy(gameObject);
    }

    public void HitPlayer()
    {
        AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, 1.0f);
        levelRunner.AddMiss(this);
        Destroy(gameObject);
    }
}
