using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    // All movement commands use this speed
    protected float _Speed = 10.0f;

    // Time stamp
    float _Time = 0.0f;

    // Execute must be implemented by each child-class
    public abstract void Execute(Rigidbody rb);

    // Undo must be implemented by the child-class
    public abstract void Undo(Rigidbody rb);
}
