using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody _rigidbody;

    Command cmd_W = new MoveForwardCommand();
    Command cmd_A = new MoveLeftCommand();
    Command cmd_S = new MoveBackwardCommand();
    Command cmd_D = new MoveRightCommand();

    CommandHistory _history = new CommandHistory();

    [SerializeField]
    bool _startUndo = false;
    [SerializeField]
    bool _undoActive = false;
    [SerializeField]
    bool _replayActive = false, _startReplay = false;
    [SerializeField]
    float _waitTime = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy isenemy = other.gameObject.GetComponent<Enemy>();
        if (isenemy != null)
        {
            isenemy.OnDamaged(2f);
            return;
        }
        Destroy(other.gameObject); // Destroy the coin!
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_startReplay)
        {
            StartCoroutine(ReplayCommands());
            _startReplay = false;
            return;
        }
        if (_startUndo && _history.undo.Count > 0)
        {
            //_undo_commands.Pop().Undo(_rigidbody);
            _undoActive = true;
            StartCoroutine(UndoCommand());
            _startUndo = false;
            return;

        }
        else if (_undoActive || _replayActive)
            return;

        _startUndo = false;

        if (Input.GetKeyDown(KeyCode.W))
        {
            cmd_W.Execute(_rigidbody);

            _history.undo.Push(cmd_W);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cmd_A.Execute(_rigidbody);

            _history.undo.Push(cmd_A);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cmd_S.Execute(_rigidbody);

            _history.undo.Push(cmd_S);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cmd_D.Execute(_rigidbody);

            _history.undo.Push(cmd_D);

            _history.redo.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(10.0f*transform.up, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Z)) { // undo, do we need to add this to replay somehow??
            if (_history.undo.Count > 0)
            {
                _history.redo.Push(_history.undo.Peek());
                _history.undo.Pop().Undo(_rigidbody);
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) // rewind undo
        {
            _startUndo = true;
        }
        if(Input.GetKeyDown(KeyCode.R)) // redo
        {
            if (_history.redo.Count > 0)
            {
                _history.undo.Push(_history.redo.Peek());
                _history.redo.Pop().Execute(_rigidbody);
            }
        }
        if(Input.GetKeyDown(KeyCode.T)) // Replay moves so far from start
        {
            _startReplay = true;
        }
        /*
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwapCommands(ref cmd_A,ref cmd_D);
        }
        */
    }

    IEnumerator UndoCommand()
    {

        do
        {
            _history.redo.Push(_history.undo.Peek());
            _history.undo.Pop().Undo(_rigidbody);
            yield return new WaitForSeconds(_waitTime);
        } while (_history.undo.Count>0);

        _undoActive = false;
    }
    IEnumerator ReplayCommands()
    {

        Stack<Command> temp = new Stack<Command>();
        while (_history.undo.Count > 0)
        {
            temp.Push(_history.undo.Peek());
            _history.undo.Pop().Undo(_rigidbody);
        }
        
        while (temp.Count > 0)
        {
            _history.undo.Push(temp.Peek());
            temp.Pop().Execute(_rigidbody);
            yield return new WaitForSeconds(_waitTime);
        }
        _replayActive = false;
    }

}
