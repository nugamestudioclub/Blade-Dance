using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadedV2
{
    private LevelRunnerV2 runner;
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

    // the beat at which the level is finished spawning all notes
    private float finishedAtBeat;

    public LevelLoadedV2(List<Note> content)
    {
        this.content = content;
    }

    public void Begin(LevelRunnerV2 runner, float distanceToCenter, float noteSpeed, float secPerBeat)
    {
        this.runner = runner;
        this.distance = distanceToCenter;
        this.speed = noteSpeed;
        this.secPerBeat = secPerBeat;

        petalOffset = ((distance - 1f) / speed) / secPerBeat;
        centerOffset = (distance / speed) / secPerBeat;

        levelComplete = false;
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
                if (!note.spawned)
                {
                    if (note is Enemy)
                    {
                        Enemy enemy = note as Enemy;
                        runner.AddEnemy(enemy);
                        // runner.SpawnEnemy(note.direction);
                    }
                    else if (note is Bullet)
                    {
                        Bullet bullet = note as Bullet;
                        runner.AddBullet(bullet);
                        // runner.SpawnBulletOffset(bullet.direction, bullet.offset);
                    }

                    note.spawned = true;

                    if (note == content[content.Count - 1])
                    {
                        levelComplete = true;
                        finishedAtBeat = beat;
                    }
                }
            }
        }
    }

    public bool IsLevelComplete()
    {
        return levelComplete;
    }

    public float GetBeatFinishedAt()
    {
        return finishedAtBeat;
    }

    public int GetTotalNoteCount()
    {
        return content.Count;
    }
}
