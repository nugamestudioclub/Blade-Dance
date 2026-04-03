using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStriker : MonoBehaviour
{
    public Collider2D[] petals;

    private bool[] keyInputs;

    private ContactFilter2D enemyFilter;

    // Start is called before the first frame update
    void Start()
    {
        keyInputs = new bool[4];

        enemyFilter = new ContactFilter2D().NoFilter();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseFunctionality.getIsPaused())
        {
            return;
        }
        
        keyInputs[0] = KeybindManager.PressedUp();
        keyInputs[1] = KeybindManager.PressedDown();
        keyInputs[2] = KeybindManager.PressedLeft();
        keyInputs[3] = KeybindManager.PressedRight();

        // Make sure only one key is pressed
        int keyCount = 0;
        for (int i = 0; i < 4; i++)
        {
            if (keyInputs[i])
            {
                keyCount += 1;
            }
        }
        if (keyCount != 1)
        {
            return;
        }

        if (KeybindManager.PressedUp())
        {
            UpPressed();
        }
        else if (KeybindManager.PressedDown())
        {
            DownPressed();
        }
        else if (KeybindManager.PressedLeft())
        {
            LeftPressed();
        }
        else if (KeybindManager.PressedRight())
        {
            RightPressed();
        }
    }

    // returns a point value from 0 to 100 based on
    // provided accuracy from 0f to 1f
    private int PointValue(float accuracy)
    {
        if (accuracy >= 0.9f)
        {
            return 100;
        }
        if (accuracy >= 0.7f)
        {
            return 70;
        }
        if (accuracy >= 0.5f)
        {
            return 50;
        }
        if (accuracy >= 0.3f)
        {
            return 30;
        }
        return 10;
    }

    void UpPressed()
    {
        Collider2D[] overlaps = new Collider2D[4];
        int overlapNumber = petals[0].OverlapCollider(enemyFilter, overlaps);

        float minY = 2f;
        int minIndex = -1;

        for (int i = 0; i < overlapNumber; i++)
        {
            float curY = overlaps[i].gameObject.transform.position.y;

            if (curY < 0.5f || curY > 1.5f)
            {
                continue;
            }

            if (curY <= minY)
            {
                minIndex = i;
                minY = curY;
            }
        }

        if (minIndex != -1)
        {
            int points = PointValue((0.5f - Mathf.Abs(minY - 1f)) * 2f);
            overlaps[minIndex].gameObject.GetComponent<EnemyMoverV2>().HitPlayer(points);
        }
    }

    void DownPressed()
    {
        Collider2D[] overlaps = new Collider2D[4];
        int overlapNumber = petals[1].OverlapCollider(enemyFilter, overlaps);

        float maxY = -2f;
        int minIndex = -1;

        for (int i = 0; i < overlapNumber; i++)
        {
            float curY = overlaps[i].gameObject.transform.position.y;

            if (curY < -1.5f || curY > -0.5f)
            {
                continue;
            }

            if (curY >= maxY)
            {
                minIndex = i;
                maxY = curY;
            }
        }

        if (minIndex != -1)
        {
            int points = PointValue((0.5f - Mathf.Abs(maxY + 1f)) * 2f);
            overlaps[minIndex].gameObject.GetComponent<EnemyMoverV2>().HitPlayer(points);
        }
    }

    void LeftPressed()
    {
        Collider2D[] overlaps = new Collider2D[4];
        int overlapNumber = petals[2].OverlapCollider(enemyFilter, overlaps);

        float maxX = -2f;
        int minIndex = -1;

        for (int i = 0; i < overlapNumber; i++)
        {
            float curX = overlaps[i].gameObject.transform.position.x;

            if (curX < -1.5f || curX > -0.5f)
            {
                continue;
            }

            if (curX >= maxX)
            {
                minIndex = i;
                maxX = curX;
            }
        }

        if (minIndex != -1)
        {
            int points = PointValue((0.5f - Mathf.Abs(maxX + 1f)) * 2f);
            overlaps[minIndex].gameObject.GetComponent<EnemyMoverV2>().HitPlayer(points);
        }
    }

    void RightPressed()
    {
        Collider2D[] overlaps = new Collider2D[4];
        int overlapNumber = petals[3].OverlapCollider(enemyFilter, overlaps);

        float minX = 2f;
        int minIndex = -1;

        for (int i = 0; i < overlapNumber; i++)
        {
            float curX = overlaps[i].gameObject.transform.position.x;

            if (curX < 0.5f || curX > 1.5f)
            {
                continue;
            }

            if (curX <= minX)
            {
                minIndex = i;
                minX = curX;
            }
        }

        if (minIndex != -1)
        {
            int points = PointValue((0.5f - Mathf.Abs(minX - 1f)) * 2f);
            overlaps[minIndex].gameObject.GetComponent<EnemyMoverV2>().HitPlayer(points);
        }
    }
}
