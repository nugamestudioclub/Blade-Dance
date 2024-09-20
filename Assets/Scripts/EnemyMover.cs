using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour, IMover
{
    public AudioClip hitSFX;
    public AudioClip missSFX;

    private Vector3 direction;
    private float speed;
    private Bounds killBounds;

    //public AudioClip debugSound;
    //private bool debugCheck = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (killBounds.Contains(transform.position))
        {
            CenterHit();
        }

        /*
        Vector3 flatPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        if (!debugCheck && flatPosition.magnitude <= 1f)
        {
            Debug.Log("test sound played");
            AudioSource.PlayClipAtPoint(debugSound, Camera.main.transform.position);
            debugCheck = true;
        }
        */
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
    }

    public void CenterHit()
    {
        AudioSource.PlayClipAtPoint(missSFX, Camera.main.transform.position, 1.0f);
        LevelRunner.AddMiss();
        Destroy(gameObject);
    }

    public void HitPlayer(int accuracy)
    {
        AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, 0.5f);
        LevelRunner.AddHit(accuracy);
        Destroy(gameObject);
    }
}
