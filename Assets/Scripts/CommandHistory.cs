using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHistory
{
    public Stack<Command> undo;
    public Stack<Command> redo;
    public CommandHistory()
    {
        undo = new Stack<Command>();
        redo = new Stack<Command>();
    }
}
