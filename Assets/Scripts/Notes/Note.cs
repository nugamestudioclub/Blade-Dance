using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Note
{
    // at what beat we want this note to be critical at
    public float targetBeat;
    // in what direction relative to the center the note starts
    public Vector3 direction;
    // whether this note has been spawned yet or not
    public bool spawned;

    public Note(float beat, Vector3 direction)
    {
        this.targetBeat = beat;
        this.direction = direction;
        this.spawned = false;
    }

    public abstract float Offset(float petal, float center);
}

public class Enemy : Note
{
    public Enemy(float beat, Vector3 direction) : base(beat, direction)
    {
        
    }

    public override float Offset(float petal, float center)
    {
        return petal;
    }
}

public class Bullet : Note
{
    public int offset;

    public Bullet(float beat, Vector3 direction, int offset): base(beat, direction)
    {
        this.offset = offset;
    }

    public Bullet(float beat, Vector3 direction): base(beat, direction)
    {
        this.offset = 0;
    }

    public override float Offset(float petal, float center)
    {
        return center;
    }
}
