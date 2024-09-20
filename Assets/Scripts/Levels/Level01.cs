using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level01 : LevelBase
{
    private LevelRunner runner;
    // in units
    private float distance;
    // in units / second
    private float speed;
    // in seconds / beat
    private float secPerBeat;

    // how many beats ahead a note has to spawn to reach a petal on target
    private float petalOffset;
    // how many beats ahead a note has to spawn to reach the center on target
    private float centerOffset;

    // we assume as an invariant that the list of notes is sorted in ascending order of target beat
    private List<Note> content;

    private bool levelComplete;

    public void Begin(LevelRunner runner, float distanceToCenter, float noteSpeed, float secPerBeat)
    {
        this.runner = runner;
        this.distance = distanceToCenter;
        this.speed = noteSpeed;
        this.secPerBeat = secPerBeat;

        petalOffset = ((distance - 1f) / speed) / secPerBeat;
        //petalOffset = 0f;
        centerOffset = (distance / speed) / secPerBeat;

        levelComplete = false;

        CreateLevel();
    }

    private void CreateLevel()
    {
        content = new List<Note>();

        content.Add(new Enemy(1, Vector3.up));
        content.Add(new Enemy(5, Vector3.down));

        content.Add(new Enemy(9, Vector3.up));
        content.Add(new Enemy(11, Vector3.left));
        content.Add(new Enemy(13, Vector3.right));
        content.Add(new Enemy(15, Vector3.down));

        content.Add(new Enemy(17, Vector3.up));
        content.Add(new Enemy(19, Vector3.up));
        content.Add(new Enemy(21, Vector3.up));
        content.Add(new Enemy(23, Vector3.up));

        content.Add(new Enemy(25, Vector3.up));
        content.Add(new Enemy(25.5f, Vector3.down));
        content.Add(new Enemy(26, Vector3.up));
        content.Add(new Enemy(27, Vector3.down));
        content.Add(new Enemy(27.5f, Vector3.up));
        content.Add(new Enemy(28, Vector3.down));

        content.Add(new Enemy(29, Vector3.left));
        content.Add(new Enemy(29.5f, Vector3.right));
        content.Add(new Enemy(30, Vector3.left));
        content.Add(new Enemy(31, Vector3.right));
        content.Add(new Enemy(31.5f, Vector3.left));
        content.Add(new Enemy(32, Vector3.right));

        content.Add(new Bullet(33, Vector3.up));
        content.Add(new Bullet(35, Vector3.up, -1));
        content.Add(new Bullet(35, Vector3.up, 1));
        content.Add(new Bullet(37, Vector3.up));
        content.Add(new Bullet(39, Vector3.up, -1));
        content.Add(new Bullet(39, Vector3.up, 1));

        content.Add(new Bullet(41, Vector3.right, 0));
        content.Add(new Bullet(41, Vector3.right, 1));

        content.Add(new Bullet(43, Vector3.right, 0));
        content.Add(new Bullet(43, Vector3.right, -1));

        content.Add(new Bullet(45, Vector3.left, 0));
        content.Add(new Bullet(45, Vector3.left, 1));

        content.Add(new Bullet(47, Vector3.left, 0));
        content.Add(new Bullet(47, Vector3.left, -1));

        content.Add(new Bullet(49, Vector3.down, 0));
        content.Add(new Bullet(49, Vector3.down, 1));

        content.Add(new Bullet(51, Vector3.down, 0));
        content.Add(new Bullet(51, Vector3.down, -1));

        content.Add(new Bullet(53, Vector3.up + Vector3.right, 0));
        content.Add(new Bullet(54, Vector3.up + Vector3.left, 0));
        content.Add(new Bullet(55, Vector3.down + Vector3.left, 0));
        content.Add(new Bullet(56, Vector3.down + Vector3.right, 0));

        content.Add(new Bullet(57, Vector3.up + Vector3.right, 1));
        content.Add(new Bullet(57, Vector3.up + Vector3.right, -1));
        content.Add(new Bullet(58, Vector3.up + Vector3.left, 1));
        content.Add(new Bullet(58, Vector3.up + Vector3.left, -1));
        content.Add(new Bullet(59, Vector3.down + Vector3.left, 1));
        content.Add(new Bullet(59, Vector3.down + Vector3.left, -1));
        content.Add(new Bullet(60, Vector3.down + Vector3.right, 1));
        content.Add(new Bullet(60, Vector3.down + Vector3.right, -1));

        content.Add(new Enemy(65, Vector3.up));
        content.Add(new Enemy(66, Vector3.left));
        content.Add(new Enemy(67, Vector3.down));
        content.Add(new Enemy(68, Vector3.right));

        content.Add(new Enemy(69, Vector3.up));
        content.Add(new Enemy(70, Vector3.right));
        content.Add(new Enemy(71, Vector3.down));
        content.Add(new Enemy(72, Vector3.left));

        content.Add(new Enemy(73, Vector3.up));
        content.Add(new Enemy(74, Vector3.down));
        content.Add(new Enemy(75, Vector3.left));
        content.Add(new Enemy(76, Vector3.right));

        content.Add(new Enemy(77, Vector3.up));
        content.Add(new Enemy(78, Vector3.down));
        content.Add(new Enemy(79, Vector3.right));
        content.Add(new Enemy(80, Vector3.left));

        content.Add(new Enemy(81, Vector3.up));
        content.Add(new Enemy(82, Vector3.down));
        content.Add(new Enemy(83, Vector3.left));
        content.Add(new Enemy(84, Vector3.right));
        content.Add(new Enemy(84.5f, Vector3.up));
        content.Add(new Enemy(85, Vector3.down));

        content.Add(new Enemy(86, Vector3.left));
        content.Add(new Enemy(87, Vector3.right));
        content.Add(new Enemy(88, Vector3.left));
        content.Add(new Enemy(88.5f, Vector3.down));
        content.Add(new Enemy(89, Vector3.up));

        content.Add(new Enemy(90, Vector3.right));
        content.Add(new Enemy(91, Vector3.left));
        content.Add(new Enemy(92, Vector3.right));
        content.Add(new Enemy(92.5f, Vector3.up));
        content.Add(new Enemy(93, Vector3.down));

        content.Add(new Enemy(94, Vector3.up));
        content.Add(new Enemy(95, Vector3.down));
        content.Add(new Enemy(96, Vector3.up));
    }

    public void AtBeat(float beat)
    {
        if (levelComplete)
        {
            return;
        }

        foreach (Note note in content)
        {
            float noteOffset = note.Offset(petalOffset, centerOffset);

            if (note.targetBeat - noteOffset > beat)
            {
                break;
            }

            if (note.targetBeat - noteOffset <= beat)
            {
                if (!note.spawned) {
                    if (note is Enemy)
                    {
                        runner.SpawnEnemy(note.direction);
                    }
                    else if (note is Bullet)
                    {
                        Bullet bullet = note as Bullet;
                        runner.SpawnBulletOffset(bullet.direction, bullet.offset);
                    }

                    note.spawned = true;

                    if (note == content[content.Count - 1])
                    {
                        levelComplete = true;
                    }
                }
            }
        }
    }

    public bool IsLevelComplete()
    {
        return levelComplete;
    }
}
