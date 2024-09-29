using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelRunner : MonoBehaviour
{
    private static int hits;
    private static int misses;

    public static TMP_Text hitLabel;
    public static TMP_Text missLabel;

    public TMP_Text hitLabelHolder;
    public TMP_Text missLabelHolder;

    public GameObject bulletPrefab;
    public GameObject enemyPrefab;

    public AudioClip countoffSFX;

    public float noteSpeed = 2f;
    // the number of beats the song will wait before starting
    public int beatsDelay = 8;

    // the distance between the edge of the square frame and the center
    // of the gameplay screen, in units
    private float edgeDistance;
    private Bounds killBounds;
    private Bounds centerBounds;

    public float songBpm;

    private float secPerBeat;
    private float secondPosition;
    private float beatPosition;
    private float dspSongTime;

    private float startDspTime;

    private AudioSource musicSource;

    private LevelBase levelContent;
    public BeatMarkerPlacer markerPlacer;
    public Metronome metronome;

    private bool activated = false;
    private float timeDelayToFinish;

    public GameObject endScreen;

    // Start is called before the first frame update
    public void LaunchRunner(List<Note> content)
    {
        if (content.Count < 1)
        {
            Debug.Log("No note content passed to LevelRunner. Level start failed.");
            return;
        }

        edgeDistance = Camera.main.orthographicSize;
        killBounds = new Bounds(Vector3.zero, Vector3.one * (edgeDistance * 2f + 4f));
        centerBounds = new Bounds(Vector3.zero, Vector3.one);

        if (hitLabelHolder != null)
        {
            hitLabel = hitLabelHolder;
        }
        if (missLabelHolder != null)
        {
            missLabel = missLabelHolder;
        }

        musicSource = GetComponent<AudioSource>();

        secPerBeat = 60f / songBpm;
        startDspTime = (float)AudioSettings.dspTime;

        levelContent = new LevelLoaded(content);
        levelContent.Begin(this, edgeDistance + 1f, noteSpeed, secPerBeat);

        musicSource.PlayScheduled(startDspTime + (beatsDelay + 1) * secPerBeat);

        // secPerbeat * speed = the amount of distance a note travels in one beat
        // use BeatMarkerPlacer to finish
        markerPlacer.PlaceMarkers(secPerBeat * noteSpeed);

        metronome.MakeClicks(startDspTime, beatsDelay, secPerBeat);

        activated = true;

        timeDelayToFinish = (edgeDistance * 2f + 4f) / noteSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (activated)
        {
            secondPosition = (float)(AudioSettings.dspTime - startDspTime);
            beatPosition = secondPosition / secPerBeat;
            levelContent.AtBeat(beatPosition - beatsDelay);

            if (levelContent.IsLevelComplete())
            {
                activated = false;
                Invoke("EndLevel", timeDelayToFinish);
            }
        }
    }

    void EndLevel()
    {
        musicSource.Stop();
        endScreen.SetActive(true);
        endScreen.GetComponent<LevelEnder>().Populate(hits, hits + misses);
        ResetCounters();
    }



    /**** BULLET FUNCTIONS *****/

    void SpawnRandomBullet()
    {
        int randDir = Random.Range(0, 4);

        Vector3 spawnPosition = Vector3.zero;
        Vector3 spawnDirection = Vector3.zero;

        if (randDir == 0)
        {
            spawnPosition = Vector3.up * (edgeDistance + 1f);
            spawnDirection = Vector3.down;
        }
        else if (randDir == 1)
        {
            spawnPosition = Vector3.down * (edgeDistance + 1f);
            spawnDirection = Vector3.up;
        }
        else if (randDir == 2)
        {
            spawnPosition = Vector3.left * (edgeDistance + 1f);
            spawnDirection = Vector3.right;
        }
        else
        {
            spawnPosition = Vector3.right * (edgeDistance + 1f);
            spawnDirection = Vector3.left;
        }

        int randPrefab = Random.Range(0, 1);

        if (randPrefab == 0)
        {
            GameObject nextBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            nextBullet.GetComponent<BulletMover>().SetProperties(spawnDirection, noteSpeed, killBounds);
        }
        else
        {
            GameObject nextEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            nextEnemy.GetComponent<EnemyMover>().SetProperties(spawnDirection, noteSpeed, centerBounds);
        }
    }

    // spawn enemy of 1 of 4 types
    public void SpawnUpEnemy()
    {
        SpawnEnemy(Vector3.up);
    }

    public void SpawnDownEnemy()
    {
        SpawnEnemy(Vector3.down);
    }

    public void SpawnLeftEnemy()
    {
        SpawnEnemy(Vector3.left);
    }

    public void SpawnRightEnemy()
    {
        SpawnEnemy(Vector3.right);
    }

    // spawn bullet of 1 of 8 types
    public void SpawnUpBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.up, Vector3.right * offset);
    }

    public void SpawnDownBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.down, Vector3.right * offset);
    }

    public void SpawnLeftBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.left, Vector3.up * offset);
    }

    public void SpawnRightBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.right, Vector3.up * offset);
    }

    public void SpawnUpRightBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.up + Vector3.right, (Vector3.up + Vector3.left) * offset);
    }

    public void SpawnUpLeftBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.up + Vector3.left, (Vector3.up + Vector3.right) * offset);
    }

    public void SpawnDownRightBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.down + Vector3.right, (Vector3.up + Vector3.right) * offset);
    }

    public void SpawnDownLeftBullet(int offset = 0)
    {
        SpawnBulletOffset(Vector3.down + Vector3.left, (Vector3.up + Vector3.left) * offset);
    }

    public void SpawnBulletOffset(Vector3 unitPosition, int offset = 0)
    {
        Vector3 offsetDirection;

        if (unitPosition.x != 0 && unitPosition.y != 0)
        {
            if (unitPosition.x + unitPosition.y == 0)
            {
                offsetDirection = (Vector3.up + Vector3.right) * 0.5f;
            }
            else
            {
                offsetDirection = (Vector3.up + Vector3.left) * 0.5f;
            }
        }
        else
        {
            if (unitPosition.x == 0)
            {
                offsetDirection = Vector3.right;
            }
            else
            {
                offsetDirection = Vector3.up;
            }
        }

        offsetDirection *= offset;

        SpawnBulletOffset(unitPosition, offsetDirection);
    }

   
    // spawn enemy based only on unit position
    public void SpawnEnemy(Vector3 unitPosition)
    {
        SpawnEnemy(unitPosition * (edgeDistance + 1f), -unitPosition);
    }

    // spawn enemy based on exact position and travel direction
    void SpawnEnemy(Vector3 position, Vector3 direction)
    {
        SpawnPrefab(enemyPrefab, centerBounds, position, direction);
    }

    void SpawnBulletOffset(Vector3 unitPosition, Vector3 offset)
    {
        SpawnBullet(unitPosition * (edgeDistance + 1f) + offset, -unitPosition);
    }

    void SpawnBullet(Vector3 position, Vector3 direction)
    {
        SpawnPrefab(bulletPrefab, killBounds, position, direction);
    }

    // given a bullet or enemy prefab, a position to spawn it, and a direction to provide its velocity,
    // spawns it in the game
    void SpawnPrefab(GameObject prefab, Bounds bounds, Vector3 position, Vector3 direction)
    {
        GameObject nextSpawn = Instantiate(prefab, position, Quaternion.identity);
        nextSpawn.GetComponent<IMover>().SetProperties(direction, noteSpeed, bounds);
    }



    /**** POINTS FUNCTIONS *****/

    public static void AddHit(int points)
    {
        // TODO point system?
        hits += 1;
        hitLabel.SetText("Hits: " + hits);
    }

    public static void AddHit()
    {
        AddHit(1);
    }

    public static void AddMiss()
    {
        misses += 1;
        missLabel.SetText("Miss: " + misses);
    }

    public static void ResetCounters()
    {
        hits = 0;
        misses = 0;
    }
}
