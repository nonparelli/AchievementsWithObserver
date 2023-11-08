using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingCommand : Command
{
    public override void Execute(Rigidbody rb)
    {
    }

    public override void Undo(Rigidbody rb)
    {
    }
}
