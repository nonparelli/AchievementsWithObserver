using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftCommand : Command
{
    public override void Execute(Rigidbody rb)
    {
        // Add an impulse that moves the rigidbody left
        //rb.AddForce(_Speed * -rb.transform.right, ForceMode.VelocityChange);
        rb.transform.position -= rb.transform.right;

    }
    public override void Undo(Rigidbody rb)
    {
        rb.transform.position += rb.transform.right;
    }
}
