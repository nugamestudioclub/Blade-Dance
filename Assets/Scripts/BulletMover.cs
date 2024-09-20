using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMover : MonoBehaviour, IMover
{
    public AudioClip hitSFX;

    private Vector3 direction;
    private float speed;
    private Bounds killBounds;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (!killBounds.Contains(transform.position))
        {
            OutOfBounds();
        }
    }

    public void SetProperties(Vector3 dir, float s, Bounds bounds)
    {
        direction = dir;
        speed = s;
        killBounds = bounds;
        ResolveRotation();
    }

    private void ResolveRotation()
    {
        if (direction == Vector3.down)
        {
            transform.Rotate(new Vector3(0f, 0f, 0f));
        }
        else if (direction == Vector3.right)
        {
            transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if (direction == Vector3.up)
        {
            transform.Rotate(new Vector3(0f, 0f, 180f));
        }
        else if (direction == Vector3.left)
        {
            transform.Rotate(new Vector3(0f, 0f, 270f));
        }
        else if (direction == (Vector3.down + Vector3.right))
        {
            transform.Rotate(new Vector3(0f, 0f, 45f));
        }
        else if (direction == (Vector3.up + Vector3.right))
        {
            transform.Rotate(new Vector3(0f, 0f, 135f));
        }
        else if (direction == (Vector3.up + Vector3.left))
        {
            transform.Rotate(new Vector3(0f, 0f, 225f));
        }
        else if (direction == (Vector3.down + Vector3.left))
        {
            transform.Rotate(new Vector3(0f, 0f, 315f));
        }
    }

    public void OutOfBounds()
    {
        LevelRunner.AddHit();
        Destroy(gameObject);
    }

    public void HitPlayer()
    {
        AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, 1.0f);
        LevelRunner.AddMiss();
        Destroy(gameObject);
    }
}
